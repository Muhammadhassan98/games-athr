# Match-3 Puzzle Game - Unity Setup Guide

## Scripts Created
- `Assets/Scripts/GameManager.cs` - Game state, moves, win/lose
- `Assets/Scripts/Board.cs` - Grid, gem spawning, gravity, fill
- `Assets/Scripts/Gem.cs` - Individual gem behavior and movement
- `Assets/Scripts/GemSwapper.cs` - Click-to-swap input handling
- `Assets/Scripts/MatchFinder.cs` - Match detection (3+ horizontal/vertical)
- `Assets/Scripts/ScoreManager.cs` - Score tracking and high score
- `Assets/Scripts/AudioManager.cs` - Procedural sound effects
- `Assets/Scripts/UIManager.cs` - UI updates and panels

## Unity Setup Steps

### 1. Create Scene
- File > New Scene > 2D

### 2. Create GameObjects
Create empty GameObjects and attach scripts:

| GameObject Name | Script |
|----------------|--------|
| GameManager | GameManager.cs |
| Board | Board.cs |
| MatchFinder | MatchFinder.cs |
| GemSwapper | GemSwapper.cs |
| ScoreManager | ScoreManager.cs |
| AudioManager | AudioManager.cs |
| UIManager | UIManager.cs |

### 3. Setup Canvas UI
Create a Canvas with:
- **Score Text** (TextMeshPro) - top left
- **Moves Text** (TextMeshPro) - top center  
- **Target Text** (TextMeshPro) - top right
- **HighScore Text** (TextMeshPro) - below score
- **Win Panel** - with "You Win!" text + Restart button
- **GameOver Panel** - with "Game Over!" text + Restart button

### 4. Wire UI References
In UIManager Inspector, drag UI elements to:
- Score Text, Moves Text, Target Text, High Score Text
- Win Panel, Game Over Panel

### 5. Board Settings (Inspector)
- Width: 8, Height: 8
- Gem Size: 1, Gem Spacing: 0.1

### 6. Game Settings (Inspector)
- Target Score: 1000
- Total Moves: 30

### 7. Camera
Set Camera to Orthographic (auto-adjusted by Board script)

### 8. Layer Setup
Make sure Physics 2D is enabled (Edit > Project Settings > Physics 2D)

## How to Play
- Click a gem to select it (it scales up)
- Click an adjacent gem to swap
- Match 3+ same-colored gems to clear them
- Reach the target score before running out of moves!

## Gem Colors
- Red, Blue, Green, Yellow, Purple, Orange

## Scoring
- Each matched gem = 50 points
- Longer chains = more points
