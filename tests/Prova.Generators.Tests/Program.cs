using System;
using Prova.Generators.Tests;

try 
{
    Console.WriteLine("Running GovernanceTests...");
    new GovernanceTests().MaxAlloc_Attribute_Generates_AllocationCheck();
    Console.WriteLine("✅ GovernanceTests Passed!");

    new DiscoveryTests().Fact_Discovers_Test();
    Console.WriteLine("✅ DiscoveryTests Passed!");

    new ConcurrencyTests().Parallel_Attribute_Generates_Semaphore();
    Console.WriteLine("✅ ConcurrencyTests Passed!");
    new DataTests().ClassData_Generates_Loop();
    Console.WriteLine("✅ DataTests Passed!");
    new TimeoutTests().Timeout_Attribute_Generates_TaskWhenAny();
    Console.WriteLine("✅ TimeoutTests Passed!");
}
catch (Exception ex)
{
    Console.WriteLine("❌ Test Failed:");
    Console.WriteLine(ex.ToString());
    Environment.Exit(1);
}
