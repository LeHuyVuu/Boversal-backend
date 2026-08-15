using System.Text.RegularExpressions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

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

// Enable static files for Swagger UI
app.UseStaticFiles();

// Use Swagger UI only to aggregate backend services
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/project-management-service/swagger/v1/swagger.json", "Project Management API");
    c.SwaggerEndpoint("/utility-service/swagger/v1/swagger.json", "Utility Service API");
    c.RoutePrefix = "swagger";
});

app.UseCors();

// Force CORS headers for all responses (including proxied requests)
app.Use(async (context, next) =>
{
    var origin = context.Request.Headers["Origin"].ToString();

    if (!string.IsNullOrEmpty(origin))
    {
        context.Response.Headers["Access-Control-Allow-Origin"] = origin;
        context.Response.Headers["Access-Control-Allow-Credentials"] = "true";
        context.Response.Headers["Access-Control-Allow-Headers"] = "*";
        context.Response.Headers["Access-Control-Allow-Methods"] = "*";
    }

    // Handle preflight requests
    if (context.Request.Method == "OPTIONS")
    {
        context.Response.StatusCode = 200;
        return;
    }

    await next();
});

// JWT from cookie middleware
app.Use(async (context, next) =>
{
    if (!context.Request.Headers.ContainsKey("Authorization"))
    {
        var cookieHeader = context.Request.Headers["Cookie"].FirstOrDefault();
        if (!string.IsNullOrEmpty(cookieHeader))
        {
            var m = Regex.Match(cookieHeader, @"\bjwt=([^;]+)");
            if (m.Success)
            {
                var token = m.Groups[1].Value;
                context.Request.Headers["Authorization"] = "Bearer " + token;
                context.Request.Headers["X-Forwarded-Jwt"] = token;
            }
        }
    }
    await next();
});

app.MapReverseProxy();
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));

app.Run();
