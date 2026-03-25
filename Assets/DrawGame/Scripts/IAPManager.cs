using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Purchasing;
using TMPro;
using DG.Tweening;

public class IAPManager : MonoBehaviour
{
    public static IAPManager Instance { get; private set; }

    public string productId = "com.drawgame.hints5";

    public GameObject loadingButton;

    [Header("Shop Panel")]
    public CanvasGroup panelGroup;
    public RectTransform panelRect;

    [Header("UI")]
    public TextMeshProUGUI priceText;
    public TextMeshProUGUI statusText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (panelGroup != null)
        {
            panelGroup.alpha = 0f;
            panelGroup.interactable = false;
            panelGroup.blocksRaycasts = false;
        }
    }

    public void Show()
    {
        if (statusText != null)
            statusText.text = "";

        if (loadingButton != null)
            loadingButton.SetActive(false);

        panelGroup.interactable = true;
        panelGroup.blocksRaycasts = true;
        panelRect.localScale = Vector3.one * 0.8f;
        panelGroup.DOFade(1f, 0.25f);
        panelRect.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
    }

    public void Hide()
    {
        panelGroup.DOFade(0f, 0.2f).OnComplete(() =>
        {
            panelGroup.interactable = false;
            panelGroup.blocksRaycasts = false;
        });
    }

    public void OnBuyClicked()
    {
        if (loadingButton != null)
            loadingButton.SetActive(true);
    }

    public void OnPurchaseComplete(Product product)
    {
        if (product.definition.id == productId)
        {
            Debug.Log("[IAP] Purchase complete - adding 5 hints");

            if (HintManager.Instance != null)
            {
                HintManager.Instance.AddPurchasedHints();
            }
            else
            {
                Debug.LogWarning("[IAP] HintManager not found!");
            }

            if (loadingButton != null)
                loadingButton.SetActive(false);

            if (statusText != null)
            {
                statusText.color = new Color(0.25f, 0.82f, 0.50f, 1f);
                statusText.text = "+5 Hints!";
            }

            if (panelRect != null)
                panelRect.DOPunchScale(Vector3.one * 0.1f, 0.3f, 5);
        }
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureDescription description)
    {
        if (product.definition.id == productId)
        {
            Debug.Log("[IAP] Failed: " + description.message);

            if (loadingButton != null)
                loadingButton.SetActive(false);

            if (statusText != null)
            {
                statusText.color = new Color(0.90f, 0.22f, 0.35f, 1f);
                statusText.text = "Purchase failed";
            }
        }
    }

    public void OnProductFetched(Product product)
    {
        Debug.Log("[IAP] Fetched: " + product.metadata.localizedPriceString);
        if (priceText != null)
            priceText.text = product.metadata.localizedPriceString;
    }
}
