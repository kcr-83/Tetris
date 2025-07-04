using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tetris.Core.Models;

namespace Tetris.Core.UI
{
    /// <summary>
    /// Dialog for configuring gameplay-specific settings such as ghost piece visibility and other game behavior preferences.
    /// </summary>
    public class GameplaySettingsDialog
    {
        #region Constants

        private const string Title = "GAMEPLAY SETTINGS";
        private const string SubTitle = "Configure game behavior and features";

        #endregion

        #region Fields

        private readonly UserSettings _settings;
        private readonly List<GameplayMenuItem> _menuItems;
        private int _selectedIndex;
        private bool _isActive;

        #endregion

        #region Properties

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
        /// Gets or sets the color used for value display.
        /// </summary>
        public ConsoleColor ValueColor { get; set; } = ConsoleColor.Green;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the GameplaySettingsDialog class.
        /// </summary>
        /// <param name="settings">The user settings to modify.</param>
        public GameplaySettingsDialog(UserSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _selectedIndex = 0;
            _isActive = false;

            _menuItems = new List<GameplayMenuItem>
            {
                new("Ghost Piece", () => ToggleGhostPiece()),
                new("Show Game Tips", () => ToggleGameTips()),
                new("Auto-Pause on Focus Loss", () => ToggleAutoPause()),
                new("Confirm Exit", () => ToggleConfirmExit()),
                new("Reset to Defaults", () => ResetGameplayToDefaults()),
                new("Return", () => ReturnToPrevious())
            };
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Shows the gameplay settings dialog and handles user interaction.
        /// </summary>
        /// <returns>A task that completes when the user exits the dialog.</returns>
        public async Task ShowAsync()
        {
            Console.CursorVisible = false;
            _isActive = true;

            while (_isActive)
            {
                RenderDialog();
                await HandleInputAsync();
                await Task.Delay(50); // Prevent high CPU usage
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Renders the gameplay settings dialog.
        /// </summary>
        private void RenderDialog()
        {
            Console.Clear();
            Console.BackgroundColor = ConsoleColor.Black;

            // Calculate positions for centered display
            int windowWidth = Console.WindowWidth;
            int windowHeight = Console.WindowHeight;

            int titleX = Math.Max(0, (windowWidth - Title.Length) / 2);
            int titleY = Math.Max(0, windowHeight / 8);

            // Draw title
            Console.ForegroundColor = TitleColor;
            Console.SetCursorPosition(titleX, titleY);
            Console.WriteLine(Title);

            // Draw subtitle
            Console.ForegroundColor = ConsoleColor.Gray;
            int subtitleX = Math.Max(0, (windowWidth - SubTitle.Length) / 2);
            Console.SetCursorPosition(subtitleX, titleY + 1);
            Console.WriteLine(SubTitle);

            // Draw menu items
            int menuStartY = titleY + 4;
            int maxItemWidth = Math.Max(50, _menuItems.Max(item => item.Text.Length) + 35);
            int menuX = Math.Max(0, (windowWidth - maxItemWidth) / 2);

            for (int i = 0; i < _menuItems.Count; i++)
            {
                Console.ForegroundColor = i == _selectedIndex ? SelectedItemColor : NormalItemColor;
                Console.SetCursorPosition(menuX, menuStartY + i * 2);

                string selectionIndicator = i == _selectedIndex ? "> " : "  ";
                string itemText = $"{selectionIndicator}{_menuItems[i].Text}";

                // Add current value display
                string valueDisplay = GetValueDisplayForMenuItem(i);
                if (!string.IsNullOrEmpty(valueDisplay))
                {
                    itemText = $"{itemText,-30} : {valueDisplay}";
                }

                Console.WriteLine(itemText);

                // Add description for selected item
                if (i == _selectedIndex)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.SetCursorPosition(menuX + 2, menuStartY + i * 2 + 1);
                    Console.WriteLine(GetDescriptionForMenuItem(i));
                }
            }

            // Draw footer
            Console.ForegroundColor = ConsoleColor.DarkGray;
            string footerText = "Use ↑↓ to navigate, ENTER/SPACE to toggle, ESC to return";
            int footerX = Math.Max(0, (windowWidth - footerText.Length) / 2);
            Console.SetCursorPosition(footerX, windowHeight - 3);
            Console.WriteLine(footerText);

            // Draw gameplay info
            DrawGameplayInfo(windowWidth, windowHeight);
        }

        /// <summary>
        /// Handles user input for the gameplay settings dialog.
        /// </summary>
        /// <returns>A task that completes when input is processed.</returns>
        private async Task HandleInputAsync()
        {
            if (!Console.KeyAvailable)
            {
                return;
            }

            ConsoleKeyInfo keyInfo = Console.ReadKey(true);

            switch (keyInfo.Key)
            {
                case ConsoleKey.UpArrow:
                    _selectedIndex = Math.Max(0, _selectedIndex - 1);
                    break;

                case ConsoleKey.DownArrow:
                    _selectedIndex = Math.Min(_menuItems.Count - 1, _selectedIndex + 1);
                    break;

                case ConsoleKey.Enter:
                case ConsoleKey.Spacebar:
                    await _menuItems[_selectedIndex].Action();
                    break;

                case ConsoleKey.Escape:
                    await ReturnToPrevious();
                    break;
            }
        }

        /// <summary>
        /// Gets the value display text for a menu item.
        /// </summary>
        /// <param name="itemIndex">The index of the menu item.</param>
        /// <returns>The value display text.</returns>
        private string GetValueDisplayForMenuItem(int itemIndex)
        {
            return itemIndex switch
            {
                0 => _settings.ShowGhostPiece ? "ON" : "OFF",
                1 => "ON", // Placeholder for game tips setting
                2 => "ON", // Placeholder for auto-pause setting
                3 => "ON", // Placeholder for confirm exit setting
                4 => "Press ENTER to reset",
                _ => string.Empty
            };
        }

        /// <summary>
        /// Gets the description text for a menu item.
        /// </summary>
        /// <param name="itemIndex">The index of the menu item.</param>
        /// <returns>The description text.</returns>
        private string GetDescriptionForMenuItem(int itemIndex)
        {
            return itemIndex switch
            {
                0 => "Show a preview of where the current piece will land",
                1 => "Display helpful tips and controls during gameplay",
                2 => "Automatically pause the game when window loses focus",
                3 => "Show confirmation dialog when exiting a game",
                4 => "Reset all gameplay settings to their default values",
                5 => "Return to the main settings menu",
                _ => string.Empty
            };
        }

        /// <summary>
        /// Draws additional gameplay information.
        /// </summary>
        /// <param name="windowWidth">The width of the console window.</param>
        /// <param name="windowHeight">The height of the console window.</param>
        private void DrawGameplayInfo(int windowWidth, int windowHeight)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;

            int infoY = windowHeight - 10;
            string[] infoLines = new[]
            {
                "Gameplay Features:",
                $"Ghost Piece: {(_settings.ShowGhostPiece ? "Enabled - Shows piece landing position" : "Disabled - No landing preview")}",
                "Game Tips: Enabled - Shows helpful information during play",
                "Auto-Pause: Enabled - Pauses when window loses focus",
                "Confirm Exit: Enabled - Asks before quitting a game"
            };

            for (int i = 0; i < infoLines.Length; i++)
            {
                string line = infoLines[i];
                int lineX = Math.Max(0, (windowWidth - line.Length) / 2);
                Console.SetCursorPosition(lineX, infoY + i);
                Console.WriteLine(line);
            }
        }

        /// <summary>
        /// Toggles the ghost piece setting.
        /// </summary>
        /// <returns>A task that completes when the setting is toggled.</returns>
        private async Task ToggleGhostPiece()
        {
            _settings.ShowGhostPiece = !_settings.ShowGhostPiece;
            
            // Show immediate feedback
            Console.Clear();
            Console.ForegroundColor = ValueColor;
            Console.WriteLine($"Ghost Piece: {(_settings.ShowGhostPiece ? "ENABLED" : "DISABLED")}");
            
            if (_settings.ShowGhostPiece)
            {
                Console.WriteLine("The ghost piece will show where your current piece will land.");
            }
            else
            {
                Console.WriteLine("The ghost piece preview has been disabled.");
            }
            
            await Task.Delay(1000);
        }

        /// <summary>
        /// Toggles the game tips setting.
        /// </summary>
        /// <returns>A task that completes when the setting is toggled.</returns>
        private async Task ToggleGameTips()
        {
            // Placeholder implementation - this would toggle a game tips setting
            Console.Clear();
            Console.ForegroundColor = ValueColor;
            Console.WriteLine("Game Tips: ENABLED");
            Console.WriteLine("Helpful tips will be shown during gameplay.");
            Console.WriteLine("(This feature is not yet implemented)");
            await Task.Delay(1500);
        }

        /// <summary>
        /// Toggles the auto-pause setting.
        /// </summary>
        /// <returns>A task that completes when the setting is toggled.</returns>
        private async Task ToggleAutoPause()
        {
            // Placeholder implementation - this would toggle auto-pause on focus loss
            Console.Clear();
            Console.ForegroundColor = ValueColor;
            Console.WriteLine("Auto-Pause: ENABLED");
            Console.WriteLine("The game will automatically pause when the window loses focus.");
            Console.WriteLine("(This feature is not yet implemented)");
            await Task.Delay(1500);
        }

        /// <summary>
        /// Toggles the confirm exit setting.
        /// </summary>
        /// <returns>A task that completes when the setting is toggled.</returns>
        private async Task ToggleConfirmExit()
        {
            // Placeholder implementation - this would toggle exit confirmation
            Console.Clear();
            Console.ForegroundColor = ValueColor;
            Console.WriteLine("Confirm Exit: ENABLED");
            Console.WriteLine("A confirmation dialog will appear when trying to exit a game.");
            Console.WriteLine("(This feature is not yet implemented)");
            await Task.Delay(1500);
        }

        /// <summary>
        /// Resets the gameplay settings to their default values.
        /// </summary>
        /// <returns>A task that completes when the settings are reset.</returns>
        private async Task ResetGameplayToDefaults()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Reset gameplay settings to defaults?");
            Console.WriteLine("This will restore:");
            Console.WriteLine("• Ghost Piece: ON");
            Console.WriteLine("• Game Tips: ON");
            Console.WriteLine("• Auto-Pause: ON");
            Console.WriteLine("• Confirm Exit: ON");
            Console.WriteLine();
            Console.WriteLine("Press Y to confirm, any other key to cancel...");

            ConsoleKeyInfo key = Console.ReadKey(true);
            if (key.Key == ConsoleKey.Y)
            {
                _settings.ShowGhostPiece = true;
                // Reset other gameplay settings when they are implemented
                
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Gameplay settings reset to defaults.");
                await Task.Delay(1500);
            }
        }

