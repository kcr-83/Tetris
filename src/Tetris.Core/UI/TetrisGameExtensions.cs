using System;
using System.Threading.Tasks;
using Tetris.Core.Models;

namespace Tetris.Core.UI
{
    /// <summary>
    /// Extension class with improved methods for TetrisGame that enhance responsiveness
    /// and provide a better user experience.
    /// </summary>
    public static class TetrisGameExtensions
    {
        /// <summary>
        /// An improved game loop for TetrisGame that supports more responsive window handling and smoother controls.
        /// </summary>
        /// <param name="game">The TetrisGame instance.</param>
        /// <param name="gameEngine">The game engine that controls game logic.</param>
        /// <param name="gameplayInterface">The interface used to render the game.</param>
        /// <param name="isPaused">Whether the game is paused.</param>
        /// <param name="isGameActive">Whether the game is active.</param>
        /// <param name="isExiting">Whether the game is exiting.</param>
        /// <returns>A task representing the game loop.</returns>
        public static async Task RunGameLoopWithBetterResponsivenessAsync(
            this TetrisGame game,
            GameEngine gameEngine,
            GameplayInterface gameplayInterface,
            bool isPaused,
            bool isGameActive,
            bool isExiting
        )
        {
            Console.Clear();
            Console.CursorVisible = false;

            // Reset state for new game
            isPaused = false;

            // Initialize the game interface
            gameplayInterface.Initialize();

            // Display initial board
            DisplayGame(gameplayInterface, isPaused);

            // Variables for controlling input and update timing
            const int baseFrameDelay = 16; // ~60 FPS target (16.67ms)
            DateTime lastGameUpdate = DateTime.Now;
            DateTime lastFrameTime = DateTime.Now;

            // Game loop continues until game is over or player exits
            while (isGameActive && !isExiting)
            {
                // Check for window resize
                bool wasResized = false;
                // Check for window resize using available methods
                int currentWidth = Console.WindowWidth;
                int currentHeight = Console.WindowHeight;

                // Use reflection to check for CheckForResizeImproved method
                var checkForResizeMethod = gameplayInterface
                    .GetType()
                    .GetMethod("CheckForResizeImproved");
                if (checkForResizeMethod != null)
                {
                    var result = checkForResizeMethod.Invoke(gameplayInterface, null);
                    if (result is bool boolResult)
                    {
                        wasResized = boolResult;
                    }
                }
                else
                {
                    // Fall back to basic resize detection with HandleResize
                    static int GetLastWindowWidth(GameplayInterface gameplayInterface)
                    {
                        var field = gameplayInterface
                            .GetType()
                            .GetField(
                                "_lastWindowWidth",
                                System.Reflection.BindingFlags.NonPublic
                                    | System.Reflection.BindingFlags.Instance
                            );
                        return field != null ? (int)field.GetValue(gameplayInterface) : 0;
                    }

                    static int GetLastWindowHeight(GameplayInterface gameplayInterface)
                    {
                        var field = gameplayInterface
                            .GetType()
                            .GetField(
                                "_lastWindowHeight",
                                System.Reflection.BindingFlags.NonPublic
                                    | System.Reflection.BindingFlags.Instance
                            );
                        return field != null ? (int)field.GetValue(gameplayInterface) : 0;
                    }

                    int lastWidth = GetLastWindowWidth(gameplayInterface);
                    int lastHeight = GetLastWindowHeight(gameplayInterface);

                    if (currentWidth != lastWidth || currentHeight != lastHeight)
                    {
                        gameplayInterface.HandleResize();
                        wasResized = true;
                    }
                }

                // Only process input and update display if game is not paused
                if (!isPaused)
                {
                    // Determine base game frame delay based on level for adaptive difficulty
                    int frameDelay = Math.Max(10, 50 - (gameEngine.Level * 2));

                    // Ensure we process all queued input to prevent lag
                    // Use a limit to prevent potential infinite loop in case of input flood
                    int maxInputProcessingPerFrame = 10;
                    int inputsProcessed = 0;

                    while (Console.KeyAvailable && inputsProcessed < maxInputProcessingPerFrame)
                    {
                        ProcessGameInput(
                            game,
                            gameEngine,
                            gameplayInterface,
                            ref isPaused,
                            ref isGameActive,
                            ref isExiting
                        );
                        inputsProcessed++;
                    }

                    // Use adaptive frame timing for display updates
                    TimeSpan timeSinceLastUpdate = DateTime.Now - lastGameUpdate;
                    if (timeSinceLastUpdate.TotalMilliseconds > frameDelay || wasResized)
                    {
                        DisplayGame(gameplayInterface, isPaused);
                        lastGameUpdate = DateTime.Now;
                    }
                }
                else
                {
                    // When paused, only check for unpause or exit commands
                    if (Console.KeyAvailable)
                    {
                        var key = Console.ReadKey(true);
                        if (key.Key == ConsoleKey.P)
                        {
                            isPaused = false;
                            DisplayGame(gameplayInterface, isPaused); // Refresh to remove pause message
                        }
                        else if (key.Key == ConsoleKey.Escape)
                        {
                            PromptExitToMenu(
                                game,
                                gameplayInterface,
                                ref isPaused,
                                ref isGameActive
                            );
                        }
                    }
                }

                // Calculate optimal delay time based on frame timing
                TimeSpan frameTime = DateTime.Now - lastFrameTime;
                int delayTime = Math.Max(1, baseFrameDelay - (int)frameTime.TotalMilliseconds);
                lastFrameTime = DateTime.Now;

                // Use adaptive frame timing
                await Task.Delay(delayTime);
            }
        }

