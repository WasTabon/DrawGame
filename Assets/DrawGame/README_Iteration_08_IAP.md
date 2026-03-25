# Iteration 8 — IAP (In-App Purchase for Hints)

## What changed since Iteration 7
- Added IAPManager with shop panel for buying 5 hints
- When hints = 0 and user taps Hint button → shop panel opens instead of shake
- Shop panel: title, description, Buy button, price text, loading overlay, status text, Close button
- LoadingButton sits on top of Buy button — blocks re-clicking while purchase processes
- IAPManager.OnBuyClicked() shows LoadingButton
- On purchase success: adds 5 hints via HintManager, shows "+5 Hints!", hides loading
- On purchase fail: shows "Purchase failed", hides loading
- Close button hides the panel

## New Scripts
- **IAPManager.cs** — Based on your template. Singleton (not DontDestroyOnLoad — lives on Game scene). Product ID: `com.drawgame.hints5` (replace later). Show/Hide with DOTween. OnBuyClicked shows loading overlay. OnPurchaseComplete/Failed/ProductFetched callbacks ready for Unity IAP Button.

## Changed Scripts
- **GameUI.cs** — Hint button now opens shop panel when hints = 0 (instead of just shaking). If IAPManager.Instance is null, falls back to shake.

## New Editor Script
- **Iteration8_IAPSetup.cs** — `DrawGame > Update Game Scene - IAP Shop (Iteration 8)`
  - Creates ShopPanel with all UI elements
  - Wires IAPManager references
  - Wires Close button → IAPManager.Hide()
  - Wires Buy button → IAPManager.OnBuyClicked()

## Setup Instructions

### Step 1 — Install Unity IAP
1. Window > Package Manager > Unity Registry
2. Find "In App Purchasing" and install it
3. Enable it in Services if needed

### Step 2 — Copy Files
- `Assets/DrawGame/Scripts/IAPManager.cs` — new
- `Assets/DrawGame/Scripts/GameUI.cs` — **replace**
- `Assets/DrawGame/Editor/Iteration8_IAPSetup.cs` — new

### Step 3 — Update Game Scene
1. Open Game scene
2. Menu: **DrawGame > Update Game Scene - IAP Shop (Iteration 8)**

### Step 4 — Set Up IAP Button (manual)
The editor script creates the shop panel and wires IAPManager, but you need to manually set up Unity IAP:

1. Find **ShopPanel > Popup > BuyButton** in hierarchy
2. Add component: **IAP Button** (from Unity IAP)
3. Set Product ID to: `com.drawgame.hints5` (or your own ID)
4. In IAP Button's events:
   - **On Purchase Complete** → drag IAPManager → select `IAPManager.OnPurchaseComplete`
   - **On Purchase Failed** → drag IAPManager → select `IAPManager.OnPurchaseFailed`
   - **On Product Fetched** → drag IAPManager → select `IAPManager.OnProductFetched`

### Step 5 — Configure Product in IAP Catalog
1. Window > Unity IAP > IAP Catalog
2. Add product: `com.drawgame.hints5`
3. Type: Consumable
4. Set price etc.

## Shop Panel Structure
```
ShopPanel (CanvasGroup + Image dim background)
  Popup (RectTransform — animated with DOScale)
    Title — "NEED MORE HINTS?"
    Description — "Get 5 extra hints"
    BuyButton (Button — you add IAP Button here)
      PriceText — shows localized price from store
    LoadingButton (Image overlay — same position as BuyButton, blocks clicks)
      Text — "Processing..."
    StatusText — shows "+5 Hints!" or "Purchase failed"
    CloseButton
      Text — "CLOSE"
```

## How to Test
1. Start game → go to level → use all 5 hints
2. Tap hint button with 0 hints → shop panel opens with animation
3. In Editor without real IAP: manually call IAPManager.Instance.OnPurchaseComplete() from debug or modify code
4. After purchase: hint count goes to 5, status shows "+5 Hints!"
5. Close button hides panel
6. For real testing: build to iOS device with IAP configured

## Expected Result
- Shop panel opens when trying to use hints with 0 remaining
- Animated show/hide (scale + fade)
- Loading overlay blocks Buy button during purchase
- Success/failure status messages
- Hints added on successful purchase
- Price text populated from store (shows "Loading..." until fetched)
