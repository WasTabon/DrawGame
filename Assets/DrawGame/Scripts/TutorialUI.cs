using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class TutorialUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup panelGroup;
    [SerializeField] private RectTransform slideContainer;
    [SerializeField] private Button nextButton;
    [SerializeField] private TextMeshProUGUI nextButtonText;
    [SerializeField] private TextMeshProUGUI pageIndicator;

    private const string KEY_TUTORIAL_SHOWN = "TutorialShown";

    public event Action OnTutorialComplete;

    private int currentSlide;
    private int totalSlides;
    private RectTransform[] slides;
    private bool isAnimating;

    public static bool HasBeenShown()
    {
        return PlayerPrefs.GetInt(KEY_TUTORIAL_SHOWN, 0) == 1;
    }

    public void Show()
    {
        gameObject.SetActive(true);
        panelGroup.alpha = 0f;
        panelGroup.interactable = true;
        panelGroup.blocksRaycasts = true;
        panelGroup.DOFade(1f, 0.4f).SetEase(Ease.OutQuad);

        currentSlide = 0;
        CollectSlides();
        ShowSlide(0, false);
        UpdatePageIndicator();
        UpdateButtonText();
    }

    public void Hide()
    {
        panelGroup.DOFade(0f, 0.3f).SetEase(Ease.InQuad).OnComplete(() =>
        {
            panelGroup.interactable = false;
            panelGroup.blocksRaycasts = false;
            gameObject.SetActive(false);
        });

        PlayerPrefs.SetInt(KEY_TUTORIAL_SHOWN, 1);
        PlayerPrefs.Save();
    }

    private void Start()
    {
        Debug.Assert(panelGroup != null, "TutorialUI: panelGroup not assigned!");
        Debug.Assert(slideContainer != null, "TutorialUI: slideContainer not assigned!");
        Debug.Assert(nextButton != null, "TutorialUI: nextButton not assigned!");
        Debug.Assert(nextButtonText != null, "TutorialUI: nextButtonText not assigned!");
        Debug.Assert(pageIndicator != null, "TutorialUI: pageIndicator not assigned!");

        nextButton.onClick.AddListener(OnNextClicked);
    }

    private void CollectSlides()
    {
        totalSlides = slideContainer.childCount;
        slides = new RectTransform[totalSlides];
        for (int i = 0; i < totalSlides; i++)
        {
            slides[i] = slideContainer.GetChild(i).GetComponent<RectTransform>();
        }
    }

    private void ShowSlide(int index, bool animate)
    {
        for (int i = 0; i < totalSlides; i++)
        {
            if (i == index)
            {
                slides[i].gameObject.SetActive(true);
                if (animate)
                {
                    var cg = slides[i].GetComponent<CanvasGroup>();
                    if (cg != null)
                    {
                        cg.alpha = 0f;
                        cg.DOFade(1f, 0.3f).SetEase(Ease.OutQuad);
                    }

                    slides[i].anchoredPosition = new Vector2(80f, 0f);
                    slides[i].DOAnchorPos(Vector2.zero, 0.35f).SetEase(Ease.OutCubic);
                }
                else
                {
                    slides[i].anchoredPosition = Vector2.zero;
                    var cg = slides[i].GetComponent<CanvasGroup>();
                    if (cg != null) cg.alpha = 1f;
                }
            }
            else
            {
                slides[i].gameObject.SetActive(false);
            }
        }
    }

    private void OnNextClicked()
    {
        if (isAnimating) return;

        if (SFXManager.Instance != null)
        {
            SFXManager.Instance.PlayClick();
        }

        if (currentSlide >= totalSlides - 1)
        {
            Hide();
            OnTutorialComplete?.Invoke();
            return;
        }

        isAnimating = true;
        currentSlide++;
        ShowSlide(currentSlide, true);
        UpdatePageIndicator();
        UpdateButtonText();

        nextButton.transform.DOScale(0.9f, 0.06f).SetEase(Ease.InQuad).OnComplete(() =>
        {
            nextButton.transform.DOScale(1f, 0.06f).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                isAnimating = false;
            });
        });
    }

    private void UpdatePageIndicator()
    {
        string dots = "";
        for (int i = 0; i < totalSlides; i++)
        {
            dots += i == currentSlide ? "●" : "○";
            if (i < totalSlides - 1) dots += "  ";
        }
        pageIndicator.text = dots;
    }

    private void UpdateButtonText()
    {
        nextButtonText.text = currentSlide >= totalSlides - 1 ? "LET'S PLAY!" : "NEXT";
    }
}
