using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Openlabs.Mgcxm.Internal
{
    public interface IListedAction
    {
    }

    public interface INongenericListedAction : IListedAction
    {
        void AddAction(Action action);
        void RemoveAction(Action action);

        List<Action> ActionList { get; }
    }
    public interface IGenericListedAction<T> : IListedAction
    {
        void AddAction(Action<T> action);
        void RemoveAction(Action<T> action);

        List<Action<T>> ActionList { get; }
    }
    public interface IGenericListedAction<T, T1> : IListedAction
    {
        void AddAction(Action<T, T1> action);
        void RemoveAction(Action<T, T1> action);

        List<Action<T, T1>> ActionList { get; }
    }
    public interface IGenericListedAction<T, T1, T2> : IListedAction
    {
        void AddAction(Action<T, T1, T2> action);
        void RemoveAction(Action<T, T1, T2> action);

        List<Action<T, T1, T2>> ActionList { get; }
    }
    public interface IGenericListedAction<T, T1, T2, T3> : IListedAction
    {
        void AddAction(Action<T, T1, T2, T3> action);
        void RemoveAction(Action<T, T1, T2, T3> action);

        List<Action<T, T1, T2, T3>> ActionList { get; }
    }
    public interface IGenericListedAction<T, T1, T2, T3, T4> : IListedAction
    {
        void AddAction(Action<T, T1, T2, T3, T4> action);
        void RemoveAction(Action<T, T1, T2, T3, T4> action);

        List<Action<T, T1, T2, T3, T4>> ActionList { get; }
    }

    public class NongenericListedAction : INongenericListedAction
    {
        public void AddAction(Action action)
        {
            if (_actionList.Count >= _actionList.Capacity)
                throw new ActionEventConnectorException(ConnectorAggregateType.Add, this);

            if (action != null && !_actionList.Contains(action))
                _actionList.Add(action);
        }

        public void RemoveAction(Action action)
        {
            if (_actionList.Count <= 0)
                throw new ActionEventConnectorException(ConnectorAggregateType.Remove, this);

            if (action != null && _actionList.Contains(action))
                _actionList.Remove(action);
        }

        public void InitializeCapacity(int cap)
            => _actionList = new(cap);

        public List<Action> ActionList
            => _actionList;

        private List<Action> _actionList;
    }
    public class GenericListedAction<T> : IGenericListedAction<T>
    {
        public void AddAction(Action<T> action)
        {
            if (_actionList.Count >= _actionList.Capacity) 
                throw new ActionEventConnectorException(ConnectorAggregateType.Add, this);

            if (action != null && !_actionList.Contains(action))
                _actionList.Add(action);
        }

        public void RemoveAction(Action<T> action)
        {
            if (_actionList.Count <= 0) 
                throw new ActionEventConnectorException(ConnectorAggregateType.Remove, this);

            if (action != null && _actionList.Contains(action))
                _actionList.Remove(action);
        }

        public void InitializeCapacity(int cap)
            => _actionList = new(cap);

        public List<Action<T>> ActionList
            => _actionList;

        private List<Action<T>> _actionList;
    }
    public class GenericListedAction<T, T1> : IGenericListedAction<T, T1>
    {
        public void AddAction(Action<T, T1> action)
        {
            if (_actionList.Count >= _actionList.Capacity)
                throw new ActionEventConnectorException(ConnectorAggregateType.Add, this);

            if (action != null && !_actionList.Contains(action))
                _actionList.Add(action);
        }

        public void RemoveAction(Action<T, T1> action)
        {
            if (_actionList.Count <= 0)
                throw new ActionEventConnectorException(ConnectorAggregateType.Remove, this);

            if (action != null && _actionList.Contains(action))
                _actionList.Remove(action);
        }

        public void InitializeCapacity(int cap)
            => _actionList = new(cap);

        public List<Action<T, T1>> ActionList
            => _actionList;

        private List<Action<T, T1>> _actionList;
    }
    public class GenericListedAction<T, T1, T2> : IGenericListedAction<T, T1, T2>
    {
        public void AddAction(Action<T, T1, T2> action)
        {
            if (_actionList.Count >= _actionList.Capacity)
                throw new ActionEventConnectorException(ConnectorAggregateType.Add, this);

            if (action != null && !_actionList.Contains(action))
                _actionList.Add(action);
        }

        public void RemoveAction(Action<T, T1, T2> action)
        {
            if (_actionList.Count <= 0)
                throw new ActionEventConnectorException(ConnectorAggregateType.Remove, this);

            if (action != null && _actionList.Contains(action))
                _actionList.Remove(action);
        }

        public void InitializeCapacity(int cap)
            => _actionList = new(cap);

        public List<Action<T, T1, T2>> ActionList
            => _actionList;

        private List<Action<T, T1, T2>> _actionList;
    }
    public class GenericListedAction<T, T1, T2, T3> : IGenericListedAction<T, T1, T2, T3>
    {
        public void AddAction(Action<T, T1, T2, T3> action)
        {
            if (_actionList.Count >= _actionList.Capacity)
                throw new ActionEventConnectorException(ConnectorAggregateType.Add, this);

            if (action != null && !_actionList.Contains(action))
                _actionList.Add(action);
        }

        public void RemoveAction(Action<T, T1, T2, T3> action)
        {
            if (_actionList.Count <= 0)
                throw new ActionEventConnectorException(ConnectorAggregateType.Remove, this);

            if (action != null && _actionList.Contains(action))
                _actionList.Remove(action);
        }

        public void InitializeCapacity(int cap)
            => _actionList = new(cap);

        public List<Action<T, T1, T2, T3>> ActionList
            => _actionList;

        private List<Action<T, T1, T2, T3>> _actionList;
    }
    public class GenericListedAction<T, T1, T2, T3, T4> : IGenericListedAction<T, T1, T2, T3, T4>
    {
        public void AddAction(Action<T, T1, T2, T3, T4> action)
        {
            if (_actionList.Count >= _actionList.Capacity)
                throw new ActionEventConnectorException(ConnectorAggregateType.Add, this);

            if (action != null && !_actionList.Contains(action))
                _actionList.Add(action);
        }

        public void RemoveAction(Action<T, T1, T2, T3, T4> action)
        {
            if (_actionList.Count <= 0)
                throw new ActionEventConnectorException(ConnectorAggregateType.Remove, this);

            if (action != null && _actionList.Contains(action))
                _actionList.Remove(action);
        }

        public void InitializeCapacity(int cap)
            => _actionList = new(cap);

        public List<Action<T, T1, T2, T3, T4>> ActionList
            => _actionList;

        private List<Action<T, T1, T2, T3, T4>> _actionList;
    }

    public class ActionEventConnectorException :
        Exception
    {
        public ActionEventConnectorException(ConnectorAggregateType type, IListedAction caller)
            : base(string.Format(ERROR_FORMAT, 
                                    type.ToString().ToLower(), 
                                    type == ConnectorAggregateType.Add ? "to" : "from", 
                                    caller.GetType().Name, 
                                    type == ConnectorAggregateType.Add ? TOO_MANY_CONNECTORS : LESS_THAN_ZERO_CONNECTORS))
        { 
        }

        private static readonly string TOO_MANY_CONNECTORS = "There is too many connectors. Please remove some and try again.";
        private static readonly string LESS_THAN_ZERO_CONNECTORS = "There is less than zero connectors. Please send a bug report.";

        private static readonly string ERROR_FORMAT = "Failed to {0} a connector {1} {2}. {3}.";
    }
    public enum ConnectorAggregateType
    {
        Add,
        Remove
    }
}
