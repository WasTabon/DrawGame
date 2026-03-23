using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DrawingManager : MonoBehaviour
{
    public static DrawingManager Instance { get; private set; }

    [SerializeField] private int maxLines = 5;
    [SerializeField] private float minPointDistance = 0.15f;
    [SerializeField] private float lineWidth = 0.15f;
    [SerializeField] private Color drawingColor = new Color(0.2f, 0.6f, 1f, 1f);
    [SerializeField] private Color frozenColor = new Color(0.3f, 0.3f, 0.35f, 1f);
    [SerializeField] private PhysicsMaterial2D lineMaterial;

    public event Action<int, int> OnLineCountChanged;

    public int CurrentLineCount => drawnLines.Count;
    public int MaxLines => maxLines;

    private List<DrawnLine> drawnLines = new List<DrawnLine>();
    private DrawnLine currentLine;
    private bool isDrawing;
    private Camera mainCam;
    private bool inputEnabled = true;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        mainCam = Camera.main;
        Debug.Assert(mainCam != null, "DrawingManager: Main Camera not found!");
    }

    private void Update()
    {
        if (!inputEnabled) return;

        if (Input.GetMouseButtonDown(0))
        {
            TryStartDrawing();
        }
        else if (Input.GetMouseButton(0) && isDrawing)
        {
            ContinueDrawing();
        }
        else if (Input.GetMouseButtonUp(0) && isDrawing)
        {
            FinishDrawing();
        }
    }

    private bool IsPointerOverUI()
    {
        if (EventSystem.current == null) return false;

        if (EventSystem.current.IsPointerOverGameObject())
            return true;

        if (Input.touchCount > 0)
        {
            return EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
        }

        return false;
    }

    private void TryStartDrawing()
    {
        if (IsPointerOverUI()) return;
        if (drawnLines.Count >= maxLines) return;

        Vector2 worldPos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        StartDrawing(worldPos);
    }

    private void StartDrawing(Vector2 startPos)
    {
        isDrawing = true;

        var go = new GameObject("DrawnLine_" + drawnLines.Count);
        currentLine = go.AddComponent<DrawnLine>();
        currentLine.Initialize(lineWidth, drawingColor);
        currentLine.AddPoint(startPos);
    }

    private void ContinueDrawing()
    {
        Vector2 worldPos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 lastPoint = currentLine.GetLastPoint();

        if (Vector2.Distance(worldPos, lastPoint) >= minPointDistance)
        {
            currentLine.AddPoint(worldPos);
        }
    }

    private void FinishDrawing()
    {
        isDrawing = false;

        if (currentLine.PointCount < 2)
        {
            Destroy(currentLine.gameObject);
            currentLine = null;
            return;
        }

        currentLine.FreezeAsPhysics(frozenColor, lineMaterial);
        drawnLines.Add(currentLine);
        currentLine = null;

        OnLineCountChanged?.Invoke(drawnLines.Count, maxLines);
    }

    public void ClearAllLines()
    {
        foreach (var line in drawnLines)
        {
            if (line != null)
                Destroy(line.gameObject);
        }
        drawnLines.Clear();

        if (isDrawing && currentLine != null)
        {
            Destroy(currentLine.gameObject);
            currentLine = null;
            isDrawing = false;
        }

        OnLineCountChanged?.Invoke(0, maxLines);
    }

    public void SetInputEnabled(bool enabled)
    {
        inputEnabled = enabled;
        if (!enabled && isDrawing && currentLine != null)
        {
            Destroy(currentLine.gameObject);
            currentLine = null;
            isDrawing = false;
        }
    }

    public void SetMaxLines(int max)
    {
        maxLines = max;
        OnLineCountChanged?.Invoke(drawnLines.Count, maxLines);
    }
}
