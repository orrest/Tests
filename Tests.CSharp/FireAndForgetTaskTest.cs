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
        // The Task.Run only create the task object and add it to the TaskScheduler.
        // The 'Task.Contunation'-like callback set is how await works.
        // So if a task is not awaited, the task is 'fire-and-forget'.

        static async void action() { await Task.Delay(DELAY_MILLISECONDS);  }

        Stopwatch stopwatch = new();

        stopwatch.Restart();
        await Task.Run(action);
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
