// Copr. (c) Nexus 2023. All rights reserved.

namespace Openlabs.Mgcxm.Startup;

public class BootstrapStartedEventArgs : EventArgs
{
    public DateTime StartedAt { get; private set; }
}