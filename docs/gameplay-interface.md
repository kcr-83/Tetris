# Gameplay Interface Documentation

## Overview

The GameplayInterface class provides a comprehensive visual interface for the Tetris game, displaying the game board, statistics, and next piece preview. It represents a key component of the game's user interface that visualizes the current state of the game to the player.

## Core Features

### 1. Game Board Rendering

The interface displays the game board with all placed blocks and the current falling piece. Each block is rendered with a color corresponding to the specific tetromino type (I, J, L, O, S, T, Z).

```
┌────────────┐
│··········││
│··········││
│··········││
│··········││
│··········││
│··████····││
│······██··││
│····████··││
│··········││
│··········││
└────────────┘
```

### 2. Ghost Piece Visualization

A ghost piece feature shows a preview of where the current tetromino will land when dropped, helping players plan their moves more effectively.

### 3. Statistics Panel

A side panel displays key game statistics:
- Current score
- Level of difficulty
- Number of cleared rows
- Current game speed
- Line clear statistics (singles, doubles, triples, Tetris clears)
- Difficulty indicator

### 4. Next Piece Preview

A preview box shows the next tetromino piece that will appear after the current one is placed, allowing for strategic planning.

```
NEXT PIECE:
┌─────┐
│     │
│  █  │
│  █  │
│  ██ │
└─────┘
```

### 5. Special Animations

The interface provides visual feedback for significant game events:

#### Level Up Animation

When the player advances to a new level, a celebratory message is briefly displayed:

```
┌────────────┐
│  LEVEL 5!  │
└────────────┘
```

#### Row Clear Animation

When rows are cleared, a message indicates the type of clear and points earned:

```
╔════════════════════╗
║      TETRIS!      ║
║    +800 pts      ║
╚════════════════════╝
```

#### Pause Overlay

When the game is paused, an overlay informs the player of the paused state:

```
┌─────────────────────┐
│       PAUSED        │
│ Press P to continue │
│ Press ESC for menu  │
└─────────────────────┘
```

### 6. Controls Help

The interface displays a help section at the bottom of the screen showing the control keys:

```
←→: Move   ↑: Rotate   ↓: Fast Drop   Space: Hard Drop   P: Pause   ESC: Menu
```

## Technical Implementation

### Color Coding

Each tetromino type has a distinct color to help players identify pieces at a glance:
- I piece (cyan): Long straight piece
- J piece (blue): L-shaped piece facing left
- L piece (orange): L-shaped piece facing right
- O piece (yellow): Square piece
- S piece (green): S-shaped piece
- T piece (purple): T-shaped piece
- Z piece (red): Z-shaped piece

### Adaptive Layout

The interface automatically adjusts its layout based on the console window size, ensuring optimal display on different terminals.

### Event-Driven Architecture

The interface responds to game events through event handlers:
- `LevelIncreased`: Triggered when the player advances to a new level
- `RowsCleared`: Triggered when one or more rows are cleared

## Design Principles

The GameplayInterface is designed with the following principles in mind:

1. **Clarity**: Information is presented clearly and is easily readable
2. **Feedback**: Players receive immediate visual feedback for their actions
3. **Guidance**: The interface helps players understand where pieces will land
4. **Engagement**: Special animations make achievements feel rewarding
5. **Adaptability**: The interface works across different console window sizes

## Integration with Game Engine

The GameplayInterface is closely integrated with the GameEngine class, which provides the core game logic. The interface observes changes in the game state and updates the display accordingly.

## Future Enhancements

Potential future enhancements for the GameplayInterface include:
- Color themes for different visual styles
- Animation options (enable/disable)
- High score display
- Time elapsed indicator
- Enhanced visual effects for special clears
