// Copr. (c) Nexus 2023. All rights reserved.

using Openlabs.Mgcxm.Common;
using Openlabs.Mgcxm.Internal.SystemObjects;

namespace Openlabs.Mgcxm.Internal;

public sealed class ActionScheduler
{
    private ActionScheduler(string name)
    {
        _name = name;
        _allocatedId = this.GetHashCode();
        MgcxmObjectManager.Register(_allocatedId, this);

        _task = Task.Factory.StartNew(ProcessDelegateQueue, TaskCreationOptions.LongRunning);
    }

    ~ActionScheduler()
    {
        _tokenSource.Cancel();
        MgcxmObjectManager.Deregister(_allocatedId);
    }

    public static ActionScheduler Create(string name) => new(name);

    public async Task Enqueue(string name, Delegate action, params object[] args)
    {
        QueuedTask task = new QueuedTask(name, action, args, _allocatedId);
        // _queuedTasks.Enqueue(task);
        Task.Factory.StartNew(async () =>
        {
            Logger.Trace($"Created '{name} - 0x{task.TaskId.Id:x8}' latching '{_name} - 0x{_allocatedId.Id:x8}'");
            
            try
            {
                task.Delegate.DynamicInvoke(task.Arguments);
                task.Finish();
            }
            catch (Exception ex)
            {
                Logger.Error(
                    $"[Scheduler({_name})] Failed to execute task. {nameof(ex)}: {ex.Message} || Task ID = 0x{task.TaskId.Id:x8}");
            }
        });
    }

    private void ProcessDelegateQueue()
    {
        while (!_tokenSource.IsCancellationRequested)
        {
            if (_queuedTasks.Count > 0)
            {
                var queuedTask = _queuedTasks.Dequeue();

                try
                {
                    QueuedTask task = queuedTask;
                
                    task.Delegate.DynamicInvoke(task.Arguments);
                    task.Finish();
                }
                catch (Exception ex)
                {
                    Logger.Error(
                        $"[Scheduler({_name})] Failed to execute task. {nameof(ex)}: {ex.Message} || Task ID = {(string)queuedTask.Value.TaskId}");
                }
            }
        }
    }
    
    private MgcxmId _allocatedId;
    private string _name;
    private Queue<MgcxmObject<QueuedTask>> _queuedTasks = new();
    private CancellationTokenSource _tokenSource = new();
    private Task _task;

    private class QueuedTask
    {
        public QueuedTask(string name, Delegate @delegate, object[] args, MgcxmId schedulerId)
        {
            _schedulerId = schedulerId;
            _name = name;
            _taskId = Random.Range(900000, 10000000);
            _delegate = @delegate;
            _arguments = args;

            var scheduler = MgcxmObjectManager.Retrieve<ActionScheduler>(schedulerId);
            
            Logger.Trace($"Created '{name} - 0x{_taskId.Id:x8}' latching '{scheduler._name} - 0x{scheduler._allocatedId.Id:x8}'");
            MgcxmObjectManager.Register(_taskId, this);
            schedulerId.Latch(_taskId, this);
        }

        ~QueuedTask()
        {
            _schedulerId.Unlatch(_taskId);
            MgcxmObjectManager.Deregister(_taskId);
        }

        public void Finish()
        {
            Finished = true;
            Logger.Trace($"Modified queued task flag pointing 0x{_taskId.Id:x8}");
        }
        
        public MgcxmId TaskId => _taskId;
        public string Name => _name;
        public Delegate Delegate => _delegate;
        public object[] Arguments => _arguments;
        
        public bool Finished { get; private set; }

        private MgcxmId _schedulerId;
        private MgcxmId _taskId;
        private string _name;
        private Delegate _delegate;
        private object[] _arguments;
    }
}