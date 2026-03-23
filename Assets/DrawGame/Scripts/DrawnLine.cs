using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DrawnLine : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private List<Vector2> points = new List<Vector2>();
    private bool isFrozen;

    public int PointCount => points.Count;

    public void Initialize(float width, Color color)
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
        lineRenderer.positionCount = 0;
        lineRenderer.useWorldSpace = false;
        lineRenderer.sortingOrder = 5;
        lineRenderer.numCapVertices = 5;
        lineRenderer.numCornerVertices = 5;
    }

    public void AddPoint(Vector2 point)
    {
        points.Add(point);
        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPosition(points.Count - 1, new Vector3(point.x, point.y, 0f));
    }

    public Vector2 GetLastPoint()
    {
        return points[points.Count - 1];
    }

    public void FreezeAsPhysics(Color frozenColor, PhysicsMaterial2D physicsMaterial)
    {
        if (isFrozen) return;
        isFrozen = true;

        lineRenderer.startColor = frozenColor;
        lineRenderer.endColor = frozenColor;

        var edgeCollider = gameObject.AddComponent<EdgeCollider2D>();
        edgeCollider.points = points.ToArray();
        if (physicsMaterial != null)
            edgeCollider.sharedMaterial = physicsMaterial;

        var rb = gameObject.AddComponent<Rigidbody2D>();
        rb.mass = CalculateMass();
        rb.gravityScale = 1f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        AnimateFreeze();
    }

    private float CalculateMass()
    {
        float totalLength = 0f;
        for (int i = 1; i < points.Count; i++)
        {
            totalLength += Vector2.Distance(points[i - 1], points[i]);
        }
        return Mathf.Clamp(totalLength * 0.5f, 0.2f, 10f);
    }

    private void AnimateFreeze()
    {
        transform.DOShakeScale(0.2f, 0.1f, 10, 90f).SetEase(Ease.OutQuad);
    }
}