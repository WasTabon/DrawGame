using UnityEngine;
using UnityEngine.UI;

public class LevelSelectUI : MonoBehaviour
{
    [SerializeField] private Transform contentParent;
    [SerializeField] private GameObject levelButtonPrefab;
    [SerializeField] private Button backButton;
    [SerializeField] private MainMenuUI mainMenuUI;

    private LevelButton[] levelButtons;

    private void Start()
    {
        Debug.Assert(contentParent != null, "LevelSelectUI: contentParent not assigned!");
        Debug.Assert(levelButtonPrefab != null, "LevelSelectUI: levelButtonPrefab not assigned!");
        Debug.Assert(backButton != null, "LevelSelectUI: backButton not assigned!");
        Debug.Assert(mainMenuUI != null, "LevelSelectUI: mainMenuUI not assigned!");

        backButton.onClick.AddListener(OnBackClicked);
        CreateButtons();
        RefreshButtons();
    }

    private void OnEnable()
    {
        RefreshButtons();
    }

    private void CreateButtons()
    {
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        levelButtons = new LevelButton[GameManager.TOTAL_LEVELS];
        for (int i = 0; i < GameManager.TOTAL_LEVELS; i++)
        {
            var go = Instantiate(levelButtonPrefab, contentParent);
            var lb = go.GetComponent<LevelButton>();
            Debug.Assert(lb != null, "LevelButton component missing on prefab!");
            lb.Setup(i + 1);
            levelButtons[i] = lb;
        }
    }

    private void RefreshButtons()
    {
        if (levelButtons == null) return;

        int maxUnlocked = GameManager.Instance != null ? GameManager.Instance.GetMaxUnlockedLevel() : 1;

        for (int i = 0; i < levelButtons.Length; i++)
        {
            int level = i + 1;
            bool unlocked = level <= maxUnlocked;
            int stars = GameManager.Instance != null ? GameManager.Instance.GetStars(level) : 0;
            levelButtons[i].UpdateState(unlocked, stars);
        }
    }

    private void OnBackClicked()
    {
        mainMenuUI.SwitchToMainMenu();
    }
}