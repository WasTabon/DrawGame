using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class GameUI : MonoBehaviour
{
    [SerializeField] private Button backButton;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Button restartButton;
    [SerializeField] private TextMeshProUGUI lineCountText;

    private void OnEnable()
    {
        SubscribeToEvents();
    }

    private void OnDisable()
    {
        UnsubscribeFromEvents();
    }

    private void Start()
    {
        Debug.Assert(backButton != null, "GameUI: backButton not assigned!");
        Debug.Assert(levelText != null, "GameUI: levelText not assigned!");
        Debug.Assert(restartButton != null, "GameUI: restartButton not assigned!");
        Debug.Assert(lineCountText != null, "GameUI: lineCountText not assigned!");

        backButton.onClick.AddListener(OnBackClicked);
        restartButton.onClick.AddListener(OnRestartClicked);

        if (GameManager.Instance != null)
        {
            levelText.text = "Level " + GameManager.Instance.SelectedLevel;
        }

        SubscribeToEvents();
        UpdateLineCount(0, DrawingManager.Instance != null ? DrawingManager.Instance.MaxLines : 5);
    }

    private void SubscribeToEvents()
    {
        if (DrawingManager.Instance == null) return;
        DrawingManager.Instance.OnLineCountChanged -= UpdateLineCount;
        DrawingManager.Instance.OnLineCountChanged += UpdateLineCount;
    }

    private void UnsubscribeFromEvents()
    {
        if (DrawingManager.Instance == null) return;
        DrawingManager.Instance.OnLineCountChanged -= UpdateLineCount;
    }

    private void UpdateLineCount(int current, int max)
    {
        lineCountText.text = current + " / " + max;

        if (current >= max)
        {
            lineCountText.color = new Color(1f, 0.4f, 0.4f, 1f);
            lineCountText.transform.DOShakePosition(0.3f, 5f, 15).SetEase(Ease.OutQuad);
        }
        else
        {
            lineCountText.color = Color.white;
        }
    }

    private void OnBackClicked()
    {
        GameManager.Instance.LoadMainMenu();
    }

    private void OnRestartClicked()
    {
        restartButton.transform.DOScale(0.85f, 0.08f).SetEase(Ease.InQuad).OnComplete(() =>
        {
            restartButton.transform.DOScale(1f, 0.08f).SetEase(Ease.OutQuad);
        });

        if (DrawingManager.Instance != null)
        {
            DrawingManager.Instance.ClearAllLines();
        }
        else
        {
            Debug.LogWarning("GameUI: DrawingManager.Instance is null!");
        }
    }
}
