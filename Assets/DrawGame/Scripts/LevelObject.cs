using UnityEngine;

public enum LevelObjectType
{
    Static,
    Dynamic,
    Destructible
}

public class LevelObject : MonoBehaviour
{
    [SerializeField] private LevelObjectType objectType = LevelObjectType.Static;
    [SerializeField] private bool isGoalTarget;

    public LevelObjectType ObjectType => objectType;
    public bool IsGoalTarget => isGoalTarget;

    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Vector3 initialScale;
    private Rigidbody2D rb;

    private void Awake()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        initialScale = transform.localScale;
        rb = GetComponent<Rigidbody2D>();
    }

    public void ResetToInitial()
    {
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        transform.localScale = initialScale;

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        gameObject.SetActive(true);
    }

    public void SetAsGoalTarget(bool value)
    {
        isGoalTarget = value;
    }
}