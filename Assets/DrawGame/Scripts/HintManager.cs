using System;
using UnityEngine;

public class HintManager : MonoBehaviour
{
    public static HintManager Instance { get; private set; }

    private const string KEY_HINT_COUNT = "HintCount";
    private const int STARTING_HINTS = 5;
    private const int HINTS_PER_PURCHASE = 5;

    public event Action<int> OnHintCountChanged;

    public int HintCount
    {
        get => PlayerPrefs.GetInt(KEY_HINT_COUNT, STARTING_HINTS);
        private set
        {
            PlayerPrefs.SetInt(KEY_HINT_COUNT, value);
            PlayerPrefs.Save();
            OnHintCountChanged?.Invoke(value);
        }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public bool UseHint()
    {
        int current = HintCount;
        if (current <= 0) return false;
        HintCount = current - 1;
        return true;
    }

    public void AddHints(int count)
    {
        HintCount = HintCount + count;
    }

    public void AddPurchasedHints()
    {
        AddHints(HINTS_PER_PURCHASE);
    }
}
