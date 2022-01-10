using System;
using System.Text;
using System.Threading;
using Rainway.SDK;


static string ReverseString(byte[] data)
{
    var str = Encoding.UTF8.GetString(data);
    var charArray = str.ToCharArray();
    Array.Reverse(charArray);
    return new string(charArray);
}

// the runtime configuration
var config = new RainwayConfig
{
    // your publishable API key should go here
    ApiKey = "XXXX",//string.Empty,
    ExternalId = string.Empty,
    // LogSink = (level, message) => Console.WriteLine($"[RW][{level.ToString().ToUpper()}] {message}"),
    // audo accepts all connection request
    OnConnectionRequest = (request) => request.Accept(),
    // auto accepts all stream request and gives full input privileges to the remote peer 
    OnStreamRequest = (requests) => {

        RainwayStreamConfig rainwayStreamConfig = new RainwayStreamConfig();
        rainwayStreamConfig.InputLevel = RainwayInputLevel.Mouse | RainwayInputLevel.Keyboard;
        requests.Accept( rainwayStreamConfig);
        },
    // reverses the data sent by a peer and echos it back
    OnPeerMessage = (peer, data) => peer.Send(ReverseString(data))
};

// initalize the runtime
using var runtime = await RainwayRuntime.Initialize(config);
/*
RainwayRuntime.SetLogSink(( level,  m,  y) =>
{

    Console.WriteLine($"{level}: {m} - {y}");
});
*/

RainwayRuntime.SetLogLevel(RainwayLogLevel.Trace, "foo");
Console.WriteLine($"Rainway SDK Version: {runtime.Version}");
Console.WriteLine($"Hostname: {runtime.PeerId}");
Console.WriteLine("Press Ctrl+C To Terminate");

var closeEvent = new AutoResetEvent(false);

Console.CancelKeyPress += (sender, eventArgs) =>
{
    eventArgs.Cancel = true;
    closeEvent.Set();
};

closeEvent.WaitOne();