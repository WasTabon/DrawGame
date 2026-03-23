# Iteration 3 — Drawing Mechanic

## What changed since Iteration 2
- Added drawing system: draw with finger/mouse, line becomes a physics object on release
- Line count limit (default 5) with counter in top bar
- Restart button clears all drawn lines
- Lines have mass based on length — longer = heavier
- Smooth rounded lines with cap/corner vertices
- Visual feedback: color change on freeze, shake animation, red counter when maxed out
- Input blocked over UI elements

## New Scripts
- **DrawingManager.cs** — Singleton. Handles touch/mouse input, creates DrawnLine objects, manages line count, input blocking over UI. Configurable: maxLines, minPointDistance, lineWidth, colors.
- **DrawnLine.cs** — Component on each drawn line. LineRenderer for visuals, EdgeCollider2D + Rigidbody2D for physics. Mass = line length × 0.5 (clamped 0.2–10). Shake animation on freeze.

## Changed Scripts
- **GameUI.cs** — Added restart button and line counter text. Subscribes to DrawingManager.OnLineCountChanged. Counter turns red and shakes when limit reached. Restart button has press animation.

## New Editor Script
- **Iteration3_GameSceneUpdate.cs** — `DrawGame > Update Game Scene - Drawing (Iteration 3)`
  - Adds DrawingManager to scene
  - Adds LineCountText to existing TopBar
  - Adds RestartButton at bottom of canvas
  - Rewires all GameUI references (keeps existing BackButton and LevelText)

## Setup Instructions

### Step 1 — Copy Files
- `Assets/DrawGame/Scripts/DrawingManager.cs` — new
- `Assets/DrawGame/Scripts/DrawnLine.cs` — new
- `Assets/DrawGame/Scripts/GameUI.cs` — **replace** existing
- `Assets/DrawGame/Editor/Iteration3_GameSceneUpdate.cs` — new

### Step 2 — Update Game Scene
1. Open Game scene
2. Menu: **DrawGame > Update Game Scene - Drawing (Iteration 3)**
3. Save scene

## How to Test
1. Open Game scene (or start from Bootstrap → navigate to Game)
2. Press Play
3. Draw with mouse (click and drag) on the game area
4. Release — line turns gray and falls with gravity
5. Draw up to 5 lines — counter shows "X / 5"
6. Try drawing a 6th line — counter turns red and shakes, drawing blocked
7. Click RESTART — all lines removed, counter resets to "0 / 5"
8. Click Back — returns to MainMenu
9. Verify: clicking on UI buttons does NOT draw a line underneath

## Expected Result
- Smooth line drawing with rounded corners
- Lines fall with realistic physics after release
- Heavier lines (longer) fall slightly differently than light ones
- Lines collide with each other
- Line counter updates in real time
- Restart clears everything
- No drawing through UI elements
