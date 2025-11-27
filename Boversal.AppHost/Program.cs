var builder = DistributedApplication.CreateBuilder(args);

// Add ProjectManagementService
var projectManagement = builder.AddProject<Projects.ProjectManagementService_API>("project-management-service");

// Add UtilityService
var utility = builder.AddProject<Projects.UtilityService>("utility-service");

// Add API Gateway (will route to above services)
builder.AddProject<Projects.Boversal_Gateway>("api-gateway")
    .WithReference(projectManagement)
    .WithReference(utility);

builder.Build().Run();
