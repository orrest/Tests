using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Tests.Udp;

[TestClass]
public sealed class UdpTests
{
    [TestMethod]
    public async Task SendReceiveTest()
    {
        const string payload = "hello-udp";
        
        using var server = new UdpClient(new IPEndPoint(IPAddress.Any, 0));
        int port = ((IPEndPoint)server.Client.LocalEndPoint!).Port;

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
        // fire and forget
        var receiveTask = server.ReceiveAsync(cts.Token);
        
        // send from client
        using var client = new UdpClient();
        byte[] data = Encoding.UTF8.GetBytes(payload);
        await client.SendAsync(data, data.Length, new IPEndPoint(IPAddress.Loopback, port));

        // await for result
        UdpReceiveResult result = await receiveTask;
        string received = Encoding.UTF8.GetString(result.Buffer);

        Assert.AreEqual(payload, received, "UDP payload mismatch.");
        Assert.IsTrue(result.RemoteEndPoint.Address.Equals(IPAddress.Loopback), "Remote address was not loopback.");
    }
}
