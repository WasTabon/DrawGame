using System;
using UnityEngine;
using DG.Tweening;

public class LevelController : MonoBehaviour
{
    public static LevelController Instance { get; private set; }

    public event Action<int> OnLevelComplete;
    public event Action OnLevelReset;

    public bool IsComplete { get; private set; }

    private GoalZone goalZone;
    private LevelObject[] levelObjects;
    private float completionTime;
    private float levelStartTime;
    private int earnedStars;

    public float CompletionTime => completionTime;
    public int LinesUsed => DrawingManager.Instance != null ? DrawingManager.Instance.CurrentLineCount : 0;
    public int EarnedStars => earnedStars;

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
        earnedStars = 0;
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

        int idealLines = 1;
        float idealTime = 15f;
        if (LevelSpawner.Instance != null)
        {
            var db = LevelSpawner.Instance.GetCurrentLevelData();
            if (db != null)
            {
                idealLines = db.idealLines;
                idealTime = db.idealTime;
            }
        }

        earnedStars = StarRating.Calculate(LinesUsed, completionTime, idealLines, idealTime);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.UnlockNextLevel(level);
            GameManager.Instance.SetStars(level, earnedStars);
        }

        PlayWinEffects();

        DOVirtual.DelayedCall(0.5f, () =>
        {
            OnLevelComplete?.Invoke(earnedStars);
        });
    }

    private void PlayWinEffects()
    {
        if (ParticleSpawner.Instance != null)
        {
            ParticleSpawner.Instance.EmitWinConfetti(Camera.main.transform.position);
        }

        if (CameraShake.Instance != null)
        {
            CameraShake.Instance.ShakeMedium();
        }

        if (SFXManager.Instance != null)
        {
            SFXManager.Instance.PlayWin();
        }

        HapticFeedback.Success();
    }

    public void ResetLevel()
    {
        IsComplete = false;
        earnedStars = 0;
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
