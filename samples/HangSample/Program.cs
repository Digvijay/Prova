using Prova;
using System.Threading.Tasks;
using System;
using System.Threading;

public class Program
{
    public static async Task Main(string[] args)
    {
        await Prova.TestRunnerExecutor.RunAllAsync(args);
    }
}

public class HangTests
{
    [Fact]
    public async Task WillHang()
    {
        Console.WriteLine("About to hang...");
        await Task.Delay(30000); // 30s
    }
}
