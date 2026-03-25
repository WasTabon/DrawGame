using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using UnityEditor.SceneManagement;

public class Iteration6_StarsAndWinUI
{
    [MenuItem("DrawGame/Update Level Data - Stars (Iteration 6)")]
    public static void UpdateLevelData()
    {
        string dataPath = "Assets/DrawGame/Data";

        for (int i = 1; i <= 30; i++)
        {
            string path = dataPath + "/Level_" + i.ToString("D2") + ".asset";
            var levelData = AssetDatabase.LoadAssetAtPath<LevelData>(path);
            if (levelData == null)
            {
                Debug.LogWarning("Level data not found: " + path);
                continue;
            }

            var so = new SerializedObject(levelData);

            int baseLevel = i <= 5 ? i : ((i - 6) % 5) + 1;
            int variation = i <= 5 ? 0 : ((i - 6) / 5) + 1;

            int idealLines = 1;
            float idealTime = 15f;

            switch (baseLevel)
            {
                case 1:
                    idealLines = 1;
                    idealTime = 12f;
                    break;
                case 2:
                    idealLines = 1;
                    idealTime = 10f;
                    break;
                case 3:
                    idealLines = 1;
                    idealTime = 15f;
                    break;
                case 4:
                    idealLines = 1;
                    idealTime = 10f;
                    break;
                case 5:
                    idealLines = 2;
                    idealTime = 15f;
                    break;
            }

            if (variation > 0)
            {
                idealTime = Mathf.Max(8f, idealTime - variation * 1f);
            }

            so.FindProperty("idealLines").intValue = idealLines;
            so.FindProperty("idealTime").floatValue = idealTime;
            so.ApplyModifiedProperties();
            EditorUtility.SetDirty(levelData);
        }

        AssetDatabase.SaveAssets();
        Debug.Log("Updated all 30 levels with ideal lines/time values.");
    }

    [MenuItem("DrawGame/Update Game Scene - Stars UI (Iteration 6)")]
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

        UpdateWinPanelWithStars();

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("Game scene updated with stars UI!");
    }

    private static void UpdateWinPanelWithStars()
    {
        var gameUI = Object.FindObjectOfType<GameUI>();
        Debug.Assert(gameUI != null, "GameUI not found!");

        var canvasGo = gameUI.gameObject;
        var levelCompletePanel = canvasGo.transform.Find("LevelCompletePanel");
        Debug.Assert(levelCompletePanel != null, "LevelCompletePanel not found!");

        var starsContainer = CreateOrGetStarsContainer(levelCompletePanel);
        var starTexts = new TextMeshProUGUI[3];
        for (int i = 0; i < 3; i++)
        {
            starTexts[i] = CreateOrGetStar(starsContainer.transform, "Star" + (i + 1));
        }

        var completeTextObj = levelCompletePanel.Find("CompleteText");
        if (completeTextObj != null)
        {
            var ctRect = completeTextObj.GetComponent<RectTransform>();
            ctRect.anchorMin = new Vector2(0f, 0.65f);
            ctRect.anchorMax = new Vector2(1f, 0.78f);
            ctRect.offsetMin = new Vector2(40f, 0f);
            ctRect.offsetMax = new Vector2(-40f, 0f);
        }

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
            var lineCount = topBar.Find("LineCountText");
            if (lineCount != null)
                so.FindProperty("lineCountText").objectReferenceValue = lineCount.GetComponent<TextMeshProUGUI>();
        }

        var restartBtn = canvasGo.transform.Find("RestartButton");
        if (restartBtn != null)
            so.FindProperty("restartButton").objectReferenceValue = restartBtn.GetComponent<Button>();

        so.FindProperty("levelCompletePanel").objectReferenceValue = levelCompletePanel.GetComponent<CanvasGroup>();

        var completeText = levelCompletePanel.Find("CompleteText");
        if (completeText != null)
            so.FindProperty("levelCompleteText").objectReferenceValue = completeText.GetComponent<TextMeshProUGUI>();

        var nextBtn = levelCompletePanel.Find("NextLevelButton");
        if (nextBtn != null)
            so.FindProperty("nextLevelButton").objectReferenceValue = nextBtn.GetComponent<Button>();

        var winRestartBtn = levelCompletePanel.Find("WinRestartButton");
        if (winRestartBtn != null)
            so.FindProperty("winRestartButton").objectReferenceValue = winRestartBtn.GetComponent<Button>();

        var starsProp = so.FindProperty("starTexts");
        starsProp.arraySize = 3;
        for (int i = 0; i < 3; i++)
        {
            starsProp.GetArrayElementAtIndex(i).objectReferenceValue = starTexts[i];
        }
        so.ApplyModifiedProperties();
    }

    private static GameObject CreateOrGetStarsContainer(Transform parent)
    {
        var existing = parent.Find("StarsContainer");
        if (existing != null) return existing.gameObject;

        var go = new GameObject("StarsContainer");
        go.transform.SetParent(parent, false);
        var rect = go.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.1f, 0.5f);
        rect.anchorMax = new Vector2(0.9f, 0.65f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        var layout = go.AddComponent<HorizontalLayoutGroup>();
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.spacing = 30f;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = false;

        return go;
    }

    private static TextMeshProUGUI CreateOrGetStar(Transform parent, string name)
    {
        var existing = parent.Find(name);
        if (existing != null) return existing.GetComponent<TextMeshProUGUI>();

        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = "\u2605";
        tmp.fontSize = 80;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = new Color(1f, 0.85f, 0.2f, 1f);

        var le = go.AddComponent<LayoutElement>();
        le.preferredWidth = 90f;
        le.preferredHeight = 90f;

        go.transform.localScale = Vector3.zero;

        return tmp;
    }
}
