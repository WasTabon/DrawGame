# Iteration 11 — Tutorial (First Launch)

## What changed since Iteration 10
- Added 4-slide tutorial that shows on first game launch
- Tutorial appears before MainMenu, blocks interaction until completed
- Slides: Draw → Lines fall → Push ball to goal → Stars tip
- Each slide has title, subtext, and simple illustration
- Page indicator dots (● ○ ○ ○) update per slide
- Button text changes: "NEXT" → "LET'S PLAY!" on last slide
- Saves to PlayerPrefs — never shows again after completion
- MainMenu fades in after tutorial completes
- Added "Reset Tutorial" to Debug tools

## Slides
1. **"DRAW WITH YOUR FINGER"** — finger icon + blue line illustration
2. **"LINES BECOME OBJECTS"** — frozen line + falling arrows + ground
3. **"REACH THE GOAL"** — ball on platform → arrow → green goal zone
4. **"READY?"** — 3 gold stars + "Less lines = More stars" tip

## New Scripts
- **TutorialUI.cs** — Fullscreen overlay. Manages slide navigation with CanvasGroup fade + slide-in animation. Show/Hide with DOTween. Saves "TutorialShown" to PlayerPrefs. OnTutorialComplete event.

## Changed Scripts
- **MainMenuUI.cs** — Added tutorialUI reference. On Start checks TutorialUI.HasBeenShown(). If first launch → hides main menu, shows tutorial. After tutorial → fades in main menu with entrance animation.
- **DrawGame_DebugTools.cs** — Added "Reset Tutorial" menu item.

## New Editor Script
- **Iteration11_TutorialSetup.cs** — `DrawGame > Setup Tutorial on MainMenu (Iteration 11)`
  - Creates TutorialPanel with 4 slides on MainMenu canvas
  - Wires TutorialUI references (panelGroup, slideContainer, nextButton, pageIndicator)
  - Wires MainMenuUI.tutorialUI reference

## Setup Instructions

### Step 1 — Copy Files
- `Assets/DrawGame/Scripts/TutorialUI.cs` — new
- `Assets/DrawGame/Scripts/MainMenuUI.cs` — **replace**
- `Assets/DrawGame/Editor/Iteration11_TutorialSetup.cs` — new
- `Assets/DrawGame/Editor/DrawGame_DebugTools.cs` — **replace**

### Step 2 — Setup Tutorial on MainMenu
1. Open MainMenu scene
2. Menu: **DrawGame > Setup Tutorial on MainMenu (Iteration 11)**
3. Save scene

## How to Test
1. Reset tutorial: **DrawGame > Debug > Reset Tutorial** (or Reset All Progress)
2. Start from Bootstrap → MainMenu loads
3. Tutorial appears (dark fullscreen overlay)
4. See slide 1: "DRAW WITH YOUR FINGER" with illustration
5. Tap "NEXT" → slide 2 animates in from right with fade
6. Page dots update: ○ ● ○ ○
7. Continue through slides 3 and 4
8. Last slide button says "LET'S PLAY!"
9. Tap "LET'S PLAY!" → tutorial fades out → main menu fades in with entrance animation
10. Restart game → tutorial does NOT show again
11. To test again: DrawGame > Debug > Reset Tutorial

## Expected Result
- Tutorial only shows on first launch (or after debug reset)
- Slides animate smoothly (fade + slide from right)
- Page indicator dots track current slide
- Button text updates on last slide
- Click sounds play on Next button
- After tutorial: normal MainMenu flow with entrance animation
- Tutorial never appears again unless PlayerPrefs cleared
