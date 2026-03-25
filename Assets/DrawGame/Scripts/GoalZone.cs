using System;
using UnityEngine;

public enum GoalType
{
    ReachZone,
    TouchTarget,
    HoldInZone
}

public class GoalZone : MonoBehaviour
{
    [SerializeField] private GoalType goalType = GoalType.ReachZone;
    [SerializeField] private float holdDuration = 2f;

    public event Action OnGoalCompleted;

    private bool isCompleted;
    private float holdTimer;
    private bool targetInZone;

    public void Init(GoalType type, float holdTime = 2f)
    {
        goalType = type;
        holdDuration = holdTime;
    }

    public void ResetGoal()
    {
        isCompleted = false;
        holdTimer = 0f;
        targetInZone = false;
    }

    private void Update()
    {
        if (isCompleted) return;

        if (goalType == GoalType.HoldInZone && targetInZone)
        {
            holdTimer += Time.deltaTime;
            if (holdTimer >= holdDuration)
            {
                CompleteGoal();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isCompleted) return;

        var levelObj = other.GetComponent<LevelObject>();
        if (levelObj != null && levelObj.IsGoalTarget)
        {
            HandleTargetEnter();
            return;
        }

        var drawnLine = other.GetComponent<DrawnLine>();
        if (drawnLine != null && goalType == GoalType.TouchTarget)
        {
            CompleteGoal();
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (isCompleted) return;
        if (goalType != GoalType.HoldInZone) return;

        var levelObj = other.GetComponent<LevelObject>();
        if (levelObj != null && levelObj.IsGoalTarget)
        {
            targetInZone = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (isCompleted) return;

        var levelObj = other.GetComponent<LevelObject>();
        if (levelObj != null && levelObj.IsGoalTarget)
        {
            targetInZone = false;
            holdTimer = 0f;
        }
    }

    private void HandleTargetEnter()
    {
        switch (goalType)
        {
            case GoalType.ReachZone:
                CompleteGoal();
                break;
            case GoalType.TouchTarget:
                CompleteGoal();
                break;
            case GoalType.HoldInZone:
                targetInZone = true;
                holdTimer = 0f;
                break;
        }
    }

    private void CompleteGoal()
    {
        if (isCompleted) return;
        isCompleted = true;
        OnGoalCompleted?.Invoke();
    }
}
