// Copr. (c) Nexus 2023. All rights reserved.

using Openlabs.Mgcxm.Internal;
using Random = Openlabs.Mgcxm.Internal.Random;

namespace Openlabs.Mgcxm.Startup;

public static class Bootstrap
{
    private static long RNG_SEED = 0x12EC2279;

    private static void OnLogMessageMade(LogMessage lMsg)
    {
        // Console.ForegroundColor = lMsg.color;
        Console.WriteLine(lMsg.message);
        // Console.ForegroundColor = lMsg.previousColor;
    }
    
    public static async Task LoadMgcxm(BootstrapOptions options)
    {
        MgcxmConfiguration.Initialize(options);
        Logger.Initialize(OnLogMessageMade);
        
        // initialize rng
        Logger.Info(string.Format("Initializing random with state 0x{0:x8}", RNG_SEED));
        Random.Init(DateTime.Now.Ticks);
        
        // initialize constants
        Logger.Info(string.Format("Loading MGCXM into current AppDomain '{0}'", AppDomain.CurrentDomain.FriendlyName));
        
        Constants.Initialize();
    }
}

public class MgcxmInitializationException : Exception
{
    public MgcxmInitializationException(string message) : base(message) {}
}

public class BootstrapOptions
{
    public bool useExperimentalOptions;
    public LogLevel minimumLogLevel;
    public ScalingOptions scalingOptions;
}

public class ScalingOptions
{
    public bool runProcessesInBackground;
    public int maxThreads;
    public int maxNodes;
    public bool useRandomPrefixes;
}