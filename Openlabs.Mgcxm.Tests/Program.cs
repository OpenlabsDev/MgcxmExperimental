// See https://aka.ms/new-console-template for more information

using Openlabs.Mgcxm.Assets;
using Openlabs.Mgcxm.Internal;
using Openlabs.Mgcxm.Startup;

using Openlabs.Mgcxm.Net.Polling;

using Openlabs.Mgcxm.Tests;
using Openlabs.Mgcxm.Tests.Assets.Responses;

AppDomain.CurrentDomain.ProcessExit += (sender, eventArgs) =>
{
    Logger.SaveLog("latest-log");
};
AppDomain.CurrentDomain.UnhandledException += (sender, eventArgs) =>
{
    Logger.SaveLog("error-log");
};

await Bootstrap.LoadMgcxm(new BootstrapOptions()
{
    useExperimentalOptions = true,
    minimumLogLevel = LogLevel.TRACE,
    scalingOptions = new ScalingOptions()
    {
        maxThreads = 10,
        maxNodes = 30,
        useRandomPrefixes = true,
        runProcessesInBackground = true
    }
});

// LoggerSink logSink = new LoggerSink("Program");
// logSink.Info("Initializing TestServer (HTTP, WS)");

TestServer server = new TestServer();
TestWsServer wsServer = new TestWsServer();

server.Start();
wsServer.Start();

while (true)
{
}