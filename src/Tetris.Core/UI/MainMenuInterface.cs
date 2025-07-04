using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Tetris.Core.Models;
using Tetris.Core.Services;

namespace Tetris.Core.UI
{
    /// <summary>
    /// Represents a responsive main menu interface for the Tetris game.
    /// Provides options to start a new game, load a saved game, and access settings.
    /// </summary>
    public class MainMenuInterface
    {
        #region Constants

        /// <summary>
        /// The directory where game saves are stored.
        /// </summary>
        private const string SaveDirectory = "Saves";

        /// <summary>
        /// The file extension for save files.
        /// </summary>
        private const string SaveExtension = ".tetris";

        #endregion

        #region Fields

        private GameEngine? _gameEngine;
        private readonly List<MenuItem> _menuItems;
        private int _selectedIndex;
        private bool _isActive;
        private bool _initialized;
        private readonly Dictionary<string, Action> _menuActions;

        #endregion

        #region Events

        /// <summary>
        /// Event raised when a new game is requested to start.
        /// </summary>
        public event EventHandler<GameModeSelectionEventArgs>? NewGameRequested;

        /// <summary>
        /// Event raised when a saved game is loaded.
        /// </summary>
        public event EventHandler<GameLoadedEventArgs>? GameLoaded;

        /// <summary>
        /// Event raised when the application is requested to exit.
        /// </summary>
        public event EventHandler? ExitRequested;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the title of the menu.
        /// </summary>
        public string Title { get; set; } = "TETRIS";

        /// <summary>
        /// Gets or sets the subtitle displayed under the title.
        /// </summary>
        public string Subtitle { get; set; } = "Main Menu";

        /// <summary>
        /// Gets or sets the footer text for the menu.
        /// </summary>
        public string FooterText { get; set; } =
            "Use arrow keys to navigate and ENTER to select. ESC to exit.";

        /// <summary>
        /// Gets or sets the color used for the title text.
        /// </summary>
        public ConsoleColor TitleColor { get; set; } = ConsoleColor.Cyan;

        /// <summary>
        /// Gets or sets the color used for selected menu items.
        /// </summary>
        public ConsoleColor SelectedItemColor { get; set; } = ConsoleColor.Yellow;

        /// <summary>
        /// Gets or sets the color used for normal menu items.
        /// </summary>
        public ConsoleColor NormalItemColor { get; set; } = ConsoleColor.White;

        /// <summary>
        /// Gets or sets the color used for the background.
        /// </summary>
        public ConsoleColor BackgroundColor { get; set; } = ConsoleColor.Black;

        /// <summary>
        /// Gets or sets the animation speed for the menu elements in milliseconds.
        /// </summary>
        public int AnimationSpeed { get; set; } = 20;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the MainMenuInterface class.
        /// </summary>
        public MainMenuInterface()
        {
            _menuItems = new List<MenuItem>
            {
                new MenuItem("New Game", "Start a new Tetris game"),
                new MenuItem("Load Game", "Load a previously saved game"),
                new MenuItem("Statistics", "View game statistics and achievements"),
                new MenuItem("Settings", "Configure game options"),
                new MenuItem("High Scores", "View the leaderboard of high scores"),
                new MenuItem("Help", "View game instructions and controls"),
                new MenuItem("Exit", "Exit the game")
            };

            _menuActions = new Dictionary<string, Action>
            {
                { "New Game", StartNewGame },
                { "Load Game", ShowLoadGameMenu },
                { "Statistics", ShowStatistics },
                { "Settings", ShowSettingsMenu },
                { "High Scores", ShowHighScores },
                { "Help", ShowHelp },
                { "Exit", Exit }
            };

            _selectedIndex = 0;
            _isActive = false;
            _initialized = false;
        }

