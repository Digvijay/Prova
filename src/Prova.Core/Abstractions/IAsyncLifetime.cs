namespace Prova
{
    /// <summary>
    /// Used to provide asynchronous lifetime for test classes and fixtures.
    /// </summary>
    public interface IAsyncLifetime
    {
        /// <summary>Called immediately after the class has been instantiated.</summary>
        Task InitializeAsync();
        
        /// <summary>Called when the test class is about to be disposed.</summary>
        Task DisposeAsync();
    }
}
