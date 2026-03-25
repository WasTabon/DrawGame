# Iteration 5 — 5 Unique Levels + 25 Variations + Level System

## What changed since Iteration 4
- Added LevelData ScriptableObject system for level configs
- Added LevelDatabase holding all 30 levels
- Added LevelSpawner that dynamically spawns levels from data
- LevelController no longer has serialized references — gets data from LevelSpawner
- 5 unique hand-designed levels + 25 variations (mirrored, shifted, fewer lines)
- "LEVEL COMPLETE" panel now has Next Level and Restart buttons
- Level text updates when switching levels
- Completing a level unlocks the next one
- Old test LevelObjects removed — levels now spawn dynamically

## New Scripts
- **LevelData.cs** — ScriptableObject. Stores: level number, maxLines, array of LevelObjectData (name, type, shape, position, size, rotation, color, isGoalTarget, mass, bounciness), GoalZoneData (position, size, type, holdDuration, color).
- **LevelDatabase.cs** — ScriptableObject. Array of 30 LevelData references. GetLevel(int) method.
- **LevelSpawner.cs** — Singleton. Reads LevelData, spawns all objects and goal zone at runtime. Creates sprites procedurally. Connects to LevelController.

## Changed Scripts
- **LevelController.cs** — Added SetupLevel(GoalZone, LevelObject[]) for runtime wiring. Added LoadNextLevel(). Unlocks next level on completion. No more serialized references.
- **GoalZone.cs** — Added Init(GoalType, holdDuration) method for runtime configuration.
- **GameUI.cs** — Added Next Level and Restart buttons in win panel. Level text updates on level change. Line counter resets properly.

## New Editor Script
- **Iteration5_LevelGenerator.cs**:
  - `DrawGame > Generate All Levels (Iteration 5)` — Creates 30 LevelData assets + LevelDatabase in Assets/DrawGame/Data/
  - `DrawGame > Update Game Scene - Levels (Iteration 5)` — Adds LevelSpawner, removes old LevelObjects, adds win panel buttons

## 5 Unique Levels
1. **Simple Push** — Ball on platform, push into zone at bottom-right (3 lines)
2. **Drop from Height** — Ball on high platform, obstacle below, guide ball to bottom zone (2 lines)
3. **Behind a Wall** — Ball behind vertical wall, draw ramp over it (3 lines)
4. **Ramp Challenge** — Ball on angled platform, guide to zone (2 lines)
5. **Cup Escape** — Ball trapped in a cup shape, tip it out to zone (3 lines)

Levels 6–30 are variations: mirrored horizontally, slightly shifted positions, sometimes fewer lines allowed.

## Setup Instructions

### Step 1 — Copy Files
- `Assets/DrawGame/Scripts/LevelData.cs` — new
- `Assets/DrawGame/Scripts/LevelDatabase.cs` — new
- `Assets/DrawGame/Scripts/LevelSpawner.cs` — new
- `Assets/DrawGame/Scripts/LevelController.cs` — **replace**
- `Assets/DrawGame/Scripts/GoalZone.cs` — **replace**
- `Assets/DrawGame/Scripts/GameUI.cs` — **replace**
- `Assets/DrawGame/Editor/Iteration5_LevelGenerator.cs` — new

### Step 2 — Generate Levels
1. Menu: **DrawGame > Generate All Levels (Iteration 5)**
2. Wait for "Generated 30 levels" log message
3. Check `Assets/DrawGame/Data/` — should have 30 Level_XX assets + LevelDatabase

### Step 3 — Update Game Scene
1. Open Game scene
2. Menu: **DrawGame > Update Game Scene - Levels (Iteration 5)**
3. This removes old test LevelObjects, adds LevelSpawner (wired to LevelDatabase), adds win buttons

### Step 4 — Verify
Make sure LevelSpawner in Game scene has LevelDatabase assigned in Inspector.

## How to Test
1. Start from Bootstrap, go to LevelSelect, pick Level 1
2. Level 1 loads: ball on platform, goal zone at bottom-right
3. Draw to push ball into zone → "LEVEL COMPLETE!" with Next Level / Restart buttons
4. Click Next Level → Level 2 loads (ball on high platform with obstacle)
5. Complete level 2 → go back to LevelSelect → Level 3 should be unlocked
6. Test Restart during gameplay and from win panel
7. Try levels 3-5 to see different layouts
8. Pick level 6+ to see variations of base levels

## Expected Result
- All 30 levels load dynamically from ScriptableObject data
- 5 distinct level layouts with increasing challenge
- Levels 6-30 are recognizable variations of levels 1-5
- Next Level flow works without scene reload
- Progress saves (unlocked levels persist between sessions)
- Win panel has animated buttons
