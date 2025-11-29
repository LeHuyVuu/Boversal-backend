using System.Text.RegularExpressions;

var builder = WebApplication.CreateBuilder(args);

// Add Aspire ServiceDefaults
builder.AddServiceDefaults();

// Add YARP
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .SetIsOriginAllowed(_ => true);
    });
});

var app = builder.Build();

// Map Aspire default endpoints
app.MapDefaultEndpoints();

app.UseCors();

// Middleware: if client sent cookie `jwt` but no Authorization header,
// copy jwt cookie into Authorization header so backend can validate it.
app.Use(async (context, next) =>
{
    if (!context.Request.Headers.ContainsKey("Authorization"))
    {
        var cookieHeader = context.Request.Headers["Cookie"].FirstOrDefault();
        if (!string.IsNullOrEmpty(cookieHeader))
        {
            try
            {
                var m = Regex.Match(cookieHeader, @"\bjwt=([^;]+)");
                if (m.Success)
                {
                    var token = m.Groups[1].Value;
                    context.Request.Headers["Authorization"] = "Bearer " + token;
                    // Also set a dedicated header so downstream services can read the JWT
                    // even if Cookie header gets modified/stripped by proxies.
                    context.Request.Headers["X-Forwarded-Jwt"] = token;
                }
            }
            catch { /* don't break the pipeline for parsing errors */ }
        }
    }

    await next();
});

// Map YARP routes
app.MapReverseProxy();

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));

app.Run();
