using Prova;
using System;
using System.Threading.Tasks;

public static class GlobalHooksSample
{
    [BeforeEvery(HookScope.Test)]
    public static void GlobalTestSetup()
    {
        Console.WriteLine("🌍 [Global] Before every test");
    }

    [BeforeEvery(HookScope.Class)]
    public static async Task GlobalClassSetup()
    {
        Console.WriteLine("🌍 [Global] Before every class");
        await Task.Delay(1);
    }
}
