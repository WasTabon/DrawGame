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
    [SerializeField] private CanvasGroup levelCompletePanel;
    [SerializeField] private TextMeshProUGUI levelCompleteText;

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
        Debug.Assert(levelCompletePanel != null, "GameUI: levelCompletePanel not assigned!");
        Debug.Assert(levelCompleteText != null, "GameUI: levelCompleteText not assigned!");

        backButton.onClick.AddListener(OnBackClicked);
        restartButton.onClick.AddListener(OnRestartClicked);

        if (GameManager.Instance != null)
        {
            levelText.text = "Level " + GameManager.Instance.SelectedLevel;
        }

        HideLevelCompleteImmediate();
        SubscribeToEvents();
        UpdateLineCount(0, DrawingManager.Instance != null ? DrawingManager.Instance.MaxLines : 5);
    }

    private void SubscribeToEvents()
    {
        if (DrawingManager.Instance != null)
        {
            DrawingManager.Instance.OnLineCountChanged -= UpdateLineCount;
            DrawingManager.Instance.OnLineCountChanged += UpdateLineCount;
        }

        if (LevelController.Instance != null)
        {
            LevelController.Instance.OnLevelComplete -= HandleLevelComplete;
            LevelController.Instance.OnLevelComplete += HandleLevelComplete;

            LevelController.Instance.OnLevelReset -= HandleLevelReset;
            LevelController.Instance.OnLevelReset += HandleLevelReset;
        }
    }

    private void UnsubscribeFromEvents()
    {
        if (DrawingManager.Instance != null)
        {
            DrawingManager.Instance.OnLineCountChanged -= UpdateLineCount;
        }

        if (LevelController.Instance != null)
        {
            LevelController.Instance.OnLevelComplete -= HandleLevelComplete;
            LevelController.Instance.OnLevelReset -= HandleLevelReset;
        }
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

    private void HandleLevelComplete()
    {
        ShowLevelComplete();
    }

    private void HandleLevelReset()
    {
        HideLevelCompleteImmediate();
    }

    private void ShowLevelComplete()
    {
        levelCompleteText.text = "LEVEL COMPLETE!";
        levelCompletePanel.gameObject.SetActive(true);
        levelCompletePanel.alpha = 0f;
        levelCompletePanel.DOFade(1f, 0.4f).SetEase(Ease.OutQuad);
        levelCompletePanel.interactable = true;
        levelCompletePanel.blocksRaycasts = true;

        var textRect = levelCompleteText.GetComponent<RectTransform>();
        textRect.localScale = Vector3.one * 0.5f;
        textRect.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
    }

    private void HideLevelCompleteImmediate()
    {
        levelCompletePanel.alpha = 0f;
        levelCompletePanel.interactable = false;
        levelCompletePanel.blocksRaycasts = false;
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

        if (LevelController.Instance != null)
        {
            LevelController.Instance.ResetLevel();
        }
        else if (DrawingManager.Instance != null)
        {
            DrawingManager.Instance.ClearAllLines();
        }
        else
        {
            Debug.LogWarning("GameUI: No LevelController or DrawingManager found!");
        }
    }
}
