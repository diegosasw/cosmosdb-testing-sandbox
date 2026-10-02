namespace Test.Common;

public abstract class GivenWhenThen
{
    protected async Task RunScenarioAsync()
    {
        try
        {
            await Given();
            await When();
            await Then();
        }
        finally
        {
            await Cleanup();
        }
    }

    protected virtual Task Given() => Task.CompletedTask;
    protected virtual Task When() => Task.CompletedTask;
    protected abstract Task Then();
    protected virtual Task Cleanup() => Task.CompletedTask;
}
