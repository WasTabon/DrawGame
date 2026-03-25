using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using UnityEditor.SceneManagement;

public class Iteration7_HintSystem
{
    [MenuItem("DrawGame/Generate Hint Data (Iteration 7)")]
    public static void GenerateHintData()
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
            bool mirrored = i > 5 && ((i - 6) / 5) % 2 == 0;

            Vector2[] baseHint = GetBaseHintPoints(baseLevel);

            if (mirrored && baseHint != null)
            {
                for (int p = 0; p < baseHint.Length; p++)
                {
                    baseHint[p] = new Vector2(-baseHint[p].x, baseHint[p].y);
                }
            }

            if (i > 5 && baseHint != null)
            {
                int variationIndex = (i - 6) / 5;
                float offsetX = (variationIndex % 3 - 1) * 0.3f;
                float offsetY = (variationIndex % 2) * 0.4f;
                for (int p = 0; p < baseHint.Length; p++)
                {
                    baseHint[p] += new Vector2(offsetX, offsetY);
                }
            }

            var hintProp = so.FindProperty("hintPoints");
            if (baseHint != null)
            {
                hintProp.arraySize = baseHint.Length;
                for (int p = 0; p < baseHint.Length; p++)
                {
                    hintProp.GetArrayElementAtIndex(p).vector2Value = baseHint[p];
                }
            }
            so.ApplyModifiedProperties();
            EditorUtility.SetDirty(levelData);
        }

        AssetDatabase.SaveAssets();
        Debug.Log("Generated hint data for all 30 levels.");
    }

    [MenuItem("DrawGame/Add HintManager to Bootstrap (Iteration 7)")]
    public static void AddHintManagerToBootstrap()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            return;

        var scene = EditorSceneManager.GetActiveScene();
        if (scene.name != "Bootstrap")
        {
            EditorUtility.DisplayDialog("Add HintManager",
                "Open Bootstrap scene first.", "OK", "");
            return;
        }

        var existing = Object.FindObjectOfType<HintManager>();
        if (existing != null)
        {
            Debug.Log("HintManager already exists on Bootstrap scene.");
            return;
        }

        var go = new GameObject("HintManager");
        go.AddComponent<HintManager>();

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("HintManager added to Bootstrap scene!");
    }

    [MenuItem("DrawGame/Update Game Scene - Hints UI (Iteration 7)")]
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

        UpdateGameUI();

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("Game scene updated with hint UI!");
    }

    private static void UpdateGameUI()
    {
        var gameUI = Object.FindObjectOfType<GameUI>();
        Debug.Assert(gameUI != null, "GameUI not found!");

        var canvasGo = gameUI.gameObject;
        var topBar = canvasGo.transform.Find("TopBar");
        Debug.Assert(topBar != null, "TopBar not found!");

        var hintButtonGo = CreateOrGetHintButton(topBar);

        var so = new SerializedObject(gameUI);

        var backBtn = topBar.Find("BackButton");
        if (backBtn != null)
            so.FindProperty("backButton").objectReferenceValue = backBtn.GetComponent<Button>();
        var levelText = topBar.Find("LevelText");
        if (levelText != null)
            so.FindProperty("levelText").objectReferenceValue = levelText.GetComponent<TextMeshProUGUI>();
        var lineCount = topBar.Find("LineCountText");
        if (lineCount != null)
            so.FindProperty("lineCountText").objectReferenceValue = lineCount.GetComponent<TextMeshProUGUI>();

        var restartBtn = canvasGo.transform.Find("RestartButton");
        if (restartBtn != null)
            so.FindProperty("restartButton").objectReferenceValue = restartBtn.GetComponent<Button>();

        var levelCompletePanel = canvasGo.transform.Find("LevelCompletePanel");
        if (levelCompletePanel != null)
        {
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

            var starsContainer = levelCompletePanel.Find("StarsContainer");
            if (starsContainer != null)
            {
                var starsProp = so.FindProperty("starTexts");
                starsProp.arraySize = 3;
                for (int i = 0; i < 3; i++)
                {
                    var star = starsContainer.Find("Star" + (i + 1));
                    if (star != null)
                        starsProp.GetArrayElementAtIndex(i).objectReferenceValue = star.GetComponent<TextMeshProUGUI>();
                }
            }
        }

        so.FindProperty("hintButton").objectReferenceValue = hintButtonGo.GetComponent<Button>();
        var hintCountTmp = hintButtonGo.transform.Find("CountText");
        if (hintCountTmp != null)
            so.FindProperty("hintCountText").objectReferenceValue = hintCountTmp.GetComponent<TextMeshProUGUI>();

        so.ApplyModifiedProperties();
    }

    private static GameObject CreateOrGetHintButton(Transform topBar)
    {
        var existing = topBar.Find("HintButton");
        if (existing != null) return existing.gameObject;

        var btnGo = new GameObject("HintButton");
        btnGo.transform.SetParent(topBar, false);
        var btnRect = btnGo.AddComponent<RectTransform>();
        btnRect.anchorMin = new Vector2(0.55f, 0.1f);
        btnRect.anchorMax = new Vector2(0.73f, 0.9f);
        btnRect.offsetMin = Vector2.zero;
        btnRect.offsetMax = Vector2.zero;

        var btnImg = btnGo.AddComponent<Image>();
        btnImg.color = new Color(1f, 0.8f, 0.2f, 1f);

        var btn = btnGo.AddComponent<Button>();
        btn.targetGraphic = btnImg;

        var iconGo = new GameObject("Icon");
        iconGo.transform.SetParent(btnGo.transform, false);
        var iconTmp = iconGo.AddComponent<TextMeshProUGUI>();
        iconTmp.text = "\U0001F4A1";
        iconTmp.fontSize = 24;
        iconTmp.alignment = TextAlignmentOptions.Center;
        var iconRect = iconGo.GetComponent<RectTransform>();
        iconRect.anchorMin = new Vector2(0f, 0f);
        iconRect.anchorMax = new Vector2(0.55f, 1f);
        iconRect.offsetMin = Vector2.zero;
        iconRect.offsetMax = Vector2.zero;

        var countGo = new GameObject("CountText");
        countGo.transform.SetParent(btnGo.transform, false);
        var countTmp = countGo.AddComponent<TextMeshProUGUI>();
        countTmp.text = "5";
        countTmp.fontSize = 22;
        countTmp.fontStyle = FontStyles.Bold;
        countTmp.alignment = TextAlignmentOptions.Center;
        countTmp.color = Color.white;
        var countRect = countGo.GetComponent<RectTransform>();
        countRect.anchorMin = new Vector2(0.55f, 0f);
        countRect.anchorMax = new Vector2(1f, 1f);
        countRect.offsetMin = Vector2.zero;
        countRect.offsetMax = Vector2.zero;

        return btnGo;
    }

    private static Vector2[] GetBaseHintPoints(int baseLevel)
    {
        switch (baseLevel)
        {
            case 1:
                return new Vector2[]
                {
                    new Vector2(-0.3f, 1.5f),
                    new Vector2(0.3f, 1.2f),
                    new Vector2(0.8f, 0.8f),
                    new Vector2(1.2f, 0.3f),
                    new Vector2(1.5f, -0.2f)
                };

            case 2:
                return new Vector2[]
                {
                    new Vector2(-1.2f, 1.5f),
                    new Vector2(-0.6f, 1.8f),
                    new Vector2(0f, 2.0f),
                    new Vector2(0.6f, 1.8f),
                    new Vector2(1.0f, 1.2f),
                    new Vector2(1.0f, 0.5f)
                };

            case 3:
                return new Vector2[]
                {
                    new Vector2(-1.5f, 0.5f),
                    new Vector2(-1.0f, 1.0f),
                    new Vector2(-0.3f, 1.3f),
                    new Vector2(0.3f, 1.3f),
                    new Vector2(1.0f, 1.0f),
                    new Vector2(1.5f, 0.5f)
                };

            case 4:
                return new Vector2[]
                {
                    new Vector2(-1.5f, 2.5f),
                    new Vector2(-1.0f, 2.2f),
                    new Vector2(-0.5f, 1.8f),
                    new Vector2(0f, 1.3f)
                };

            case 5:
                return new Vector2[]
                {
                    new Vector2(0.5f, 0.5f),
                    new Vector2(0.8f, 0.8f),
                    new Vector2(1.2f, 1.0f),
                    new Vector2(1.6f, 0.8f),
                    new Vector2(1.8f, 0.3f),
                    new Vector2(1.8f, -0.5f)
                };

            default:
                return new Vector2[]
                {
                    new Vector2(-0.5f, 1f),
                    new Vector2(0.5f, 1f)
                };
        }
    }
}
