using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class LevelButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI levelNumberText;
    [SerializeField] private GameObject lockIcon;
    [SerializeField] private GameObject[] starIcons;
    [SerializeField] private Image backgroundImage;

    private int level;
    private bool isUnlocked;

    private Color unlockedColor = new Color(255f, 255f, 255f, 255f);
    private Color lockedColor = new Color(0.3f, 0.3f, 0.35f, 1f);
    private Color completedColor = new Color(255f, 255f, 255f, 255f);

    public void Setup(int levelNumber)
    {
        level = levelNumber;
        levelNumberText.text = level.ToString();
        button.onClick.AddListener(OnClicked);
    }

    public void UpdateState(bool unlocked, int stars)
    {
        isUnlocked = unlocked;

        button.interactable = unlocked;
        lockIcon.SetActive(!unlocked);
        levelNumberText.gameObject.SetActive(unlocked);

        for (int i = 0; i < starIcons.Length; i++)
        {
            starIcons[i].SetActive(unlocked && i < stars);
        }

        if (!unlocked)
            backgroundImage.color = lockedColor;
        else if (stars > 0)
            backgroundImage.color = completedColor;
        else
            backgroundImage.color = unlockedColor;
    }

    private void OnClicked()
    {
        if (!isUnlocked) return;

        if (SFXManager.Instance != null)
        {
            SFXManager.Instance.PlayClick();
        }

        transform.DOScale(0.9f, 0.08f).SetEase(Ease.InQuad).OnComplete(() =>
        {
            transform.DOScale(1f, 0.08f).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                GameManager.Instance.LoadGameScene(level);
            });
        });
    }
}
