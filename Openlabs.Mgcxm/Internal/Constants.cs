// Copr. (c) Nexus 2023. All rights reserved.

using System.Diagnostics;
using System.Reflection;

namespace Openlabs.Mgcxm.Internal;

public static class Constants
{
    public static void Initialize()
    {
        CurrentScheduler = ActionScheduler.Create("Main Scheduler");
    }

    public static readonly Version Version = new Version(2, 0, 3);

    public static bool CleanupItems => false;
    public static string ApplicationName => Assembly.GetEntryAssembly().GetName().Name;
    public static int ThreadId => Environment.CurrentManagedThreadId;
    public static int ThreadCount => Process.GetCurrentProcess().Threads.Count;
    public static string WorkingDirectory => Environment.CurrentDirectory;
    public static string AssetsDirectory => WorkingDirectory + "/assets";
    public static ActionScheduler CurrentScheduler { get; private set; } = null!;
}