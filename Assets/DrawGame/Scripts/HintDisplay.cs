using UnityEngine;
using DG.Tweening;

public class HintDisplay : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private bool isShowing;
    private Tween fadeTween;

    public void Initialize(Vector2[] hintPoints)
    {
        if (hintPoints == null || hintPoints.Length < 2)
        {
            Debug.LogWarning("HintDisplay: not enough hint points!");
            return;
        }

        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.2f;
        lineRenderer.endWidth = 0.2f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = new Color(1f, 1f, 0.3f, 0f);
        lineRenderer.endColor = new Color(1f, 1f, 0.3f, 0f);
        lineRenderer.positionCount = hintPoints.Length;
        lineRenderer.useWorldSpace = true;
        lineRenderer.sortingOrder = 10;
        lineRenderer.numCapVertices = 5;
        lineRenderer.numCornerVertices = 5;

        for (int i = 0; i < hintPoints.Length; i++)
        {
            lineRenderer.SetPosition(i, new Vector3(hintPoints[i].x, hintPoints[i].y, 0f));
        }

        SetAlpha(0f);
    }

    public void ShowHint(float duration = 3f)
    {
        if (lineRenderer == null) return;
        if (isShowing) return;
        isShowing = true;

        if (fadeTween != null) fadeTween.Kill();

        DOTween.To(() => 0f, SetAlpha, 0.5f, 0.3f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            DOTween.To(() => 0.5f, SetAlpha, 0.5f, duration).OnComplete(() =>
            {
                DOTween.To(() => 0.5f, SetAlpha, 0f, 0.5f).SetEase(Ease.InQuad).OnComplete(() =>
                {
                    isShowing = false;
                });
            });
        });
    }

    public void HideHint()
    {
        if (lineRenderer == null) return;
        if (fadeTween != null) fadeTween.Kill();

        DOTween.To(() => lineRenderer.startColor.a, SetAlpha, 0f, 0.3f).SetEase(Ease.InQuad).OnComplete(() =>
        {
            isShowing = false;
        });
    }

    private void SetAlpha(float alpha)
    {
        if (lineRenderer == null) return;
        Color c = new Color(1f, 1f, 0.3f, alpha);
        lineRenderer.startColor = c;
        lineRenderer.endColor = c;
    }

    private void OnDestroy()
    {
        if (fadeTween != null) fadeTween.Kill();
    }
}
