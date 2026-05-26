using Microsoft.Extensions.DependencyInjection;

namespace Tests.MSDI;

[TestClass]
public sealed class DependencyInjectionLifetimeTests
{
    [TestMethod]
    public void Scoped_service_is_shared_within_a_scope_and_disposed_when_scope_ends()
    {
        var services = new ServiceCollection();
        services.AddScoped<TrackingService>();

        using var provider = services.BuildServiceProvider();

        TrackingService first;
        TrackingService second;

        using (var scope = provider.CreateScope())
        {
            first = scope.ServiceProvider.GetRequiredService<TrackingService>();
            second = scope.ServiceProvider.GetRequiredService<TrackingService>();

            Assert.AreSame(first, second);
            Assert.AreEqual(0, first.DisposeCount);
        }

        Assert.AreEqual(1, first.DisposeCount);
        Assert.AreEqual(1, second.DisposeCount);
    }

    [TestMethod]
    public void Transient_service_is_recreated_on_each_resolution_and_disposed_when_scope_ends()
    {
        var services = new ServiceCollection();
        services.AddTransient<TrackingService>();

        using var provider = services.BuildServiceProvider();

        TrackingService first;
        TrackingService second;

        using (var scope = provider.CreateScope())
        {
            first = scope.ServiceProvider.GetRequiredService<TrackingService>();
            second = scope.ServiceProvider.GetRequiredService<TrackingService>();

            Assert.AreNotSame(first, second);
            Assert.AreEqual(0, first.DisposeCount);
            Assert.AreEqual(0, second.DisposeCount);
        }

        Assert.AreEqual(1, first.DisposeCount);
        Assert.AreEqual(1, second.DisposeCount);
    }

    [TestMethod]
    public void Transient_service_is_not_disposed_before_the_scope_ends()
    {
        var services = new ServiceCollection();
        services.AddTransient<TrackingService>();

        using var provider = services.BuildServiceProvider();

        TrackingService instance;

        using (var scope = provider.CreateScope())
        {
            instance = scope.ServiceProvider.GetRequiredService<TrackingService>();

            Assert.AreEqual(0, instance.DisposeCount);
        }

        Assert.AreEqual(1, instance.DisposeCount);
    }

    private sealed class TrackingService : IDisposable
    {
        public int DisposeCount { get; private set; }

        public void Dispose()
        {
            DisposeCount++;
        }
    }
}
