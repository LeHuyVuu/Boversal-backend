using Amazon.S3;
using DotNetEnv;
using UtilityService.Infrastructure.Repositories;
using UtilityService.Infrastructure.Services;
using UtilityService.Models;

var builder = WebApplication.CreateBuilder(args);

// Load .env variables (for local development)
// Try multiple paths to find .env file
var possiblePaths = new[]
{
    Path.Combine(Directory.GetCurrentDirectory(), "..", ".env"), // ../boversal-backend/.env
    Path.Combine(Directory.GetCurrentDirectory(), "..", "..", ".env"), // ../../.env  
    Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", ".env"), // From bin/Debug/net8.0
    ".env" // Current directory
};

string? foundEnvPath = null;
foreach (var path in possiblePaths)
{
    var fullPath = Path.GetFullPath(path);
    Console.WriteLine($"[DEBUG UtilityService] Checking .env at: {fullPath}");
    if (File.Exists(fullPath))
    {
        foundEnvPath = fullPath;
        Console.WriteLine($"[DEBUG UtilityService] ✅ Found .env at: {fullPath}");
        Env.Load(fullPath);
        Console.WriteLine($"[DEBUG UtilityService] Loaded KAFKA_BOOTSTRAP_SERVERS: {Environment.GetEnvironmentVariable("KAFKA_BOOTSTRAP_SERVERS")}");
        break;
    }
}

if (foundEnvPath == null)
{
    Console.WriteLine($"[DEBUG UtilityService] ❌ .env file not found! Current directory: {Directory.GetCurrentDirectory()}");
    Console.WriteLine($"[DEBUG UtilityService] ❌ Base directory: {AppContext.BaseDirectory}");
}

// Add Aspire ServiceDefaults
builder.AddServiceDefaults();

// Add services to the container with Unicode support
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.WriteIndented = true;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { 
        Title = "Utility Service API", 
        Version = "v1",
        Description = "Email and File Upload Service"
    });
});

// Configure Email Settings - Read from environment variables loaded from .env
builder.Services.Configure<EmailSettings>(emailSettings =>
{
    emailSettings.SmtpServer = Environment.GetEnvironmentVariable("EMAIL_SMTP_HOST") ?? "smtp.gmail.com";
    emailSettings.Port = int.Parse(Environment.GetEnvironmentVariable("EMAIL_SMTP_PORT") ?? "587");
    emailSettings.SenderEmail = Environment.GetEnvironmentVariable("EMAIL_FROM_ADDRESS") ?? "";
    emailSettings.SenderName = Environment.GetEnvironmentVariable("EMAIL_FROM_NAME") ?? "Boversal Meeting";
    emailSettings.Password = Environment.GetEnvironmentVariable("EMAIL_SMTP_PASSWORD") ?? "";
});

// Register AWS S3 Client
builder.Services.AddAWSService<IAmazonS3>();

// Register Services & Repositories
builder.Services.AddScoped<EmailRepository>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<S3StorageService>();

// Register Kafka Consumer & Email Service cho Meeting
builder.Services.AddScoped<UtilityService.Infrastructure.IEmailService, UtilityService.Infrastructure.EmailService>();
builder.Services.AddHostedService<UtilityService.Messaging.KafkaConsumerService>();

// CORS (if needed)
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Map Aspire default endpoints
app.MapDefaultEndpoints();

// Configure the HTTP request pipeline - Always enable Swagger
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();