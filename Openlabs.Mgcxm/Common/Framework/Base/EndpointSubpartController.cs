// Copr. (c) Nexus 2023. All rights reserved.

using Openlabs.Mgcxm.Internal;
using Openlabs.Mgcxm.Net;

namespace Openlabs.Mgcxm.Common.Framework;

public abstract class EndpointSubpartController
{
    internal void NotifyUpdate(EndpointResolver resolver)
    {
        if (resolver == null) return;
        OnSubpartUpdated.InvokeSafe();
    }

    internal void AddEndpoint(MgcxmHttpEndpoint endpoint) => _allEndpoints.Add(endpoint);


    public IReadOnlyList<MgcxmHttpEndpoint> AllEndpoints => _allEndpoints;

    public event Action OnSubpartUpdated;
    private List<MgcxmHttpEndpoint> _allEndpoints = new();
}