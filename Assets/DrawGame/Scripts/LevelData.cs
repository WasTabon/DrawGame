using UnityEngine;
using System;

[Serializable]
public enum LevelObjectShape
{
    Box,
    Circle
}

[Serializable]
public class LevelObjectData
{
    public string name;
    public LevelObjectType objectType;
    public LevelObjectShape shape;
    public Vector2 position;
    public Vector2 size;
    public float rotation;
    public Color color;
    public bool isGoalTarget;
    public float mass;
    public float bounciness;
}

[Serializable]
public class GoalZoneData
{
    public Vector2 position;
    public Vector2 size;
    public GoalType goalType;
    public float holdDuration;
    public Color color;
}

[CreateAssetMenu(fileName = "LevelData", menuName = "DrawGame/Level Data")]
public class LevelData : ScriptableObject
{
    public int levelNumber;
    public int maxLines = 3;
    public int idealLines = 1;
    public float idealTime = 15f;
    public LevelObjectData[] objects;
    public GoalZoneData goalZone;
    public Vector2[] hintPoints;
}
