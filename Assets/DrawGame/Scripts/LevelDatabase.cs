using UnityEngine;

[CreateAssetMenu(fileName = "LevelDatabase", menuName = "DrawGame/Level Database")]
public class LevelDatabase : ScriptableObject
{
    public LevelData[] levels;

    public LevelData GetLevel(int levelNumber)
    {
        if (levelNumber < 1 || levelNumber > levels.Length)
        {
            Debug.LogWarning("LevelDatabase: level " + levelNumber + " out of range!");
            return null;
        }
        return levels[levelNumber - 1];
    }
}
