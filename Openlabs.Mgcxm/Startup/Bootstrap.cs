// Copr. (c) Nexus 2023. All rights reserved.

using Openlabs.Mgcxm.Internal;
using Random = Openlabs.Mgcxm.Common.Random;

namespace Openlabs.Mgcxm.Startup;

/// <summary>
/// A static class responsible for bootstrapping the Mgcxm application.
/// </summary>
public static class Bootstrap
{
    private static long RNG_SEED = 0x12EC2279;

    /// <summary>
    /// Loads and initializes the Mgcxm application with the specified options.
    /// </summary>
    /// <param name="options">The bootstrap options to configure Mgcxm.</param>
    public static async Task LoadMgcxm(BootstrapOptions options)
    {
        MgcxmConfiguration.Initialize(options);

        Task.Factory.StartNew(() =>
        {
            BootstrapInitializer.Initialize();
        }).ConfigureAwait(false);

        // initialize rng
        Logger.Info(string.Format("Initializing random with state 0x{0:x8}", RNG_SEED));
        Random.Init(DateTime.Now.Ticks);

        // initialize constants
        Logger.Info(string.Format("Loading MGCXM into current AppDomain '{0}'", AppDomain.CurrentDomain.FriendlyName));

        Constants.Initialize();
    }
}

/// <summary>
/// Exception thrown when there's an error during Mgcxm initialization.
/// </summary>
public class MgcxmInitializationException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MgcxmInitializationException"/> class with the specified message.
    /// </summary>
    /// <param name="message">The error message.</param>
    public MgcxmInitializationException(string message) : base(message) { }
}

/// <summary>
/// Options for configuring the Mgcxm bootstrap process.
/// </summary>
public class BootstrapOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether to use experimental options during bootstrap.
    /// </summary>
    public bool useExperimentalOptions;

    /// <summary>
    /// Gets or sets a value indicating whether to log requests to a file named httpLog.log
    /// </summary>
    public bool logRequests;

    /// <summary>
    /// Gets or sets a value indicating whether to block weird hosts
    /// </summary>
    public bool blockWeirdHosts;

    /// <summary>
    /// Gets or sets a value indicating whether to use a GUI.
    /// </summary>
    public bool useGui;

    /// <summary>
    /// Gets or sets the minimum log level for logging messages.
    /// </summary>
    public LogLevel minimumLogLevel;

    /// <summary>
    /// Gets or sets the scaling options for Mgcxm.
    /// </summary>
    public ScalingOptions scalingOptions;
}

/// <summary>
/// Options for scaling Mgcxm application.
/// </summary>
public class ScalingOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether to run processes in the background.
    /// </summary>
    public bool runProcessesInBackground;

    /// <summary>
    /// Gets or sets the maximum number of threads to be used.
    /// </summary>
    public int maxThreads;

    /// <summary>
    /// Gets or sets the maximum number of nodes to be used.
    /// </summary>
    public int maxNodes;

    /// <summary>
    /// Gets or sets a value indicating whether to use random prefixes.
    /// </summary>
    public bool useRandomPrefixes;
}