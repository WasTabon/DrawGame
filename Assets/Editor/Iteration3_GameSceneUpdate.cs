using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using UnityEditor.SceneManagement;

public class Iteration3_GameSceneUpdate
{
    [MenuItem("DrawGame/Update Game Scene - Drawing (Iteration 3)")]
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

        SetupDrawingManager();
        UpdateGameCanvas();

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("Game scene updated with drawing system!");
    }

    private static void SetupDrawingManager()
    {
        var existing = Object.FindObjectOfType<DrawingManager>();
        if (existing != null)
        {
            Debug.Log("DrawingManager already exists.");
            return;
        }

        var go = new GameObject("DrawingManager");
        go.AddComponent<DrawingManager>();
    }

    private static void UpdateGameCanvas()
    {
        var gameUI = Object.FindObjectOfType<GameUI>();
        Debug.Assert(gameUI != null, "GameUI not found! Run Iteration 2 setup first.");

        var canvasGo = gameUI.gameObject;
        var topBar = canvasGo.transform.Find("TopBar");
        Debug.Assert(topBar != null, "TopBar not found on GameCanvas!");

        var lineCountText = CreateOrGetLineCountText(topBar);
        var restartButton = CreateOrGetRestartButton(canvasGo.transform);

        var so = new SerializedObject(gameUI);

        var backBtn = topBar.Find("BackButton");
        if (backBtn != null)
            so.FindProperty("backButton").objectReferenceValue = backBtn.GetComponent<Button>();

        var levelText = topBar.Find("LevelText");
        if (levelText != null)
            so.FindProperty("levelText").objectReferenceValue = levelText.GetComponent<TextMeshProUGUI>();

        so.FindProperty("restartButton").objectReferenceValue = restartButton.GetComponent<Button>();
        so.FindProperty("lineCountText").objectReferenceValue = lineCountText.GetComponent<TextMeshProUGUI>();
        so.ApplyModifiedProperties();

        Debug.Log("GameUI references updated.");
    }

    private static GameObject CreateOrGetLineCountText(Transform topBar)
    {
        var existing = topBar.Find("LineCountText");
        if (existing != null) return existing.gameObject;

        var go = new GameObject("LineCountText");
        go.transform.SetParent(topBar, false);
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = "0 / 5";
        tmp.fontSize = 28;
        tmp.fontStyle = FontStyles.Bold;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
        var rect = go.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.75f, 0f);
        rect.anchorMax = new Vector2(0.98f, 1f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        return go;
    }

    private static GameObject CreateOrGetRestartButton(Transform canvasTransform)
    {
        var existing = canvasTransform.Find("RestartButton");
        if (existing != null) return existing.gameObject;

        var btnGo = new GameObject("RestartButton");
        btnGo.transform.SetParent(canvasTransform, false);
        var btnRect = btnGo.AddComponent<RectTransform>();
        btnRect.anchorMin = new Vector2(0.35f, 0.02f);
        btnRect.anchorMax = new Vector2(0.65f, 0.065f);
        btnRect.offsetMin = Vector2.zero;
        btnRect.offsetMax = Vector2.zero;

        var btnImg = btnGo.AddComponent<Image>();
        btnImg.color = new Color(0.9f, 0.35f, 0.35f, 1f);

        var btn = btnGo.AddComponent<Button>();
        btn.targetGraphic = btnImg;

        var textGo = new GameObject("Text");
        textGo.transform.SetParent(btnGo.transform, false);
        var tmp = textGo.AddComponent<TextMeshProUGUI>();
        tmp.text = "RESTART";
        tmp.fontSize = 28;
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
}
