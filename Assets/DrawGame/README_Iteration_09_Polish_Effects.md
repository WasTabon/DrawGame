# Iteration 9 — Polish: Effects, Animations, Game Feel

## What changed since Iteration 8
- Particle effects: draw trail, freeze burst, win confetti, goal glow
- Camera shake on line freeze (light) and win (medium)
- iOS haptic feedback: light on freeze, success on win
- Goal zone pulses (scale ping-pong) and animates on completion (scale up → shrink away)
- All particles created procedurally in code — no prefabs needed
- Native iOS haptic plugin (UIImpactFeedbackGenerator)

## New Scripts
- **ParticleSpawner.cs** — Singleton. Creates 4 ParticleSystems in code: DrawTrail (small blue dots following finger), FreezeBurst (gray burst when line freezes), WinConfetti (multicolor confetti explosion), GoalGlow (green upward particles). All use Emit() for manual control.
- **CameraShake.cs** — Singleton on Main Camera. ShakeLight/ShakeMedium/ShakeHeavy using DOTween.DOShakePosition. Returns to original position after shake.
- **HapticFeedback.cs** — Static class. Light/Medium/Heavy impact, Success/Warning/Error notification, Selection. Uses native iOS UIImpactFeedbackGenerator via .mm plugin. Falls back to Handheld.Vibrate on Android.
- **HapticPlugin.mm** — Native iOS Objective-C++ plugin for haptic feedback.

## Changed Scripts
- **DrawnLine.cs** — AddPoint emits draw trail particles. FreezeAsPhysics emits freeze burst + light camera shake + light haptic. Keeps all previous fixes (useWorldSpace=false, RecenterToLineCenter).
- **LevelController.cs** — HandleGoalCompleted now calls PlayWinEffects(): confetti particles at camera position, medium camera shake, success haptic.
- **GoalZone.cs** — Added pulsating scale animation (DOTween loop). On completion: scale up with OutBack → shrink to zero with InBack, emit goal glow particles. Pulse restarts on reset.

## New Editor Script
- **Iteration9_PolishSetup.cs** — `DrawGame > Update Game Scene - Polish Effects (Iteration 9)`
  - Adds ParticleSpawner to scene
  - Adds CameraShake to Main Camera

## Setup Instructions

### Step 1 — Copy Files
- `Assets/DrawGame/Scripts/ParticleSpawner.cs` — new
- `Assets/DrawGame/Scripts/CameraShake.cs` — new
- `Assets/DrawGame/Scripts/HapticFeedback.cs` — new
- `Assets/DrawGame/Plugins/iOS/HapticPlugin.mm` — new
- `Assets/DrawGame/Scripts/DrawnLine.cs` — **replace**
- `Assets/DrawGame/Scripts/LevelController.cs` — **replace**
- `Assets/DrawGame/Scripts/GoalZone.cs` — **replace**
- `Assets/DrawGame/Editor/Iteration9_PolishSetup.cs` — new

### Step 2 — Update Game Scene
1. Open Game scene
2. Menu: **DrawGame > Update Game Scene - Polish Effects (Iteration 9)**
3. Save scene

## How to Test
1. Play a level
2. **Draw a line** — see small blue particle trail following your finger
3. **Release line** — gray burst particles at line center, light camera shake
4. **Push ball into goal** — goal zone pulses, scales up and disappears with green particles
5. **Win** — multicolor confetti explosion, medium camera shake
6. **On iOS device** — feel haptic feedback on freeze (light tap) and win (success vibration)

## Expected Result
- Blue particles trail behind finger while drawing
- Satisfying burst when line freezes into physics
- Goal zone breathes with gentle pulse animation
- Goal zone dramatically disappears when ball enters
- Confetti party on level complete
- Camera shakes add weight to interactions
- Haptic feedback on iOS (won't work in Editor, only on device)

## Effects Summary
| Action | Particles | Camera | Haptic |
|--------|-----------|--------|--------|
| Drawing | Blue trail | — | — |
| Line freeze | Gray burst | Light shake | Light |
| Goal reached | Green glow | — | — |
| Level win | Confetti | Medium shake | Success |
