using Xunit;

namespace Test.Common;

public abstract class GivenWhenThen
    : IAsyncLifetime
{
    public async Task InitializeAsync()
    {
        await Given();
        await When();
    }

    public Task DisposeAsync() => Cleanup();

    protected abstract Task Given();

    protected abstract Task When();

    protected virtual Task Cleanup()
        => Task.CompletedTask;
}