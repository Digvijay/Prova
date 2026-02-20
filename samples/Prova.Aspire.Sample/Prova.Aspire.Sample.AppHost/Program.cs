var builder = DistributedApplication.CreateBuilder(args);

// Add a simple redis container for testing
var cache = builder.AddRedis("cache");

// For now, we don't have a backend project to add, so we just stick with Redis to prove the concept.
// Ideally we would add: builder.AddProject<Projects.MyApi>("api");

builder.Build().Run();
