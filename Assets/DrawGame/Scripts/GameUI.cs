using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUI : MonoBehaviour
{
    [SerializeField] private Button backButton;
    [SerializeField] private TextMeshProUGUI levelText;

    private void Start()
    {
        Debug.Assert(backButton != null, "GameUI: backButton not assigned!");
        Debug.Assert(levelText != null, "GameUI: levelText not assigned!");

        backButton.onClick.AddListener(OnBackClicked);

        if (GameManager.Instance != null)
        {
            levelText.text = "Level " + GameManager.Instance.SelectedLevel;
        }
    }

    private void OnBackClicked()
    {
        GameManager.Instance.LoadMainMenu();
    }
}
