using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using UnityEditor.SceneManagement;
using System.Collections.Generic;

public class Iteration5_LevelGenerator
{
    private static readonly Color platformColor = new Color(0.4f, 0.4f, 0.45f, 1f);
    private static readonly Color wallColor = new Color(0.5f, 0.45f, 0.4f, 1f);
    private static readonly Color ballColor = new Color(1f, 0.5f, 0.2f, 1f);
    private static readonly Color goalColor = new Color(0.2f, 0.9f, 0.3f, 0.4f);

    [MenuItem("DrawGame/Generate All Levels (Iteration 5)")]
    public static void GenerateAllLevels()
    {
        string dataPath = "Assets/DrawGame/Data";
        System.IO.Directory.CreateDirectory(dataPath);

        LevelData[] baseLevels = new LevelData[5];
        baseLevels[0] = CreateLevel1();
        baseLevels[1] = CreateLevel2();
        baseLevels[2] = CreateLevel3();
        baseLevels[3] = CreateLevel4();
        baseLevels[4] = CreateLevel5();

        for (int i = 0; i < 5; i++)
        {
            baseLevels[i].levelNumber = i + 1;
            AssetDatabase.CreateAsset(baseLevels[i], dataPath + "/Level_" + (i + 1).ToString("D2") + ".asset");
        }

        LevelData[] allLevels = new LevelData[30];
        for (int i = 0; i < 5; i++)
        {
            allLevels[i] = baseLevels[i];
        }

        for (int i = 5; i < 30; i++)
        {
            int baseIndex = (i - 5) % 5;
            int variationIndex = (i - 5) / 5;
            var variation = CreateVariation(baseLevels[baseIndex], i + 1, variationIndex);
            AssetDatabase.CreateAsset(variation, dataPath + "/Level_" + (i + 1).ToString("D2") + ".asset");
            allLevels[i] = variation;
        }

        var database = ScriptableObject.CreateInstance<LevelDatabase>();
        database.levels = allLevels;

        string dbPath = dataPath + "/LevelDatabase.asset";
        var existingDb = AssetDatabase.LoadAssetAtPath<LevelDatabase>(dbPath);
        if (existingDb != null)
        {
            AssetDatabase.DeleteAsset(dbPath);
        }
        AssetDatabase.CreateAsset(database, dbPath);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Generated 30 levels and LevelDatabase at " + dataPath);
    }

    [MenuItem("DrawGame/Update Game Scene - Levels (Iteration 5)")]
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

