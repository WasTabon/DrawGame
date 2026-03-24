using System;
using UnityEngine;
using DG.Tweening;

public class LevelController : MonoBehaviour
{
    public static LevelController Instance { get; private set; }

    [SerializeField] private GoalZone goalZone;
    [SerializeField] private LevelObject[] levelObjects;

    public event Action OnLevelComplete;
    public event Action OnLevelReset;

    public bool IsComplete { get; private set; }

    private float completionTime;
    private float levelStartTime;

    public float CompletionTime => completionTime;
    public int LinesUsed => DrawingManager.Instance != null ? DrawingManager.Instance.CurrentLineCount : 0;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        levelStartTime = Time.time;

        if (goalZone != null)
        {
            goalZone.OnGoalCompleted -= HandleGoalCompleted;
            goalZone.OnGoalCompleted += HandleGoalCompleted;
        }
        else
        {
            Debug.LogWarning("LevelController: goalZone is not assigned!");
        }
    }

    private void OnDestroy()
    {
        if (goalZone != null)
        {
            goalZone.OnGoalCompleted -= HandleGoalCompleted;
        }
    }

    private void HandleGoalCompleted()
    {
        if (IsComplete) return;
        IsComplete = true;
        completionTime = Time.time - levelStartTime;

        if (DrawingManager.Instance != null)
        {
            DrawingManager.Instance.SetInputEnabled(false);
        }

        DOVirtual.DelayedCall(0.5f, () =>
        {
            OnLevelComplete?.Invoke();
        });
    }

    public void ResetLevel()
    {
        IsComplete = false;
        levelStartTime = Time.time;
        completionTime = 0f;

        if (goalZone != null)
        {
            goalZone.ResetGoal();
        }

        if (levelObjects != null)
        {
            foreach (var obj in levelObjects)
            {
                if (obj != null)
                    obj.ResetToInitial();
            }
        }

        if (DrawingManager.Instance != null)
        {
            DrawingManager.Instance.ClearAllLines();
            DrawingManager.Instance.SetInputEnabled(true);
        }

        OnLevelReset?.Invoke();
    }
}
