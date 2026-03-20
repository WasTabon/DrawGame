using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using UnityEditor.SceneManagement;

public class Iteration1_BootstrapSetup
{
    [MenuItem("DrawGame/Setup Bootstrap Scene (Iteration 1)")]
    public static void SetupBootstrapScene()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            return;

        var scene = EditorSceneManager.GetActiveScene();
        if (scene.name != "Bootstrap" && scene.name != "Untitled")
        {
            if (!EditorUtility.DisplayDialog("Setup Bootstrap Scene",
                "Current scene is '" + scene.name + "'. This will modify the current scene. Continue?",
                "Yes", "Cancel"))
                return;
        }

        SetupCamera();
        SetupEventSystem();
        var addressableLoader = SetupAddressableLoader();
        var audioManager = SetupAudioManager();
        var sceneTransition = SetupSceneTransition();
        SetupBootstrapCanvas(addressableLoader);

        if (scene.name == "Untitled" || scene.name != "Bootstrap")
        {
            string path = "Assets/Scenes/Bootstrap.unity";
            System.IO.Directory.CreateDirectory("Assets/Scenes");
            EditorSceneManager.SaveScene(scene, path);
            Debug.Log("Scene saved as: " + path);
        }

        AddSceneToBuildSettings("Assets/Scenes/Bootstrap.unity", 0);
        AddSceneToBuildSettings("Assets/Scenes/MainMenu.unity", 1);
        AddSceneToBuildSettings("Assets/Scenes/Game.unity", 2);

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Debug.Log("Bootstrap scene setup complete!");
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
        cam.backgroundColor = new Color(0.1f, 0.1f, 0.15f, 1f);
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

    private static GameObject SetupAddressableLoader()
    {
        var existing = Object.FindObjectOfType<AddressableLoader>();
        if (existing != null) return existing.gameObject;

        var go = new GameObject("AddressableLoader");
        go.AddComponent<AddressableLoader>();
        return go;
    }

    private static GameObject SetupAudioManager()
    {
        var existing = Object.FindObjectOfType<AudioManager>();
        if (existing != null) return existing.gameObject;

        var go = new GameObject("AudioManager");
        go.AddComponent<AudioManager>();
        return go;
    }

    private static GameObject SetupSceneTransition()
    {
        var existing = Object.FindObjectOfType<SceneTransition>();
        if (existing != null)
        {
            var existingCg = existing.transform.GetComponentInChildren<CanvasGroup>();
            if (existingCg != null)
            {
                var so = new SerializedObject(existing);
                so.FindProperty("fadeCanvasGroup").objectReferenceValue = existingCg;
                so.ApplyModifiedProperties();
            }
            return existing.gameObject;
        }

        var go = new GameObject("SceneTransition");
        var st = go.AddComponent<SceneTransition>();

        var canvasGo = new GameObject("TransitionCanvas");
        canvasGo.transform.SetParent(go.transform);
        var canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;
        canvasGo.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        var scaler = canvasGo.GetComponent<CanvasScaler>();
        scaler.referenceResolution = new Vector2(1080, 1920);
        scaler.matchWidthOrHeight = 0.5f;
        canvasGo.AddComponent<GraphicRaycaster>();

        var fadeGo = new GameObject("FadePanel");
        fadeGo.transform.SetParent(canvasGo.transform, false);
        var fadeImg = fadeGo.AddComponent<Image>();
        fadeImg.color = Color.black;
        var fadeRect = fadeGo.GetComponent<RectTransform>();
        fadeRect.anchorMin = Vector2.zero;
        fadeRect.anchorMax = Vector2.one;
        fadeRect.offsetMin = Vector2.zero;
        fadeRect.offsetMax = Vector2.zero;

        var fadeCg = fadeGo.AddComponent<CanvasGroup>();
        fadeCg.alpha = 0f;
        fadeCg.blocksRaycasts = false;

        var so2 = new SerializedObject(st);
        so2.FindProperty("fadeCanvasGroup").objectReferenceValue = fadeCg;
        so2.FindProperty("fadeDuration").floatValue = 0.4f;
        so2.ApplyModifiedProperties();

        return go;
    }

    private static void SetupBootstrapCanvas(GameObject addressableLoaderGo)
    {
        var existingUI = Object.FindObjectOfType<BootstrapUI>();
        if (existingUI != null)
        {
            RewireBootstrapUI(existingUI);
            return;
        }

        var canvasGo = new GameObject("BootstrapCanvas");
        var canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10;
        var scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        scaler.matchWidthOrHeight = 0.5f;
        canvasGo.AddComponent<GraphicRaycaster>();

        var bgGo = new GameObject("Background");
        bgGo.transform.SetParent(canvasGo.transform, false);
        var bgImg = bgGo.AddComponent<Image>();
        bgImg.color = new Color(0.1f, 0.1f, 0.15f, 1f);
        var bgRect = bgGo.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;

        var titleGo = new GameObject("GameTitle");
        titleGo.transform.SetParent(canvasGo.transform, false);
        var titleTmp = titleGo.AddComponent<TextMeshProUGUI>();
        titleTmp.text = "DRAW GAME";
        titleTmp.fontSize = 72;
        titleTmp.fontStyle = FontStyles.Bold;
        titleTmp.alignment = TextAlignmentOptions.Center;
        titleTmp.color = Color.white;
        var titleRect = titleGo.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0f, 0.6f);
        titleRect.anchorMax = new Vector2(1f, 0.8f);
        titleRect.offsetMin = new Vector2(40f, 0f);
        titleRect.offsetMax = new Vector2(-40f, 0f);

        var progressGo = CreateProgressBar(canvasGo.transform);
        var statusGo = CreateStatusText(canvasGo.transform);
        var retryGo = CreateRetryButton(canvasGo.transform);

