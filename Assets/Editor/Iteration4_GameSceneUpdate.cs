using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using UnityEditor.SceneManagement;

public class Iteration4_GameSceneUpdate
{
    [MenuItem("DrawGame/Update Game Scene - Physics & Level Objects (Iteration 4)")]
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

        SetupLevelBounds();
        var levelController = SetupLevelController();
        SetupTestLevel(levelController);
        UpdateGameUI();

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("Game scene updated with physics & level objects!");
    }

    private static void SetupLevelBounds()
    {
        var existing = Object.FindObjectOfType<LevelBounds>();
        if (existing != null)
        {
            Debug.Log("LevelBounds already exists.");
            return;
        }

        var go = new GameObject("LevelBounds");
        go.AddComponent<LevelBounds>();
    }

    private static LevelController SetupLevelController()
    {
        var existing = Object.FindObjectOfType<LevelController>();
        if (existing != null)
        {
            Debug.Log("LevelController already exists.");
            return existing;
        }

        var go = new GameObject("LevelController");
        return go.AddComponent<LevelController>();
    }

    private static void SetupTestLevel(LevelController levelController)
    {
        var levelRoot = GameObject.Find("LevelObjects");
        if (levelRoot != null)
        {
            Debug.Log("LevelObjects already exist, skipping test level creation.");
            RewireLevelController(levelController);
            return;
        }

        levelRoot = new GameObject("LevelObjects");

        var platform = CreatePlatform(levelRoot.transform, "Platform",
            new Vector2(0f, -1f), new Vector2(4f, 0.3f),
            new Color(0.4f, 0.4f, 0.45f, 1f));

        var ball = CreateBall(levelRoot.transform, "Ball",
            new Vector2(0f, 0.5f), 0.4f,
            new Color(1f, 0.5f, 0.2f, 1f), true);

        var goalZone = CreateGoalZone(levelRoot.transform, "GoalZone",
            new Vector2(3f, -3.5f), new Vector2(2f, 1.5f),
            new Color(0.2f, 0.9f, 0.3f, 0.4f));

        var so = new SerializedObject(levelController);
        so.FindProperty("goalZone").objectReferenceValue = goalZone.GetComponent<GoalZone>();

        var levelObjects = levelRoot.GetComponentsInChildren<LevelObject>();
        var objsProp = so.FindProperty("levelObjects");
        objsProp.arraySize = levelObjects.Length;
        for (int i = 0; i < levelObjects.Length; i++)
        {
            objsProp.GetArrayElementAtIndex(i).objectReferenceValue = levelObjects[i];
        }
        so.ApplyModifiedProperties();
    }

    private static void RewireLevelController(LevelController levelController)
    {
        var so = new SerializedObject(levelController);

        var goalZone = Object.FindObjectOfType<GoalZone>();
        if (goalZone != null)
            so.FindProperty("goalZone").objectReferenceValue = goalZone;

        var levelObjects = Object.FindObjectsOfType<LevelObject>();
        var objsProp = so.FindProperty("levelObjects");
        objsProp.arraySize = levelObjects.Length;
        for (int i = 0; i < levelObjects.Length; i++)
        {
            objsProp.GetArrayElementAtIndex(i).objectReferenceValue = levelObjects[i];
        }
        so.ApplyModifiedProperties();
    }

    private static GameObject CreatePlatform(Transform parent, string name, Vector2 position, Vector2 size, Color color)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent);
        go.transform.position = new Vector3(position.x, position.y, 0f);

        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = CreateSquareSprite();
        sr.color = color;
        sr.sortingOrder = 1;
        go.transform.localScale = new Vector3(size.x, size.y, 1f);

        var col = go.AddComponent<BoxCollider2D>();

        var rb = go.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Static;

        var levelObj = go.AddComponent<LevelObject>();
        var so = new SerializedObject(levelObj);
        so.FindProperty("objectType").enumValueIndex = (int)LevelObjectType.Static;
        so.FindProperty("isGoalTarget").boolValue = false;
        so.ApplyModifiedProperties();

        return go;
    }

    private static GameObject CreateBall(Transform parent, string name, Vector2 position, float radius, Color color, bool isGoalTarget)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent);
        go.transform.position = new Vector3(position.x, position.y, 0f);

        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = CreateCircleSprite();
        sr.color = color;
        sr.sortingOrder = 2;
        go.transform.localScale = new Vector3(radius * 2f, radius * 2f, 1f);

        var col = go.AddComponent<CircleCollider2D>();

        var rb = go.AddComponent<Rigidbody2D>();
        rb.mass = 1f;
        rb.gravityScale = 1f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        var levelObj = go.AddComponent<LevelObject>();
        var so = new SerializedObject(levelObj);
        so.FindProperty("objectType").enumValueIndex = (int)LevelObjectType.Dynamic;
        so.FindProperty("isGoalTarget").boolValue = isGoalTarget;
        so.ApplyModifiedProperties();

        return go;
    }

    private static GameObject CreateGoalZone(Transform parent, string name, Vector2 position, Vector2 size, Color color)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent);
        go.transform.position = new Vector3(position.x, position.y, 0f);

        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = CreateSquareSprite();
        sr.color = color;
        sr.sortingOrder = 0;
        go.transform.localScale = new Vector3(size.x, size.y, 1f);

        var col = go.AddComponent<BoxCollider2D>();
        col.isTrigger = true;

        var goalZone = go.AddComponent<GoalZone>();
        var so = new SerializedObject(goalZone);
        so.FindProperty("goalType").enumValueIndex = (int)GoalType.ReachZone;
        so.FindProperty("holdDuration").floatValue = 2f;
        so.ApplyModifiedProperties();

        return go;
    }

    private static void UpdateGameUI()
    {
        var gameUI = Object.FindObjectOfType<GameUI>();
        Debug.Assert(gameUI != null, "GameUI not found! Run Iteration 2 & 3 setup first.");

        var canvasGo = gameUI.gameObject;
        var topBar = canvasGo.transform.Find("TopBar");

        var levelCompletePanel = CreateOrGetLevelCompletePanel(canvasGo.transform);

        var so = new SerializedObject(gameUI);

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

        var completeText = levelCompletePanel.transform.Find("CompleteText");
        if (completeText != null)
            so.FindProperty("levelCompleteText").objectReferenceValue = completeText.GetComponent<TextMeshProUGUI>();

        so.ApplyModifiedProperties();
        Debug.Log("GameUI references updated with level complete panel.");
    }

    private static GameObject CreateOrGetLevelCompletePanel(Transform canvasTransform)
    {
        var existing = canvasTransform.Find("LevelCompletePanel");
        if (existing != null) return existing.gameObject;

        var panelGo = new GameObject("LevelCompletePanel");
        panelGo.transform.SetParent(canvasTransform, false);
        var panelRect = panelGo.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        var panelImg = panelGo.AddComponent<Image>();
        panelImg.color = new Color(0f, 0f, 0f, 0.6f);

        var cg = panelGo.AddComponent<CanvasGroup>();
        cg.alpha = 0f;
        cg.interactable = false;
        cg.blocksRaycasts = false;

        var textGo = new GameObject("CompleteText");
        textGo.transform.SetParent(panelGo.transform, false);
        var tmp = textGo.AddComponent<TextMeshProUGUI>();
        tmp.text = "LEVEL COMPLETE!";
        tmp.fontSize = 60;
        tmp.fontStyle = FontStyles.Bold;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = new Color(0.3f, 1f, 0.5f, 1f);
        var textRect = textGo.GetComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0f, 0.4f);
        textRect.anchorMax = new Vector2(1f, 0.6f);
        textRect.offsetMin = new Vector2(40f, 0f);
        textRect.offsetMax = new Vector2(-40f, 0f);

        return panelGo;
    }

    private static Sprite CreateSquareSprite()
    {
        var tex = new Texture2D(4, 4);
        var pixels = new Color[16];
        for (int i = 0; i < 16; i++) pixels[i] = Color.white;
        tex.SetPixels(pixels);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, 4, 4), new Vector2(0.5f, 0.5f), 4f);
    }

    private static Sprite CreateCircleSprite()
    {
        int size = 64;
        var tex = new Texture2D(size, size);
        float center = size / 2f;
        float radius = size / 2f;

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), new Vector2(center, center));
                if (dist <= radius)
                    tex.SetPixel(x, y, Color.white);
                else
                    tex.SetPixel(x, y, Color.clear);
            }
        }

        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
    }
}