        RemoveOldLevelObjects();
        SetupLevelSpawner();
        RemoveOldLevelController();
        SetupLevelController();
        UpdateGameUIWithWinButtons();

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("Game scene updated with level system!");
    }

    private static void RemoveOldLevelObjects()
    {
        var levelRoot = GameObject.Find("LevelObjects");
        if (levelRoot != null)
        {
            Object.DestroyImmediate(levelRoot);
            Debug.Log("Removed old LevelObjects.");
        }
    }

    private static void RemoveOldLevelController()
    {
        var existing = Object.FindObjectOfType<LevelController>();
        if (existing != null)
        {
            var go = existing.gameObject;
            if (go.GetComponent<LevelSpawner>() == null && go.name == "LevelController")
            {
                Object.DestroyImmediate(go);
                Debug.Log("Removed old standalone LevelController.");
            }
        }
    }

    private static void SetupLevelSpawner()
    {
        var existing = Object.FindObjectOfType<LevelSpawner>();
        if (existing != null)
        {
            WireLevelSpawner(existing);
            return;
        }

        var go = new GameObject("LevelSpawner");
        var spawner = go.AddComponent<LevelSpawner>();
        WireLevelSpawner(spawner);
    }

    private static void WireLevelSpawner(LevelSpawner spawner)
    {
        var database = AssetDatabase.LoadAssetAtPath<LevelDatabase>("Assets/DrawGame/Data/LevelDatabase.asset");
        Debug.Assert(database != null, "LevelDatabase not found! Run 'Generate All Levels' first.");

        var so = new SerializedObject(spawner);
        so.FindProperty("levelDatabase").objectReferenceValue = database;
        so.ApplyModifiedProperties();
    }

    private static void SetupLevelController()
    {
        var existing = Object.FindObjectOfType<LevelController>();
        if (existing != null) return;

        var go = new GameObject("LevelController");
        go.AddComponent<LevelController>();
    }

    private static void UpdateGameUIWithWinButtons()
    {
        var gameUI = Object.FindObjectOfType<GameUI>();
        Debug.Assert(gameUI != null, "GameUI not found!");

        var canvasGo = gameUI.gameObject;
        var levelCompletePanel = canvasGo.transform.Find("LevelCompletePanel");
        Debug.Assert(levelCompletePanel != null, "LevelCompletePanel not found! Run iteration 4 setup first.");

        var nextBtn = CreateOrGetButton(levelCompletePanel, "NextLevelButton",
            new Vector2(0.3f, 0.28f), new Vector2(0.7f, 0.35f),
            "NEXT LEVEL", new Color(0.3f, 0.85f, 0.5f, 1f));

        var winRestartBtn = CreateOrGetButton(levelCompletePanel, "WinRestartButton",
            new Vector2(0.3f, 0.2f), new Vector2(0.7f, 0.27f),
            "RESTART", new Color(0.9f, 0.35f, 0.35f, 1f));

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

        so.FindProperty("nextLevelButton").objectReferenceValue = nextBtn.GetComponent<Button>();
        so.FindProperty("winRestartButton").objectReferenceValue = winRestartBtn.GetComponent<Button>();
        so.ApplyModifiedProperties();
    }

    private static GameObject CreateOrGetButton(Transform parent, string name,
        Vector2 anchorMin, Vector2 anchorMax, string text, Color color)
    {
        var existing = parent.Find(name);
        if (existing != null) return existing.gameObject;

        var btnGo = new GameObject(name);
        btnGo.transform.SetParent(parent, false);
        var btnRect = btnGo.AddComponent<RectTransform>();
        btnRect.anchorMin = anchorMin;
        btnRect.anchorMax = anchorMax;
        btnRect.offsetMin = Vector2.zero;
        btnRect.offsetMax = Vector2.zero;

        var btnImg = btnGo.AddComponent<Image>();
        btnImg.color = color;

        var btn = btnGo.AddComponent<Button>();
        btn.targetGraphic = btnImg;

        var textGo = new GameObject("Text");
        textGo.transform.SetParent(btnGo.transform, false);
        var tmp = textGo.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
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

    private static LevelData CreateLevel1()
    {
        var data = ScriptableObject.CreateInstance<LevelData>();
        data.maxLines = 3;
        data.objects = new LevelObjectData[]
        {
            new LevelObjectData
            {
                name = "Platform",
                objectType = LevelObjectType.Static,
                shape = LevelObjectShape.Box,
                position = new Vector2(-0.5f, -1f),
                size = new Vector2(3f, 0.3f),
                color = platformColor,
                mass = 0f
            },
            new LevelObjectData
            {
                name = "Ball",
                objectType = LevelObjectType.Dynamic,
                shape = LevelObjectShape.Circle,
                position = new Vector2(-0.5f, 0.2f),
                size = new Vector2(0.8f, 0.8f),
                color = ballColor,
                isGoalTarget = true,
                mass = 1f,
                bounciness = 0.3f
            }
        };
        data.goalZone = new GoalZoneData
        {
            position = new Vector2(1.5f, -3.5f),
            size = new Vector2(1.5f, 1.2f),
            goalType = GoalType.ReachZone,
            color = goalColor
        };
        return data;
    }

    private static LevelData CreateLevel2()
    {
        var data = ScriptableObject.CreateInstance<LevelData>();
        data.maxLines = 2;
        data.objects = new LevelObjectData[]
        {
            new LevelObjectData
            {
                name = "HighPlatform",
                objectType = LevelObjectType.Static,
                shape = LevelObjectShape.Box,
                position = new Vector2(0f, 2f),
                size = new Vector2(2.5f, 0.3f),
                color = platformColor,
                mass = 0f
            },
            new LevelObjectData
            {
                name = "Ball",
                objectType = LevelObjectType.Dynamic,
                shape = LevelObjectShape.Circle,
                position = new Vector2(0f, 3f),
                size = new Vector2(0.8f, 0.8f),
                color = ballColor,
                isGoalTarget = true,
                mass = 1f,
                bounciness = 0.3f
            },
            new LevelObjectData
            {
                name = "Obstacle",
                objectType = LevelObjectType.Static,
                shape = LevelObjectShape.Box,
                position = new Vector2(0f, -0.5f),
                size = new Vector2(2f, 0.3f),
                color = wallColor,
                mass = 0f
            }
        };
        data.goalZone = new GoalZoneData
        {
            position = new Vector2(0f, -3.5f),
            size = new Vector2(2f, 1.2f),
            goalType = GoalType.ReachZone,
            color = goalColor
        };
        return data;
    }

    private static LevelData CreateLevel3()
    {
        var data = ScriptableObject.CreateInstance<LevelData>();
        data.maxLines = 3;
        data.objects = new LevelObjectData[]
        {
            new LevelObjectData
            {
                name = "Wall",
                objectType = LevelObjectType.Static,
                shape = LevelObjectShape.Box,
                position = new Vector2(0f, -1f),
                size = new Vector2(0.3f, 3f),
                color = wallColor,
                mass = 0f
            },
            new LevelObjectData
            {
                name = "BallPlatform",
                objectType = LevelObjectType.Static,
                shape = LevelObjectShape.Box,
                position = new Vector2(-1.5f, -2f),
                size = new Vector2(1.8f, 0.3f),
                color = platformColor,
                mass = 0f
            },
            new LevelObjectData
            {
                name = "Ball",
                objectType = LevelObjectType.Dynamic,
                shape = LevelObjectShape.Circle,
                position = new Vector2(-1.5f, -1f),
                size = new Vector2(0.8f, 0.8f),
                color = ballColor,
                isGoalTarget = true,
                mass = 1f,
                bounciness = 0.3f
            }
        };
        data.goalZone = new GoalZoneData
        {
            position = new Vector2(1.5f, -3.5f),
            size = new Vector2(1.5f, 1.2f),
            goalType = GoalType.ReachZone,
            color = goalColor
        };
        return data;
    }

    private static LevelData CreateLevel4()
    {
        var data = ScriptableObject.CreateInstance<LevelData>();
        data.maxLines = 2;
        data.objects = new LevelObjectData[]
        {
            new LevelObjectData
            {
                name = "RampPlatform",
                objectType = LevelObjectType.Static,
                shape = LevelObjectShape.Box,
                position = new Vector2(-0.5f, 0f),
                size = new Vector2(3.5f, 0.3f),
                rotation = 15f,
                color = platformColor,
                mass = 0f
            },
            new LevelObjectData
            {
                name = "Ball",
                objectType = LevelObjectType.Dynamic,
                shape = LevelObjectShape.Circle,
                position = new Vector2(-1.8f, 1.5f),
                size = new Vector2(0.8f, 0.8f),
                color = ballColor,
                isGoalTarget = true,
                mass = 1f,
                bounciness = 0.3f
            }
        };
        data.goalZone = new GoalZoneData
        {
            position = new Vector2(1.5f, -3.5f),
            size = new Vector2(1.5f, 1.2f),
            goalType = GoalType.ReachZone,
            color = goalColor
        };
        return data;
    }

    private static LevelData CreateLevel5()
    {
        var data = ScriptableObject.CreateInstance<LevelData>();
        data.maxLines = 3;
        data.objects = new LevelObjectData[]
        {
            new LevelObjectData
            {
                name = "CupLeft",
                objectType = LevelObjectType.Static,
                shape = LevelObjectShape.Box,
                position = new Vector2(-1.3f, -1f),
                size = new Vector2(0.3f, 2.5f),
                color = wallColor,
                mass = 0f
            },
            new LevelObjectData
            {
                name = "CupRight",
                objectType = LevelObjectType.Static,
                shape = LevelObjectShape.Box,
                position = new Vector2(0.3f, -1f),
                size = new Vector2(0.3f, 2.5f),
                color = wallColor,
                mass = 0f
            },
            new LevelObjectData
            {
                name = "CupBottom",
                objectType = LevelObjectType.Static,
                shape = LevelObjectShape.Box,
                position = new Vector2(-0.5f, -2.3f),
                size = new Vector2(1.9f, 0.3f),
                color = platformColor,
                mass = 0f
            },
            new LevelObjectData
            {
                name = "Ball",
                objectType = LevelObjectType.Dynamic,
                shape = LevelObjectShape.Circle,
                position = new Vector2(-0.5f, -1.2f),
                size = new Vector2(0.7f, 0.7f),
                color = ballColor,
                isGoalTarget = true,
                mass = 1f,
                bounciness = 0.3f
            }
        };
        data.goalZone = new GoalZoneData
        {
            position = new Vector2(1.8f, -3.5f),
            size = new Vector2(1.2f, 1.2f),
            goalType = GoalType.ReachZone,
            color = goalColor
        };
        return data;
    }

    private static LevelData CreateVariation(LevelData baseLevel, int newLevelNumber, int variationIndex)
    {
        var data = ScriptableObject.CreateInstance<LevelData>();
        data.levelNumber = newLevelNumber;

        bool mirrorX = variationIndex % 2 == 0;
        float offsetX = (variationIndex % 3 - 1) * 0.3f;
        float offsetY = (variationIndex % 2) * 0.4f;
        int maxLinesBonus = variationIndex >= 3 ? -1 : 0;

        data.maxLines = Mathf.Max(1, baseLevel.maxLines + maxLinesBonus);

        if (baseLevel.objects != null)
        {
            data.objects = new LevelObjectData[baseLevel.objects.Length];
            for (int i = 0; i < baseLevel.objects.Length; i++)
            {
                var src = baseLevel.objects[i];
                var dst = new LevelObjectData();
                dst.name = src.name;
                dst.objectType = src.objectType;
                dst.shape = src.shape;

                float px = src.position.x;
                float py = src.position.y;

                if (mirrorX) px = -px;
                px += offsetX;
                py += offsetY;

                px = Mathf.Clamp(px, -2.3f, 2.3f);
                py = Mathf.Clamp(py, -4f, 4f);

                dst.position = new Vector2(px, py);
                dst.size = src.size;
                dst.rotation = mirrorX ? -src.rotation : src.rotation;
                dst.color = src.color;
                dst.isGoalTarget = src.isGoalTarget;
                dst.mass = src.mass;
                dst.bounciness = src.bounciness;

                data.objects[i] = dst;
            }
        }

        if (baseLevel.goalZone != null)
        {
            data.goalZone = new GoalZoneData();
            float gx = baseLevel.goalZone.position.x;
            float gy = baseLevel.goalZone.position.y;
            if (mirrorX) gx = -gx;
            gx += offsetX;
            gx = Mathf.Clamp(gx, -2.3f, 2.3f);
            gy = Mathf.Clamp(gy, -4.2f, 4f);

            data.goalZone.position = new Vector2(gx, gy);
            data.goalZone.size = baseLevel.goalZone.size;
            data.goalZone.goalType = baseLevel.goalZone.goalType;
            data.goalZone.holdDuration = baseLevel.goalZone.holdDuration;
            data.goalZone.color = baseLevel.goalZone.color;
        }

        return data;
    }
}
