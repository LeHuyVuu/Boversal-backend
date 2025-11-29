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

// Load .env variables
Env.Load();

// Build connection string
// var connectionString =
//     $"Server={Environment.GetEnvironmentVariable("DB_HOST")};" +
//     $"Port={Environment.GetEnvironmentVariable("DB_PORT")};" +
//     $"Database={Environment.GetEnvironmentVariable("DB_NAME")};" +
//     $"User={Environment.GetEnvironmentVariable("DB_USER")};" +
//     $"Password={Environment.GetEnvironmentVariable("DB_PASSWORD")};" +
//     $"SslMode=Required;";

// Read connection string from appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new Exception("Missing connection string: DefaultConnection");

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

// JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"] ?? "your-super-secret-key-min-32-characters-long-12345";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "ProjectManagementAPI";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "ProjectManagementClient";

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
            // Ưu tiên đọc từ Cookie tên "jwt"
            var token = context.Request.Cookies["jwt"];

            // Nếu request parsing cookie failed (vd. Cookie header not parsed),
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

            // Nếu vẫn null, fallback về Authorization header (cho Postman/Swagger test)
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
                logger?.LogWarning("JWT authentication failed: {Message}", context.Exception?.Message);
            }
            catch { }
            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            try
            {
                var logger = context.HttpContext.RequestServices.GetService<ILoggerFactory>()?.CreateLogger("JwtAuth");
                logger?.LogInformation("JWT challenge: {Error} - {ErrorDescription}", context.Error, context.ErrorDescription);
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