        /// <summary>
        /// An enhanced game loop specifically for use with the complete gameplay interface,
        /// with additional optimizations and responsiveness improvements.
        /// </summary>
        public static async Task RunEnhancedGameLoopAsync(
            this TetrisGame game,
            GameEngine gameEngine,
            GameplayInterfaceComplete gameplayInterface,
            bool isPaused,
            bool isGameActive,
            bool isExiting
        )
        {
            Console.Clear();
            Console.CursorVisible = false;

            // Reset state for new game
            isPaused = false;

            // Initialize the game interface with double buffering enabled
            gameplayInterface.EnableDoubleBuffering = true;
            gameplayInterface.Initialize();

            // Display initial board
            gameplayInterface.Render();

            // Variables for controlling timing
            const int targetFrameRate = 60;
            const int targetFrameTime = 1000 / targetFrameRate;
            DateTime lastFrameTime = DateTime.Now;
            DateTime lastGameUpdate = DateTime.Now;
            int gameUpdateInterval = 50; // Start with 20 updates per second

            // Game loop continues until game is over or player exits
            while (isGameActive && !isExiting)
            {
                // Start frame timing
                DateTime frameStartTime = DateTime.Now;

                // Update game update interval based on level (faster updates at higher levels)
                gameUpdateInterval = Math.Max(10, 50 - (gameEngine.Level * 2));

                // Check for window resize
                gameplayInterface.CheckForResize();

                // Only process input and update display if game is not paused
                if (!isPaused)
                {
                    // Process all available input with priority for responsiveness
                    int maxInputProcessingPerFrame = 10; // Prevent input flood
                    int inputsProcessed = 0;

                    while (Console.KeyAvailable && inputsProcessed < maxInputProcessingPerFrame)
                    {
                        var key = Console.ReadKey(true);

                        // Use the enhanced input processing if available
                        bool fastDropActive = gameplayInterface.ProcessGameInput(key);
                        // Handle non-movement keys
                        HandleSpecialKeysForResponsiveInterface(
                            game,
                            key,
                            gameplayInterface,
                            ref isPaused,
                            ref isGameActive,
                            ref isExiting
                        );

                        // Ensure fast drop state is consistent
                        if (!fastDropActive)
                        {
                            gameEngine.DeactivateFastDrop();
                        }

                        inputsProcessed++;
                    }

                    // Update game state at appropriate intervals
                    TimeSpan timeSinceLastUpdate = DateTime.Now - lastGameUpdate;
                    if (timeSinceLastUpdate.TotalMilliseconds > gameUpdateInterval)
                    {
                        // Render the game with the current state
                        gameplayInterface.Render();
                        lastGameUpdate = DateTime.Now;
                    }
                }
                else
                {
                    // When paused, only check for unpause or exit commands
                    if (Console.KeyAvailable)
                    {
                        var key = Console.ReadKey(true);
                        if (key.Key == ConsoleKey.P)
                        {
                            isPaused = false;
                            gameplayInterface.Render(); // Refresh to remove pause message
                        }
                        else if (key.Key == ConsoleKey.Escape)
                        {
                            // Handle escape key in pause mode
                            HandleSpecialKeysForResponsiveInterface(
                                game,
                                key,
                                gameplayInterface,
                                ref isPaused,
                                ref isGameActive,
                                ref isExiting
                            );
                        }
                    }

                    // Show pause overlay
                    gameplayInterface.ShowPauseOverlay();
                }

                // Calculate frame timing and delay to maintain target framerate
                TimeSpan frameTime = DateTime.Now - frameStartTime;
                int delayTime = Math.Max(1, targetFrameTime - (int)frameTime.TotalMilliseconds);

                // Use adaptive frame timing for smoother experience
                await Task.Delay(delayTime);
                lastFrameTime = DateTime.Now;
            }
        }

