# Iteration 7 — Hint System

## What changed since Iteration 6
- Added HintManager (singleton, DontDestroyOnLoad) that tracks hint count in PlayerPrefs
- 5 free hints on start
- Hint button in top bar shows lightbulb icon + remaining count
- Pressing hint shows yellow semi-transparent silhouette line on the level for 3 seconds
- Silhouette fades in, stays visible, then fades out
- Each hint use costs 1 hint from the counter
- Button disabled when hints = 0 (shakes on tap)
- Each of the 30 levels has unique hint points showing approximate solution path
- HintDisplay created dynamically by LevelSpawner
- Variations (levels 6-30) have hint points adjusted to match their mirrored/offset positions

## New Scripts
- **HintManager.cs** — Singleton, DontDestroyOnLoad. Manages hint count via PlayerPrefs. Starting hints = 5. UseHint() returns bool. AddHints(int). AddPurchasedHints() adds 5. Event OnHintCountChanged.
- **HintDisplay.cs** — LineRenderer-based silhouette. Initialize(Vector2[] points). ShowHint(duration) with fade in/out using DOTween. Yellow semi-transparent color.

## Changed Scripts
- **LevelData.cs** — Added Vector2[] hintPoints field
- **LevelSpawner.cs** — Added HintDisplay spawning. CurrentHintDisplay getter. Clears hint on level clear.
- **GameUI.cs** — Added hint button + count text. OnHintClicked uses HintManager.UseHint() and shows HintDisplay. Subscribes to HintManager.OnHintCountChanged.

## New Editor Script
- **Iteration7_HintSystem.cs**:
  - `DrawGame > Generate Hint Data (Iteration 7)` — Sets hintPoints on all 30 LevelData assets
  - `DrawGame > Add HintManager to Bootstrap (Iteration 7)` — Adds HintManager to Bootstrap scene
  - `DrawGame > Update Game Scene - Hints UI (Iteration 7)` — Adds hint button to TopBar, rewires all GameUI references

## Setup Instructions

### Step 1 — Copy Files
- `Assets/DrawGame/Scripts/HintManager.cs` — new
- `Assets/DrawGame/Scripts/HintDisplay.cs` — new
- `Assets/DrawGame/Scripts/LevelData.cs` — **replace**
- `Assets/DrawGame/Scripts/LevelSpawner.cs` — **replace**
- `Assets/DrawGame/Scripts/GameUI.cs` — **replace**
- `Assets/DrawGame/Editor/Iteration7_HintSystem.cs` — new

### Step 2 — Generate Hint Data
1. Menu: **DrawGame > Generate Hint Data (Iteration 7)**
2. This sets hintPoints on all 30 LevelData assets

### Step 3 — Add HintManager to Bootstrap
1. Open Bootstrap scene
2. Menu: **DrawGame > Add HintManager to Bootstrap (Iteration 7)**
3. Save scene

### Step 4 — Update Game Scene
1. Open Game scene
2. Menu: **DrawGame > Update Game Scene - Hints UI (Iteration 7)**
3. Save scene

## How to Test
1. Start from Bootstrap → MainMenu → Level 1
2. See hint button (yellow with lightbulb) in top bar showing "5"
3. Tap hint button → yellow line silhouette appears on level for 3 seconds, then fades
4. Counter decreases to "4"
5. Use all 5 hints → button becomes non-interactable, shakes on tap
6. Hint silhouette shows approximate path of where to draw
7. Restart level → hint silhouette is gone (can use another hint)
8. Next level → new hint silhouette for that level
9. Close and reopen game → hint count persists

## Expected Result
- Yellow semi-transparent line silhouette showing where to draw
- Smooth fade in (0.3s) → visible (3s) → fade out (0.5s)
- Counter updates immediately
- Different hint paths for each level
- Hint count persists between sessions
- No hint usage during win state
