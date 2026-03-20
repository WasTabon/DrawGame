using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition Instance { get; private set; }

    [SerializeField] private CanvasGroup fadeCanvasGroup;
    [SerializeField] private float fadeDuration = 0.4f;

    private bool isTransitioning;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        Debug.Assert(fadeCanvasGroup != null, "SceneTransition: fadeCanvasGroup is not assigned!");
    }

    public void LoadScene(string sceneName, Action onComplete = null)
    {
        if (isTransitioning) return;
        StartCoroutine(TransitionCoroutine(sceneName, onComplete));
    }

    private IEnumerator TransitionCoroutine(string sceneName, Action onComplete)
    {
        isTransitioning = true;

        fadeCanvasGroup.blocksRaycasts = true;
        yield return fadeCanvasGroup.DOFade(1f, fadeDuration).SetEase(Ease.InOutQuad).WaitForCompletion();

        var asyncOp = SceneManager.LoadSceneAsync(sceneName);
        yield return asyncOp;

        yield return fadeCanvasGroup.DOFade(0f, fadeDuration).SetEase(Ease.InOutQuad).WaitForCompletion();
        fadeCanvasGroup.blocksRaycasts = false;

        isTransitioning = false;
        onComplete?.Invoke();
    }

    public void FadeFromBlack(Action onComplete = null)
    {
        fadeCanvasGroup.alpha = 1f;
        fadeCanvasGroup.blocksRaycasts = true;
        fadeCanvasGroup.DOFade(0f, fadeDuration).SetEase(Ease.InOutQuad).OnComplete(() =>
        {
            fadeCanvasGroup.blocksRaycasts = false;
            onComplete?.Invoke();
        });
    }
}
