using DotNetEnv;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using ProjectManagementService.API.Middleware;
using ProjectManagementService.Application;
using ProjectManagementService.Infrastructure;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

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

var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL")
    ?? throw new Exception("Missing DATABASE_URL environment variable");

builder.Services.AddInfrastructureServices(connectionString);
builder.Services.AddApplicationServices();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.WriteIndented = true;
    });

var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY") ?? "your-super-secret-key-min-32-characters-long-12345";
var jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "ProjectManagementAPI";
var jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "ProjectManagementClient";

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

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var token = context.Request.Headers["X-Forwarded-Jwt"].FirstOrDefault();

            if (string.IsNullOrEmpty(token))
            {
                token = context.Request.Cookies["jwt"];
            }

            if (string.IsNullOrEmpty(token))
            {
                var rawCookie = context.Request.Headers["Cookie"].FirstOrDefault();
                if (!string.IsNullOrEmpty(rawCookie))
                {
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
            }

            if (string.IsNullOrEmpty(token))
            {
                token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(' ').Last();
            }

            context.Token = token;
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials()
              .SetIsOriginAllowed(_ => true);
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Project Management API", Version = "v1" });
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    try
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ProjectManagementService.Infrastructure.Persistence.MyDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        logger.LogInformation("Checking database connection and creating schema...");
        dbContext.Database.EnsureCreated();
        logger.LogInformation("✅ Database schema created successfully!");
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "❌ An error occurred while creating the database schema.");
    }
}

var forwardedOptions = new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
};
forwardedOptions.KnownNetworks.Clear();
forwardedOptions.KnownProxies.Clear();
app.UseForwardedHeaders(forwardedOptions);

app.MapDefaultEndpoints();

app.UseSwagger(c =>
{
    c.PreSerializeFilters.Add((swagger, httpReq) =>
    {
        var scheme = httpReq.Headers["X-Forwarded-Proto"].FirstOrDefault() ?? httpReq.Scheme;
        var host = httpReq.Headers["X-Forwarded-Host"].FirstOrDefault() ?? httpReq.Host.Value;

        swagger.Servers = new List<Microsoft.OpenApi.Models.OpenApiServer>
        {
            new() { Url = $"{scheme}://{host}/project-management-service", Description = "Via Gateway" },
            new() { Url = $"{scheme}://{host}", Description = "Direct" }
        };
    });
});

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("v1/swagger.json", "Project Management API v1");
    c.RoutePrefix = "swagger";
});

app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseRouting();
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
