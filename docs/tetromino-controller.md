# Tetromino Controller for Tetris Game

## Overview

The TetrominoController class provides a unified interface for controlling Tetris blocks (tetrominoes) in a Tetris game. It enables the following functionality:

- Movement of blocks left and right
- Rotation of blocks clockwise and counterclockwise
- Soft drop (accelerated falling)
- Hard drop (immediate placement at the bottom)
- Board visualization with the current piece and hard drop preview

## Key Components

### TetrominoController

The main controller class that provides methods for manipulating Tetris pieces:

- `MoveLeft()` - Moves the current block one unit to the left if possible
- `MoveRight()` - Moves the current block one unit to the right if possible
- `RotateClockwise()` - Rotates the current block 90 degrees clockwise if possible
- `RotateCounterClockwise()` - Rotates the current block 90 degrees counter-clockwise if possible
- `ToggleSoftDrop(bool)` - Activates or deactivates fast falling mode
- `HardDrop()` - Drops the current block to the bottom immediately
- `ProcessInput(TetrisInput)` - Processes a controller input and performs the corresponding action
- `GetBoardWithCurrentPiece()` - Returns the game board with the current piece included
- `GetHardDropPreview()` - Returns a preview of where the piece would land if hard-dropped

### TetrisInput Enum

Defines the possible input actions for the controller:

- `MoveLeft` - Move the piece left
- `MoveRight` - Move the piece right
- `RotateClockwise` - Rotate the piece clockwise
- `RotateCounterClockwise` - Rotate the piece counter-clockwise
- `SoftDropStart` - Begin accelerated falling
- `SoftDropEnd` - End accelerated falling
- `HardDrop` - Immediately drop the piece to the bottom

## Usage Example

```csharp
// Create a game engine
var gameEngine = new GameEngine();

// Create a controller
var controller = new TetrominoController(gameEngine);

// Start a new game
gameEngine.StartNewGame();

// Move the piece left
controller.MoveLeft();

// Rotate the piece clockwise
controller.RotateClockwise();

// Perform a hard drop
controller.HardDrop();

// Process user input
controller.ProcessInput(TetrominoController.TetrisInput.MoveRight);
```

## Demo Application

The TetrominoControllerDemo class provides a sample implementation showing how to use the TetrominoController in a console-based Tetris game:

- Interactive mode with keyboard controls
- Automatic demonstration mode showing all controller actions
- Visualization of the game board with the current piece
- Hard drop preview showing where the piece will land

## Integration

The TetrominoController integrates with the existing GameEngine class to provide a more user-friendly interface for controlling Tetris pieces. It extends the base functionality with additional features like hard drop preview visualization.

## Design Considerations

1. **Separation of Concerns**: The controller focuses solely on handling user input and translating it into game actions.
2. **Extensibility**: The TetrisInput enum makes it easy to add new control schemes or input methods.
3. **Visual Feedback**: Methods like GetHardDropPreview provide visual aids for players.
4. **Simplicity**: The controller provides a simple, unified interface for various game actions.
