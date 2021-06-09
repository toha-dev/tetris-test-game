using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class Logger
{
    public static void LogErrorIfNull<T>(T someObject)
    {
#if UNITY_EDITOR
        if (someObject != null)
            return;
        
        StackTrace stackTrace = new StackTrace();
        StackFrame previousStackFrame = stackTrace.GetFrame(1);

        string objectTypeName = typeof(T).FullName;
        string callingMethodName = previousStackFrame.GetMethod().Name;
        string callingClassName = previousStackFrame.GetMethod().DeclaringType.Name;

        UnityEngine.Debug.LogError($"Object of type <b>{objectTypeName}</b>" +
            $" is null in method <b>{callingMethodName}</b> of class " +
            $"<b>{callingClassName}</b>.");
#endif
    }
}
