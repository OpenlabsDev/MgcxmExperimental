// Copr. (c) Nexus 2023. All rights reserved.

using System.Reflection;

namespace Openlabs.Mgcxm.Internal;

/// <summary>
/// Static class that handles the safe invocation of an <see cref="Action"/>.
/// </summary>
public static class SafeInvocation
{
    public static void InvokeSafe(this Action action)
        => InvokeSafeExplicit(action, Array.Empty<object>());
    
    public static void InvokeSafe<T>(this Action<T> action, T arg1)
        => InvokeSafeExplicit(action, new object[] { arg1 });
    
    public static void InvokeSafe<T, T1>(this Action<T> action, T arg1, T1 arg2)
        => InvokeSafeExplicit(action, new object[] { arg1, arg2 });
    
    public static void InvokeSafe<T, T1, T2>(this Action<T> action, T arg1, T1 arg2, T2 arg3)
        => InvokeSafeExplicit(action, new object[] { arg1, arg2, arg3 });
    
    public static void InvokeSafe<T, T1, T2, T3>(this Action<T> action, T arg1, T1 arg2, T2 arg3, T3 arg4)
        => InvokeSafeExplicit(action, new object[] { arg1, arg2, arg3, arg4 });
    
    public static void InvokeSafe<T, T1, T2, T3, T4>(this Action<T> action, T arg1, T1 arg2, T2 arg3, T3 arg4, T4 arg5)
        => InvokeSafeExplicit(action, new object[] { arg1, arg2, arg3, arg4, arg5 });
    
    public static void InvokeSafe<T, T1, T2, T3, T4, T5>(this Action<T> action, T arg1, T1 arg2, T2 arg3, T3 arg4, T4 arg5, T5 arg6)
        => InvokeSafeExplicit(action, new object[] { arg1, arg2, arg3, arg4, arg5, arg6 });
    
    public static void InvokeSafe<T, T1, T2, T3, T4, T5, T6>(this Action<T> action, T arg1, T1 arg2, T2 arg3, T3 arg4, T4 arg5, T5 arg6, T6 arg7)
        => InvokeSafeExplicit(action, new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7 });
    
    /// <summary>
    /// Invokes the <paramref name="action"/> safely.
    /// </summary>
    /// <param name="action">The action to invoke.</param>
    /// <param name="args">The arguments needed to invoke the action.</param>
    /// <param name="exceptionToThrow">The exception to throw if the invocation was a failure. Defaults to <see cref="Exception"/>.</param>
    /// <param name="message">The message to throw.</param>
    /// <exception cref="InvalidOperationException">If the <paramref name="exceptionToThrow"/> does not directly inherit <see cref="Exception"/>.</exception>
    /// <exception cref="Exception">Default exception that throws if no <paramref name="exceptionToThrow"/> type was given.</exception>
    public static void InvokeSafeExplicit(Delegate action, object[] args, Type exceptionToThrow = null, string message = DEFAULT_ERROR_MESSAGE)
    {
        if (exceptionToThrow != null! && !IsExceptionType(exceptionToThrow))
            throw new InvalidOperationException($"Tried to parse {TYPE_FULL_NAME} 'exceptionToThrow' as {EXCEPTION_FULL_NAME}");
        
        try
        { if (action != null) action.DynamicInvoke(args); }
        catch (Exception ex)
        {
            if (exceptionToThrow != null)
                throw (Exception)Activator.CreateInstance(exceptionToThrow, message + ": " + ex.Message)!;
        }
    }

    private static bool IsExceptionType(Type type)
    {
        return type != null && 
               ((type.BaseType == null || 
                (type.BaseType != null && type.BaseType == typeof(Exception))) || type == typeof(Exception));
    }

    private const string DEFAULT_ERROR_MESSAGE = "Unexpected error while safely invoking action";
    private const string TYPE_FULL_NAME = "System.Type";
    private const string EXCEPTION_FULL_NAME = "System.Exception";
}

public class SafeInvocationException : Exception
{
    public SafeInvocationException()
    {
    }

    public SafeInvocationException(string message) : base(message)
    {
    }

    public SafeInvocationException(string message, Exception inner) : base(message, inner)
    {
    }
}