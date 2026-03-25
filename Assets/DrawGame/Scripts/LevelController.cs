using System;
using UnityEngine;
using DG.Tweening;

public class LevelController : MonoBehaviour
{
    public static LevelController Instance { get; private set; }

    public event Action OnLevelComplete;
    public event Action OnLevelReset;

    public bool IsComplete { get; private set; }

    private GoalZone goalZone;
    private LevelObject[] levelObjects;
    private float completionTime;
    private float levelStartTime;

    public float CompletionTime => completionTime;
    public int LinesUsed => DrawingManager.Instance != null ? DrawingManager.Instance.CurrentLineCount : 0;

    private void Awake()
    {
        Instance = this;
    }

    public void SetupLevel(GoalZone newGoalZone, LevelObject[] newLevelObjects)
    {
        if (goalZone != null)
        {
            goalZone.OnGoalCompleted -= HandleGoalCompleted;
        }

        goalZone = newGoalZone;
        levelObjects = newLevelObjects;
        IsComplete = false;
        levelStartTime = Time.time;
        completionTime = 0f;

        if (goalZone != null)
        {
            goalZone.OnGoalCompleted -= HandleGoalCompleted;
            goalZone.OnGoalCompleted += HandleGoalCompleted;
        }
        else
        {
            Debug.LogWarning("LevelController: goalZone is null!");
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

        int level = GameManager.Instance != null ? GameManager.Instance.SelectedLevel : 1;
        if (GameManager.Instance != null)
        {
            GameManager.Instance.UnlockNextLevel(level);
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

    public void LoadNextLevel()
    {
        int currentLevel = GameManager.Instance != null ? GameManager.Instance.SelectedLevel : 1;
        int nextLevel = currentLevel + 1;

        if (nextLevel > GameManager.TOTAL_LEVELS)
        {
            GameManager.Instance.LoadMainMenu();
            return;
        }

        GameManager.Instance.SelectedLevel = nextLevel;

        if (LevelSpawner.Instance != null)
        {
            LevelSpawner.Instance.SpawnLevel(nextLevel);
        }

        if (DrawingManager.Instance != null)
        {
            DrawingManager.Instance.ClearAllLines();
            DrawingManager.Instance.SetInputEnabled(true);
        }

        OnLevelReset?.Invoke();
    }
}
