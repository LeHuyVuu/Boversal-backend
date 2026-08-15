using Amazon.S3;
using DotNetEnv;
using UtilityService.Infrastructure.Repositories;
using UtilityService.Infrastructure.Services;
using UtilityService.Models;

var builder = WebApplication.CreateBuilder(args);

var possiblePaths = new[]
{
    Path.Combine(Directory.GetCurrentDirectory(), "..", ".env"),
    Path.Combine(Directory.GetCurrentDirectory(), "..", "..", ".env"),
    Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", ".env"),
    ".env"
};

foreach (var path in possiblePaths)
{
    if (File.Exists(Path.GetFullPath(path)))
    {
        Env.Load(Path.GetFullPath(path));
        break;
    }
}

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
    c.SwaggerDoc("v1", new()
    {
        Title = "Utility Service API",
        Version = "v1",
        Description = "Email and File Upload Service"
    });
});

builder.Services.Configure<EmailSettings>(emailSettings =>
{
    emailSettings.SmtpServer = Environment.GetEnvironmentVariable("EMAIL_SMTP_HOST") ?? "smtp.gmail.com";
    emailSettings.Port = int.Parse(Environment.GetEnvironmentVariable("EMAIL_SMTP_PORT") ?? "587");
    emailSettings.SenderEmail = Environment.GetEnvironmentVariable("EMAIL_FROM_ADDRESS") ?? "";
    emailSettings.SenderName = Environment.GetEnvironmentVariable("EMAIL_FROM_NAME") ?? "Boversal Meeting";
    emailSettings.Password = Environment.GetEnvironmentVariable("EMAIL_SMTP_PASSWORD") ?? "";
});

builder.Services.AddAWSService<IAmazonS3>();

builder.Services.AddScoped<EmailRepository>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<S3StorageService>();
builder.Services.AddScoped<UtilityService.Infrastructure.IEmailService, UtilityService.Infrastructure.EmailService>();
builder.Services.AddHostedService<UtilityService.Messaging.KafkaConsumerService>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials()
              .SetIsOriginAllowed(_ => true);
    });
});

var app = builder.Build();

app.UseSwagger(c =>
{
    c.PreSerializeFilters.Add((swagger, httpReq) =>
    {
        var scheme = httpReq.Headers["X-Forwarded-Proto"].FirstOrDefault() ?? httpReq.Scheme;
        var host = httpReq.Headers["X-Forwarded-Host"].FirstOrDefault() ?? httpReq.Host.Value;

        swagger.Servers = new List<Microsoft.OpenApi.Models.OpenApiServer>
        {
            new() { Url = $"{scheme}://{host}/utility-service", Description = "Via Gateway" },
            new() { Url = $"{scheme}://{host}", Description = "Direct" }
        };
    });
});

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("v1/swagger.json", "Utility Service API v1");
    c.RoutePrefix = "swagger";
});

app.UseCors();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "Utility", timestamp = DateTime.UtcNow }));

app.Run();
