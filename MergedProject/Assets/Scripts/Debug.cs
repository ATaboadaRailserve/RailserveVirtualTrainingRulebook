using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Debug
{
    public static void Log(object message)
    {
        UnityEngine.Debug.Log("[Log]" + message);
    }
    public static void Log(object message, Object context)
    {
        UnityEngine.Debug.Log("[Log]" + message, context);
    }
	
    public static void LogError(object message)
    {
        UnityEngine.Debug.LogError("[Error]" + message);
    }
    public static void LogError(object message, Object context)
    {
        UnityEngine.Debug.LogError("[Error]" + message, context);
    }
	
    public static void LogWarning(object message)
    {
        UnityEngine.Debug.LogWarning("[Warning]" + message);
    }
    public static void LogWarning(object message, Object context)
    {
        UnityEngine.Debug.LogWarning("[Warning]" + message, context);
    }
	
    public static void LogAssertion(object message)
    {
        UnityEngine.Debug.LogAssertion("[Assertion]" + message);
    }
    public static void LogAssertion(object message, Object context)
    {
        UnityEngine.Debug.LogAssertion("[Assertion]" + message, context);
    }
}