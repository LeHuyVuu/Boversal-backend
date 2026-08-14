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
