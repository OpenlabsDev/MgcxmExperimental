using Openlabs.Mgcxm.Internal;
using Openlabs.Mgcxm.Startup;

namespace Mgcxm.Test
{
    public static class Program
    {
        static LoggerSink s = new LoggerSink("s");

        public static async Task Main(string[] args)
        {
            await Bootstrap.LoadMgcxm(new BootstrapOptions
            {
                logRequests = true,
                blockWeirdHosts = false,
                minimumLogLevel = Serilog.Events.LogEventLevel.Debug,
                useExperimentalOptions = true,
                useGui = false,
                scalingOptions = new ScalingOptions
                {
                    maxNodes = 10,
                    maxThreads = 10,
                    useRandomPrefixes = true,
                    runProcessesInBackground = true,
                }
            });
            Logger.Error("Testing {Test}", "idk");
            s.Error("Testing {Test}", "idk");

            new HttpServerTest().Start();

            await Task.Delay(-1);
        }
    }
}