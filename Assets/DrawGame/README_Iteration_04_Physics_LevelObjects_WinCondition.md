# Iteration 4 — Physics, Level Objects, Win Condition

## What changed since Iteration 3
- Added invisible walls/floor/ceiling at camera boundaries (LevelBounds)
- Added level object system with Static/Dynamic/Destructible types and reset support
- Added goal zone with 3 win condition types (ReachZone, TouchTarget, HoldInZone)
- Added LevelController that manages level state, win/reset flow
- Added "LEVEL COMPLETE!" overlay panel with animated text
- Restart now resets all level objects to initial positions (not just drawn lines)
- Test level: ball on platform, goal zone at bottom-right — push ball into zone to win

## New Scripts
- **LevelBounds.cs** — Creates invisible BoxCollider2D walls at camera edges (floor, ceiling, left, right). Walls extend upward for extra height.
- **LevelObject.cs** — Base component for all level objects. Stores initial transform for reset. Types: Static, Dynamic, Destructible. Can be marked as goal target.
- **GoalZone.cs** — Trigger zone. Types: ReachZone (object enters = win), TouchTarget (anything touches = win), HoldInZone (object stays N seconds = win). Fires OnGoalCompleted event.
- **LevelController.cs** — Singleton. Connects GoalZone and LevelObjects. Handles win (disables drawing input, fires OnLevelComplete after 0.5s delay) and reset (resets all objects + goal + drawing).

## Changed Scripts
- **GameUI.cs** — Added LevelCompletePanel with animated "LEVEL COMPLETE!" text. Subscribes to LevelController events. Restart now calls LevelController.ResetLevel() instead of just ClearAllLines.

## New Editor Script
- **Iteration4_GameSceneUpdate.cs** — `DrawGame > Update Game Scene - Physics & Level Objects (Iteration 4)`
  - Adds LevelBounds
  - Adds LevelController
  - Creates test level (Platform + Ball + GoalZone) under "LevelObjects" parent
  - Adds LevelCompletePanel to GameCanvas
  - Rewires all GameUI references

## Setup Instructions

### Step 1 — Copy Files
- `Assets/DrawGame/Scripts/LevelBounds.cs` — new
- `Assets/DrawGame/Scripts/LevelObject.cs` — new
- `Assets/DrawGame/Scripts/GoalZone.cs` — new
- `Assets/DrawGame/Scripts/LevelController.cs` — new
- `Assets/DrawGame/Scripts/GameUI.cs` — **replace** existing
- `Assets/DrawGame/Editor/Iteration4_GameSceneUpdate.cs` — new

### Step 2 — Update Game Scene
1. Open Game scene
2. Menu: **DrawGame > Update Game Scene - Physics & Level Objects (Iteration 4)**
3. Save scene

## How to Test
1. Open Game scene, press Play
2. You should see:
   - Orange ball sitting on a gray platform in the center
   - Green semi-transparent goal zone at bottom-right
   - Invisible walls at screen edges (ball won't fall off screen)
3. Draw a line above/to the left of the ball to push it right toward the goal zone
4. When ball enters goal zone → "LEVEL COMPLETE!" appears with animation
5. Drawing is disabled after win
6. Click RESTART → everything resets (ball back on platform, lines cleared, can draw again)
7. Ball and drawn lines should collide with invisible walls and not leave screen

## Expected Result
- Ball has physics (falls, bounces, rolls)
- Drawn lines push the ball on collision
- Invisible boundaries keep everything on screen
- Goal zone detection works (ball enters → win)
- Full reset works (objects + lines + goal state)
- "LEVEL COMPLETE!" overlay with scale animation
