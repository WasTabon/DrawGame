using UnityEngine;
using System.Collections.Generic;

public class LevelSpawner : MonoBehaviour
{
    public static LevelSpawner Instance { get; private set; }

    [SerializeField] private LevelDatabase levelDatabase;

    private GameObject levelRoot;
    private List<LevelObject> spawnedObjects = new List<LevelObject>();
    private GoalZone spawnedGoalZone;

    public GoalZone CurrentGoalZone => spawnedGoalZone;
    public LevelObject[] SpawnedObjects => spawnedObjects.ToArray();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Debug.Assert(levelDatabase != null, "LevelSpawner: levelDatabase not assigned!");

        int level = GameManager.Instance != null ? GameManager.Instance.SelectedLevel : 1;
        SpawnLevel(level);
    }

    public void SpawnLevel(int levelNumber)
    {
        ClearLevel();

        var data = levelDatabase.GetLevel(levelNumber);
        if (data == null)
        {
            Debug.LogWarning("LevelSpawner: No data for level " + levelNumber);
            return;
        }

        levelRoot = new GameObject("LevelObjects");

        if (data.objects != null)
        {
            foreach (var objData in data.objects)
            {
                SpawnObject(objData);
            }
        }

        if (data.goalZone != null)
        {
            SpawnGoalZone(data.goalZone);
        }

        if (DrawingManager.Instance != null)
        {
            DrawingManager.Instance.SetMaxLines(data.maxLines);
            DrawingManager.Instance.SetInputEnabled(true);
        }

        if (LevelController.Instance != null)
        {
            LevelController.Instance.SetupLevel(spawnedGoalZone, spawnedObjects.ToArray());
        }
    }

    private void SpawnObject(LevelObjectData data)
    {
        var go = new GameObject(data.name);
        go.transform.SetParent(levelRoot.transform);
        go.transform.position = new Vector3(data.position.x, data.position.y, 0f);
        go.transform.rotation = Quaternion.Euler(0f, 0f, data.rotation);

        var sr = go.AddComponent<SpriteRenderer>();
        sr.color = data.color;
        sr.sortingOrder = data.isGoalTarget ? 2 : 1;

        if (data.shape == LevelObjectShape.Circle)
        {
            sr.sprite = CreateCircleSprite();
            float diameter = data.size.x;
            go.transform.localScale = new Vector3(diameter, diameter, 1f);
            go.AddComponent<CircleCollider2D>();
        }
        else
        {
            sr.sprite = CreateSquareSprite();
            go.transform.localScale = new Vector3(data.size.x, data.size.y, 1f);
            go.AddComponent<BoxCollider2D>();
        }

        var rb = go.AddComponent<Rigidbody2D>();
        if (data.objectType == LevelObjectType.Static)
        {
            rb.bodyType = RigidbodyType2D.Static;
        }
        else
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.mass = data.mass > 0f ? data.mass : 1f;
            rb.gravityScale = 1f;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        }

        if (data.bounciness > 0f)
        {
            var mat = new PhysicsMaterial2D("Bouncy_" + data.name);
            mat.bounciness = data.bounciness;
            mat.friction = 0.4f;
            var col = go.GetComponent<Collider2D>();
            col.sharedMaterial = mat;
        }

        var levelObj = go.AddComponent<LevelObject>();
        levelObj.SetAsGoalTarget(data.isGoalTarget);

        spawnedObjects.Add(levelObj);
    }

    private void SpawnGoalZone(GoalZoneData data)
    {
        var go = new GameObject("GoalZone");
        go.transform.SetParent(levelRoot.transform);
        go.transform.position = new Vector3(data.position.x, data.position.y, 0f);

        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = CreateSquareSprite();
        sr.color = data.color;
        sr.sortingOrder = 0;
        go.transform.localScale = new Vector3(data.size.x, data.size.y, 1f);

        var col = go.AddComponent<BoxCollider2D>();
        col.isTrigger = true;

        spawnedGoalZone = go.AddComponent<GoalZone>();
        spawnedGoalZone.Init(data.goalType, data.holdDuration);
    }

    public void ClearLevel()
    {
        if (levelRoot != null)
        {
            Destroy(levelRoot);
            levelRoot = null;
        }
        spawnedObjects.Clear();
        spawnedGoalZone = null;
    }

    private Sprite CreateSquareSprite()
    {
        var tex = new Texture2D(4, 4);
        var pixels = new Color[16];
        for (int i = 0; i < 16; i++) pixels[i] = Color.white;
        tex.SetPixels(pixels);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, 4, 4), new Vector2(0.5f, 0.5f), 4f);
    }

    private Sprite CreateCircleSprite()
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
                tex.SetPixel(x, y, dist <= radius ? Color.white : Color.clear);
            }
        }
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
    }
}