        /// <summary>
        /// Initializes a new instance of the MainMenuInterface class with a specific game engine.
        /// </summary>
        /// <param name="gameEngine">The game engine to use for game operations.</param>
        public MainMenuInterface(GameEngine gameEngine)
            : this()
        {
            _gameEngine = gameEngine ?? throw new ArgumentNullException(nameof(gameEngine));
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Shows the main menu and starts handling user input.
        /// </summary>
        /// <returns>A task that completes when the menu is closed.</returns>
        public async Task ShowAsync()
        {
            if (!_initialized)
            {
                Initialize();
            }

            _isActive = true;
            await AnimateMenuOpeningAsync();

            while (_isActive)
            {
                RenderMenu();
                await HandleInputAsync();
                await Task.Delay(50); // Prevents CPU overuse
            }
        }

        /// <summary>
        /// Sets the game engine to use for operations.
        /// </summary>
        /// <param name="gameEngine">The game engine to use.</param>
        public void SetGameEngine(GameEngine gameEngine)
        {
            _gameEngine = gameEngine ?? throw new ArgumentNullException(nameof(gameEngine));
        }

        /// <summary>
        /// Closes the menu.
        /// </summary>
        public void Close()
        {
            _isActive = false;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Initializes the menu interface.
        /// </summary>
        private void Initialize()
        {
            Console.CursorVisible = false;
            Console.BackgroundColor = BackgroundColor;
            _initialized = true;

            // Create save directory if it doesn't exist
            if (!Directory.Exists(SaveDirectory))
            {
                try
                {
                    Directory.CreateDirectory(SaveDirectory);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to create save directory: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Renders the menu with all its elements.
        /// </summary>
        private void RenderMenu()
        {
            Console.Clear();
            Console.BackgroundColor = BackgroundColor;

            // Calculate position for centered title
            int titleX = Console.WindowWidth / 2 - Title.Length / 2;
            int titleY = Console.WindowHeight / 4;

            // Draw title
            Console.ForegroundColor = TitleColor;
            Console.SetCursorPosition(titleX, titleY);
            Console.WriteLine(Title);

            // Draw subtitle
            Console.SetCursorPosition(Console.WindowWidth / 2 - Subtitle.Length / 2, titleY + 1);
            Console.WriteLine(Subtitle);

            // Draw menu items
            int menuStartY = titleY + 3;
            for (int i = 0; i < _menuItems.Count; i++)
            {
                // Center each menu item
                int itemX = Console.WindowWidth / 2 - _menuItems[i].Text.Length / 2;

                Console.ForegroundColor = i == _selectedIndex ? SelectedItemColor : NormalItemColor;
                Console.SetCursorPosition(itemX, menuStartY + i);

                // Draw selection indicator
                if (i == _selectedIndex)
                {
                    Console.Write("> ");
                }
                else
                {
                    Console.Write("  ");
                }

                Console.WriteLine(_menuItems[i].Text);
            }

            // Draw footer text
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.SetCursorPosition(
                Console.WindowWidth / 2 - FooterText.Length / 2,
                menuStartY + _menuItems.Count + 2
            );
            Console.WriteLine(FooterText);

            // Draw tetromino decoration in the background
            DrawTetrisPieces();
        }

        /// <summary>
        /// Handles user input for menu navigation.
        /// </summary>
        private async Task HandleInputAsync()
        {
            if (Console.KeyAvailable)
            {
                var keyInfo = Console.ReadKey(true);

                switch (keyInfo.Key)
                {
                    case ConsoleKey.UpArrow:
                        _selectedIndex = Math.Max(0, _selectedIndex - 1);
                        break;

                    case ConsoleKey.DownArrow:
                        _selectedIndex = Math.Min(_menuItems.Count - 1, _selectedIndex + 1);
                        break;

                    case ConsoleKey.Enter:
                        await ExecuteSelectedMenuItemAsync();
                        break;

                    case ConsoleKey.Escape:
                        Close();
                        break;
                }
            }
        }

        /// <summary>
        /// Executes the action associated with the selected menu item.
        /// </summary>
        private async Task ExecuteSelectedMenuItemAsync()
        {
            var selectedItem = _menuItems[_selectedIndex].Text;

            if (_menuActions.ContainsKey(selectedItem))
            {
                await Task.Run(() => _menuActions[selectedItem].Invoke());
            }
        }

        /// <summary>
        /// Starts a new game by showing difficulty selection and then invoking the NewGameRequested event.
        /// </summary>
        private async void StartNewGame()
        {
            // Show difficulty selection dialog
            var difficultyDialog = new DifficultySelectionDialog();
            DifficultyLevel selectedDifficulty = await difficultyDialog.ShowAsync();

            // Show game mode selection dialog
            var gameModeDialog = new GameModeSelectionDialog(selectedDifficulty);
            GameMode selectedGameMode = await gameModeDialog.ShowAsync();

            // Close the main menu
            _isActive = false;

            // Pass the selected difficulty and game mode as event args
            NewGameRequested?.Invoke(
                this,
                new GameModeSelectionEventArgs(selectedGameMode, selectedDifficulty)
            );
        }

        /// <summary>
        /// Shows the load game menu using the new LoadGameDialog.
        /// </summary>
        private async void ShowLoadGameMenu()
        {
            try
            {
                var loadDialog = new LoadGameDialog();
                SaveFileInfo? selectedSave = await loadDialog.ShowAsync();
                
                if (selectedSave != null)
                {
                    // Load the selected game
                    var saveService = new GameSaveService();
                    GameState gameState = await saveService.LoadGameAsync(selectedSave.SaveName);
                    
                    // Close the main menu
                    _isActive = false;
                    
                    // Raise event to load the game
                    GameLoaded?.Invoke(this, new GameLoadedEventArgs(gameState));
                }
            }
            catch (Exception ex)
            {
                // Show error message
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Failed to load game: {ex.Message}");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("\nPress any key to return to the main menu...");
                Console.ReadKey(true);
            }
        }

        /// <summary>
        /// Shows the settings menu using the new SettingsInterface.
        /// </summary>
        private async void ShowSettingsMenu()
        {
            try
            {
                var settingsService = new Services.UserSettingsService();
                var settingsInterface = new SettingsInterface(settingsService);
                
                settingsInterface.ReturnToMenuRequested += (sender, args) => 
                {
                    // Return to menu is handled by the main game loop
                };
                
                settingsInterface.SettingsSaved += (sender, args) => 
                {
                    // Settings have been saved, could show confirmation or apply them
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Settings saved successfully!");
                    Console.WriteLine("Changes will take effect immediately.");
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine("Press any key to return to the main menu...");
                    Console.ReadKey(true);
                };

                await settingsInterface.ShowAsync();
            }
            catch (Exception ex)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error opening settings: {ex.Message}");
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("Press any key to return to the main menu...");
                Console.ReadKey(true);
            }
        }

        /// <summary>
        /// Shows the statistics screen.
        /// </summary>
        private async void ShowStatistics()
        {
            try
            {
                var statisticsInterface = new StatisticsInterface();
                statisticsInterface.ReturnToMenuRequested += (sender, args) => 
                {
                    // Return to menu is handled by the main game loop
                };
                
                await statisticsInterface.ShowAsync();
            }
            catch (Exception ex)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error loading statistics: {ex.Message}");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("\nPress any key to return to the main menu...");
                Console.ReadKey(true);
            }
        }

        /// <summary>
        /// Shows the high scores screen.
        /// </summary>
        private void ShowHighScores()
        {
            Console.Clear();
            Console.ForegroundColor = TitleColor;
            Console.WriteLine("===== HIGH SCORES =====");
            Console.WriteLine();

            // In a real implementation, this would load high scores from a file
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("1. ABC .......... 20000");
            Console.WriteLine("2. DEF .......... 15000");
            Console.WriteLine("3. GHI .......... 10000");
            Console.WriteLine("4. JKL .......... 5000");
            Console.WriteLine("5. MNO .......... 2500");

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("\nPress any key to return to the main menu...");
            Console.ReadKey(true);
        }

        /// <summary>
        /// Shows the help screen with game instructions.
        /// </summary>
        private void ShowHelp()
        {
            Console.Clear();
            Console.ForegroundColor = TitleColor;
            Console.WriteLine("===== HELP =====");
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Game Controls:");
            Console.WriteLine();
            Console.WriteLine("Left/Right Arrow - Move piece left/right");
            Console.WriteLine("Up Arrow - Rotate piece clockwise");
            Console.WriteLine("Down Arrow - Soft drop (accelerate fall)");
            Console.WriteLine("Spacebar - Hard drop (instant placement)");
            Console.WriteLine("P - Pause game");
            Console.WriteLine("Esc - Exit to main menu");
            Console.WriteLine();

            Console.WriteLine("Game Rules:");
            Console.WriteLine("1. Complete lines by filling all horizontal blocks.");
            Console.WriteLine("2. Completed lines disappear and award points.");
            Console.WriteLine("3. Game speeds up as your level increases.");
            Console.WriteLine("4. Game ends when blocks reach the top of the board.");

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("\nPress any key to return to the main menu...");
            Console.ReadKey(true);
        }

        /// <summary>
        /// Exits the application.
        /// </summary>
        private void Exit()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Are you sure you want to exit? (Y/N)");

            var key = Console.ReadKey(true);
            if (key.Key == ConsoleKey.Y)
            {
                _isActive = false;
                ExitRequested?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Draws Tetris pieces as a background decoration.
        /// </summary>
        private void DrawTetrisPieces()
        {
            // Draw some tetris pieces in the background with a faded color
            Console.ForegroundColor = ConsoleColor.DarkCyan;

            // Draw an I piece
            DrawPieceAtPosition(2, 2, new[] { "####" });

            // Draw a T piece
            DrawPieceAtPosition(Console.WindowWidth - 8, 3, new[] { " # ", "###" });

            // Draw an L piece
            DrawPieceAtPosition(5, Console.WindowHeight - 5, new[] { "#  ", "###" });

            // Draw a Z piece
            DrawPieceAtPosition(
                Console.WindowWidth - 10,
                Console.WindowHeight - 4,
                new[] { "## ", " ##" }
            );
        }

        /// <summary>
        /// Draws a Tetris piece at the specified position.
        /// </summary>
        /// <param name="x">The x-coordinate.</param>
        /// <param name="y">The y-coordinate.</param>
        /// <param name="pattern">The pattern of the piece.</param>
        private void DrawPieceAtPosition(int x, int y, string[] pattern)
        {
            for (int row = 0; row < pattern.Length; row++)
            {
                for (int col = 0; col < pattern[row].Length; col++)
                {
                    if (pattern[row][col] == '#')
                    {
                        // Make sure we're not drawing outside the console window
                        if (
                            x + col >= 0
                            && x + col < Console.WindowWidth
                            && y + row >= 0
                            && y + row < Console.WindowHeight
                        )
                        {
                            Console.SetCursorPosition(x + col, y + row);
                            Console.Write("█");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Animates the menu opening.
        /// </summary>
        private async Task AnimateMenuOpeningAsync()
        {
            // First clear the screen
            Console.Clear();

            // Animate the title
            int titleX = Console.WindowWidth / 2 - Title.Length / 2;
            int titleY = Console.WindowHeight / 4;

            Console.ForegroundColor = TitleColor;

            // Fade in effect
            for (int i = 0; i < Title.Length; i++)
            {
                Console.SetCursorPosition(titleX + i, titleY);
                Console.Write(Title[i]);
                await Task.Delay(AnimationSpeed);
            }

            // Animate subtitle
            Console.ForegroundColor = ConsoleColor.White;
            int subtitleX = Console.WindowWidth / 2 - Subtitle.Length / 2;

            for (int i = 0; i < Subtitle.Length; i++)
            {
                Console.SetCursorPosition(subtitleX + i, titleY + 1);
                Console.Write(Subtitle[i]);
                await Task.Delay(AnimationSpeed / 2);
            }

            // Animate menu items appearing
            int menuStartY = titleY + 3;

            for (int i = 0; i < _menuItems.Count; i++)
            {
                Console.ForegroundColor = i == _selectedIndex ? SelectedItemColor : NormalItemColor;
                int itemX = Console.WindowWidth / 2 - _menuItems[i].Text.Length / 2 - 2; // Account for selection indicator

                Console.SetCursorPosition(itemX, menuStartY + i);
                Console.Write(i == _selectedIndex ? "> " : "  ");
                Console.Write(_menuItems[i].Text);

                await Task.Delay(AnimationSpeed * 3);
            }
        }

        #endregion
    }

    /// <summary>
    /// Represents a menu item with a display text and a description.
    /// </summary>
    public class MenuItem
    {
        /// <summary>
        /// Gets the display text of the menu item.
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// Gets the description of the menu item.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Initializes a new instance of the MenuItem class.
        /// </summary>
        /// <param name="text">The display text of the menu item.</param>
        /// <param name="description">The description of the menu item.</param>
        public MenuItem(string text, string description)
        {
            Text = text;
            Description = description;
        }
    }
}
