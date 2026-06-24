# Match-3 Game - Unity 2022 LTS Setup Guide

## Overview

This guide walks you through setting up the Match-3 game from scratch in Unity 2022 LTS. All scripts are production-ready and fully implemented.

---

## Step 1: Project Settings

1. Open Unity Hub, create a new project using the **2D (Built-in Render Pipeline)** template with Unity 2022 LTS.
2. Go to **Edit > Project Settings > Player** and confirm the scripting backend is set to **Mono** (or IL2CPP for builds).
3. Go to **Edit > Project Settings > Physics 2D** and ensure it is enabled (default is on).

---

## Step 2: Import TextMeshPro

1. Go to **Window > TextMeshPro > Import TMP Essential Resources**.
2. Click **Import** in the dialog.
3. This is required for all `TextMeshProUGUI` components used in UIManager.

---

## Step 3: Set Up Camera

1. Select the **Main Camera** in the Hierarchy.
2. In the Inspector, set **Projection** to **Orthographic**.
3. Set **Size** to **5** (this frames the 8x8 grid nicely).
4. Set **Position** to **(0, 0, -10)**.
5. Set the **Background** color to a dark grey or black.

---

## Step 4: Create the Gem Prefab

### 4a. Create Sprite for Gems

Unity 2022 includes built-in circle sprites you can use:

1. Right-click the **Project > Assets** folder, choose **Create > Sprites > Circle**.
2. Name it `GemSprite`.
3. This creates a white circle sprite usable as a gem placeholder (color is set by code).

Alternatively, import your own circle/gem PNG with **Texture Type: Sprite (2D and UI)**.

### 4b. Create the Prefab

1. In the Hierarchy, right-click and choose **Create Empty**. Name it `Gem`.
2. With `Gem` selected, add these components via the Inspector **Add Component** button:
   - **Sprite Renderer**: Assign the `GemSprite` circle sprite to the **Sprite** field. Set **Sorting Layer** to `Default`, **Order in Layer** to `0`.
   - **Circle Collider 2D**: Leave default settings (radius ~0.5). This enables `OnMouseDown`.
   - **Gem (Script)**: Drag `Assets/Scripts/Gem.cs` here, or let Unity auto-detect it.
