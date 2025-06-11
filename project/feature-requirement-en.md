# Tetris - Functional Requirements

## Epics

### EP01: Basic Game Mechanics
Implementation of basic Tetris game mechanisms, including game board, falling blocks, and scoring mechanics.

### EP02: User Interface
Creation of an intuitive and responsive user interface for the game.

### EP03: Gameplay Features
Additional features enriching gameplay, such as difficulty levels, game modes, etc.

### EP04: Game Management
Game management functions, such as save/load game state, statistics, and settings.

## Features and User Stories

### EP01: Basic Game Mechanics

#### F01-01: Game Board
- **US01-01-01**: As a player, I want to see a game board with standard dimensions (10x20), so I can play Tetris.
- **US01-01-02**: As a player, I want the game board to be clearly visible and separated from the rest of the interface.

#### F01-02: Tetris Blocks (Tetrominos)
- **US01-02-01**: As a player, I want to have access to all 7 standard Tetris blocks (I, J, L, O, S, T, Z).
- **US01-02-02**: As a player, I want the blocks to be colorful and easy to distinguish.
- **US01-02-03**: As a player, I want to see a preview of the next block that will appear on the board.

#### F01-03: Block Falling Mechanics
- **US01-03-01**: As a player, I want blocks to fall from the top of the board at a specific speed.
- **US01-03-02**: As a player, I want the falling speed of blocks to increase as the game progresses.
- **US01-03-03**: As a player, I want to be able to accelerate the falling of blocks when I decide to do so.

#### F01-04: Block Controls
- **US01-04-01**: As a player, I want to be able to move falling blocks left and right.
- **US01-04-02**: As a player, I want to be able to rotate falling blocks clockwise and counterclockwise.
- **US01-04-03**: As a player, I want to be able to immediately drop a block to the bottom of the board.

#### F01-05: Row Clearing and Scoring
- **US01-05-01**: As a player, I want full rows to be removed from the board.
- **US01-05-02**: As a player, I want to receive points for clearing rows.
- **US01-05-03**: As a player, I want to receive more points for clearing multiple rows at once (e.g., double, triple, Tetris).

#### F01-06: Game Over
- **US01-06-01**: As a player, I want the game to end when there is no space to place a new block.
- **US01-06-02**: As a player, I want to see a clear game over message along with my score.

### EP02: User Interface

#### F02-01: Main Menu
- **US02-01-01**: As a player, I want to have access to a main menu with options to start a new game, load a saved game, and settings.
- **US02-01-02**: As a player, I want the menu to be intuitive and aesthetically pleasing.

#### F02-02: In-Game Information
- **US02-02-01**: As a player, I want to see the current score during the game.
- **US02-02-02**: As a player, I want to see the current difficulty level during the game.
- **US02-02-03**: As a player, I want to see the number of cleared rows during the game.

#### F02-03: Interface Responsiveness
- **US02-03-01**: As a player, I want the game interface to be responsive and adapt to different screen sizes.
- **US02-03-02**: As a player, I want the controls to work smoothly and without delays.

#### F02-07: Gameplay Interface
- **US02-07-01**: As a player, I want to see the current game board with all placed blocks clearly displayed.
- **US02-07-02**: As a player, I want to see my current score prominently displayed during gameplay.
- **US02-07-03**: As a player, I want to see my current level and how it affects the gameplay speed.
- **US02-07-04**: As a player, I want to see how many rows I've cleared during the current game.
- **US02-07-05**: As a player, I want to see a preview of the next piece that will appear.
- **US02-07-06**: As a player, I want to see a ghost piece showing where my current piece will land.
- **US02-07-07**: As a player, I want to receive visual feedback when I clear rows or reach a new level.
- **US02-07-08**: As a player, I want to see game statistics such as singles, doubles, triples, and tetris clears.
- **US02-07-09**: As a player, I want a clear indication of the current difficulty level.
- **US02-07-10**: As a player, I want to see a pause overlay when I pause the game.

### EP03: Gameplay Features

#### F03-01: Difficulty Levels
- **US03-01-01**: As a player, I want to be able to choose the difficulty level before starting the game.
- **US03-01-02**: As a player, I want the difficulty level to affect the falling speed of blocks and the scoring system.

#### F03-02: Game Modes
- **US03-02-01**: As a player, I want to have access to the classic Tetris game mode.
- **US03-02-02**: As a player, I want to have access to a timed mode, where I need to score as many points as possible in a given time.
- **US03-02-03**: As a player, I want to have access to a challenge mode, where I need to clear a specific number of rows.

### EP04: Game Management

#### F04-01: Save and Load Game
- **US04-01-01**: As a player, I want to be able to save the game state so I can return to it later.
- **US04-01-02**: As a player, I want to be able to load a saved game and continue from where I left off.

#### F04-02: Statistics
- **US04-02-01**: As a player, I want to see statistics of my games (highest score, average score, number of games played).
- **US04-02-02**: As a player, I want to be able to reset my statistics if I choose to.

#### F04-03: Settings
- **US04-03-01**: As a player, I want to be able to customize the controls to my preferences.
- **US04-03-02**: As a player, I want to be able to turn sound effects and music on/off.
- **US04-03-03**: As a player, I want to be able to customize the look of the game (e.g., color theme).

## Technical Requirements
- The application will be built using .Net Core 6.0 or newer
- Programming language: C#
- The application will function as a web application
- The user interface should be responsive and compatible with popular browsers
