using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Tests.Udp;

[TestClass]
public sealed class DeviceTests
{
    [TestMethod]
    public async Task Device_SendReceive_LocalLoopback()
    {
        const string payload = "device-hello";

        await using var deviceA = new Device("A", port: 0);
        await using var deviceB = new Device("B", port: 0);

        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
        var token = cts.Token;

        var tcs = new TaskCompletionSource<UdpReceiveResult>(TaskCreationOptions.RunContinuationsAsynchronously);
        deviceB.MessageReceived += (_, result) => tcs.TrySetResult(result);

        deviceA.Start(token);
        deviceB.Start(token);

        byte[] data = Encoding.UTF8.GetBytes(payload);
        await deviceA.SendAsync(data, deviceB.Port);

        UdpReceiveResult result = await tcs.Task;
        string received = Encoding.UTF8.GetString(result.Buffer);

        Assert.AreEqual(payload, received, "UDP payload mismatch.");
        Assert.IsTrue(result.RemoteEndPoint.Address.Equals(IPAddress.Loopback), "Remote address was not loopback.");
        Assert.AreEqual(deviceA.Port, result.RemoteEndPoint.Port, "Remote port did not match sender port.");

    }
}