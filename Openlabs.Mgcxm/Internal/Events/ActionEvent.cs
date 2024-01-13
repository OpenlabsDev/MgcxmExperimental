using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Openlabs.Mgcxm.Internal
{
    public class ActionEvent : NongenericListedAction
    {
        public ActionEvent() : this(24)
        {

        }
        public ActionEvent(int capacity) 
        {
            this.InitializeCapacity(capacity);
        }

        public void Invoke(bool useTryCatchClause = true)
        {
            foreach (var item in this.ActionList)
            {
                if (useTryCatchClause)
                {
                    try
                    {
                        item();
                    }
                    catch (Exception ex)
                    {
                        Logger.Exception("Failed to execute ActionEvent", ex);
                    }
                }
                else
                    item();
            }
        }

        public static ActionEvent operator +(ActionEvent lhs, Action rhs)
        {
            lhs.AddAction(rhs);
            return lhs;
        }
        public static ActionEvent operator -(ActionEvent lhs, Action rhs)
        {
            lhs.RemoveAction(rhs);
            return lhs;
        }
    }
    public class ActionEvent<T> : GenericListedAction<T>
    {
        public ActionEvent() : this(24)
        {

        }
        public ActionEvent(int capacity)
        {
            this.InitializeCapacity(capacity);
        }

        public void Invoke(T arg1, bool useTryCatchClause = true)
        {
            foreach (var item in this.ActionList)
            {
                if (useTryCatchClause)
                {
                    try
                    {
                        item(arg1);
                    }
                    catch (Exception ex)
                    {
                        Logger.Exception("Failed to execute ActionEvent", ex);
                    }
                }
                else
                    item(arg1);
            }
        }

        public static ActionEvent<T> operator +(ActionEvent<T> lhs, Action<T> rhs)
        {
            lhs.AddAction(rhs);
            return lhs;
        }
        public static ActionEvent<T> operator -(ActionEvent<T> lhs, Action<T> rhs)
        {
            lhs.RemoveAction(rhs);
            return lhs;
        }
    }
    public class ActionEvent<T, T1> : GenericListedAction<T, T1>
    {
        public ActionEvent() : this(24)
        {

        }
        public ActionEvent(int capacity)
        {
            this.InitializeCapacity(capacity);
        }

        public void Invoke(T arg1, T1 arg2, bool useTryCatchClause = true)
        {
            foreach (var item in this.ActionList)
            {
                if (useTryCatchClause)
                {
                    try
                    {
                        item(arg1, arg2);
                    }
                    catch (Exception ex)
                    {
                        Logger.Exception("Failed to execute ActionEvent", ex);
                    }
                }
                else
                    item(arg1, arg2);
            }
        }

        public static ActionEvent<T, T1> operator +(ActionEvent<T, T1> lhs, Action<T, T1> rhs)
        {
            lhs.AddAction(rhs);
            return lhs;
        }
        public static ActionEvent<T, T1> operator -(ActionEvent<T, T1> lhs, Action<T, T1> rhs)
        {
            lhs.RemoveAction(rhs);
            return lhs;
        }
    }
    public class ActionEvent<T, T1, T2> : GenericListedAction<T, T1, T2>
    {
        public ActionEvent() : this(24)
        {

        }
        public ActionEvent(int capacity)
        {
            this.InitializeCapacity(capacity);
        }

        public void Invoke(T arg1, T1 arg2, T2 arg3, bool useTryCatchClause = true)
        {
            foreach (var item in this.ActionList)
            {
                if (useTryCatchClause)
                {
                    try
                    {
                        item(arg1, arg2, arg3);
                    }
                    catch (Exception ex)
                    {
                        Logger.Exception("Failed to execute ActionEvent", ex);
                    }
                }
                else
                    item(arg1, arg2, arg3);
            }
        }

        public static ActionEvent<T, T1, T2> operator +(ActionEvent<T, T1, T2> lhs, Action<T, T1, T2> rhs)
        {
            lhs.AddAction(rhs);
            return lhs;
        }
        public static ActionEvent<T, T1, T2> operator -(ActionEvent<T, T1, T2> lhs, Action<T, T1, T2> rhs)
        {
            lhs.RemoveAction(rhs);
            return lhs;
        }
    }
    public class ActionEvent<T, T1, T2, T3> : GenericListedAction<T, T1, T2, T3>
    {
        public ActionEvent() : this(24)
        {

        }
        public ActionEvent(int capacity)
        {
            this.InitializeCapacity(capacity);
        }

        public void Invoke(T arg1, T1 arg2, T2 arg3, T3 arg4, bool useTryCatchClause = true)
        {
            foreach (var item in this.ActionList)
            {
                if (useTryCatchClause)
                {
                    try
                    {
                        item(arg1, arg2, arg3, arg4);
                    }
                    catch (Exception ex)
                    {
                        Logger.Exception("Failed to execute ActionEvent", ex);
                    }
                }
                else
                    item(arg1, arg2, arg3, arg4);
            }
        }

        public static ActionEvent<T, T1, T2, T3> operator +(ActionEvent<T, T1, T2, T3> lhs, Action<T, T1, T2, T3> rhs)
        {
            lhs.AddAction(rhs);
            return lhs;
        }
        public static ActionEvent<T, T1, T2, T3> operator -(ActionEvent<T, T1, T2, T3> lhs, Action<T, T1, T2, T3> rhs)
        {
            lhs.RemoveAction(rhs);
            return lhs;
        }
    }
    public class ActionEvent<T, T1, T2, T3, T4> : GenericListedAction<T, T1, T2, T3, T4>
    {
        public ActionEvent() : this(24)
        {

        }
        public ActionEvent(int capacity)
        {
            this.InitializeCapacity(capacity);
        }

        public void Invoke(T arg1, T1 arg2, T2 arg3, T3 arg4, T4 arg5, bool useTryCatchClause = true)
        {
            foreach (var item in this.ActionList)
            {
                if (useTryCatchClause)
                {
                    try
                    {
                        item(arg1, arg2, arg3, arg4, arg5);
                    }
                    catch (Exception ex)
                    {
                        Logger.Exception("Failed to execute ActionEvent", ex);
                    }
                }
                else
                    item(arg1, arg2, arg3, arg4, arg5);
            }
        }

        public static ActionEvent<T, T1, T2, T3, T4> operator +(ActionEvent<T, T1, T2, T3, T4> lhs, Action<T, T1, T2, T3, T4> rhs)
        {
            lhs.AddAction(rhs);
            return lhs;
        }
        public static ActionEvent<T, T1, T2, T3, T4> operator -(ActionEvent<T, T1, T2, T3, T4> lhs, Action<T, T1, T2, T3, T4> rhs)
        {
            lhs.RemoveAction(rhs);
            return lhs;
        }
    }
}