        var bootstrapUI = canvasGo.AddComponent<BootstrapUI>();
        var so = new SerializedObject(bootstrapUI);
        so.FindProperty("progressBar").objectReferenceValue = progressGo.GetComponent<Slider>();
        so.FindProperty("statusText").objectReferenceValue = statusGo.GetComponent<TextMeshProUGUI>();
        so.FindProperty("retryButton").objectReferenceValue = retryGo.GetComponent<Button>();
        so.FindProperty("retryCanvasGroup").objectReferenceValue = retryGo.GetComponent<CanvasGroup>();
        so.ApplyModifiedProperties();
    }

    private static void RewireBootstrapUI(BootstrapUI ui)
    {
        var slider = ui.GetComponentInChildren<Slider>(true);
        var texts = ui.GetComponentsInChildren<TextMeshProUGUI>(true);
        TextMeshProUGUI statusTmp = null;
        foreach (var t in texts)
        {
            if (t.gameObject.name == "StatusText")
            {
                statusTmp = t;
                break;
            }
        }
        var btn = ui.GetComponentInChildren<Button>(true);
        CanvasGroup retryCg = null;
        if (btn != null)
            retryCg = btn.GetComponent<CanvasGroup>();

        var so = new SerializedObject(ui);
        if (slider != null) so.FindProperty("progressBar").objectReferenceValue = slider;
        if (statusTmp != null) so.FindProperty("statusText").objectReferenceValue = statusTmp;
        if (btn != null) so.FindProperty("retryButton").objectReferenceValue = btn;
        if (retryCg != null) so.FindProperty("retryCanvasGroup").objectReferenceValue = retryCg;
        so.ApplyModifiedProperties();
    }

    private static GameObject CreateProgressBar(Transform parent)
    {
        var sliderGo = new GameObject("ProgressBar");
        sliderGo.transform.SetParent(parent, false);
        var sliderRect = sliderGo.AddComponent<RectTransform>();
        sliderRect.anchorMin = new Vector2(0.1f, 0.4f);
        sliderRect.anchorMax = new Vector2(0.9f, 0.43f);
        sliderRect.offsetMin = Vector2.zero;
        sliderRect.offsetMax = Vector2.zero;

        var bgGo = new GameObject("Background");
        bgGo.transform.SetParent(sliderGo.transform, false);
        var bgImg = bgGo.AddComponent<Image>();
        bgImg.color = new Color(0.2f, 0.2f, 0.25f, 1f);
        var bgRect = bgGo.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;

        var fillAreaGo = new GameObject("Fill Area");
        fillAreaGo.transform.SetParent(sliderGo.transform, false);
        var fillAreaRect = fillAreaGo.AddComponent<RectTransform>();
        fillAreaRect.anchorMin = Vector2.zero;
        fillAreaRect.anchorMax = Vector2.one;
        fillAreaRect.offsetMin = Vector2.zero;
        fillAreaRect.offsetMax = Vector2.zero;

        var fillGo = new GameObject("Fill");
        fillGo.transform.SetParent(fillAreaGo.transform, false);
        var fillImg = fillGo.AddComponent<Image>();
        fillImg.color = new Color(0.3f, 0.7f, 1f, 1f);
        var fillRect = fillGo.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;

        var slider = sliderGo.AddComponent<Slider>();
        slider.fillRect = fillRect;
        slider.targetGraphic = fillImg;
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = 0f;
        slider.interactable = false;
        slider.transition = Selectable.Transition.None;

        return sliderGo;
    }

    private static GameObject CreateStatusText(Transform parent)
    {
        var go = new GameObject("StatusText");
        go.transform.SetParent(parent, false);
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = "Loading...";
        tmp.fontSize = 28;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = new Color(0.7f, 0.7f, 0.7f, 1f);
        var rect = go.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.1f, 0.32f);
        rect.anchorMax = new Vector2(0.9f, 0.38f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        return go;
    }

    private static GameObject CreateRetryButton(Transform parent)
    {
        var btnGo = new GameObject("RetryButton");
        btnGo.transform.SetParent(parent, false);
        var btnRect = btnGo.AddComponent<RectTransform>();
        btnRect.anchorMin = new Vector2(0.3f, 0.22f);
        btnRect.anchorMax = new Vector2(0.7f, 0.28f);
        btnRect.offsetMin = Vector2.zero;
        btnRect.offsetMax = Vector2.zero;

        var btnImg = btnGo.AddComponent<Image>();
        btnImg.color = new Color(0.3f, 0.7f, 1f, 1f);

        btnGo.AddComponent<Button>();
        var cg = btnGo.AddComponent<CanvasGroup>();
        cg.alpha = 0f;
        cg.interactable = false;
        cg.blocksRaycasts = false;

        var textGo = new GameObject("Text");
        textGo.transform.SetParent(btnGo.transform, false);
        var tmp = textGo.AddComponent<TextMeshProUGUI>();
        tmp.text = "RETRY";
        tmp.fontSize = 32;
        tmp.fontStyle = FontStyles.Bold;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
        var textRect = textGo.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        return btnGo;
    }

    private static void AddSceneToBuildSettings(string scenePath, int desiredIndex)
    {
        var scenes = new System.Collections.Generic.List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);

        for (int i = scenes.Count - 1; i >= 0; i--)
        {
            if (scenes[i].path == scenePath)
                scenes.RemoveAt(i);
        }

        var newScene = new EditorBuildSettingsScene(scenePath, true);

        if (desiredIndex >= scenes.Count)
            scenes.Add(newScene);
        else
            scenes.Insert(desiredIndex, newScene);

        EditorBuildSettings.scenes = scenes.ToArray();
    }
}
