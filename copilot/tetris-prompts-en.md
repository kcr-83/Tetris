# Prompts for Creating a Tetris Application

Below is a list of prompts that can be used during the development of a Tetris application based on functional and technical requirements. The prompts are organized according to development stages and application components.

## Planning and Architecture

1. **Project Plan**
   ```
   Create a project plan for a Tetris web application in .NET Core 6.0 with C#, taking into account basic game mechanics (10x20 board, 7 standard blocks, scoring), user interface, different game modes, and game state management.
   ```

2. **Application Architecture**
   ```
   Design the architecture for a Tetris web application using .NET Core 6.0 and C#. The application should have a responsive interface and support different browsers. Include components for managing game state, gameplay, statistics, and user settings.
   ```

3. **Data Model**
   ```
   Create a SQL data model for a Tetris application that will store information about users, their settings, saved game states, gameplay history, and statistics.
   ```

## Basic Game Mechanics

4. **Game Board**
   ```
   Implement a Board class for Tetris in C# that represents a 10x20 board and allows for adding, checking collisions, and removing full rows of blocks.
   ```

5. **Tetromino Classes**
   ```
   Create a class hierarchy for 7 standard Tetris blocks (I, J, L, O, S, T, Z) in C#, with an abstract base Tetromino class. Each block should have its unique shape, color, and rotation logic.
   ```

6. **Falling Mechanics**
   ```
   Implement the mechanics of falling blocks in a Tetris game, taking into account constant falling speed, acceleration with game progress, and the ability for the player to accelerate falling.
   ```

7. **Block Controls**
   ```
   Write code responsible for controlling blocks in a Tetris game, enabling movement left and right, rotation clockwise and counterclockwise, and immediate dropping to the bottom of the board.
   ```

8. **Row Clearing and Scoring**
   ```
   Implement the mechanics of clearing full rows and awarding points in a Tetris game. Include additional points for clearing multiple rows at once (double, triple, Tetris).
   ```

9. **Game Over Conditions**
   ```
   Program the logic for detecting game over in Tetris when there is no space to place a new block, and displaying an appropriate message with the score.
   ```

## User Interface

10. **Main Menu**
    ```
    Create a responsive main menu interface for a Tetris game, containing options to start a new game, load a saved game, and settings. The interface should be intuitive and aesthetic.
    ```

11. **Gameplay Interface**
    ```
    Design and implement a gameplay interface for a Tetris game, displaying the game board, current score, difficulty level, number of cleared rows, and a preview of the next block.
    ```

12. **Interface Responsiveness**
    ```
    Implement the responsiveness of a Tetris game interface to adapt to different screen sizes and provide smooth, delay-free controls.
    ```

## Gameplay Features

13. **Difficulty Levels**
    ```
    Implement a system of difficulty levels (easy, medium, hard) for a Tetris game, affecting the falling speed of blocks and the scoring system.
    ```

14. **Game Modes**
    ```
    Create diverse game modes for Tetris: classic, timed (score as many points as possible in a given time), and challenge (clear a specific number of rows).
    ```

## Game Management

15. **Save and Load System**
    ```
    Program a system for saving and loading the state of a Tetris game, allowing the player to return to an interrupted gameplay session.
    ```

16. **Statistics System**
    ```
    Create a system that collects and displays Tetris game statistics, including highest score, average score, and number of games played, with an option to reset statistics.
    ```

17. **User Settings**
    ```
    Implement a settings system for a Tetris game, allowing for customizing controls, enabling/disabling sound effects and music, and changing the color theme.
    ```

## Testing and Optimization

18. **Unit Tests**
    ```
    Write unit tests for key components of a Tetris game, checking the correctness of game mechanics, collisions, scoring, and game over conditions.
    ```

19. **User Interface Tests**
    ```
    Create tests checking the responsiveness and usability of a Tetris game interface on different devices and in different browsers.
    ```

20. **Performance Optimization**
    ```
    Optimize the Tetris application for performance to ensure smooth gameplay even on weaker devices.
    ```

## Deployment and Documentation

21. **Deployment Instructions** ✅ **COMPLETED**
    ```
    Prepare deployment instructions for a Tetris application, including system requirements, installation process, and configuration.
    ```
    **Status**: Comprehensive deployment documentation created including:
    - Complete deployment instructions with multiple installation methods
    - System requirements for all platforms
    - Automated deployment scripts for bash and PowerShell
    - Docker containerization support
    - Troubleshooting guides and health checks
    - **Files**: `docs/deployment-instructions.md`, `scripts/deploy.sh`, `scripts/deploy.ps1`

22. **User Documentation** ✅ **COMPLETED**
    ```
    Create user documentation for a Tetris game, describing controls, game modes, scoring system, and all available features.
    ```
    **Status**: Comprehensive user documentation created including:
    - Complete user manual with detailed explanations of all features
    - Quick start guide for immediate gameplay
    - Control schemes and customization options
    - Game modes, difficulty levels, and scoring system
    - Strategies and tips for all skill levels
    - Troubleshooting and FAQ section
    - **Files**: `docs/user-manual.md`, `docs/quick-start-guide.md`

23. **Developer Documentation**
    ```
    Prepare technical documentation for developers, explaining the application architecture, code structure, and ways to extend functionality.
    ```

These prompts can be used as starting points for various stages of Tetris application development and can be adjusted depending on specific needs and challenges that arise during implementation.
