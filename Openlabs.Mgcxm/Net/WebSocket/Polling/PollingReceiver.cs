// Copr. (c) Nexus 2023. All rights reserved.

using System.Diagnostics;
using Openlabs.Mgcxm.Common;
using Openlabs.Mgcxm.Internal;
using Openlabs.Mgcxm.Internal.SystemObjects;
using Random = Openlabs.Mgcxm.Common.Random;

namespace Openlabs.Mgcxm.Net.Polling;

/// <summary>
/// Represents a polling receiver that can send poll requests and handle poll responses.
/// </summary>
public sealed class PollingReceiver : IMgcxmSystemObject
{
    private Stopwatch _timeoutStopwatch;

    /// <summary>
    /// Initializes a new instance of the <see cref="PollingReceiver"/> class.
    /// </summary>
    public PollingReceiver()
    {
        _timeoutStopwatch = new Stopwatch();

        AllocatedId = GetHashCode();
        MgcxmObjectManager.Register(AllocatedId, this);
    }

    /// <summary>
    /// Finalizer (destructor) for the <see cref="PollingReceiver"/> class.
    /// </summary>
    ~PollingReceiver() => Trash();

    /// <summary>
    /// Sends a poll request with the provided callback function.
    /// </summary>
    /// <param name="sender">The object that initiated the poll.</param>
    /// <param name="callback">The callback function to be invoked when the poll is accepted.</param>
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

    /// <summary>
    /// Gets the timeout stopwatch associated with the polling receiver.
    /// </summary>
    public Stopwatch TimeoutStopwatch => _timeoutStopwatch;

    /// <summary>
    /// Event that is raised when a poll request is received.
    /// </summary>
    public ActionEvent<PollRequest> OnPollReceived { get; } = new ActionEvent<PollRequest>();

    /// <summary>
    /// The maximum timeout duration for a poll request.
    /// </summary>
    public static readonly Duration MaxTimeout = Duration.Create(DurationType.Seconds, 5);

    #region IMgcxmSystemObject

    /// <summary>
    /// Disposes of the polling receiver and deregisters it from the object manager.
    /// </summary>
    public void Trash() => MgcxmObjectManager.Deregister(AllocatedId);

    /// <summary>
    /// Gets the unique identifier of the polling receiver.
    /// </summary>
    public MgcxmId AllocatedId { get; }

    #endregion
}

/// <summary>
/// Represents a poll request that can be sent to a <see cref="PollingReceiver"/>.
/// </summary>
public class PollRequest
{
    /// <summary>
    /// The time the poll request was sent.
    /// </summary>
    public DateTime timeOfPoll;

    /// <summary>
    /// The callback function to be invoked when the poll is accepted.
    /// </summary>
    public OnPollAccepted onPollAccepted;

    /// <summary>
    /// The polling receiver associated with this poll request.
    /// </summary>
    public PollingReceiver receiver;

    /// <summary>
    /// Accepts the poll request with the specified status code.
    /// </summary>
    /// <param name="status">The status code to accept the poll request with.</param>
    public void Accept(int status)
    {
        if (receiver.TimeoutStopwatch.ElapsedTicks > PollingReceiver.MaxTimeout.GetTicks())
            throw new TimeoutException("Cannot accept PollRequest. The operation timed out.");

        receiver.TimeoutStopwatch.Reset();
        SafeInvocation.InvokeSafeExplicit(onPollAccepted, new object[] { status });
    }
}

/// <summary>
/// Represents a delegate for the callback function used when a poll is accepted.
/// </summary>
/// <param name="status">The status code representing the acceptance of the poll request.</param>
public delegate void OnPollAccepted(int status);