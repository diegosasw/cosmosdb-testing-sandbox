using Test.Common.Fixtures;

namespace IntegrationTests.Contracts;

[CollectionDefinition("Integration")]
public class IntegrationTestCollection
    : ICollectionFixture<TestContainerFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
    // As per https://xunit.net/docs/shared-context
    // Fixtures can be shared across assemblies, but collection definitions must be in the same assembly as the test that uses them.
}