namespace AuthService.IntegrationTests.Common
{
    /// <summary>
    /// <b>xUnit Collection Definition:</b> Defines a test collection that links all tests 
    /// belonging to this collection to a single instance of the <see cref="CustomWebApplicationFactory{TProgram}"/>.
    /// This ensures that the application host is created only once and shared across all integration tests.
    /// </summary>
    [CollectionDefinition("IntegrationTestsCollection")]
    public class IntegrationTestsCollection : ICollectionFixture<CustomWebApplicationFactory<Program>>
    {
        // This class is empty because its purpose is only for definition.
    }
}