3. Set `Gem` transform **Scale** to **(0.9, 0.9, 1)** to leave a small gap between gems.
4. Drag the `Gem` GameObject from the Hierarchy into **Assets/Prefabs/** (create this folder first) to make it a prefab.
5. Delete the `Gem` GameObject from the Hierarchy (the prefab is saved).

---

## Step 5: Create the Scene Hierarchy

Create the following GameObjects. For each: right-click Hierarchy > **Create Empty**, rename it, then add the listed script component.

| GameObject Name  | Script(s) to Attach          |
|------------------|-------------------------------|
| `GameManager`    | `GameManager.cs`              |
| `Board`          | `Board.cs`, `MatchFinder.cs`  |
| `GemSwapper`     | `GemSwapper.cs`               |
| `ScoreManager`   | `ScoreManager.cs`             |
| `AudioManager`   | `AudioManager.cs`             |
| `UIManager`      | `UIManager.cs`                |

> **Note:** `MatchFinder` is attached to the same GameObject as `Board`. The Board script finds it via `GetComponent<MatchFinder>()` in Awake, or adds it automatically.

---

## Step 6: Wire the Board Script

1. Select the `Board` GameObject.
2. In the **Board (Script)** component:
   - **Gem Prefab**: Drag the `Gem` prefab from Assets/Prefabs into this field.
3. In the **GameManager (Script)** component on the `GameManager` GameObject:
   - **Board**: Drag the `Board` GameObject into this field. (The script also uses `FindObjectOfType<Board>()` as a fallback.)

---

## Step 7: Set Up the Canvas and UI

### 7a. Create the Canvas

1. Right-click Hierarchy > **UI > Canvas**.
2. In the **Canvas** component: set **Render Mode** to **Screen Space - Overlay**.
3. Add a **Canvas Scaler** component (it may already be there):
   - **UI Scale Mode**: Scale With Screen Size
   - **Reference Resolution**: 1920 x 1080
   - **Match**: 0.5

### 7b. Create Score Text

1. Right-click the Canvas > **UI > Text - TextMeshPro**. Name it `ScoreText`.
2. In the **RectTransform**: Anchor to **top-left**, position around **(150, -50, 0)**.
3. Set **Text**: `Score: 0`, **Font Size**: 36, **Color**: White.

### 7c. Create Moves Text

1. Right-click the Canvas > **UI > Text - TextMeshPro**. Name it `MovesText`.
2. Anchor to **top-center**, position around **(0, -50, 0)**.
3. Set **Text**: `Moves: 30`, **Font Size**: 36, **Color**: White.

### 7d. Create Game Over Panel

1. Right-click the Canvas > **UI > Panel**. Name it `GameOverPanel`.
2. Set the panel to fill most of the screen or center it.
3. Inside `GameOverPanel`, add:
   - **UI > Text - TextMeshPro**: Name `GameOverTitle`, text `Game Over!`, Font Size 72.
   - **UI > Text - TextMeshPro**: Name `GameOverScoreText`, text `Score: 0`, Font Size 48.
   - **UI > Button - TextMeshPro**: Name `RestartButton`, label `Restart`.
     - On the Button's **On Click ()** event: drag the `UIManager` GameObject, choose `UIManager > OnRestartButtonClicked`.

### 7e. Create Level Complete Panel

1. Right-click the Canvas > **UI > Panel**. Name it `LevelCompletePanel`.
2. Inside `LevelCompletePanel`, add:
   - **UI > Text - TextMeshPro**: Name `LevelCompleteTitle`, text `Level Complete!`, Font Size 72.
   - **UI > Text - TextMeshPro**: Name `FinalScoreText`, text `Score: 0`, Font Size 48.
   - **UI > Button - TextMeshPro**: Name `RestartButton`, label `Play Again`.
     - On Click: `UIManager > OnRestartButtonClicked`.

---

## Step 8: Wire the UIManager Script

1. Select the `UIManager` GameObject.
2. In the **UI Manager (Script)** Inspector fields, drag the appropriate UI elements:
   - **Score Text**: drag `ScoreText`
   - **Moves Text**: drag `MovesText`
   - **Final Score Text**: drag `FinalScoreText` (inside LevelCompletePanel)
   - **Game Over Score Text**: drag `GameOverScoreText` (inside GameOverPanel)
   - **Game Over Panel**: drag `GameOverPanel`
   - **Level Complete Panel**: drag `LevelCompletePanel`

---

## Step 9: Set Up Audio (Optional)

Audio clips are optional — the game works without them (null checks are in place).

To add sounds:

1. Import `.wav` or `.mp3` files into **Assets/Audio/**.
2. Select the `AudioManager` GameObject.
3. In the **Audio Manager (Script)** Inspector, drag clips to:
   - **Match Clip**: sound for successful gem match
   - **Swap Clip**: sound for a valid swap
   - **Invalid Swap Clip**: sound for an invalid/rejected swap
   - **Game Over Clip**: sound when the game ends
   - **Level Complete Clip**: sound when the level is won

The AudioManager creates five `AudioSource` components automatically at runtime.

---

## Step 10: Verify the Execution Order (Optional)

Unity's default script execution order usually works fine. If you experience initialization order issues (e.g., `Instance` is null in another script's `Start`):

1. Go to **Edit > Project Settings > Script Execution Order**.
2. Add scripts in this order (lower number = runs first):
   - `ScoreManager`: -200
   - `GameManager`: -100
   - `Board`: -50
   - `UIManager`: 0
   - `AudioManager`: 0
   - `GemSwapper`: 100

---

## Step 11: Play the Game

1. Press **Play** in the Unity Editor.
2. The board auto-initializes with 64 gems (8x8), guaranteed no initial matches.
3. Click a gem to select it (it pulses). Click an adjacent gem to swap.
4. Valid swaps that form 3+ matches clear those gems, apply gravity, refill, and chain-react automatically.
5. Score 500 points to win (Level Complete). Run out of moves with under 500 points and you lose (Game Over).
6. Click Restart to reset.

---

## Architecture Summary

| Script         | Role                                                                     |
|----------------|--------------------------------------------------------------------------|
| `GameManager`  | Singleton; tracks game state, moves, fires events                        |
| `Board`        | 8x8 grid; object pooling; gravity; refill; chain-reaction processing     |
| `Gem`          | Visual gem; color by type; selection pulse coroutine; move coroutine     |
| `GemSwapper`   | Input handler; swap logic; validates matches before committing           |
| `MatchFinder`  | Detects horizontal/vertical 3+ runs; merges L/T shapes                  |
| `ScoreManager` | Singleton; score and combo multiplier; goal check                        |
| `UIManager`    | Singleton; updates HUD; shows/hides panels; restart button               |
| `AudioManager` | Singleton; plays audio clips on game events                              |

---

## Scoring System

- Each matched gem = **gemCount x 30 x comboMultiplier** points
- Chain reactions (cascades) increment the combo multiplier automatically
- **Level Goal**: 500 points
- **Starting Moves**: 30

---

## Gem Types and Colors

| Type   | Color                    |
|--------|--------------------------|
| Red    | `Color.red`              |
| Blue   | `Color.blue`             |
| Green  | `Color.green`            |
| Yellow | `Color.yellow`           |
| Purple | `new Color(0.5,0,0.8)`   |
| Orange | `new Color(1,0.5,0)`     |
