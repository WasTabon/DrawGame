using System;
using UnityEngine;
using DG.Tweening;

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
    private Tween pulseTween;

    public void Init(GoalType type, float holdTime = 2f)
    {
        goalType = type;
        holdDuration = holdTime;
    }

    private void Start()
    {
        StartPulse();
    }

    private void StartPulse()
    {
        if (pulseTween != null) pulseTween.Kill();
        pulseTween = transform.DOScale(transform.localScale * 1.05f, 0.8f)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    public void ResetGoal()
    {
        isCompleted = false;
        holdTimer = 0f;
        targetInZone = false;
        StartPulse();
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

        if (pulseTween != null) pulseTween.Kill();

        transform.DOScale(transform.localScale * 1.2f, 0.3f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack);
        });

        if (ParticleSpawner.Instance != null)
        {
            ParticleSpawner.Instance.EmitGoalGlow(transform.position);
        }

        OnGoalCompleted?.Invoke();
    }

    private void OnDestroy()
    {
        if (pulseTween != null) pulseTween.Kill();
    }
}
