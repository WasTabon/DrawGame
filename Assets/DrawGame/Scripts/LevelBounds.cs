using UnityEngine;

public class LevelBounds : MonoBehaviour
{
    [SerializeField] private float wallThickness = 1f;
    [SerializeField] private float extraHeight = 3f;

    private void Start()
    {
        CreateBounds();
    }

    private void CreateBounds()
    {
        var cam = Camera.main;
        Debug.Assert(cam != null, "LevelBounds: Main Camera not found!");

        float camHeight = cam.orthographicSize * 2f;
        float camWidth = camHeight * cam.aspect;
        Vector2 camPos = cam.transform.position;

        CreateWall("Floor",
            camPos + new Vector2(0f, -camHeight / 2f - wallThickness / 2f),
            new Vector2(camWidth + wallThickness * 2f, wallThickness));

        CreateWall("Ceiling",
            camPos + new Vector2(0f, camHeight / 2f + wallThickness / 2f + extraHeight / 2f),
            new Vector2(camWidth + wallThickness * 2f, wallThickness + extraHeight));

        CreateWall("WallLeft",
            camPos + new Vector2(-camWidth / 2f - wallThickness / 2f, extraHeight / 2f),
            new Vector2(wallThickness, camHeight + wallThickness * 2f + extraHeight));

        CreateWall("WallRight",
            camPos + new Vector2(camWidth / 2f + wallThickness / 2f, extraHeight / 2f),
            new Vector2(wallThickness, camHeight + wallThickness * 2f + extraHeight));
    }

    private void CreateWall(string wallName, Vector2 position, Vector2 size)
    {
        var existing = transform.Find(wallName);
        if (existing != null) return;

        var go = new GameObject(wallName);
        go.transform.SetParent(transform);
        go.transform.position = new Vector3(position.x, position.y, 0f);

        var col = go.AddComponent<BoxCollider2D>();
        col.size = size;

        var rb = go.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Static;

        go.layer = LayerMask.NameToLayer("Default");
    }
}
