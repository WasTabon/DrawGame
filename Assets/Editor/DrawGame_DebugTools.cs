using UnityEngine;
using UnityEditor;

public class DrawGame_DebugTools
{
    [MenuItem("DrawGame/Debug/Reset Hints to 5")]
    public static void ResetHints()
    {
        PlayerPrefs.SetInt("HintCount", 5);
        PlayerPrefs.Save();
        Debug.Log("Hints reset to 5");
    }

    [MenuItem("DrawGame/Debug/Reset All Progress")]
    public static void ResetAllProgress()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("All progress reset (including tutorial)");
    }

    [MenuItem("DrawGame/Debug/Unlock All Levels")]
    public static void UnlockAllLevels()
    {
        PlayerPrefs.SetInt("MaxUnlockedLevel", 30);
        PlayerPrefs.Save();
        Debug.Log("All 30 levels unlocked");
    }

    [MenuItem("DrawGame/Debug/Add 10 Hints")]
    public static void Add10Hints()
    {
        int current = PlayerPrefs.GetInt("HintCount", 5);
        PlayerPrefs.SetInt("HintCount", current + 10);
        PlayerPrefs.Save();
        Debug.Log("Added 10 hints. Total: " + (current + 10));
    }

    [MenuItem("DrawGame/Debug/Reset Tutorial")]
    public static void ResetTutorial()
    {
        PlayerPrefs.SetInt("TutorialShown", 0);
        PlayerPrefs.Save();
        Debug.Log("Tutorial reset - will show on next MainMenu load");
    }
}
