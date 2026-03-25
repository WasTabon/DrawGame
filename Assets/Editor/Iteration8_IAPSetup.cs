using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using UnityEditor.SceneManagement;

public class Iteration8_IAPSetup
{
    [MenuItem("DrawGame/Update Game Scene - IAP Shop (Iteration 8)")]
    public static void UpdateGameScene()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            return;

        var scene = EditorSceneManager.GetActiveScene();
        if (scene.name != "Game")
        {
            if (!EditorUtility.DisplayDialog("Update Game Scene",
                "Current scene is '" + scene.name + "'. Are you on the Game scene?",
                "Yes, continue", "Cancel"))
                return;
        }

        SetupShopPanel();

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("Game scene updated with IAP Shop panel!");
    }

    private static void SetupShopPanel()
    {
        var gameUI = Object.FindObjectOfType<GameUI>();
        Debug.Assert(gameUI != null, "GameUI not found!");
        var canvasGo = gameUI.gameObject;

        var existing = Object.FindObjectOfType<IAPManager>();
        if (existing != null)
        {
            Debug.Log("IAPManager already exists, rewiring...");
            RewireIAPManager(existing);
            return;
        }

        var shopPanelGo = CreateShopPanel(canvasGo.transform);
        var iapManager = shopPanelGo.AddComponent<IAPManager>();
        WireIAPManager(iapManager, shopPanelGo);
    }

    private static GameObject CreateShopPanel(Transform canvasTransform)
    {
        var existing = canvasTransform.Find("ShopPanel");
        if (existing != null) return existing.gameObject;

        var panelGo = new GameObject("ShopPanel");
        panelGo.transform.SetParent(canvasTransform, false);
        var panelRect = panelGo.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        var dimBg = panelGo.AddComponent<Image>();
        dimBg.color = new Color(0f, 0f, 0f, 0.6f);

        var cg = panelGo.AddComponent<CanvasGroup>();
        cg.alpha = 0f;
        cg.interactable = false;
        cg.blocksRaycasts = false;

        var popupGo = new GameObject("Popup");
        popupGo.transform.SetParent(panelGo.transform, false);
        var popupRect = popupGo.AddComponent<RectTransform>();
        popupRect.anchorMin = new Vector2(0.1f, 0.3f);
        popupRect.anchorMax = new Vector2(0.9f, 0.7f);
        popupRect.offsetMin = Vector2.zero;
        popupRect.offsetMax = Vector2.zero;

        var popupImg = popupGo.AddComponent<Image>();
        popupImg.color = new Color(0.18f, 0.18f, 0.25f, 1f);

        var titleGo = new GameObject("Title");
        titleGo.transform.SetParent(popupGo.transform, false);
        var titleTmp = titleGo.AddComponent<TextMeshProUGUI>();
        titleTmp.text = "NEED MORE HINTS?";
        titleTmp.fontSize = 36;
        titleTmp.fontStyle = FontStyles.Bold;
        titleTmp.alignment = TextAlignmentOptions.Center;
        titleTmp.color = Color.white;
        var titleRt = titleGo.GetComponent<RectTransform>();
        titleRt.anchorMin = new Vector2(0.05f, 0.75f);
        titleRt.anchorMax = new Vector2(0.95f, 0.95f);
        titleRt.offsetMin = Vector2.zero;
        titleRt.offsetMax = Vector2.zero;

        var descGo = new GameObject("Description");
        descGo.transform.SetParent(popupGo.transform, false);
        var descTmp = descGo.AddComponent<TextMeshProUGUI>();
        descTmp.text = "Get 5 extra hints";
        descTmp.fontSize = 26;
        descTmp.alignment = TextAlignmentOptions.Center;
        descTmp.color = new Color(0.7f, 0.7f, 0.7f, 1f);
        var descRt = descGo.GetComponent<RectTransform>();
        descRt.anchorMin = new Vector2(0.1f, 0.6f);
        descRt.anchorMax = new Vector2(0.9f, 0.75f);
        descRt.offsetMin = Vector2.zero;
        descRt.offsetMax = Vector2.zero;

        var buyBtnGo = new GameObject("BuyButton");
        buyBtnGo.transform.SetParent(popupGo.transform, false);
        var buyBtnRect = buyBtnGo.AddComponent<RectTransform>();
        buyBtnRect.anchorMin = new Vector2(0.15f, 0.32f);
        buyBtnRect.anchorMax = new Vector2(0.85f, 0.55f);
        buyBtnRect.offsetMin = Vector2.zero;
        buyBtnRect.offsetMax = Vector2.zero;

        var buyBtnImg = buyBtnGo.AddComponent<Image>();
        buyBtnImg.color = new Color(0.3f, 0.85f, 0.5f, 1f);

        var buyBtn = buyBtnGo.AddComponent<Button>();
        buyBtn.targetGraphic = buyBtnImg;

        var priceGo = new GameObject("PriceText");
        priceGo.transform.SetParent(buyBtnGo.transform, false);
        var priceTmp = priceGo.AddComponent<TextMeshProUGUI>();
        priceTmp.text = "Loading...";
        priceTmp.fontSize = 30;
        priceTmp.fontStyle = FontStyles.Bold;
        priceTmp.alignment = TextAlignmentOptions.Center;
        priceTmp.color = Color.white;
        var priceRt = priceGo.GetComponent<RectTransform>();
        priceRt.anchorMin = Vector2.zero;
        priceRt.anchorMax = Vector2.one;
        priceRt.offsetMin = Vector2.zero;
        priceRt.offsetMax = Vector2.zero;

        var loadingGo = new GameObject("LoadingButton");
        loadingGo.transform.SetParent(popupGo.transform, false);
        var loadingRect = loadingGo.AddComponent<RectTransform>();
        loadingRect.anchorMin = new Vector2(0.15f, 0.32f);
        loadingRect.anchorMax = new Vector2(0.85f, 0.55f);
        loadingRect.offsetMin = Vector2.zero;
        loadingRect.offsetMax = Vector2.zero;

        var loadingImg = loadingGo.AddComponent<Image>();
        loadingImg.color = new Color(0.3f, 0.3f, 0.35f, 0.9f);
        loadingImg.raycastTarget = true;

        var loadingTextGo = new GameObject("Text");
        loadingTextGo.transform.SetParent(loadingGo.transform, false);
        var loadingTmp = loadingTextGo.AddComponent<TextMeshProUGUI>();
        loadingTmp.text = "Processing...";
        loadingTmp.fontSize = 26;
        loadingTmp.alignment = TextAlignmentOptions.Center;
        loadingTmp.color = new Color(0.8f, 0.8f, 0.8f, 1f);
        var loadingTextRt = loadingTextGo.GetComponent<RectTransform>();
        loadingTextRt.anchorMin = Vector2.zero;
        loadingTextRt.anchorMax = Vector2.one;
        loadingTextRt.offsetMin = Vector2.zero;
        loadingTextRt.offsetMax = Vector2.zero;

        loadingGo.SetActive(false);

        var statusGo = new GameObject("StatusText");
        statusGo.transform.SetParent(popupGo.transform, false);
        var statusTmp = statusGo.AddComponent<TextMeshProUGUI>();
        statusTmp.text = "";
        statusTmp.fontSize = 22;
        statusTmp.alignment = TextAlignmentOptions.Center;
        statusTmp.color = Color.white;
        var statusRt = statusGo.GetComponent<RectTransform>();
        statusRt.anchorMin = new Vector2(0.1f, 0.2f);
        statusRt.anchorMax = new Vector2(0.9f, 0.32f);
        statusRt.offsetMin = Vector2.zero;
        statusRt.offsetMax = Vector2.zero;

        var closeBtnGo = new GameObject("CloseButton");
        closeBtnGo.transform.SetParent(popupGo.transform, false);
        var closeBtnRect = closeBtnGo.AddComponent<RectTransform>();
        closeBtnRect.anchorMin = new Vector2(0.3f, 0.05f);
        closeBtnRect.anchorMax = new Vector2(0.7f, 0.18f);
        closeBtnRect.offsetMin = Vector2.zero;
        closeBtnRect.offsetMax = Vector2.zero;

        var closeBtnImg = closeBtnGo.AddComponent<Image>();
        closeBtnImg.color = new Color(0.4f, 0.4f, 0.45f, 1f);

        var closeBtn = closeBtnGo.AddComponent<Button>();
        closeBtn.targetGraphic = closeBtnImg;

        var closeTextGo = new GameObject("Text");
        closeTextGo.transform.SetParent(closeBtnGo.transform, false);
        var closeTmp = closeTextGo.AddComponent<TextMeshProUGUI>();
        closeTmp.text = "CLOSE";
        closeTmp.fontSize = 26;
        closeTmp.fontStyle = FontStyles.Bold;
        closeTmp.alignment = TextAlignmentOptions.Center;
        closeTmp.color = Color.white;
        var closeTextRt = closeTextGo.GetComponent<RectTransform>();
        closeTextRt.anchorMin = Vector2.zero;
        closeTextRt.anchorMax = Vector2.one;
        closeTextRt.offsetMin = Vector2.zero;
        closeTextRt.offsetMax = Vector2.zero;

        return panelGo;
    }

    private static void WireIAPManager(IAPManager iapManager, GameObject shopPanel)
    {
        var so = new SerializedObject(iapManager);

        var popup = shopPanel.transform.Find("Popup");

        so.FindProperty("panelGroup").objectReferenceValue = shopPanel.GetComponent<CanvasGroup>();
        so.FindProperty("panelRect").objectReferenceValue = popup.GetComponent<RectTransform>();

        var loadingBtn = popup.Find("LoadingButton");
        so.FindProperty("loadingButton").objectReferenceValue = loadingBtn.gameObject;

        var priceText = popup.Find("BuyButton/PriceText");
        so.FindProperty("priceText").objectReferenceValue = priceText.GetComponent<TextMeshProUGUI>();

        var statusText = popup.Find("StatusText");
        so.FindProperty("statusText").objectReferenceValue = statusText.GetComponent<TextMeshProUGUI>();

        so.ApplyModifiedProperties();

        var closeBtn = popup.Find("CloseButton");
        var closeBtnComp = closeBtn.GetComponent<Button>();
        if (closeBtnComp.onClick.GetPersistentEventCount() > 0)
            UnityEditor.Events.UnityEventTools.RemovePersistentListener(closeBtnComp.onClick, 0);
        UnityEditor.Events.UnityEventTools.AddPersistentListener(closeBtnComp.onClick, iapManager.Hide);

        var buyBtn = popup.Find("BuyButton");
        var buyBtnComp = buyBtn.GetComponent<Button>();
        if (buyBtnComp.onClick.GetPersistentEventCount() > 0)
            UnityEditor.Events.UnityEventTools.RemovePersistentListener(buyBtnComp.onClick, 0);
        UnityEditor.Events.UnityEventTools.AddPersistentListener(buyBtnComp.onClick, iapManager.OnBuyClicked);

        Debug.Log("IAPManager wired. You need to:\n" +
            "1. Add IAP Button component to BuyButton (or replace it)\n" +
            "2. Set Product ID to: com.drawgame.hints5\n" +
            "3. Wire OnPurchaseComplete, OnPurchaseFailed, OnProductFetched to IAPManager");
    }

    private static void RewireIAPManager(IAPManager iapManager)
    {
        var shopPanel = iapManager.transform.parent != null ? iapManager.transform.parent.gameObject : iapManager.gameObject;
        if (iapManager.GetComponent<CanvasGroup>() != null)
            shopPanel = iapManager.gameObject;
        else
        {
            var cg = iapManager.GetComponentInParent<CanvasGroup>();
            if (cg != null)
                shopPanel = cg.gameObject;
        }

        WireIAPManager(iapManager, shopPanel);
    }
}