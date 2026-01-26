namespace Prova
{
    /// <summary>
    /// Used to decorate xAdmin unit test classes that want to use a single fixture instance
    /// across all tests in the same test class.
    /// </summary>
    /// <typeparam name="TFixture">The type of the fixture.</typeparam>
    public interface IClassFixture<TFixture> where TFixture : class
    {
    }
}
