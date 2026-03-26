using UnityEngine;

public class ParticleSpawner : MonoBehaviour
{
    public static ParticleSpawner Instance { get; private set; }

    private ParticleSystem drawTrailPS;
    private ParticleSystem freezeBurstPS;
    private ParticleSystem winConfettiPS;
    private ParticleSystem goalGlowPS;

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
        CreateDrawTrail();
        CreateFreezeBurst();
        CreateWinConfetti();
        CreateGoalGlow();
    }

    public void EmitDrawTrail(Vector3 position)
    {
        drawTrailPS.transform.position = position;
        drawTrailPS.Emit(1);
    }

    public void EmitFreezeBurst(Vector3 position)
    {
        freezeBurstPS.transform.position = position;
        freezeBurstPS.Emit(12);
    }

    public void EmitWinConfetti(Vector3 position)
    {
        winConfettiPS.transform.position = position;
        winConfettiPS.Emit(40);
    }

    public void EmitGoalGlow(Vector3 position)
    {
        goalGlowPS.transform.position = position;
        goalGlowPS.Emit(8);
    }

    private void CreateDrawTrail()
    {
        var go = new GameObject("DrawTrailPS");
        go.transform.SetParent(transform);
        drawTrailPS = go.AddComponent<ParticleSystem>();
        var emission = drawTrailPS.emission;
        emission.enabled = false;

        var main = drawTrailPS.main;
        main.startLifetime = 0.4f;
        main.startSpeed = 0.5f;
        main.startSize = new ParticleSystem.MinMaxCurve(0.05f, 0.12f);
        main.startColor = new Color(0.3f, 0.7f, 1f, 0.6f);
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.maxParticles = 100;
        main.gravityModifier = 0f;

        var sizeOverLifetime = drawTrailPS.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.Linear(0f, 1f, 1f, 0f));

        var colorOverLifetime = drawTrailPS.colorOverLifetime;
        colorOverLifetime.enabled = true;
        var gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(Color.white, 1f) },
            new GradientAlphaKey[] { new GradientAlphaKey(0.6f, 0f), new GradientAlphaKey(0f, 1f) }
        );
        colorOverLifetime.color = gradient;

        var renderer = go.GetComponent<ParticleSystemRenderer>();
        renderer.material = new Material(Shader.Find("Sprites/Default"));
        renderer.sortingOrder = 15;
    }

    private void CreateFreezeBurst()
    {
        var go = new GameObject("FreezeBurstPS");
        go.transform.SetParent(transform);
        freezeBurstPS = go.AddComponent<ParticleSystem>();
        var emission = freezeBurstPS.emission;
        emission.enabled = false;

        var main = freezeBurstPS.main;
        main.startLifetime = 0.5f;
        main.startSpeed = new ParticleSystem.MinMaxCurve(2f, 4f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.06f, 0.15f);
        main.startColor = new Color(0.5f, 0.5f, 0.6f, 0.8f);
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.maxParticles = 200;
        main.gravityModifier = 1f;

        var shape = freezeBurstPS.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 0.3f;

        var sizeOverLifetime = freezeBurstPS.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.Linear(0f, 1f, 1f, 0f));

        var renderer = go.GetComponent<ParticleSystemRenderer>();
        renderer.material = new Material(Shader.Find("Sprites/Default"));
        renderer.sortingOrder = 15;
    }

    private void CreateWinConfetti()
    {
        var go = new GameObject("WinConfettiPS");
        go.transform.SetParent(transform);
        winConfettiPS = go.AddComponent<ParticleSystem>();
        var emission = winConfettiPS.emission;
        emission.enabled = false;

        var main = winConfettiPS.main;
        main.startLifetime = new ParticleSystem.MinMaxCurve(1.5f, 2.5f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(3f, 6f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.1f, 0.25f);
        main.startRotation = new ParticleSystem.MinMaxCurve(0f, Mathf.PI * 2f);
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.maxParticles = 200;
        main.gravityModifier = 1.5f;

        var startColor = main.startColor;
        var confettiGradient = new Gradient();
        confettiGradient.SetKeys(
            new GradientColorKey[]
            {
                new GradientColorKey(new Color(1f, 0.85f, 0.2f), 0f),
                new GradientColorKey(new Color(0.3f, 0.85f, 0.5f), 0.25f),
                new GradientColorKey(new Color(0.3f, 0.7f, 1f), 0.5f),
                new GradientColorKey(new Color(1f, 0.4f, 0.4f), 0.75f),
                new GradientColorKey(new Color(0.8f, 0.4f, 1f), 1f)
            },
            new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) }
        );
        main.startColor = new ParticleSystem.MinMaxGradient(confettiGradient);

        var shape = winConfettiPS.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 1f;

        var rotOverLifetime = winConfettiPS.rotationOverLifetime;
        rotOverLifetime.enabled = true;
        rotOverLifetime.z = new ParticleSystem.MinMaxCurve(-3f, 3f);

        var renderer = go.GetComponent<ParticleSystemRenderer>();
        renderer.material = new Material(Shader.Find("Sprites/Default"));
        renderer.sortingOrder = 20;
    }

    private void CreateGoalGlow()
    {
        var go = new GameObject("GoalGlowPS");
        go.transform.SetParent(transform);
        goalGlowPS = go.AddComponent<ParticleSystem>();
        var emission = goalGlowPS.emission;
        emission.enabled = false;

        var main = goalGlowPS.main;
        main.startLifetime = 0.6f;
        main.startSpeed = new ParticleSystem.MinMaxCurve(1f, 2f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.08f, 0.18f);
        main.startColor = new Color(0.2f, 1f, 0.4f, 0.7f);
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.maxParticles = 100;
        main.gravityModifier = -0.5f;

        var shape = goalGlowPS.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 0.5f;

        var sizeOverLifetime = goalGlowPS.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.Linear(0f, 1f, 1f, 0f));

        var renderer = go.GetComponent<ParticleSystemRenderer>();
        renderer.material = new Material(Shader.Find("Sprites/Default"));
        renderer.sortingOrder = 15;
    }
}
