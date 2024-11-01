using Xunit;

namespace Test.Common;

public abstract class GivenWhenThen
    : IAsyncLifetime
{
    public async Task InitializeAsync()
    {
        await PreConditions();
        await Given();
        await When();
    }

    public Task DisposeAsync() => Cleanup();
    
    protected virtual Task PreConditions()
        => Task.CompletedTask;
    
    protected virtual Task Given()
        => Task.CompletedTask;

    protected virtual Task When()
        => Task.CompletedTask;

    protected virtual Task Cleanup()
        => Task.CompletedTask;
}