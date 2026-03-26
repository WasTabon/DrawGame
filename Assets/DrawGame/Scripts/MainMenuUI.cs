using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup mainMenuPanel;
    [SerializeField] private CanvasGroup levelSelectPanel;
    [SerializeField] private Button playButton;

    private void Start()
    {
        Debug.Assert(mainMenuPanel != null, "MainMenuUI: mainMenuPanel not assigned!");
        Debug.Assert(levelSelectPanel != null, "MainMenuUI: levelSelectPanel not assigned!");
        Debug.Assert(playButton != null, "MainMenuUI: playButton not assigned!");

        playButton.onClick.AddListener(OnPlayClicked);

        mainMenuPanel.alpha = 1f;
        mainMenuPanel.interactable = true;
        mainMenuPanel.blocksRaycasts = true;

        levelSelectPanel.alpha = 0f;
        levelSelectPanel.interactable = false;
        levelSelectPanel.blocksRaycasts = false;

        AnimateEntrance();
    }

    private void AnimateEntrance()
    {
        var titleRect = mainMenuPanel.transform.Find("GameTitle");
        if (titleRect != null)
        {
            var rt = titleRect.GetComponent<RectTransform>();
            Vector2 targetPos = rt.anchoredPosition;
            rt.anchoredPosition = targetPos + new Vector2(0f, 100f);
            rt.DOAnchorPos(targetPos, 0.6f).SetEase(Ease.OutBack);
        }

        var playRect = playButton.GetComponent<RectTransform>();
        playRect.localScale = Vector3.zero;
        playRect.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack).SetDelay(0.3f);
    }

    private void OnPlayClicked()
    {
        playButton.interactable = false;

        if (SFXManager.Instance != null)
        {
            SFXManager.Instance.PlayClick();
        }

        SwitchToLevelSelect();
    }

    private void SwitchToLevelSelect()
    {
        mainMenuPanel.DOFade(0f, 0.3f).SetEase(Ease.InQuad);
        mainMenuPanel.interactable = false;
        mainMenuPanel.blocksRaycasts = false;

        levelSelectPanel.DOFade(1f, 0.3f).SetEase(Ease.OutQuad).SetDelay(0.15f).OnComplete(() =>
        {
            levelSelectPanel.interactable = true;
            levelSelectPanel.blocksRaycasts = true;
        });
    }

    public void SwitchToMainMenu()
    {
        if (SFXManager.Instance != null)
        {
            SFXManager.Instance.PlayClick();
        }

        levelSelectPanel.DOFade(0f, 0.3f).SetEase(Ease.InQuad);
        levelSelectPanel.interactable = false;
        levelSelectPanel.blocksRaycasts = false;

        mainMenuPanel.DOFade(1f, 0.3f).SetEase(Ease.OutQuad).SetDelay(0.15f).OnComplete(() =>
        {
            mainMenuPanel.interactable = true;
            mainMenuPanel.blocksRaycasts = true;
            playButton.interactable = true;
        });
    }
}
