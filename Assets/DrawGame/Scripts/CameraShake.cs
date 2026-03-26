using UnityEngine;
using DG.Tweening;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    private Vector3 originalPosition;
    private bool isShaking;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        originalPosition = transform.position;
    }

    public void ShakeLight()
    {
        if (isShaking) return;
        isShaking = true;
        transform.DOShakePosition(0.15f, 0.08f, 15, 90f).OnComplete(() =>
        {
            transform.position = originalPosition;
            isShaking = false;
        });
    }

    public void ShakeMedium()
    {
        if (isShaking) return;
        isShaking = true;
        transform.DOShakePosition(0.25f, 0.15f, 20, 90f).OnComplete(() =>
        {
            transform.position = originalPosition;
            isShaking = false;
        });
    }

    public void ShakeHeavy()
    {
        if (isShaking) return;
        isShaking = true;
        transform.DOShakePosition(0.35f, 0.25f, 25, 90f).OnComplete(() =>
        {
            transform.position = originalPosition;
            isShaking = false;
        });
    }
}
