using Sample.Api;
using Test.Common;
using Test.Common.Fixtures;
using Xunit.Abstractions;

namespace IntegrationTests.Contracts;

[Collection("Integration")]
public abstract class IntegrationTestBase(TestContainerFixture fixture, ITestOutputHelper output)
    : TestServerBase<Program>(Guid.NewGuid().ToString("N"), fixture, output);