using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public const int TOTAL_LEVELS = 30;

    public int SelectedLevel { get; set; }

    private const string KEY_MAX_LEVEL = "MaxUnlockedLevel";
    private const string KEY_STARS_PREFIX = "LevelStars_";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
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

    public int GetMaxUnlockedLevel()
    {
        return PlayerPrefs.GetInt(KEY_MAX_LEVEL, 1);
    }

    public void UnlockNextLevel(int completedLevel)
    {
        int current = GetMaxUnlockedLevel();
        if (completedLevel >= current && completedLevel < TOTAL_LEVELS)
        {
            PlayerPrefs.SetInt(KEY_MAX_LEVEL, completedLevel + 1);
            PlayerPrefs.Save();
        }
    }

    public int GetStars(int level)
    {
        return PlayerPrefs.GetInt(KEY_STARS_PREFIX + level, 0);
    }

    public void SetStars(int level, int stars)
    {
        int current = GetStars(level);
        if (stars > current)
        {
            PlayerPrefs.SetInt(KEY_STARS_PREFIX + level, stars);
            PlayerPrefs.Save();
        }
    }

    public void LoadGameScene(int level)
    {
        SelectedLevel = level;
        SceneTransition.Instance.LoadScene("Game");
    }

    public void LoadMainMenu()
    {
        SceneTransition.Instance.LoadScene("MainMenu");
    }
}
