using System.Net;
using System.Net.Sockets;

namespace Tests.Udp;

public sealed class Device : IAsyncDisposable
{
    public string Name { get; }
    public int Port { get; private set; }
    public IPEndPoint EndPoint => new(IPAddress.Loopback, Port);

    private UdpClient? _udp;
    private Task? _receiveTask;
    private CancellationTokenSource? _cts;

    public event EventHandler<UdpReceiveResult>? MessageReceived;

    public Device(string name, int port = 0)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Port = port;
    }

    public void Start(CancellationToken cancellationToken = default)
    {
        _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _udp = new UdpClient(new IPEndPoint(IPAddress.Loopback, Port));
        Port = ((IPEndPoint)_udp.Client.LocalEndPoint!).Port;
        _receiveTask = ReceiveLoopAsync(_cts.Token);
    }

    private async Task ReceiveLoopAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            UdpReceiveResult result;
            try
            {
                result = await _udp!.ReceiveAsync(token);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch
            {
                // ignore
                break;
            }

            MessageReceived?.Invoke(this, result);
        }
    }

    public Task<int> SendAsync(byte[] data, int remotePort)
    {
        if (_udp is null) throw new InvalidOperationException("Device not started.");
        return _udp.SendAsync(data, data.Length, new IPEndPoint(IPAddress.Loopback, remotePort));
    }

    public async Task StopAsync()
    {
        _cts?.Cancel();
        if (_receiveTask != null)
        {
            try { await _receiveTask.ConfigureAwait(false); } catch { }
        }

        _udp?.Close();
        _udp?.Dispose();
        _udp = null;

        _cts?.Dispose();
        _cts = null;
    }

    public async ValueTask DisposeAsync()
    {
        await StopAsync().ConfigureAwait(continueOnCapturedContext: false);
    }
}