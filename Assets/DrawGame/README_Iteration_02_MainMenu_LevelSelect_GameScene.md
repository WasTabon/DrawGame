# Iteration 2 — MainMenu, LevelSelect, Game Scene

## What changed since Iteration 1
- Added MainMenu scene with title and Play button
- Added LevelSelect panel with vertical ScrollView and 30 level buttons
- Added Game scene with top bar (Back button, level text)
- Added GameManager (singleton, DontDestroyOnLoad) for progress tracking
- LevelButton saved as prefab in `Assets/DrawGame/Prefabs/LevelButtonPrefab.prefab`

## New Scripts
- **GameManager.cs** — Singleton, DontDestroyOnLoad. Stores selected level, saves progress (max unlocked level, stars per level) in PlayerPrefs.
- **MainMenuUI.cs** — Main menu panel with Play button. Animated entrance (title slides down, play button scales in). Switches between MainMenu and LevelSelect panels with fade.
- **LevelSelectUI.cs** — Creates 30 buttons from prefab in ScrollView. Refreshes button states (locked/unlocked/stars) on enable. Back button returns to main menu.
- **LevelButton.cs** — Prefab component. Shows level number, lock icon, up to 3 stars. Color-coded: blue=unlocked, green=completed, gray=locked. Tap animation before loading level.
- **GameUI.cs** — Game scene UI with Back button and level text display.

## New Editor Scripts
- **Iteration2_MainMenuSetup.cs**:
  - `DrawGame > Setup MainMenu Scene (Iteration 2)` — creates MainMenu canvas with both panels
  - `DrawGame > Add GameManager to Bootstrap (Iteration 2)` — adds GameManager to Bootstrap scene
- **Iteration2_GameSceneSetup.cs**:
  - `DrawGame > Setup Game Scene (Iteration 2)` — creates Game canvas with top bar

## Setup Instructions

### Step 1 — Copy Files
Copy all files maintaining folder structure:
- `Assets/DrawGame/Scripts/` — 4 new scripts
- `Assets/DrawGame/Editor/` — 2 new editor scripts

### Step 2 — Add GameManager to Bootstrap
1. Open Bootstrap scene
2. Menu: **DrawGame > Add GameManager to Bootstrap (Iteration 2)**
3. Save scene

### Step 3 — Setup MainMenu Scene
1. Open MainMenu scene
2. Menu: **DrawGame > Setup MainMenu Scene (Iteration 2)**
3. This will create the canvas, both panels, ScrollView, and save `LevelButtonPrefab` to `Assets/DrawGame/Prefabs/`

### Step 4 — Setup Game Scene
1. Open Game scene
2. Menu: **DrawGame > Setup Game Scene (Iteration 2)**

### Step 5 — Verify Build Settings
Scenes should already be in Build Settings from Iteration 1:
- 0: Bootstrap
- 1: MainMenu
- 2: Game

## How to Test
1. Open Bootstrap scene, press Play
2. Loading completes → fades to MainMenu
3. See "DRAW GAME" title with animated entrance and "PLAY" button
4. Click PLAY → fades to LevelSelect panel
5. See 30 level buttons in a 4-column grid (level 1 is blue/unlocked, rest are gray/locked)
6. Click level 1 → button press animation → fades to Game scene
7. See "Level 1" in top bar
8. Click Back ("<") → fades back to MainMenu
9. In LevelSelect, click Back ("<") → returns to main menu panel

## Expected Result
- Full navigation flow: Bootstrap → MainMenu → LevelSelect → Game → MainMenu
- Animated UI transitions everywhere (fades, scale, slide)
- ScrollView with 4 columns of level buttons
- Only level 1 unlocked by default
- LevelButtonPrefab saved in `Assets/DrawGame/Prefabs/` — change art there, all 30 buttons update

## LevelButton Prefab
Located at: `Assets/DrawGame/Prefabs/LevelButtonPrefab.prefab`
Structure:
- LevelButton (Image + Button + LevelButton script)
  - LevelNumber (TMP text)
  - LockIcon (TMP text with lock emoji)
  - Stars
    - Star1 (TMP ★)
    - Star2 (TMP ★)
    - Star3 (TMP ★)

Replace Image component sprite/color and text styling as needed. All 30 instances reference this prefab.
