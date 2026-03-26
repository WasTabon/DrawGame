using UnityEngine;
using UnityEngine.SceneManagement;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance { get; private set; }

    private AudioSource sfxSource;
    private AudioClip drawClip;
    private AudioClip freezeClip;
    private AudioClip winClip;
    private AudioClip clickClip;
    private AudioClip errorClip;
    private AudioClip starClip;

    [SerializeField] private float sfxVolume = 0.5f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;

        GenerateAllClips();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
    }

    public void PlayDraw()
    {
        sfxSource.PlayOneShot(drawClip, sfxVolume * 0.3f);
    }

    public void PlayFreeze()
    {
        sfxSource.PlayOneShot(freezeClip, sfxVolume * 0.7f);
    }

    public void PlayWin()
    {
        sfxSource.PlayOneShot(winClip, sfxVolume);
    }

    public void PlayClick()
    {
        sfxSource.PlayOneShot(clickClip, sfxVolume * 0.5f);
    }

    public void PlayError()
    {
        sfxSource.PlayOneShot(errorClip, sfxVolume * 0.5f);
    }

    public void PlayStar()
    {
        sfxSource.PlayOneShot(starClip, sfxVolume * 0.6f);
    }

    public void SetVolume(float volume)
    {
        sfxVolume = volume;
    }

    private void GenerateAllClips()
    {
        drawClip = GenerateDrawSound();
        freezeClip = GenerateFreezeSound();
        winClip = GenerateWinSound();
        clickClip = GenerateClickSound();
        errorClip = GenerateErrorSound();
        starClip = GenerateStarSound();
    }

    private AudioClip GenerateDrawSound()
    {
        int sampleRate = 44100;
        int samples = sampleRate / 15;
        var clip = AudioClip.Create("Draw", samples, 1, sampleRate, false);
        float[] data = new float[samples];

        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / samples;
            float envelope = 1f - t;
            float noise = (Random.value * 2f - 1f) * 0.15f;
            float tone = Mathf.Sin(2f * Mathf.PI * 800f * t) * 0.1f;
            data[i] = (noise + tone) * envelope;
        }

        clip.SetData(data, 0);
        return clip;
    }

    private AudioClip GenerateFreezeSound()
    {
        int sampleRate = 44100;
        int samples = sampleRate / 6;
        var clip = AudioClip.Create("Freeze", samples, 1, sampleRate, false);
        float[] data = new float[samples];

        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / samples;
            float envelope = (1f - t) * (1f - t);
            float freq = Mathf.Lerp(400f, 150f, t);
            float tone = Mathf.Sin(2f * Mathf.PI * freq * t) * 0.4f;
            float noise = (Random.value * 2f - 1f) * 0.1f * (1f - t);
            data[i] = (tone + noise) * envelope;
        }

        clip.SetData(data, 0);
        return clip;
    }

    private AudioClip GenerateWinSound()
    {
        int sampleRate = 44100;
        int samples = sampleRate;
        var clip = AudioClip.Create("Win", samples, 1, sampleRate, false);
        float[] data = new float[samples];

        float[] notes = { 523.25f, 659.25f, 783.99f, 1046.50f };
        int noteLength = samples / notes.Length;

        for (int n = 0; n < notes.Length; n++)
        {
            for (int i = 0; i < noteLength; i++)
            {
                int idx = n * noteLength + i;
                if (idx >= samples) break;

                float t = (float)i / noteLength;
                float globalT = (float)idx / samples;
                float envelope = Mathf.Sin(t * Mathf.PI) * (1f - globalT * 0.3f);
                float tone = Mathf.Sin(2f * Mathf.PI * notes[n] * globalT) * 0.3f;
                float harmonic = Mathf.Sin(2f * Mathf.PI * notes[n] * 2f * globalT) * 0.1f;
                data[idx] = (tone + harmonic) * envelope;
            }
        }

        clip.SetData(data, 0);
        return clip;
    }

    private AudioClip GenerateClickSound()
    {
        int sampleRate = 44100;
        int samples = sampleRate / 20;
        var clip = AudioClip.Create("Click", samples, 1, sampleRate, false);
        float[] data = new float[samples];

        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / samples;
            float envelope = Mathf.Pow(1f - t, 3f);
            float tone = Mathf.Sin(2f * Mathf.PI * 1200f * t) * 0.3f;
            float tone2 = Mathf.Sin(2f * Mathf.PI * 1800f * t) * 0.15f;
            data[i] = (tone + tone2) * envelope;
        }

        clip.SetData(data, 0);
        return clip;
    }

    private AudioClip GenerateErrorSound()
    {
        int sampleRate = 44100;
        int samples = sampleRate / 4;
        var clip = AudioClip.Create("Error", samples, 1, sampleRate, false);
        float[] data = new float[samples];

        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / samples;
            float envelope = (1f - t) * (1f - t);
            float freq = Mathf.Lerp(300f, 150f, t);
            float tone = Mathf.Sin(2f * Mathf.PI * freq * t) * 0.3f;
            data[i] = tone * envelope;
        }

        clip.SetData(data, 0);
        return clip;
    }

    private AudioClip GenerateStarSound()
    {
        int sampleRate = 44100;
        int samples = sampleRate / 4;
        var clip = AudioClip.Create("Star", samples, 1, sampleRate, false);
        float[] data = new float[samples];

        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / samples;
            float envelope = Mathf.Sin(t * Mathf.PI);
            float freq = Mathf.Lerp(600f, 1200f, t * 0.5f);
            float tone = Mathf.Sin(2f * Mathf.PI * freq * t) * 0.25f;
            float shimmer = Mathf.Sin(2f * Mathf.PI * freq * 3f * t) * 0.08f;
            data[i] = (tone + shimmer) * envelope;
        }

        clip.SetData(data, 0);
        return clip;
    }
}
