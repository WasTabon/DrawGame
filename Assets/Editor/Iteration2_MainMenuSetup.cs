using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using UnityEditor.SceneManagement;

public class Iteration2_MainMenuSetup
{
    [MenuItem("DrawGame/Setup MainMenu Scene (Iteration 2)")]
    public static void SetupMainMenuScene()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            return;

        var scene = EditorSceneManager.GetActiveScene();
        if (scene.name != "MainMenu")
        {
            if (!EditorUtility.DisplayDialog("Setup MainMenu",
                "Current scene is '" + scene.name + "'. Are you on the MainMenu scene?",
                "Yes, continue", "Cancel"))
                return;
        }

        SetupCamera();
        SetupEventSystem();
        var prefab = CreateOrGetLevelButtonPrefab();
        SetupMainCanvas(prefab);

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        Debug.Log("MainMenu scene setup complete!");
    }

    [MenuItem("DrawGame/Add GameManager to Bootstrap (Iteration 2)")]
    public static void AddGameManagerToBootstrap()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            return;

        var scene = EditorSceneManager.GetActiveScene();
        if (scene.name != "Bootstrap")
        {
            if (!EditorUtility.DisplayDialog("Add GameManager",
                "Current scene is '" + scene.name + "'. Open Bootstrap scene first.",
                "OK", ""))
                return;
            return;
        }

        var existing = Object.FindObjectOfType<GameManager>();
        if (existing != null)
        {
            Debug.Log("GameManager already exists on Bootstrap scene.");
            return;
        }

        var go = new GameObject("GameManager");
        go.AddComponent<GameManager>();

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("GameManager added to Bootstrap scene!");
    }

    private static void SetupCamera()
    {
        var cam = Camera.main;
        if (cam == null)
        {
            var camGo = new GameObject("Main Camera");
            cam = camGo.AddComponent<Camera>();
            camGo.tag = "MainCamera";
        }
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.12f, 0.12f, 0.18f, 1f);
        cam.orthographic = true;
        cam.orthographicSize = 5f;
    }

    private static void SetupEventSystem()
    {
        var es = Object.FindObjectOfType<UnityEngine.EventSystems.EventSystem>();
        if (es == null)
        {
            var esGo = new GameObject("EventSystem");
            esGo.AddComponent<UnityEngine.EventSystems.EventSystem>();
            esGo.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }
    }

    private static GameObject CreateOrGetLevelButtonPrefab()
    {
        string prefabPath = "Assets/DrawGame/Prefabs/LevelButtonPrefab.prefab";

        var existing = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (existing != null)
        {
            Debug.Log("LevelButtonPrefab already exists, skipping creation.");
            return existing;
        }

        System.IO.Directory.CreateDirectory("Assets/DrawGame/Prefabs");

        var btnGo = new GameObject("LevelButton");
        var btnRect = btnGo.AddComponent<RectTransform>();
        btnRect.sizeDelta = new Vector2(200f, 220f);

        var bgImg = btnGo.AddComponent<Image>();
        bgImg.color = new Color(0.3f, 0.7f, 1f, 1f);

        var btn = btnGo.AddComponent<Button>();
        btn.targetGraphic = bgImg;

        var colors = btn.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(0.9f, 0.9f, 0.9f, 1f);
        colors.pressedColor = new Color(0.7f, 0.7f, 0.7f, 1f);
        colors.disabledColor = new Color(0.5f, 0.5f, 0.5f, 1f);
        btn.colors = colors;

        var numberGo = new GameObject("LevelNumber");
        numberGo.transform.SetParent(btnGo.transform, false);
        var numberTmp = numberGo.AddComponent<TextMeshProUGUI>();
        numberTmp.text = "1";
        numberTmp.fontSize = 48;
        numberTmp.fontStyle = FontStyles.Bold;
        numberTmp.alignment = TextAlignmentOptions.Center;
        numberTmp.color = Color.white;
        var numberRect = numberGo.GetComponent<RectTransform>();
        numberRect.anchorMin = new Vector2(0f, 0.3f);
        numberRect.anchorMax = new Vector2(1f, 0.9f);
        numberRect.offsetMin = Vector2.zero;
        numberRect.offsetMax = Vector2.zero;

        var lockGo = new GameObject("LockIcon");
        lockGo.transform.SetParent(btnGo.transform, false);
        var lockTmp = lockGo.AddComponent<TextMeshProUGUI>();
        lockTmp.text = "\U0001F512";
        lockTmp.fontSize = 40;
        lockTmp.alignment = TextAlignmentOptions.Center;
        lockTmp.color = new Color(0.6f, 0.6f, 0.6f, 1f);
        var lockRect = lockGo.GetComponent<RectTransform>();
        lockRect.anchorMin = new Vector2(0f, 0.2f);
        lockRect.anchorMax = new Vector2(1f, 0.85f);
        lockRect.offsetMin = Vector2.zero;
        lockRect.offsetMax = Vector2.zero;
        lockGo.SetActive(false);

        var starsContainer = new GameObject("Stars");
        starsContainer.transform.SetParent(btnGo.transform, false);
        var starsRect = starsContainer.AddComponent<RectTransform>();
        starsRect.anchorMin = new Vector2(0f, 0f);
        starsRect.anchorMax = new Vector2(1f, 0.3f);
        starsRect.offsetMin = Vector2.zero;
        starsRect.offsetMax = Vector2.zero;

        var starsLayout = starsContainer.AddComponent<HorizontalLayoutGroup>();
        starsLayout.childAlignment = TextAnchor.MiddleCenter;
        starsLayout.spacing = 2f;
        starsLayout.childControlWidth = true;
        starsLayout.childControlHeight = true;
        starsLayout.childForceExpandWidth = false;
        starsLayout.childForceExpandHeight = false;

        GameObject[] starObjs = new GameObject[3];
        for (int i = 0; i < 3; i++)
        {
            var starGo = new GameObject("Star" + (i + 1));
            starGo.transform.SetParent(starsContainer.transform, false);
            var starTmp = starGo.AddComponent<TextMeshProUGUI>();
            starTmp.text = "\u2605";
            starTmp.fontSize = 28;
            starTmp.alignment = TextAlignmentOptions.Center;
            starTmp.color = new Color(1f, 0.85f, 0.2f, 1f);
            var le = starGo.AddComponent<LayoutElement>();
            le.preferredWidth = 35f;
            le.preferredHeight = 35f;
            starGo.SetActive(false);
            starObjs[i] = starGo;
        }

        var levelButton = btnGo.AddComponent<LevelButton>();
        var so = new SerializedObject(levelButton);
        so.FindProperty("button").objectReferenceValue = btn;
        so.FindProperty("levelNumberText").objectReferenceValue = numberTmp;
        so.FindProperty("lockIcon").objectReferenceValue = lockGo;
        so.FindProperty("backgroundImage").objectReferenceValue = bgImg;

        var starsProp = so.FindProperty("starIcons");
        starsProp.arraySize = 3;
        for (int i = 0; i < 3; i++)
        {
            starsProp.GetArrayElementAtIndex(i).objectReferenceValue = starObjs[i];
        }
        so.ApplyModifiedProperties();

        var prefab = PrefabUtility.SaveAsPrefabAsset(btnGo, prefabPath);
        Object.DestroyImmediate(btnGo);

        Debug.Log("LevelButtonPrefab created at: " + prefabPath);
        return prefab;
    }

    private static void SetupMainCanvas(GameObject levelButtonPrefab)
    {
        var existingMainMenuUI = Object.FindObjectOfType<MainMenuUI>();
        if (existingMainMenuUI != null)
        {
            RewireMainMenu(existingMainMenuUI, levelButtonPrefab);
            return;
        }

        var canvasGo = new GameObject("MainCanvas");
        var canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 0;
        var scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        scaler.matchWidthOrHeight = 0.5f;
        canvasGo.AddComponent<GraphicRaycaster>();

        var mainMenuPanel = CreateMainMenuPanel(canvasGo.transform);
        var levelSelectPanel = CreateLevelSelectPanel(canvasGo.transform, levelButtonPrefab);

        var mainMenuUI = canvasGo.AddComponent<MainMenuUI>();
        var so = new SerializedObject(mainMenuUI);
        so.FindProperty("mainMenuPanel").objectReferenceValue = mainMenuPanel.GetComponent<CanvasGroup>();
        so.FindProperty("levelSelectPanel").objectReferenceValue = levelSelectPanel.GetComponent<CanvasGroup>();

        var playBtn = mainMenuPanel.transform.Find("PlayButton");
        Debug.Assert(playBtn != null, "PlayButton not found!");
        so.FindProperty("playButton").objectReferenceValue = playBtn.GetComponent<Button>();
        so.ApplyModifiedProperties();
    }

    private static void RewireMainMenu(MainMenuUI mainMenuUI, GameObject levelButtonPrefab)
    {
        var canvasGo = mainMenuUI.gameObject;
        var so = new SerializedObject(mainMenuUI);

        var mainPanel = canvasGo.transform.Find("MainMenuPanel");
        var levelPanel = canvasGo.transform.Find("LevelSelectPanel");

        if (mainPanel != null)
            so.FindProperty("mainMenuPanel").objectReferenceValue = mainPanel.GetComponent<CanvasGroup>();
        if (levelPanel != null)
            so.FindProperty("levelSelectPanel").objectReferenceValue = levelPanel.GetComponent<CanvasGroup>();

        if (mainPanel != null)
        {
            var playBtn = mainPanel.Find("PlayButton");
            if (playBtn != null)
                so.FindProperty("playButton").objectReferenceValue = playBtn.GetComponent<Button>();
        }
        so.ApplyModifiedProperties();

        var levelSelectUI = levelPanel != null ? levelPanel.GetComponent<LevelSelectUI>() : null;
        if (levelSelectUI != null)
        {
            var lso = new SerializedObject(levelSelectUI);
            var content = levelPanel.Find("Scroll View/Viewport/Content");
            if (content != null)
                lso.FindProperty("contentParent").objectReferenceValue = content;
            lso.FindProperty("levelButtonPrefab").objectReferenceValue = levelButtonPrefab;
            var backBtn = levelPanel.Find("BackButton");
            if (backBtn != null)
                lso.FindProperty("backButton").objectReferenceValue = backBtn.GetComponent<Button>();
            lso.FindProperty("mainMenuUI").objectReferenceValue = mainMenuUI;
            lso.ApplyModifiedProperties();
        }

        Debug.Log("MainMenu references rewired.");
    }

    private static GameObject CreateMainMenuPanel(Transform parent)
    {
        var panelGo = new GameObject("MainMenuPanel");
        panelGo.transform.SetParent(parent, false);
        var panelRect = panelGo.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;
        panelGo.AddComponent<CanvasGroup>();

        var titleGo = new GameObject("GameTitle");
        titleGo.transform.SetParent(panelGo.transform, false);
        var titleTmp = titleGo.AddComponent<TextMeshProUGUI>();
        titleTmp.text = "DRAW GAME";
        titleTmp.fontSize = 80;
        titleTmp.fontStyle = FontStyles.Bold;
        titleTmp.alignment = TextAlignmentOptions.Center;
        titleTmp.color = Color.white;
        var titleRect = titleGo.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0f, 0.55f);
        titleRect.anchorMax = new Vector2(1f, 0.75f);
        titleRect.offsetMin = new Vector2(40f, 0f);
        titleRect.offsetMax = new Vector2(-40f, 0f);

        var playGo = new GameObject("PlayButton");
        playGo.transform.SetParent(panelGo.transform, false);
        var playRect = playGo.AddComponent<RectTransform>();
        playRect.anchorMin = new Vector2(0.25f, 0.35f);
        playRect.anchorMax = new Vector2(0.75f, 0.42f);
        playRect.offsetMin = Vector2.zero;
        playRect.offsetMax = Vector2.zero;

        var playImg = playGo.AddComponent<Image>();
        playImg.color = new Color(0.3f, 0.7f, 1f, 1f);
        var playBtn = playGo.AddComponent<Button>();
        playBtn.targetGraphic = playImg;

        var playTextGo = new GameObject("Text");
        playTextGo.transform.SetParent(playGo.transform, false);
        var playTmp = playTextGo.AddComponent<TextMeshProUGUI>();
        playTmp.text = "PLAY";
        playTmp.fontSize = 48;
        playTmp.fontStyle = FontStyles.Bold;
        playTmp.alignment = TextAlignmentOptions.Center;
        playTmp.color = Color.white;
        var playTextRect = playTextGo.GetComponent<RectTransform>();
        playTextRect.anchorMin = Vector2.zero;
        playTextRect.anchorMax = Vector2.one;
        playTextRect.offsetMin = Vector2.zero;
        playTextRect.offsetMax = Vector2.zero;

        return panelGo;
    }

    private static GameObject CreateLevelSelectPanel(Transform parent, GameObject levelButtonPrefab)
    {
        var panelGo = new GameObject("LevelSelectPanel");
        panelGo.transform.SetParent(parent, false);
        var panelRect = panelGo.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;
        var panelCg = panelGo.AddComponent<CanvasGroup>();
        panelCg.alpha = 0f;
        panelCg.interactable = false;
        panelCg.blocksRaycasts = false;

        var headerGo = new GameObject("Header");
        headerGo.transform.SetParent(panelGo.transform, false);
        var headerRect = headerGo.AddComponent<RectTransform>();
        headerRect.anchorMin = new Vector2(0f, 0.9f);
        headerRect.anchorMax = new Vector2(1f, 1f);
        headerRect.offsetMin = Vector2.zero;
        headerRect.offsetMax = Vector2.zero;

        var headerBg = headerGo.AddComponent<Image>();
        headerBg.color = new Color(0.15f, 0.15f, 0.2f, 1f);

        var titleGo = new GameObject("Title");
        titleGo.transform.SetParent(headerGo.transform, false);
        var titleTmp = titleGo.AddComponent<TextMeshProUGUI>();
        titleTmp.text = "SELECT LEVEL";
        titleTmp.fontSize = 42;
        titleTmp.fontStyle = FontStyles.Bold;
        titleTmp.alignment = TextAlignmentOptions.Center;
        titleTmp.color = Color.white;
        var titleRect = titleGo.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.2f, 0f);
        titleRect.anchorMax = new Vector2(0.8f, 1f);
        titleRect.offsetMin = Vector2.zero;
        titleRect.offsetMax = Vector2.zero;

        var backGo = new GameObject("BackButton");
        backGo.transform.SetParent(headerGo.transform, false);
        var backRect = backGo.AddComponent<RectTransform>();
        backRect.anchorMin = new Vector2(0f, 0.1f);
        backRect.anchorMax = new Vector2(0.15f, 0.9f);
        backRect.offsetMin = new Vector2(10f, 0f);
        backRect.offsetMax = new Vector2(0f, 0f);

        var backImg = backGo.AddComponent<Image>();
        backImg.color = new Color(0.25f, 0.25f, 0.3f, 1f);
        var backBtn = backGo.AddComponent<Button>();
        backBtn.targetGraphic = backImg;

        var backTextGo = new GameObject("Text");
        backTextGo.transform.SetParent(backGo.transform, false);
        var backTmp = backTextGo.AddComponent<TextMeshProUGUI>();
        backTmp.text = "<";
        backTmp.fontSize = 36;
        backTmp.fontStyle = FontStyles.Bold;
        backTmp.alignment = TextAlignmentOptions.Center;
        backTmp.color = Color.white;
        var backTextRect = backTextGo.GetComponent<RectTransform>();
        backTextRect.anchorMin = Vector2.zero;
        backTextRect.anchorMax = Vector2.one;
        backTextRect.offsetMin = Vector2.zero;
        backTextRect.offsetMax = Vector2.zero;

        var scrollGo = CreateScrollView(panelGo.transform);

        var levelSelectUI = panelGo.AddComponent<LevelSelectUI>();
        var so = new SerializedObject(levelSelectUI);

        var content = scrollGo.transform.Find("Viewport/Content");
        Debug.Assert(content != null, "Content not found in ScrollView!");
        so.FindProperty("contentParent").objectReferenceValue = content;
        so.FindProperty("levelButtonPrefab").objectReferenceValue = levelButtonPrefab;
        so.FindProperty("backButton").objectReferenceValue = backBtn;

        var mainMenuUI = parent.GetComponent<MainMenuUI>();
        if (mainMenuUI == null)
            mainMenuUI = parent.gameObject.AddComponent<MainMenuUI>();
        so.FindProperty("mainMenuUI").objectReferenceValue = mainMenuUI;
        so.ApplyModifiedProperties();

        return panelGo;
    }

    private static GameObject CreateScrollView(Transform parent)
    {
        var scrollGo = new GameObject("Scroll View");
        scrollGo.transform.SetParent(parent, false);
        var scrollRect = scrollGo.AddComponent<RectTransform>();
        scrollRect.anchorMin = new Vector2(0f, 0f);
        scrollRect.anchorMax = new Vector2(1f, 0.9f);
        scrollRect.offsetMin = new Vector2(20f, 20f);
        scrollRect.offsetMax = new Vector2(-20f, 0f);

        var scrollImage = scrollGo.AddComponent<Image>();
        scrollImage.color = new Color(0f, 0f, 0f, 0.01f);

        var sr = scrollGo.AddComponent<ScrollRect>();
        sr.horizontal = false;
        sr.vertical = true;
        sr.movementType = ScrollRect.MovementType.Elastic;
        sr.elasticity = 0.1f;
        sr.decelerationRate = 0.135f;
        sr.scrollSensitivity = 30f;

        var viewportGo = new GameObject("Viewport");
        viewportGo.transform.SetParent(scrollGo.transform, false);
        var viewportRect = viewportGo.AddComponent<RectTransform>();
        viewportRect.anchorMin = Vector2.zero;
        viewportRect.anchorMax = Vector2.one;
        viewportRect.offsetMin = Vector2.zero;
        viewportRect.offsetMax = Vector2.zero;

        var viewportImg = viewportGo.AddComponent<Image>();
        viewportImg.color = Color.white;
        var mask = viewportGo.AddComponent<Mask>();
        mask.showMaskGraphic = false;

        var contentGo = new GameObject("Content");
        contentGo.transform.SetParent(viewportGo.transform, false);
        var contentRect = contentGo.AddComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0f, 1f);
        contentRect.anchorMax = new Vector2(1f, 1f);
        contentRect.pivot = new Vector2(0.5f, 1f);
        contentRect.offsetMin = Vector2.zero;
        contentRect.offsetMax = Vector2.zero;

        var gridLayout = contentGo.AddComponent<GridLayoutGroup>();
        gridLayout.cellSize = new Vector2(200f, 220f);
        gridLayout.spacing = new Vector2(25f, 25f);
        gridLayout.padding = new RectOffset(30, 30, 20, 20);
        gridLayout.childAlignment = TextAnchor.UpperCenter;
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = 4;

        var csf = contentGo.AddComponent<ContentSizeFitter>();
        csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        sr.viewport = viewportRect;
        sr.content = contentRect;

        return scrollGo;
    }
}
