using System;
using System.Threading.Tasks;
using Prova;
using Prova.Core;
using Prova.Core.Attributes;

// 1. Define your services
public interface IEmailService
{
    void SendEmail(string to, string subject);
}

public class SmtpEmailService : IEmailService
{
    public void SendEmail(string to, string subject)
    {
        Console.WriteLine($"[SMTP] Sending '{subject}' to {to}");
    }
}

public class Calculator
{
    public int Add(int a, int b) => a + b;
}

// 2. Configure usage
public static class TestConfiguration
{
    // Mark this method to wire up DI
    [ConfigureServices]
    public static void Configure(ProvaServiceCollection services)
    {
        // Register Singleton (Lazy)
        services.AddSingleton<IEmailService>(() => new SmtpEmailService());

        // Register Transient (Factory)
        services.AddTransient<Calculator>(() => new Calculator());
    }
}

// 3. Consume in tests
public class UserRegistrationTests
{
    private readonly IEmailService _emailService;
    private readonly Calculator _calculator;

    // Constructor Injection!
    public UserRegistrationTests(IEmailService emailService, Calculator calculator)
    {
        _emailService = emailService;
        _calculator = calculator;
    }

    [Fact]
    public void RegisterUser_SendsEmail()
    {
        _emailService.SendEmail("user@example.com", "Welcome!");
        // Output: [SMTP] Sending 'Welcome!' to user@example.com
    }

    [Fact]
    public void Calculate_Stats()
    {
        var result = _calculator.Add(10, 20);
        Assert.Equal(30, result);
    }
}
