# Iteration 6 — Star Rating System + Win UI

## What changed since Iteration 5
- Added star rating system: 1-3 stars per level based on lines used and time
- Each LevelData now has idealLines and idealTime thresholds
- Win panel shows 3 animated stars (earned = gold + big bounce, unearned = gray + small)
- Stars appear one by one with delay for satisfying feedback
- Stars saved to PlayerPrefs and displayed on level select buttons
- LevelSpawner now exposes GetCurrentLevelData() for star calculations
- LevelController.OnLevelComplete now passes star count

## Star Calculation Logic
- 3 stars: lines <= idealLines AND time <= idealTime
- 2 stars: lines <= idealLines OR time <= idealTime
- 1 star: neither condition met (but level completed)

## New Scripts
- **StarRating.cs** — Static utility class. Calculate(linesUsed, timeTaken, idealLines, idealTime) → 1-3

## Changed Scripts
- **LevelData.cs** — Added idealLines (int) and idealTime (float) fields
- **LevelSpawner.cs** — Added currentLevelData field and GetCurrentLevelData() method
- **LevelController.cs** — OnLevelComplete event now passes int (stars). Calculates stars using StarRating. Saves via GameManager.SetStars().
- **GameUI.cs** — Added starTexts array (3 TMP elements). AnimateStars() shows stars one by one with scale bounce. Earned stars are gold, unearned are gray.

## New Editor Script
- **Iteration6_StarsAndWinUI.cs**:
  - `DrawGame > Update Level Data - Stars (Iteration 6)` — Sets idealLines and idealTime on all 30 level assets
  - `DrawGame > Update Game Scene - Stars UI (Iteration 6)` — Adds StarsContainer with 3 star texts to win panel, repositions elements, rewires all references

## Setup Instructions

### Step 1 — Copy Files
- `Assets/DrawGame/Scripts/StarRating.cs` — new
- `Assets/DrawGame/Scripts/LevelData.cs` — **replace**
- `Assets/DrawGame/Scripts/LevelSpawner.cs` — **replace**
- `Assets/DrawGame/Scripts/LevelController.cs` — **replace**
- `Assets/DrawGame/Scripts/GameUI.cs` — **replace**
- `Assets/DrawGame/Editor/Iteration6_StarsAndWinUI.cs` — new

### Step 2 — Update Level Data
1. Menu: **DrawGame > Update Level Data - Stars (Iteration 6)**
2. This sets idealLines and idealTime on all 30 LevelData assets

### Step 3 — Update Game Scene
1. Open Game scene
2. Menu: **DrawGame > Update Game Scene - Stars UI (Iteration 6)**
3. Save scene

## How to Test
1. Play level 1 — draw 1 line quickly (under 12 seconds) → 3 stars
2. Play level 1 — draw 1 line slowly (over 12 seconds) → 2 stars
3. Play level 1 — draw 3 lines slowly → 1 star
4. Watch stars animate: appear one by one with bounce, gold = earned, gray = not earned
5. Go back to LevelSelect — stars should show on completed level buttons
6. Restart app — stars persist

## Expected Result
- 3 animated stars in win panel (gold bounce for earned, gray for not)
- Stars appear with cascading delay
- Next Level and Restart buttons appear after stars
- Stars saved and visible on level select screen
- Different performance → different star counts
