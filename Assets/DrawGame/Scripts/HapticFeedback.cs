using UnityEngine;
#if UNITY_IOS
using System.Runtime.InteropServices;
#endif

public static class HapticFeedback
{
#if UNITY_IOS
    [DllImport("__Internal")]
    private static extern void _hapticImpact(int style);

    [DllImport("__Internal")]
    private static extern void _hapticNotification(int type);

    [DllImport("__Internal")]
    private static extern void _hapticSelection();

    private static bool IsSupported()
    {
        return SystemInfo.supportsVibration;
    }
#endif

    public static void Light()
    {
#if UNITY_IOS
        if (IsSupported()) _hapticImpact(0);
#elif UNITY_ANDROID
        Handheld.Vibrate();
#endif
    }

    public static void Medium()
    {
#if UNITY_IOS
        if (IsSupported()) _hapticImpact(1);
#elif UNITY_ANDROID
        Handheld.Vibrate();
#endif
    }

    public static void Heavy()
    {
#if UNITY_IOS
        if (IsSupported()) _hapticImpact(2);
#elif UNITY_ANDROID
        Handheld.Vibrate();
#endif
    }

    public static void Success()
    {
#if UNITY_IOS
        if (IsSupported()) _hapticNotification(0);
#endif
    }

    public static void Warning()
    {
#if UNITY_IOS
        if (IsSupported()) _hapticNotification(1);
#endif
    }

    public static void Error()
    {
#if UNITY_IOS
        if (IsSupported()) _hapticNotification(2);
#endif
    }

    public static void Selection()
    {
#if UNITY_IOS
        if (IsSupported()) _hapticSelection();
#endif
    }
}
