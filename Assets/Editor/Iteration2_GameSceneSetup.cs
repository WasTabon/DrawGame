using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using UnityEditor.SceneManagement;

public class Iteration2_GameSceneSetup
{
    [MenuItem("DrawGame/Setup Game Scene (Iteration 2)")]
    public static void SetupGameScene()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            return;

        var scene = EditorSceneManager.GetActiveScene();
        if (scene.name != "Game")
        {
            if (!EditorUtility.DisplayDialog("Setup Game Scene",
                "Current scene is '" + scene.name + "'. Are you on the Game scene?",
                "Yes, continue", "Cancel"))
                return;
        }

        SetupCamera();
        SetupEventSystem();
        SetupGameCanvas();

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        Debug.Log("Game scene setup complete!");
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
        cam.backgroundColor = new Color(0.95f, 0.95f, 0.92f, 1f);
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

    private static void SetupGameCanvas()
    {
        var existingUI = Object.FindObjectOfType<GameUI>();
        if (existingUI != null)
        {
            RewireGameUI(existingUI);
            return;
        }

        var canvasGo = new GameObject("GameCanvas");
        var canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10;
        var scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        scaler.matchWidthOrHeight = 0.5f;
        canvasGo.AddComponent<GraphicRaycaster>();

        var topBar = new GameObject("TopBar");
        topBar.transform.SetParent(canvasGo.transform, false);
        var topBarRect = topBar.AddComponent<RectTransform>();
        topBarRect.anchorMin = new Vector2(0f, 0.92f);
        topBarRect.anchorMax = new Vector2(1f, 1f);
        topBarRect.offsetMin = Vector2.zero;
        topBarRect.offsetMax = Vector2.zero;

        var topBarBg = topBar.AddComponent<Image>();
        topBarBg.color = new Color(0.15f, 0.15f, 0.2f, 0.9f);

        var backGo = new GameObject("BackButton");
        backGo.transform.SetParent(topBar.transform, false);
        var backRect = backGo.AddComponent<RectTransform>();
        backRect.anchorMin = new Vector2(0f, 0.1f);
        backRect.anchorMax = new Vector2(0.12f, 0.9f);
        backRect.offsetMin = new Vector2(15f, 0f);
        backRect.offsetMax = new Vector2(0f, 0f);

        var backImg = backGo.AddComponent<Image>();
        backImg.color = new Color(0.25f, 0.25f, 0.3f, 1f);
        var backBtn = backGo.AddComponent<Button>();
        backBtn.targetGraphic = backImg;

        var backTextGo = new GameObject("Text");
        backTextGo.transform.SetParent(backGo.transform, false);
        var backTmp = backTextGo.AddComponent<TextMeshProUGUI>();
        backTmp.text = "<";
        backTmp.fontSize = 32;
        backTmp.fontStyle = FontStyles.Bold;
        backTmp.alignment = TextAlignmentOptions.Center;
        backTmp.color = Color.white;
        var backTextRect = backTextGo.GetComponent<RectTransform>();
        backTextRect.anchorMin = Vector2.zero;
        backTextRect.anchorMax = Vector2.one;
        backTextRect.offsetMin = Vector2.zero;
        backTextRect.offsetMax = Vector2.zero;

        var levelTextGo = new GameObject("LevelText");
        levelTextGo.transform.SetParent(topBar.transform, false);
        var levelTmp = levelTextGo.AddComponent<TextMeshProUGUI>();
        levelTmp.text = "Level 1";
        levelTmp.fontSize = 36;
        levelTmp.fontStyle = FontStyles.Bold;
        levelTmp.alignment = TextAlignmentOptions.Center;
        levelTmp.color = Color.white;
        var levelTextRect = levelTextGo.GetComponent<RectTransform>();
        levelTextRect.anchorMin = new Vector2(0.15f, 0f);
        levelTextRect.anchorMax = new Vector2(0.85f, 1f);
        levelTextRect.offsetMin = Vector2.zero;
        levelTextRect.offsetMax = Vector2.zero;

        var gameUI = canvasGo.AddComponent<GameUI>();
        var so = new SerializedObject(gameUI);
        so.FindProperty("backButton").objectReferenceValue = backBtn;
        so.FindProperty("levelText").objectReferenceValue = levelTmp;
        so.ApplyModifiedProperties();
    }

    private static void RewireGameUI(GameUI gameUI)
    {
        var canvasGo = gameUI.gameObject;
        var so = new SerializedObject(gameUI);

        var topBar = canvasGo.transform.Find("TopBar");
        if (topBar != null)
        {
            var backBtn = topBar.Find("BackButton");
            if (backBtn != null)
                so.FindProperty("backButton").objectReferenceValue = backBtn.GetComponent<Button>();

            var levelText = topBar.Find("LevelText");
            if (levelText != null)
                so.FindProperty("levelText").objectReferenceValue = levelText.GetComponent<TextMeshProUGUI>();
        }
        so.ApplyModifiedProperties();
        Debug.Log("GameUI references rewired.");
    }
}
