using System;
using System.Collections.Generic;
using Prova;
using Prova.Core;

namespace Prova.Demo
{
    // Assembly-level hooks must be static and can be anywhere in the assembly.
    public static class GlobalSetup
    {
        [BeforeAssembly]
        public static Task Init()
        {
            Console.WriteLine("🌍 [Assembly] Global Initialization...");
            return Task.CompletedTask;
        }

        [AfterAssembly]
        public static Task Cleanup()
        {
            Console.WriteLine("🌍 [Assembly] Global Cleanup...");
            return Task.CompletedTask;
        }
    }

    public class LifecycleHooksSample
    {
        private readonly List<string> _log = new List<string>();

        // Class-level hooks must be static.
        [BeforeClass]
        public static Task SetupClass()
        {
            Console.WriteLine("🏠 [Class] One-time setup for LifecycleHooksSample");
            return Task.CompletedTask;
        }

        [AfterClass]
        public static Task TeardownClass()
        {
            Console.WriteLine("🏠 [Class] One-time teardown for LifecycleHooksSample");
            return Task.CompletedTask;
        }

        // Test-level hooks (default)
        [Before]
        public void Setup()
        {
            _log.Clear();
            _log.Add("Setup");
            Console.WriteLine("🧪 [Test] Setup");
        }

        [After]
        public void Teardown()
        {
            _log.Add("Teardown");
            Console.WriteLine("🧪 [Test] Teardown. Log: " + string.Join(" -> ", _log));
        }

        [Fact]
        public void Test_With_Lifecycle()
        {
            _log.Add("Execution");
            Console.WriteLine("   🏃 Running Test_With_Lifecycle...");
        }

        [Fact]
        public void Another_Test()
        {
            _log.Add("Execution");
            Console.WriteLine("   🏃 Running Another_Test...");
        }
    }
}
