// Copr. (c) Nexus 2023. All rights reserved.

using Openlabs.Mgcxm.Startup;

namespace Openlabs.Mgcxm.Internal;

public static class MgcxmConfiguration
{
    public static void Initialize(BootstrapOptions options)
        => CurrentBootstrapConfiguration = !HasBootstrapConfiguration ? options : null!;
    
    public static BootstrapOptions CurrentBootstrapConfiguration { get; private set; }
    public static bool HasBootstrapConfiguration => CurrentBootstrapConfiguration != null;
}