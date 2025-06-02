# Game Over Detection in Tetris

This document explains how game over detection is implemented in the Tetris game. Game over occurs when there is no more space to place a new Tetromino block.

## Implementation Details

The game can end in two different ways:

1. **Board Full**: Blocks have reached the top row of the board.
2. **No Space for New Piece**: A new piece cannot be placed at its initial position because there are already blocks in the way.

### Detection Logic

The game over detection is implemented in the following steps:

1. In the `Board` class:
   - `IsGameOver()` method checks if any blocks are in the top row of the board.

2. In the `GameEngine` class:
   - After locking a piece in place, it checks if the board is full using `Board.IsGameOver()`.
   - After spawning a new piece, it checks if the piece can be placed on the board without collisions.

### Game Over Flow

1. When a game over condition is detected, the `EndGame()` method in `GameEngine` is called.
2. The `EndGame()` method sets `_isGameOver` to true, stops the timer, and raises the `GameOver` event.
3. The `GameOver` event includes detailed statistics about the game session.

### Game Over Event

The `GameOver` event provides information about the final game state, including:

- Final score
- Level reached
- Number of rows cleared
- Statistics about different types of line clears (singles, doubles, triples, tetris)
- The reason why the game ended

## Displaying Game Over Message

The `GameOverDisplay` class handles displaying the game over message to the player. It subscribes to the `GameOver` event from the `GameEngine` and formats a message showing:

- Game over announcement
- Reason for game over
- Final score and level
- Statistics about lines cleared
- Instructions for starting a new game or exiting

## Example Usage

```csharp
// Create the game engine
var gameEngine = new GameEngine();

// Create the game over display handler
var gameOverDisplay = new GameOverDisplay(gameEngine);

// Start the game
gameEngine.StartNewGame();
```

## Testing

To test the game over detection, you can:

1. Fill the board to the top to trigger the "Board Full" condition.
2. Create a situation where the next spawned piece would collide with existing blocks.

See `GameOverExample.cs` for a demonstration of how to handle game over events.