        /// <summary>
        /// Creates a responsive interface for the game and starts the enhanced game loop.
        /// </summary>
        /// <param name="game">The TetrisGame instance.</param>
        /// <returns>A task representing the game loop.</returns>
        public static async Task RunWithResponsiveInterfaceAsync(this TetrisGame game)
        {
            // Use reflection to access the private game engine field
            var gameEngineField = game.GetType()
                .GetField(
                    "_gameEngine",
                    System.Reflection.BindingFlags.NonPublic
                        | System.Reflection.BindingFlags.Instance
                );

            if (gameEngineField == null)
            {
                throw new InvalidOperationException("Could not access the game engine field.");
            }

            var gameEngine = gameEngineField.GetValue(game) as GameEngine;

            if (gameEngine == null)
            {
                throw new InvalidOperationException("Game engine is null.");
            }

            // Create responsive interface
            var gameplayInterfaceComplete = new GameplayInterfaceComplete(gameEngine);

            // Set initial state
            bool isPaused = false;
            bool isGameActive = true;
            bool isExiting = false;

            // Run the enhanced game loop
            await RunEnhancedGameLoopAsync(
                game,
                gameEngine,
                gameplayInterfaceComplete,
                isPaused,
                isGameActive,
                isExiting
            );
        }

        /// <summary>
        /// Displays the current game state using the GameplayInterface.
        /// </summary>
        private static void DisplayGame(GameplayInterface gameplayInterface, bool isPaused)
        {
            // Use the GameplayInterface to render the game
            gameplayInterface.Render();

            // Show pause overlay if game is paused
            if (isPaused)
            {
                gameplayInterface.ShowPauseOverlay();
            }
        }

        /// <summary>
        /// Processes user input during gameplay with improved responsiveness.
        /// </summary>
        private static void ProcessGameInput(
            TetrisGame game,
            GameEngine gameEngine,
            GameplayInterface gameplayInterface,
            ref bool isPaused,
            ref bool isGameActive,
            ref bool isExiting
        )
        {
            var key = Console.ReadKey(true);
            bool fastDropActive = false; // Try to use the improved input processing method if available
            var processGameInputMethod = gameplayInterface
                .GetType()
                .GetMethod("ProcessGameInputImproved");
            if (processGameInputMethod != null)
            {
                var result = processGameInputMethod.Invoke(gameplayInterface, new object[] { key });
                if (result is bool boolResult)
                {
                    fastDropActive = boolResult;
                }
            }
            else
            {
                // Fall back to standard input processing
                switch (key.Key)
                {
                    case ConsoleKey.LeftArrow:
                    case ConsoleKey.A: // Alternative control
                        gameEngine.MovePieceLeft();
                        break;

                    case ConsoleKey.RightArrow:
                    case ConsoleKey.D: // Alternative control
                        gameEngine.MovePieceRight();
                        break;

                    case ConsoleKey.UpArrow:
                    case ConsoleKey.W: // Alternative control
                        gameEngine.RotatePieceClockwise();
                        break;

                    case ConsoleKey.DownArrow:
                    case ConsoleKey.S: // Alternative control
                        gameEngine.ActivateFastDrop();
                        fastDropActive = true;
                        break;

                    case ConsoleKey.Spacebar:
                        gameEngine.HardDrop();
                        break;

                    case ConsoleKey.Z:
                        // Alternative rotation (counterclockwise) if implemented
                        var counterClockwiseMethod = gameEngine
                            .GetType()
                            .GetMethod("RotatePieceCounterClockwise");
                        counterClockwiseMethod?.Invoke(gameEngine, null);
                        break;

                    case ConsoleKey.C:
                    case ConsoleKey.X: // Alternative hold key
                        // Hold piece functionality if implemented
                        var holdMethod = gameEngine.GetType().GetMethod("HoldCurrentPiece");
                        holdMethod?.Invoke(gameEngine, null);
                        break;
                }
            }

            // Handle special keys for game control
            HandleSpecialKeys(game, key, ref isPaused, ref isGameActive, ref isExiting);

            // Release fast drop if not active
            if (!fastDropActive)
            {
                gameEngine.DeactivateFastDrop();
            }
        }

