using Openlabs.Mgcxm.Startup;

namespace Mgcxm.Test
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            await Bootstrap.LoadMgcxm(new BootstrapOptions
            {
                logRequests = true,
                blockWeirdHosts = false,
                minimumLogLevel = Openlabs.Mgcxm.Internal.LogLevel.INFO,
                useExperimentalOptions = true,
                useGui = true,
                scalingOptions = new ScalingOptions
                {
                    maxNodes = 10,
                    maxThreads = 10,
                    useRandomPrefixes = true,
                    runProcessesInBackground = true,
                }
            });

            new HttpServerTest().Start();

            await Task.Delay(-1);
        }
    }
}