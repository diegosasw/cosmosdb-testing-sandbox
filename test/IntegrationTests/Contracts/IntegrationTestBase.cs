using Sample.Api;
using Test.Common;
using Test.Common.Fixtures;
using Xunit.Abstractions;

namespace IntegrationTests.Contracts;

[Collection("Integration")]
public abstract class IntegrationTestBase(TestContainerFixture fixture, ITestOutputHelper output, bool clearContainers = false)
    : TestServerBase<Program>(Guid.NewGuid().ToString("N"), fixture, output, clearContainers);