using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Prova;
using System.Linq;

// Check if we are explicitly asked to run tests
if (args.Contains("--test"))
{
    var testArgs = args.Where(a => a != "--test").ToArray();
    await Prova.TestRunnerExecutor.RunAllAsync(testArgs);
    return;
}

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

var app = builder.Build();
app.MapGet("/", () => "Hello from Prova ASP.NET Core!");
app.MapGet("/api/items", () => SharedData.Items);

app.Run();

// Needed for WebApplicationFactory
public partial class Program { }

internal static class SharedData
{
    public static readonly string[] Items = new[] { "Item1", "Item2" };
}
