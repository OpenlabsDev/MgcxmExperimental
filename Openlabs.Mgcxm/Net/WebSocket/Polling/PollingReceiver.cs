// Copr. (c) Nexus 2023. All rights reserved.

using System.Diagnostics;
using Openlabs.Mgcxm.Internal;
using Openlabs.Mgcxm.Internal.SystemObjects;
using Random = Openlabs.Mgcxm.Internal.Random;

namespace Openlabs.Mgcxm.Net.Polling;

public sealed class PollingReceiver : IMgcxmSystemObject
{
    public PollingReceiver()
    {
        _timeoutStopwatch = new Stopwatch();
        
        AllocatedId = GetHashCode();
        MgcxmObjectManager.Register(AllocatedId, this);
    }
    ~PollingReceiver() => Trash();
    
    public void SendPoll(object? sender, OnPollAccepted callback)
    {
        _timeoutStopwatch.Start();
        
        OnPollReceived.Invoke(new PollRequest
        {
            timeOfPoll = DateTime.Now,
            onPollAccepted = callback,
            receiver = this
        });
    }

    public Stopwatch TimeoutStopwatch => _timeoutStopwatch;
    public ActionEvent<PollRequest> OnPollReceived { get; } = new ActionEvent<PollRequest>();

    private Stopwatch _timeoutStopwatch;
    public static readonly Duration MaxTimeout = Duration.Create(DurationType.Seconds, 5);
    
    #region IMgcxmSystemObject
    public void Trash() => MgcxmObjectManager.Deregister(AllocatedId);
    public MgcxmId AllocatedId { get; }
    #endregion
}

public class PollRequest
{
    public DateTime timeOfPoll;
    public OnPollAccepted onPollAccepted;
    public PollingReceiver receiver;
    
    public void Accept(int status)
    {
        if (receiver.TimeoutStopwatch.ElapsedTicks > PollingReceiver.MaxTimeout.GetTicks())
            throw new TimeoutException("Cannot accept PollRequest. The operation timed out.");
        
        receiver.TimeoutStopwatch.Reset();
        SafeInvocation.InvokeSafeExplicit(onPollAccepted, new object[] { status });
    }
}

public delegate void OnPollAccepted(int status);