        /// <summary>
        /// Shows detailed information about a specific gameplay feature.
        /// </summary>
        /// <param name="featureName">The name of the feature to explain.</param>
        /// <param name="description">The detailed description of the feature.</param>
        /// <returns>A task that completes when the info is dismissed.</returns>
        private async Task ShowFeatureInfo(string featureName, string description)
        {
            Console.Clear();
            Console.ForegroundColor = TitleColor;
            Console.WriteLine($"=== {featureName.ToUpper()} ===");
            Console.WriteLine();
            
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(description);
            Console.WriteLine();
            
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("Press any key to return...");
            Console.ReadKey(true);
        }

        /// <summary>
        /// Returns to the previous menu.
        /// </summary>
        /// <returns>A task that completes when returning to the previous menu.</returns>
        private async Task ReturnToPrevious()
        {
            _isActive = false;
            await Task.CompletedTask;
        }

        #endregion

        #region Nested Classes

        /// <summary>
        /// Represents a menu item in the gameplay settings dialog.
        /// </summary>
        private class GameplayMenuItem
        {
            /// <summary>
            /// Gets the text to display for this menu item.
            /// </summary>
            public string Text { get; }

            /// <summary>
            /// Gets the action to execute when this menu item is selected.
            /// </summary>
            public Func<Task> Action { get; }

            /// <summary>
            /// Initializes a new instance of the GameplayMenuItem class.
            /// </summary>
            /// <param name="text">The text to display for this menu item.</param>
            /// <param name="action">The action to execute when this menu item is selected.</param>
            public GameplayMenuItem(string text, Func<Task> action)
            {
                Text = text ?? throw new ArgumentNullException(nameof(text));
                Action = action ?? throw new ArgumentNullException(nameof(action));
            }
        }

        #endregion
    }
}
