using System.Diagnostics;

namespace Tests.CSharp;

[TestClass]
public class FireAndForgetTaskTest
{
    private const int DELAY_MILLISECONDS = 3000;

    [TestMethod]
    public async Task FireAndForgetTaskReturnsImmediately()
    {
        Stopwatch stopwatch = new();

        stopwatch.Restart();
        FireAndForgetMethod();
        stopwatch.Stop();

        // time comsuming of just a method call
        Assert.IsLessThan(100, stopwatch.ElapsedMilliseconds);
    }

    /// <summary>
    /// Actually when we say 'return', it is something like Task.Continuation callback.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task AwaitableTaskReturnsAfterRunning()
    {
        Stopwatch stopwatch = new();

        stopwatch.Restart();
        await AwaitableMethod();
        stopwatch.Stop();

        Assert.IsGreaterThan(DELAY_MILLISECONDS, stopwatch.ElapsedMilliseconds);
    }

    [TestMethod]
    public async Task AsyncActionIsFireAndForget()
    {
        Action action = async () => { await Task.Delay(DELAY_MILLISECONDS); };
        // Equals:
        //static async void action() { await Task.Delay(MILLISECONDS); }

        Stopwatch stopwatch = new();

        stopwatch.Restart();
        // cannot await void
        action.Invoke();
        stopwatch.Stop();

        Assert.IsLessThan(100, stopwatch.ElapsedMilliseconds);
    }

    [TestMethod]
    public async Task FireAndForgetMethodIsNotAwaitableEventItsInTaskRun()
    {
        static async void action() { await Task.Delay(DELAY_MILLISECONDS);  }

        Stopwatch stopwatch = new();

        stopwatch.Restart();
        await Task.Run(action);
        stopwatch.Stop();

        Assert.IsLessThan(100, stopwatch.ElapsedMilliseconds);
    }

    [TestMethod]
    public async Task CallAsyncMethodWithoutAwaitMakesItFireAndForget()
    {
        Stopwatch stopwatch = new();

        stopwatch.Restart();
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        AwaitableMethod();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        stopwatch.Stop();

        Assert.IsLessThan(100, stopwatch.ElapsedMilliseconds);
    }

    [TestMethod]
    public async Task AwaitableMethodIsAwaitableEventItsInTaskRun()
    {
        static async Task action() { await Task.Delay(DELAY_MILLISECONDS);  }

        Stopwatch stopwatch = new();

        stopwatch.Restart();
        await Task.Run(action);
        stopwatch.Stop();

        Assert.IsGreaterThan(DELAY_MILLISECONDS, stopwatch.ElapsedMilliseconds);
    }

    /// <summary>
    /// Cannot await void
    /// </summary>
    private static async void FireAndForgetMethod()
    {
        await Task.Delay(DELAY_MILLISECONDS);
    }

    private static async Task AwaitableMethod()
    {
        await Task.Delay(DELAY_MILLISECONDS);
    }
}
