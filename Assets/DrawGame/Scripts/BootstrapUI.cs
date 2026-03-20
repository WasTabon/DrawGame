using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class BootstrapUI : MonoBehaviour
{
    [SerializeField] private Slider progressBar;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private Button retryButton;
    [SerializeField] private CanvasGroup retryCanvasGroup;

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
        Debug.Assert(progressBar != null, "BootstrapUI: progressBar not assigned!");
        Debug.Assert(statusText != null, "BootstrapUI: statusText not assigned!");
        Debug.Assert(retryButton != null, "BootstrapUI: retryButton not assigned!");
        Debug.Assert(retryCanvasGroup != null, "BootstrapUI: retryCanvasGroup not assigned!");

        retryButton.onClick.AddListener(OnRetryClicked);
        HideRetryImmediate();
        progressBar.value = 0f;

        Debug.Assert(AddressableLoader.Instance != null, "BootstrapUI: AddressableLoader.Instance is null!");
        SubscribeToEvents();
        AddressableLoader.Instance.StartLoading();
    }

    private void SubscribeToEvents()
    {
        if (AddressableLoader.Instance == null) return;

        AddressableLoader.Instance.OnDownloadProgress -= HandleProgress;
        AddressableLoader.Instance.OnDownloadProgress += HandleProgress;

        AddressableLoader.Instance.OnStatusChanged -= HandleStatus;
        AddressableLoader.Instance.OnStatusChanged += HandleStatus;

        AddressableLoader.Instance.OnDownloadComplete -= HandleComplete;
        AddressableLoader.Instance.OnDownloadComplete += HandleComplete;

        AddressableLoader.Instance.OnDownloadFailed -= HandleFailed;
        AddressableLoader.Instance.OnDownloadFailed += HandleFailed;
    }

    private void UnsubscribeFromEvents()
    {
        if (AddressableLoader.Instance == null) return;

        AddressableLoader.Instance.OnDownloadProgress -= HandleProgress;
        AddressableLoader.Instance.OnStatusChanged -= HandleStatus;
        AddressableLoader.Instance.OnDownloadComplete -= HandleComplete;
        AddressableLoader.Instance.OnDownloadFailed -= HandleFailed;
    }

    private void HandleProgress(float progress)
    {
        progressBar.DOValue(progress, 0.3f).SetEase(Ease.OutQuad);
    }

    private void HandleStatus(string status)
    {
        statusText.text = status;
    }

    private void HandleComplete()
    {
        statusText.text = "Ready!";

        if (AddressableLoader.Instance.LoadedMusicClip != null)
        {
            AudioManager.Instance.PlayMusic(AddressableLoader.Instance.LoadedMusicClip);
        }

        DOVirtual.DelayedCall(0.5f, () =>
        {
            SceneTransition.Instance.LoadScene("MainMenu");
        });
    }

    private void HandleFailed(string error)
    {
        statusText.text = error;
        ShowRetryButton();
    }

    private void ShowRetryButton()
    {
        retryCanvasGroup.DOFade(1f, 0.3f);
        retryCanvasGroup.interactable = true;
        retryCanvasGroup.blocksRaycasts = true;
    }

    private void HideRetryImmediate()
    {
        retryCanvasGroup.alpha = 0f;
        retryCanvasGroup.interactable = false;
        retryCanvasGroup.blocksRaycasts = false;
    }

    private void HideRetryButton()
    {
        retryCanvasGroup.DOFade(0f, 0.3f);
        retryCanvasGroup.interactable = false;
        retryCanvasGroup.blocksRaycasts = false;
    }

    private void OnRetryClicked()
    {
        HideRetryButton();
        progressBar.value = 0f;
        AddressableLoader.Instance.StartLoading();
    }
}