        /// <summary>
        /// Handles special keys like pause and exit.
        /// </summary>
        private static void HandleSpecialKeys(
            TetrisGame game,
            ConsoleKeyInfo key,
            ref bool isPaused,
            ref bool isGameActive,
            ref bool isExiting
        )
        {
            switch (key.Key)
            {
                case ConsoleKey.P:
                    TogglePause(game, ref isPaused);
                    break;
                case ConsoleKey.Escape:
                    // For escape key, we only set the pause flag - the caller will handle showing the exit menu
                    isPaused = true;
                    break;
            }
        }

        /// <summary>
        /// Toggles the pause state of the game.
        /// </summary>
        private static void TogglePause(TetrisGame game, ref bool isPaused)
        {
            isPaused = !isPaused;
            // Call the display method from the parent class through reflection if needed
            var displayMethod = game.GetType()
                .GetMethod(
                    "DisplayGame",
                    System.Reflection.BindingFlags.NonPublic
                        | System.Reflection.BindingFlags.Instance
                );
            if (displayMethod != null)
            {
                displayMethod.Invoke(game, null);
            }
        }

        /// <summary>
        /// Prompts the user if they want to exit to the main menu.
        /// </summary>
        private static void PromptExitToMenu(
            TetrisGame game,
            GameplayInterface gameplayInterface,
            ref bool isPaused,
            ref bool isGameActive
        )
        {
            isPaused = true;

            // If we have a gameplay interface, use it to show the pause overlay
            if (gameplayInterface != null)
            {
                gameplayInterface.Render();
                gameplayInterface.ShowPauseOverlay();
            }
            else
            {
                // Call the display method from the parent class through reflection if needed
                var displayMethod = game.GetType()
                    .GetMethod(
                        "DisplayGame",
                        System.Reflection.BindingFlags.NonPublic
                            | System.Reflection.BindingFlags.Instance
                    );
                if (displayMethod != null)
                {
                    displayMethod.Invoke(game, null);
                }
            }

            int centerX = Console.WindowWidth / 2 - 15;
            int centerY = Console.WindowHeight / 2 - 2;

            Console.ForegroundColor = ConsoleColor.Yellow;

            Console.SetCursorPosition(centerX, centerY);
            Console.WriteLine("┌───────────────────────────┐");
            Console.SetCursorPosition(centerX, centerY + 1);
            Console.WriteLine("│  Exit to main menu? (Y/N) │");
            Console.SetCursorPosition(centerX, centerY + 2);
            Console.WriteLine("└───────────────────────────┘");

            Console.ForegroundColor = ConsoleColor.White;
            bool answered = false;
            while (!answered)
            {
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Y)
                {
                    isGameActive = false;
                    answered = true;
                }
                else if (key.Key == ConsoleKey.N)
                {
                    isPaused = false;
                    answered = true;

                    if (gameplayInterface != null)
                    {
                        gameplayInterface.Render();
                    }
                    else
                    {
                        var displayMethod = game.GetType()
                            .GetMethod(
                                "DisplayGame",
                                System.Reflection.BindingFlags.NonPublic
                                    | System.Reflection.BindingFlags.Instance
                            );
                        if (displayMethod != null)
                        {
                            displayMethod.Invoke(game, null);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Enhanced exit prompt with improved visual design.
        /// </summary>
        private static void PromptExitMenuEnhanced(
            TetrisGame game,
            GameplayInterfaceComplete gameplayInterface,
            ref bool isPaused,
            ref bool isGameActive
        )
        {
            isPaused = true;

            // Render the game and pause overlay
            gameplayInterface.Render();
            gameplayInterface.ShowPauseOverlay();

            int centerX = Console.WindowWidth / 2 - 15;
            int centerY = Console.WindowHeight / 2 - 2;

            Console.ForegroundColor = ConsoleColor.Yellow;

            Console.SetCursorPosition(centerX, centerY);
            Console.WriteLine("┌───────────────────────────┐");
            Console.SetCursorPosition(centerX, centerY + 1);
            Console.WriteLine("│  Exit to main menu? (Y/N) │");
            Console.SetCursorPosition(centerX, centerY + 2);
            Console.WriteLine("└───────────────────────────┘");

            Console.ForegroundColor = ConsoleColor.White;

            bool answered = false;
            while (!answered)
            {
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Y || key.Key == ConsoleKey.Enter)
                {
                    isGameActive = false;
                    answered = true;
                }
                else if (key.Key == ConsoleKey.N || key.Key == ConsoleKey.Escape)
                {
                    isPaused = false;
                    answered = true;
                    gameplayInterface.Render();
                }
            }
        }

        /// <summary>
        /// Handles special keys like pause and exit for the responsive interface.
        /// </summary>
        private static void HandleSpecialKeysForResponsiveInterface(
            TetrisGame game,
            ConsoleKeyInfo key,
            GameplayInterfaceComplete gameplayInterface,
            ref bool isPaused,
            ref bool isGameActive,
            ref bool isExiting
        )
        {
            switch (key.Key)
            {
                case ConsoleKey.P:
                    // Toggle pause state
                    isPaused = !isPaused;
                    if (!isPaused)
                    {
                        gameplayInterface.Render(); // Refresh to remove pause message
                    }
                    break;

                case ConsoleKey.Escape:
                    if (isPaused)
                    {
                        PromptExitMenuEnhanced(
                            game,
                            gameplayInterface,
                            ref isPaused,
                            ref isGameActive
                        );
                    }
                    else
                    {
                        isPaused = true;
                        gameplayInterface.ShowPauseOverlay();
                    }
                    break;
            }
        }

        /// <summary>
        /// Enables the use of the responsive interface in the TetrisGame class.
        /// </summary>
        /// <param name="game">The TetrisGame instance.</param>
        /// <returns>The same TetrisGame instance for method chaining.</returns>
        public static TetrisGame EnableResponsiveInterface(this TetrisGame game)
        {
            // Use reflection to add an event handler to the NewGameRequested event
            var mainMenuField = game.GetType()
                .GetField(
                    "_mainMenu",
                    System.Reflection.BindingFlags.NonPublic
                        | System.Reflection.BindingFlags.Instance
                );

            if (mainMenuField != null)
            {
                var mainMenu = mainMenuField.GetValue(game);
                if (mainMenu != null)
                {
                    // Get the event field
                    var eventField = mainMenu.GetType().GetEvent("NewGameRequested");

                    if (eventField != null)
                    {
                        // Create a handler for the NewGameRequested event that will start the responsive interface
                        EventHandler handler = async (sender, e) =>
                        {
                            // Set game active flag through reflection
                            var isActiveField = game.GetType()
                                .GetField(
                                    "_isGameActive",
                                    System.Reflection.BindingFlags.NonPublic
                                        | System.Reflection.BindingFlags.Instance
                                );
                            if (isActiveField != null)
                            {
                                isActiveField.SetValue(game, true);
                            }

                            // Get game engine and start a new game
                            var gameEngineField = game.GetType()
                                .GetField(
                                    "_gameEngine",
                                    System.Reflection.BindingFlags.NonPublic
                                        | System.Reflection.BindingFlags.Instance
                                );
                            if (gameEngineField != null)
                            {
                                var gameEngine = gameEngineField.GetValue(game) as GameEngine;
                                if (gameEngine != null)
                                {
                                    gameEngine.StartNewGame();

                                    // Run the game with responsive interface
                                    await RunWithResponsiveInterfaceAsync(game);
                                }
                            }
                        };

                        // Add the handler to the event
                        eventField.AddEventHandler(mainMenu, handler);
                    }
                }
            }

            return game;
        }
    }
}
