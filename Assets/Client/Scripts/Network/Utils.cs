using System;
using UnityEngine;

public static class Utils
{    
    public static void SafeDispose(this IDisposable self)
    {
        if (self == null)
            return;
        try
        {
            self.Dispose();
            self = null;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }
}