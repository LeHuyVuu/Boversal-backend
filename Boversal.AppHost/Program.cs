var builder = DistributedApplication.CreateBuilder(args);

// Load .env file to get environment variables
DotNetEnv.Env.Load(Path.Combine(builder.AppHostDirectory, "..", ".env"));

// Add ProjectManagementService with environment variables from .env
var projectManagement = builder.AddProject<Projects.ProjectManagementService_API>("project-management-service")
    .WithEnvironment("DATABASE_URL", Environment.GetEnvironmentVariable("DATABASE_URL") ?? "")
    .WithEnvironment("JWT_KEY", Environment.GetEnvironmentVariable("JWT_KEY") ?? "")
    .WithEnvironment("JWT_ISSUER", Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "")
    .WithEnvironment("JWT_AUDIENCE", Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "")
    .WithEnvironment("JWT_EXPIRATION_HOURS", Environment.GetEnvironmentVariable("JWT_EXPIRATION_HOURS") ?? "")
    .WithEnvironment("KAFKA_BOOTSTRAP_SERVERS", Environment.GetEnvironmentVariable("KAFKA_BOOTSTRAP_SERVERS") ?? "");

// Add UtilityService with environment variables from .env
var utility = builder.AddProject<Projects.UtilityService>("utility-service")
    .WithEnvironment("DATABASE_URL", Environment.GetEnvironmentVariable("DATABASE_URL") ?? "")
    .WithEnvironment("KAFKA_BOOTSTRAP_SERVERS", Environment.GetEnvironmentVariable("KAFKA_BOOTSTRAP_SERVERS") ?? "")
    .WithEnvironment("EMAIL_SMTP_HOST", Environment.GetEnvironmentVariable("EMAIL_SMTP_HOST") ?? "")
    .WithEnvironment("EMAIL_SMTP_PORT", Environment.GetEnvironmentVariable("EMAIL_SMTP_PORT") ?? "")
    .WithEnvironment("EMAIL_SMTP_USER", Environment.GetEnvironmentVariable("EMAIL_SMTP_USER") ?? "")
    .WithEnvironment("EMAIL_SMTP_PASSWORD", Environment.GetEnvironmentVariable("EMAIL_SMTP_PASSWORD") ?? "")
    .WithEnvironment("EMAIL_FROM_ADDRESS", Environment.GetEnvironmentVariable("EMAIL_FROM_ADDRESS") ?? "")
    .WithEnvironment("EMAIL_FROM_NAME", Environment.GetEnvironmentVariable("EMAIL_FROM_NAME") ?? "")
    .WithEnvironment("AWS_REGION", Environment.GetEnvironmentVariable("AWS_REGION") ?? "")
    .WithEnvironment("AWS_BUCKET_NAME", Environment.GetEnvironmentVariable("AWS_BUCKET_NAME") ?? "");

// Add API Gateway (will route to above services)
builder.AddProject<Projects.Boversal_Gateway>("api-gateway")
    .WithReference(projectManagement)
    .WithReference(utility);

builder.Build().Run();
