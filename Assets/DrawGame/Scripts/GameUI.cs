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
    [SerializeField] private Button nextLevelButton;
    [SerializeField] private Button winRestartButton;
    [SerializeField] private TextMeshProUGUI[] starTexts;
    [SerializeField] private Button hintButton;
    [SerializeField] private TextMeshProUGUI hintCountText;

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
        Debug.Assert(nextLevelButton != null, "GameUI: nextLevelButton not assigned!");
        Debug.Assert(winRestartButton != null, "GameUI: winRestartButton not assigned!");
        Debug.Assert(starTexts != null && starTexts.Length == 3, "GameUI: starTexts must have 3 elements!");
        Debug.Assert(hintButton != null, "GameUI: hintButton not assigned!");
        Debug.Assert(hintCountText != null, "GameUI: hintCountText not assigned!");

        backButton.onClick.AddListener(OnBackClicked);
        restartButton.onClick.AddListener(OnRestartClicked);
        nextLevelButton.onClick.AddListener(OnNextLevelClicked);
        winRestartButton.onClick.AddListener(OnWinRestartClicked);
        hintButton.onClick.AddListener(OnHintClicked);

        HideLevelCompleteImmediate();
        SubscribeToEvents();
        UpdateLevelText();
        UpdateLineCount(0, DrawingManager.Instance != null ? DrawingManager.Instance.MaxLines : 5);
        UpdateHintCount();
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

        if (HintManager.Instance != null)
        {
            HintManager.Instance.OnHintCountChanged -= HandleHintCountChanged;
            HintManager.Instance.OnHintCountChanged += HandleHintCountChanged;
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

        if (HintManager.Instance != null)
        {
            HintManager.Instance.OnHintCountChanged -= HandleHintCountChanged;
        }
    }

    private void UpdateLevelText()
    {
        if (GameManager.Instance != null)
        {
            levelText.text = "Level " + GameManager.Instance.SelectedLevel;
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

    private void UpdateHintCount()
    {
        int count = HintManager.Instance != null ? HintManager.Instance.HintCount : 0;
        hintCountText.text = count.ToString();
    }

    private void HandleHintCountChanged(int newCount)
    {
        hintCountText.text = newCount.ToString();
    }

    private void HandleLevelComplete(int stars)
    {
        ShowLevelComplete(stars);
    }

    private void HandleLevelReset()
    {
        HideLevelCompleteImmediate();
        UpdateLevelText();
        UpdateLineCount(0, DrawingManager.Instance != null ? DrawingManager.Instance.MaxLines : 5);
    }

    private void ShowLevelComplete(int stars)
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

        AnimateStars(stars);

        nextLevelButton.transform.localScale = Vector3.zero;
        nextLevelButton.transform.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack).SetDelay(0.6f + stars * 0.2f);

        winRestartButton.transform.localScale = Vector3.zero;
        winRestartButton.transform.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack).SetDelay(0.7f + stars * 0.2f);
    }

    private void AnimateStars(int count)
    {
        Color activeColor = new Color(1f, 0.85f, 0.2f, 1f);
        Color inactiveColor = new Color(0.4f, 0.4f, 0.4f, 0.5f);

        for (int i = 0; i < starTexts.Length; i++)
        {
            var star = starTexts[i];
            star.transform.localScale = Vector3.zero;

            bool earned = i < count;
            star.color = earned ? activeColor : inactiveColor;

            float delay = 0.4f + i * 0.2f;
            star.transform.DOScale(earned ? Vector3.one * 1.3f : Vector3.one * 0.8f, 0.35f)
                .SetEase(Ease.OutBack)
                .SetDelay(delay)
                .OnComplete(() =>
                {
                    if (earned)
                    {
                        star.transform.DOScale(Vector3.one, 0.15f).SetEase(Ease.InOutQuad);
                    }
                });
        }
    }

    private void HideLevelCompleteImmediate()
    {
        levelCompletePanel.alpha = 0f;
        levelCompletePanel.interactable = false;
        levelCompletePanel.blocksRaycasts = false;

        if (starTexts != null)
        {
            foreach (var star in starTexts)
            {
                if (star != null)
                    star.transform.localScale = Vector3.zero;
            }
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

    private void OnNextLevelClicked()
    {
        if (LevelController.Instance != null)
        {
            LevelController.Instance.LoadNextLevel();
        }
    }

    private void OnWinRestartClicked()
    {
        if (LevelController.Instance != null)
        {
            LevelController.Instance.ResetLevel();
        }
    }

    private void OnHintClicked()
    {
        if (LevelController.Instance != null && LevelController.Instance.IsComplete) return;

        if (HintManager.Instance == null)
        {
            Debug.LogWarning("GameUI: HintManager not found!");
            return;
        }

        if (HintManager.Instance.HintCount <= 0)
        {
            if (IAPManager.Instance != null)
            {
                IAPManager.Instance.Show();
            }
            else
            {
                hintButton.transform.DOShakePosition(0.3f, 5f, 15).SetEase(Ease.OutQuad);
            }
            return;
        }

        if (LevelSpawner.Instance == null || LevelSpawner.Instance.CurrentHintDisplay == null)
        {
            Debug.LogWarning("GameUI: No hint available for this level!");
            return;
        }

        if (HintManager.Instance.UseHint())
        {
            hintButton.transform.DOScale(0.85f, 0.08f).SetEase(Ease.InQuad).OnComplete(() =>
            {
                hintButton.transform.DOScale(1f, 0.08f).SetEase(Ease.OutQuad);
            });

            LevelSpawner.Instance.CurrentHintDisplay.ShowHint(3f);
        }
    }
}
