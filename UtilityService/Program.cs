using Amazon.S3;
using UtilityService.Infrastructure.Repositories;
using UtilityService.Infrastructure.Services;
using UtilityService.Models;

var builder = WebApplication.CreateBuilder(args);

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

// Configure Email Settings
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

// Register AWS S3 Client
builder.Services.AddAWSService<IAmazonS3>();

// Register Services & Repositories
builder.Services.AddScoped<EmailRepository>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<S3StorageService>();

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

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();