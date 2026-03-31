using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class Fix_RegenerateLevels
{
    private static readonly Color platformColor = new Color(0.4f, 0.4f, 0.45f, 1f);
    private static readonly Color wallColor = new Color(0.5f, 0.45f, 0.4f, 1f);
    private static readonly Color ballColor = new Color(1f, 0.5f, 0.2f, 1f);
    private static readonly Color goalColor = new Color(0.2f, 0.9f, 0.3f, 0.4f);

    [MenuItem("DrawGame/Regenerate All Levels (Fix 3-4-5)")]
    public static void RegenerateAllLevels()
    {
        string dataPath = "Assets/DrawGame/Data";
        System.IO.Directory.CreateDirectory(dataPath);

        string dbPath = dataPath + "/LevelDatabase.asset";
        var existingDb = AssetDatabase.LoadAssetAtPath<LevelDatabase>(dbPath);

        for (int i = 1; i <= 30; i++)
        {
            string path = dataPath + "/Level_" + i.ToString("D2") + ".asset";
            var existing = AssetDatabase.LoadAssetAtPath<LevelData>(path);
            if (existing != null)
                AssetDatabase.DeleteAsset(path);
        }
        if (existingDb != null)
            AssetDatabase.DeleteAsset(dbPath);

        LevelData[] baseLevels = new LevelData[5];
        baseLevels[0] = CreateLevel1();
        baseLevels[1] = CreateLevel2();
        baseLevels[2] = CreateLevel3();
        baseLevels[3] = CreateLevel4();
        baseLevels[4] = CreateLevel5();

        for (int i = 0; i < 5; i++)
        {
            baseLevels[i].levelNumber = i + 1;
            baseLevels[i].hintPoints = GetBaseHintPoints(i + 1);

            int idealLines = 1;
            float idealTime = 15f;
            switch (i + 1)
            {
                case 1: idealLines = 1; idealTime = 12f; break;
                case 2: idealLines = 1; idealTime = 10f; break;
                case 3: idealLines = 1; idealTime = 12f; break;
                case 4: idealLines = 1; idealTime = 10f; break;
                case 5: idealLines = 1; idealTime = 12f; break;
            }
            baseLevels[i].idealLines = idealLines;
            baseLevels[i].idealTime = idealTime;

            AssetDatabase.CreateAsset(baseLevels[i], dataPath + "/Level_" + (i + 1).ToString("D2") + ".asset");
        }

        LevelData[] allLevels = new LevelData[30];
        for (int i = 0; i < 5; i++)
            allLevels[i] = baseLevels[i];

        for (int i = 5; i < 30; i++)
        {
            int baseIndex = (i - 5) % 5;
            int variationIndex = (i - 5) / 5;
            var variation = CreateVariation(baseLevels[baseIndex], i + 1, variationIndex);
            AssetDatabase.CreateAsset(variation, dataPath + "/Level_" + (i + 1).ToString("D2") + ".asset");
            allLevels[i] = variation;
        }

        var database = ScriptableObject.CreateInstance<LevelDatabase>();
        database.levels = allLevels;
        AssetDatabase.CreateAsset(database, dbPath);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Regenerated all 30 levels with fixed levels 3, 4, 5!");
    }

    private static LevelData CreateLevel1()
    {
        var data = ScriptableObject.CreateInstance<LevelData>();
        data.maxLines = 3;
        data.objects = new LevelObjectData[]
        {
            new LevelObjectData
            {
                name = "Platform",
                objectType = LevelObjectType.Static,
                shape = LevelObjectShape.Box,
                position = new Vector2(-0.5f, -1f),
                size = new Vector2(3f, 3f),
                color = platformColor
            },
            new LevelObjectData
            {
                name = "Ball",
                objectType = LevelObjectType.Dynamic,
                shape = LevelObjectShape.Circle,
                position = new Vector2(-0.5f, 0.2f),
                size = new Vector2(0.8f, 0.8f),
                color = ballColor,
                isGoalTarget = true,
                mass = 1f,
                bounciness = 0.3f
            }
        };
        data.goalZone = new GoalZoneData
        {
            position = new Vector2(1.5f, -3.5f),
            size = new Vector2(1.5f, 1.5f),
            goalType = GoalType.ReachZone,
            color = goalColor
        };
        return data;
    }

    private static LevelData CreateLevel2()
    {
        var data = ScriptableObject.CreateInstance<LevelData>();
        data.maxLines = 2;
        data.objects = new LevelObjectData[]
        {
            new LevelObjectData
            {
                name = "HighPlatform",
                objectType = LevelObjectType.Static,
                shape = LevelObjectShape.Box,
                position = new Vector2(0f, 2f),
                size = new Vector2(2.5f, 2.5f),
                color = platformColor
            },
            new LevelObjectData
            {
                name = "Ball",
                objectType = LevelObjectType.Dynamic,
                shape = LevelObjectShape.Circle,
                position = new Vector2(0f, 3f),
                size = new Vector2(0.8f, 0.8f),
                color = ballColor,
                isGoalTarget = true,
                mass = 1f,
                bounciness = 0.3f
            },
            new LevelObjectData
            {
                name = "Obstacle",
                objectType = LevelObjectType.Static,
                shape = LevelObjectShape.Box,
                position = new Vector2(0f, -0.5f),
                size = new Vector2(2f, 2f),
                color = wallColor
            }
        };
        data.goalZone = new GoalZoneData
        {
            position = new Vector2(0f, -3.5f),
            size = new Vector2(2f, 2f),
            goalType = GoalType.ReachZone,
            color = goalColor
        };
        return data;
    }

    private static LevelData CreateLevel3()
    {
        var data = ScriptableObject.CreateInstance<LevelData>();
        data.maxLines = 3;
        data.objects = new LevelObjectData[]
        {
            new LevelObjectData
            {
                name = "Platform",
                objectType = LevelObjectType.Static,
                shape = LevelObjectShape.Box,
                position = new Vector2(0f, 0f),
                size = new Vector2(2f, 2f),
                color = platformColor
            },
            new LevelObjectData
            {
                name = "Ball",
                objectType = LevelObjectType.Dynamic,
                shape = LevelObjectShape.Circle,
                position = new Vector2(0f, 1f),
                size = new Vector2(0.8f, 0.8f),
                color = ballColor,
                isGoalTarget = true,
                mass = 1f,
                bounciness = 0.3f
            }
        };
        data.goalZone = new GoalZoneData
        {
            position = new Vector2(0f, -3.5f),
            size = new Vector2(2.5f, 2.5f),
            goalType = GoalType.ReachZone,
            color = goalColor
        };
        return data;
    }

    private static LevelData CreateLevel4()
    {
        var data = ScriptableObject.CreateInstance<LevelData>();
        data.maxLines = 2;
        data.objects = new LevelObjectData[]
        {
            new LevelObjectData
            {
                name = "SmallPlatform",
                objectType = LevelObjectType.Static,
                shape = LevelObjectShape.Box,
                position = new Vector2(-1f, 0.5f),
                size = new Vector2(1.5f, 1.5f),
                color = platformColor
            },
            new LevelObjectData
            {
                name = "Ball",
                objectType = LevelObjectType.Dynamic,
                shape = LevelObjectShape.Circle,
                position = new Vector2(-1f, 1.5f),
                size = new Vector2(0.8f, 0.8f),
                color = ballColor,
                isGoalTarget = true,
                mass = 1f,
                bounciness = 0.3f
            }
        };
        data.goalZone = new GoalZoneData
        {
            position = new Vector2(1.2f, -3.5f),
            size = new Vector2(2f, 2f),
            goalType = GoalType.ReachZone,
            color = goalColor
        };
        return data;
    }

    private static LevelData CreateLevel5()
    {
        var data = ScriptableObject.CreateInstance<LevelData>();
        data.maxLines = 2;
        data.objects = new LevelObjectData[]
        {
            new LevelObjectData
            {
                name = "Platform",
                objectType = LevelObjectType.Static,
                shape = LevelObjectShape.Box,
                position = new Vector2(1f, 0.5f),
                size = new Vector2(1.5f, 1.5f),
                color = platformColor
            },
            new LevelObjectData
            {
                name = "Ball",
                objectType = LevelObjectType.Dynamic,
                shape = LevelObjectShape.Circle,
                position = new Vector2(1f, 1.5f),
                size = new Vector2(0.8f, 0.8f),
                color = ballColor,
                isGoalTarget = true,
                mass = 1f,
                bounciness = 0.3f
            }
        };
        data.goalZone = new GoalZoneData
        {
            position = new Vector2(-1.2f, -3.5f),
            size = new Vector2(2f, 2f),
            goalType = GoalType.ReachZone,
            color = goalColor
        };
        return data;
    }

    private static LevelData CreateVariation(LevelData baseLevel, int newLevelNumber, int variationIndex)
    {
        var data = ScriptableObject.CreateInstance<LevelData>();
        data.levelNumber = newLevelNumber;

        bool mirrorX = variationIndex % 2 == 0;
        float offsetX = (variationIndex % 3 - 1) * 0.3f;
        float offsetY = (variationIndex % 2) * 0.3f;
        int maxLinesBonus = variationIndex >= 3 ? -1 : 0;

        data.maxLines = Mathf.Max(1, baseLevel.maxLines + maxLinesBonus);
        data.idealLines = baseLevel.idealLines;
        data.idealTime = Mathf.Max(8f, baseLevel.idealTime - variationIndex * 1f);

        if (baseLevel.objects != null)
        {
            data.objects = new LevelObjectData[baseLevel.objects.Length];
            for (int i = 0; i < baseLevel.objects.Length; i++)
            {
                var src = baseLevel.objects[i];
                var dst = new LevelObjectData();
                dst.name = src.name;
                dst.objectType = src.objectType;
                dst.shape = src.shape;
                float px = mirrorX ? -src.position.x : src.position.x;
                float py = src.position.y;
                px = Mathf.Clamp(px + offsetX, -2.3f, 2.3f);
                py = Mathf.Clamp(py + offsetY, -4f, 4f);
                dst.position = new Vector2(px, py);
                dst.size = src.size;
                dst.rotation = mirrorX ? -src.rotation : src.rotation;
                dst.color = src.color;
                dst.isGoalTarget = src.isGoalTarget;
                dst.mass = src.mass;
                dst.bounciness = src.bounciness;
                data.objects[i] = dst;
            }
        }

        if (baseLevel.goalZone != null)
        {
            data.goalZone = new GoalZoneData();
            float gx = mirrorX ? -baseLevel.goalZone.position.x : baseLevel.goalZone.position.x;
            float gy = baseLevel.goalZone.position.y;
            gx = Mathf.Clamp(gx + offsetX, -2.3f, 2.3f);
            gy = Mathf.Clamp(gy, -4.2f, 4f);
            data.goalZone.position = new Vector2(gx, gy);
            data.goalZone.size = baseLevel.goalZone.size;
            data.goalZone.goalType = baseLevel.goalZone.goalType;
            data.goalZone.holdDuration = baseLevel.goalZone.holdDuration;
            data.goalZone.color = baseLevel.goalZone.color;
        }

        if (baseLevel.hintPoints != null)
        {
            data.hintPoints = new Vector2[baseLevel.hintPoints.Length];
            for (int i = 0; i < baseLevel.hintPoints.Length; i++)
            {
                float hx = mirrorX ? -baseLevel.hintPoints[i].x : baseLevel.hintPoints[i].x;
                float hy = baseLevel.hintPoints[i].y;
                data.hintPoints[i] = new Vector2(hx + offsetX, hy + offsetY);
            }
        }

        return data;
    }

    private static Vector2[] GetBaseHintPoints(int baseLevel)
    {
        switch (baseLevel)
        {
            case 1:
                return new Vector2[]
                {
                    new Vector2(-0.3f, 1.5f),
                    new Vector2(0.2f, 1.3f),
                    new Vector2(0.7f, 1.0f),
                    new Vector2(1.2f, 0.5f),
                    new Vector2(1.5f, 0f)
                };
            case 2:
                return new Vector2[]
                {
                    new Vector2(-0.8f, 3.5f),
                    new Vector2(-0.3f, 3.3f),
                    new Vector2(0.2f, 3.5f),
                    new Vector2(0.7f, 3.3f)
                };
            case 3:
                return new Vector2[]
                {
                    new Vector2(-0.3f, 2f),
                    new Vector2(0f, 1.8f),
                    new Vector2(0.3f, 2f)
                };
            case 4:
                return new Vector2[]
                {
                    new Vector2(-0.5f, 2.5f),
                    new Vector2(-0.8f, 2.2f),
                    new Vector2(-1.2f, 2f)
                };
            case 5:
                return new Vector2[]
                {
                    new Vector2(0.5f, 2.5f),
                    new Vector2(0.8f, 2.2f),
                    new Vector2(1.2f, 2f)
                };
            default:
                return new Vector2[]
                {
                    new Vector2(-0.5f, 1f),
                    new Vector2(0.5f, 1f)
                };
        }
    }
}
