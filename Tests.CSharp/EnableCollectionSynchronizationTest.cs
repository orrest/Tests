using System.Collections.ObjectModel;
using System.Windows.Data;
using Bogus;

namespace Tests.CSharp;

[TestClass]
public class EnableCollectionSynchronizationTest
{
    private readonly Faker faker = new();

    [STATestMethod]
    public async Task Add_From_Multi_Threads_Without_Lock_May_Not_Return_Sepecified_Count()
    {
        var items = new ObservableCollection<string>();

        var tasks = new List<Task>();

        const int COUNT = 2000;
        // do not await it, make it run on thread pool concurrently
        for (int i = 0; i < COUNT; i++)
        {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            var task = new Task(() =>
            {
                items.Add(faker.Random.Word());
            });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            tasks.Add(task);
        }

        Assert.HasCount(COUNT, tasks);

        foreach (var task in tasks)
        {
            task.Start();
        }

        await Task.WhenAll(tasks);

        Console.WriteLine(items.Count);

        Assert.Throws<AssertFailedException>(() =>
        {
            Assert.HasCount(COUNT, items);
        });
    }

    [STATestMethod]
    public async Task Add_From_Multi_Threads_With_Lock_Should_Return_Sepecified_Count()
    {
        var items = new ObservableCollection<string>();
        var tasks = new List<Task>();
        const int COUNT = 2000;

        var LOCK = new object();

        // do not await it, make it run on thread pool concurrently
        for (int i = 0; i < COUNT; i++)
        {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            var task = new Task(() =>
            {
                lock (LOCK)
                {
                    items.Add(faker.Random.Word());
                }
            });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            tasks.Add(task);
        }

        Assert.HasCount(COUNT, tasks);

        foreach (var task in tasks)
        {
            task.Start();
        }

        await Task.WhenAll(tasks);

        Console.WriteLine(items.Count);

        Assert.HasCount(COUNT, items);
    }

    [STATestMethod]
    public async Task Change_CollectionView_In_Different_Thread_Should_Throw()
    {
        var items = new ObservableCollection<string>([
            faker.Lorem.Sentence(),
            faker.Lorem.Sentence(),
            faker.Lorem.Sentence(),
        ]);

        CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(items);
        Assert.HasCount(items.Count, view);

        await Task.Run(() =>
        {
            // different thread for collectionview
            Assert.Throws<NotSupportedException>(() =>
            {
                items.Add(faker.Lorem.Sentence());
            });
        });

        // but this still 4
        view.Refresh();
        Assert.HasCount(4, view);
    }

    [STATestMethod]
    public async Task Change_CollectionView_In_Different_Thread_With_Lock_Should_Success()
    {
        // create tasks
        var items = new ObservableCollection<string>();
        var tasks = new List<Task>();
        const int COUNT = 2000;

        var LOCK = new object();

        // do not await it, make it run on thread pool concurrently
        for (int i = 0; i < COUNT; i++)
        {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            var task = new Task(() =>
            {
                lock (LOCK)
                {
                    items.Add(faker.Random.Word());
                }
            });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            tasks.Add(task);
        }

        Assert.HasCount(COUNT, tasks);

        CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(items);
        // THIS ENSURES THE view GET SYNC WITH items
        BindingOperations.EnableCollectionSynchronization(items, LOCK);

        // start tasks
        foreach (var task in tasks)
        {
            task.Start();

            if (faker.Random.Int(1, 10) == 1)
            {
                // view

                view.Refresh();
            }
        }

        await Task.WhenAll(tasks);

        Console.WriteLine(items.Count);

        Assert.HasCount(COUNT, items);
    }
}
