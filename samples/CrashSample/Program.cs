using Prova;
using System.Threading.Tasks;
using System;

public class Program
{
    public static async Task Main(string[] args)
    {
        await Prova.TestRunnerExecutor.RunAllAsync(args);
    }
}

public class CrashTests
{
    [Fact]
    public void WillCrash()
    {
        Console.WriteLine("About to crash...");
        Environment.FailFast("Intentional Crash for Testing Dump Generation");
    }
}
