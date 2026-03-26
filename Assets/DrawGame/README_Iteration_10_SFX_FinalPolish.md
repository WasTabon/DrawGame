# Iteration 10 — SFX + Final Polish (FINAL)

## What changed since Iteration 9
- Added SFXManager with 6 procedurally generated sound effects (no audio files needed)
- Sound effects on: drawing, line freeze, level win, button clicks, errors, star appearance
- Draw SFX plays every 4th point (not every point, to avoid spamming)
- Error sound when trying to draw beyond max lines
- Star chime plays as each star appears in win panel
- All UI buttons have click sounds
- Back button in level select has click sound

## New Scripts
- **SFXManager.cs** — Singleton, DontDestroyOnLoad. Generates 6 AudioClips procedurally at Awake using AudioClip.Create + waveform synthesis:
  - **Draw** — soft high-frequency noise + tone, very short
  - **Freeze** — descending tone with noise, medium
  - **Win** — ascending 4-note arpeggio (C5→E5→G5→C6) with harmonics
  - **Click** — sharp high-frequency tick
  - **Error** — low descending tone
  - **Star** — ascending shimmer tone

## Changed Scripts
- **DrawnLine.cs** — PlayDraw() every 4th point added, PlayFreeze() on freeze
- **LevelController.cs** — PlayWin() in win effects
- **LevelButton.cs** — PlayClick() on level button tap
- **MainMenuUI.cs** — PlayClick() on Play and Back
- **GameUI.cs** — PlayClick() on all buttons (Back, Restart, NextLevel, WinRestart, Hint). PlayError() when max lines reached. PlayStar() when each star animates in win panel.

## New Editor Script
- **Iteration10_FinalPolish.cs** — `DrawGame > Add SFXManager to Bootstrap (Iteration 10)`

## Setup Instructions

### Step 1 — Copy Files
- `Assets/DrawGame/Scripts/SFXManager.cs` — new
- `Assets/DrawGame/Scripts/DrawnLine.cs` — **replace**
- `Assets/DrawGame/Scripts/LevelController.cs` — **replace**
- `Assets/DrawGame/Scripts/LevelButton.cs` — **replace**
- `Assets/DrawGame/Scripts/MainMenuUI.cs` — **replace**
- `Assets/DrawGame/Scripts/GameUI.cs` — **replace**
- `Assets/DrawGame/Editor/Iteration10_FinalPolish.cs` — new

### Step 2 — Add SFXManager to Bootstrap
1. Open Bootstrap scene
2. Menu: **DrawGame > Add SFXManager to Bootstrap (Iteration 10)**
3. Save scene

## How to Test
1. Start from Bootstrap → hear nothing yet (loading screen)
2. Go to MainMenu → click Play → hear click sound
3. Select level → hear click sound
4. In game: draw a line → hear soft draw sounds
5. Release line → hear freeze "thunk" sound
6. Try to draw when at max lines → hear error sound + red counter
7. Push ball into goal → hear win arpeggio
8. Win panel → hear star chimes as each star appears
9. Click Next Level / Restart → hear click sounds

## Expected Result
- Every interaction has audio feedback
- Sounds are procedural — no audio files needed in project
- Draw sound is subtle and not annoying (plays every 4th point)
- Freeze sound has satisfying weight
- Win sound is celebratory ascending arpeggio
- Star chimes punctuate the star reveal
- Click sounds are crisp and short
- Error sound is clearly different from success sounds

## Full Feature Summary (All Iterations)

### Iteration 1 — Bootstrap, Addressables, Audio
- Cloudflare R2 music download, AudioManager, scene transitions

### Iteration 2 — MainMenu, LevelSelect, Game Scene
- 30 level buttons (prefab), navigation flow, GameManager

### Iteration 3 — Drawing Mechanic
- Touch/mouse drawing → physical lines, line counter, restart

### Iteration 4 — Physics, Level Objects, Win Condition
- Level bounds, objects, goal zones, win detection, level complete panel

### Iteration 5 — Levels System
- 5 unique levels + 25 variations, LevelData ScriptableObjects, next level flow

### Iteration 6 — Star Rating + Win UI
- 1-3 stars based on lines/time, animated stars in win panel

### Iteration 7 — Hint System
- 5 free hints, silhouette display, HintManager

### Iteration 8 — IAP
- Shop panel for buying 5 hints, Unity IAP integration

### Iteration 9 — Polish Effects
- Particles (draw trail, freeze burst, confetti, goal glow), camera shake, iOS haptics

### Iteration 10 — SFX + Final Polish
- 6 procedural sound effects on all interactions
