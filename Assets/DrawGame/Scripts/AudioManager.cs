using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    private AudioSource musicSource;
    private float targetVolume = 0.5f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.playOnAwake = false;
        musicSource.volume = 0f;
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

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("AudioManager: clip is null!");
            return;
        }

        if (musicSource.clip == clip && musicSource.isPlaying) return;

        musicSource.clip = clip;
        musicSource.volume = 0f;
        musicSource.Play();
        musicSource.DOFade(targetVolume, 1f).SetEase(Ease.InOutQuad);
    }

    public void StopMusic(float fadeDuration = 0.5f)
    {
        musicSource.DOFade(0f, fadeDuration).SetEase(Ease.InOutQuad).OnComplete(() =>
        {
            musicSource.Stop();
        });
    }

    public void SetMusicVolume(float volume)
    {
        targetVolume = volume;
        if (musicSource.isPlaying)
        {
            musicSource.DOFade(targetVolume, 0.3f);
        }
    }
}
