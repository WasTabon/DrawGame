# Iteration 1 — Bootstrap, Addressables, Audio, Scene Transitions

## What's in this iteration

### New Scripts
- **AddressableLoader.cs** — Singleton (DontDestroyOnLoad). Downloads music from Cloudflare R2 via Unity Addressables. Uses Completed callback pattern. In Editor mode with `skipDownloadInEditor = true`, skips download and proceeds immediately.
- **AudioManager.cs** — Singleton (DontDestroyOnLoad). Plays background music with fade-in. Resubscribes on scene load.
- **SceneTransition.cs** — Singleton (DontDestroyOnLoad). Black fade overlay (CanvasGroup) for smooth scene transitions. Sort order 999 to be always on top.
- **BootstrapUI.cs** — Loading screen UI. Progress bar, status text, retry button. Listens to AddressableLoader events. On success → transitions to MainMenu.

### Editor Script
- **Iteration1_BootstrapSetup.cs** — Menu: `DrawGame > Setup Bootstrap Scene (Iteration 1)`

## Setup Instructions

### Step 1 — Install Dependencies
1. Open Package Manager (Window > Package Manager)
2. Install **Addressables** (version 1.22.3 or higher) — Add package by name: `com.unity.addressables`
3. Install **DOTween** (free) from Asset Store if not already installed
4. **TextMeshPro** should already be included. If prompted to import TMP Essentials, do so.

### Step 2 — Create Scenes
1. Create 3 empty scenes and save them:
   - `Assets/Scenes/Bootstrap.unity`
   - `Assets/Scenes/MainMenu.unity`
   - `Assets/Scenes/Game.unity`
2. Open `Bootstrap` scene

### Step 3 — Run Editor Script
1. Go to menu: **DrawGame > Setup Bootstrap Scene (Iteration 1)**
2. This will create all objects on the Bootstrap scene and add scenes to Build Settings

### Step 4 — Configure Addressables (for production, skip for now)
When you have your Cloudflare R2 bucket ready:
1. Open Addressables Groups (Window > Asset Management > Addressables > Groups)
2. Create a new group called "Music"
3. Open Addressables Profiles (Window > Asset Management > Addressables > Profiles)
4. Set **Remote.LoadPath** to: `https://pub-9ec825916e0a4cf0a10e995611990f24.r2.dev/DrawGame/[BuildTarget]`
5. Add your .wav music file to the Music group
6. Set its address to `GameMusic`
7. Add label `music` to it
8. In AddressablesAssetSettings, set Build Remote Catalog = true
9. Build Addressables (Build > New Build > Default Build Script)
10. Upload the generated files from `ServerData/` folder to your R2 bucket under `DrawGame/[BuildTarget]/` path

### Step 5 — Testing in Editor
1. Open Bootstrap scene
2. Press Play
3. **Expected flow:** AddressableLoader skips download (Editor mode) → progress bar fills → "Ready!" text → fade transition to MainMenu scene
4. MainMenu scene will be empty — that's expected, it's for Iteration 2

### Important Notes
- `skipDownloadInEditor` is enabled by default on AddressableLoader. Set it to `false` when you want to test actual downloading.
- In a build without music configured, the game will show "Failed to initialize" with a Retry button — this is intended behavior ("game doesn't launch without music").
- The 3 scenes must be in Build Settings in order: Bootstrap (0), MainMenu (1), Game (2). The editor script does this automatically.

## How to Test
1. Open Bootstrap scene → Play
2. Watch the loading flow complete
3. Verify the scene transitions to MainMenu
4. If you want to test the Retry button: set `skipDownloadInEditor = false` on AddressableLoader (without configuring Addressables, it will fail and show Retry)

## Expected Result
- Bootstrap scene with dark background, "DRAW GAME" title, progress bar, status text
- Automatic loading flow with status updates
- Smooth fade transition to empty MainMenu scene
- Retry button appears on failure
