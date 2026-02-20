# ASP.NET Core Integration

Prova provides seamless integration with ASP.NET Core for functional and end-to-end testing, supporting `WebApplicationFactory` in an AOT-compatible manner.

## Getting Started

1. **Install the Package**
   Add the `Prova.AspNetCore` package to your test project.

   ```xml
   <ItemGroup>
     <PackageReference Include="Prova.AspNetCore" Version="1.0.0" />
     <PackageReference Include="Microsoft.AspNetCore.Mvc.Testing" Version="..." />
   </ItemGroup>
   ```

2. **Configure Your Application Entry Point**
   Since Prova runs as a console application, you need to ensure your `Program.cs` can handle both being a test runner and a web server (when invoked by `WebApplicationFactory`).

   Modify your `Program.cs` to separate execution modes using a command-line flag (e.g., `--test`).

   ```csharp
   using Microsoft.AspNetCore.Builder;
   using Microsoft.Extensions.DependencyInjection;
   using Prova;
   using System.Linq;

   // 1. Check if explicitly running tests
   if (args.Contains("--test"))
   {
       // Remove the flag and run Prova
       var testArgs = args.Where(a => a != "--test").ToArray();
       await Prova.TestRunnerExecutor.RunAllAsync(testArgs);
       return;
   }

   // 2. Otherwise/Default: Run the Web Application
   var builder = WebApplication.CreateBuilder(args);
   builder.Services.AddControllers();
   var app = builder.Build();
   
   app.MapGet("/", () => "Hello World!");

   app.Run();

   // 3. Expose Program for WebApplicationFactory
   public partial class Program { }
   ```

3. **Register WebApplicationFactory**
   Use Prova's `[ConfigureServices]` attribute to register the factory in the dependency injection container. You must handle the content root explicitly for reliable AOT/single-file execution.

   ```csharp
   using Microsoft.AspNetCore.Mvc.Testing;
   using Prova;
   using Prova.AspNetCore;

   public class MyApiTests
   {
       private readonly HttpClient _client;

       [ConfigureServices]
       public static void Configure(ProvaServiceProvider services)
       {
           // Register the Factory as a Singleton
           services.AddSingleton<WebApplicationFactory<Program>>(() => {
               var factory = new ProvaWebApplicationFactory<Program>();
               
               // Explicitly set content root to avoid solution-file dependency issues
               return factory.WithWebHostBuilder(builder => {
                   builder.UseContentRoot(AppContext.BaseDirectory);
               });
           });

           // Register HttpClient as Transient
           services.AddTransient<HttpClient>(() => 
               services.Get<WebApplicationFactory<Program>>().CreateClient());
       }

       // Inject HttpClient automatically
       public MyApiTests(HttpClient client)
       {
           _client = client;
       }

       [Fact]
       public async Task Get_Endpoints_Return_Success()
       {
           var response = await _client.GetAsync("/");
           response.EnsureSuccessStatusCode();
       }
   }
   ```

## Running Tests

Execute your tests by passing the test flag you defined in `Program.cs`:

```bash
dotnet run -- --test
```

## How It Works

- **ProvaServiceProvider**: Handles the lifecycle of `WebApplicationFactory`. It automatically calls `Dispose()` on the factory when the test run completes, ensuring resources are freed.
- **ProvaWebApplicationFactory**: A lightweight helper (optional) that can be extended for custom setup.
- **AOT Compatibility**: This approach avoids Reflection-heavy test discovery and is fully compatible with Native AOT.
