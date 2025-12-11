using DotNetEnv;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using ProjectManagementService.API.Middleware;
using ProjectManagementService.Application;
using ProjectManagementService.Infrastructure;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add Aspire ServiceDefaults
builder.AddServiceDefaults();

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
    Console.WriteLine($"[DEBUG] Checking .env at: {fullPath}");
    if (File.Exists(fullPath))
    {
        foundEnvPath = fullPath;
        Console.WriteLine($"[DEBUG] ✅ Found .env at: {fullPath}");
        Env.Load(fullPath);
        break;
    }
}

if (foundEnvPath == null)
{
    Console.WriteLine($"[DEBUG] ❌ .env file not found! Current directory: {Directory.GetCurrentDirectory()}");
    Console.WriteLine($"[DEBUG] ❌ Base directory: {AppContext.BaseDirectory}");
}

// Read connection string from environment variable loaded from .env
var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL")
    ?? throw new Exception("Missing connection string: DATABASE_URL in .env file");

// Register Infrastructure with the connection string
builder.Services.AddInfrastructureServices(connectionString);

// Register Application layer
builder.Services.AddApplicationServices();

// Add Controllers with Unicode support
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.WriteIndented = true;
    });

// JWT Authentication - Read from environment variables loaded from .env
var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY") ?? "your-super-secret-key-min-32-characters-long-12345";
var jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "ProjectManagementAPI";
var jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "ProjectManagementClient";

Console.WriteLine($"[DEBUG] JWT Key from JWT_KEY: {Environment.GetEnvironmentVariable("JWT_KEY")}");
Console.WriteLine($"[DEBUG] JWT Key final: {jwtKey}");
Console.WriteLine($"[DEBUG] JWT Key length: {jwtKey?.Length ?? 0}");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ClockSkew = TimeSpan.Zero
    };

    // QUAN TRỌNG: Đọc JWT token từ Cookie thay vì Authorization header
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            // First, prefer header set by gateway if present (X-Forwarded-Jwt)
            var token = context.Request.Headers["X-Forwarded-Jwt"].FirstOrDefault();

            // If not present, prefer reading from Cookie named "jwt"
            if (string.IsNullOrEmpty(token))
            {
                token = context.Request.Cookies["jwt"];
            }

            // If cookie parsing failed (e.g., Cookie header not parsed),
            // fallback: parse raw Cookie header to find jwt value.
            if (string.IsNullOrEmpty(token))
            {
                var rawCookie = context.Request.Headers["Cookie"].FirstOrDefault();
                if (!string.IsNullOrEmpty(rawCookie))
                {
                    try
                    {
                        // look for jwt=... (simple parse)
                        var parts = rawCookie.Split(';');
                        foreach (var p in parts)
                        {
                            var kv = p.Split('=', 2);
                            if (kv.Length == 2 && kv[0].Trim() == "jwt")
                            {
                                token = kv[1].Trim();
                                break;
                            }
                        }
                    }
                    catch { /* ignore parse issues */ }
                }
            }

            // If still null, fallback to Authorization header (for Postman/Swagger testing)
            if (string.IsNullOrEmpty(token))
            {
                token = context.Request.Headers["Authorization"]
                    .FirstOrDefault()?.Split(' ').Last();
            }

            context.Token = token;
            return Task.CompletedTask;
        },
        OnAuthenticationFailed = context =>
        {
            try
            {
                var logger = context.HttpContext.RequestServices.GetService<ILoggerFactory>()?.CreateLogger("JwtAuth");
                logger?.LogError("JWT authentication failed: {Message} | Exception: {Exception}", context.Exception?.Message, context.Exception);
                logger?.LogError("Headers: {Headers}", string.Join("; ", context.HttpContext.Request.Headers.Select(h => h.Key + "=" + h.Value)));
                logger?.LogError("Cookies: {Cookies}", string.Join("; ", context.HttpContext.Request.Cookies.Select(c => c.Key + "=" + c.Value)));
            }
            catch { }
            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            try
            {
                var logger = context.HttpContext.RequestServices.GetService<ILoggerFactory>()?.CreateLogger("JwtAuth");
                logger?.LogWarning("JWT challenge: {Error} - {ErrorDescription}", context.Error, context.ErrorDescription);
                logger?.LogWarning("Headers: {Headers}", string.Join("; ", context.HttpContext.Request.Headers.Select(h => h.Key + "=" + h.Value)));
                logger?.LogWarning("Cookies: {Cookies}", string.Join("; ", context.HttpContext.Request.Cookies.Select(c => c.Key + "=" + c.Value)));
            }
            catch { }
            return Task.CompletedTask;
        }
    };
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .SetIsOriginAllowed(_ => true); // Allow all origins when credentials enabled
    });
});


// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Project Management API", Version = "v1" });
    
    // Enable XML comments trong Swagger UI
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

// Process forwarded headers (X-Forwarded-For, X-Forwarded-Proto) so Request.IsHttps is correct behind proxies
var forwardedOptions = new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
};
// Clear known networks/proxies so headers from our reverse proxy are accepted
forwardedOptions.KnownNetworks.Clear();
forwardedOptions.KnownProxies.Clear();
app.UseForwardedHeaders(forwardedOptions);

// Map Aspire default endpoints (health checks, etc.)
app.MapDefaultEndpoints();

// Swagger UI - enabled in all environments so frontend and QA can access API docs
app.UseSwagger();
app.UseSwaggerUI();

// Custom Middlewares (order matters!)
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseRouting();
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();