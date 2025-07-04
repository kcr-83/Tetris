using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tetris.Core.Models;

namespace Tetris.Core.UI
{
    /// <summary>
    /// Dialog for configuring control settings including control schemes and key mappings.
    /// </summary>
    public class ControlSettingsDialog
    {
        #region Constants

        private const string Title = "CONTROL SETTINGS";
        private const string SubTitle = "Configure game controls and key mappings";

        #endregion

        #region Fields

        private readonly UserSettings _settings;
        private readonly List<ControlMenuItem> _menuItems;
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
        /// Initializes a new instance of the ControlSettingsDialog class.
        /// </summary>
        /// <param name="settings">The user settings to modify.</param>
        public ControlSettingsDialog(UserSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _selectedIndex = 0;
            _isActive = false;

            _menuItems = new List<ControlMenuItem>
            {
                new("Control Scheme", () => ChangeControlScheme()),
                new("Customize Key Mappings", () => CustomizeKeyMappings()),
                new("Key Repeat", () => ToggleKeyRepeat()),
                new("Key Repeat Delay", () => AdjustKeyRepeatDelay()),
                new("Reset to Defaults", () => ResetControlsToDefaults()),
                new("Return", () => ReturnToPrevious())
            };
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Shows the control settings dialog and handles user interaction.
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
        /// Renders the control settings dialog.
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
            }

            // Draw footer
            Console.ForegroundColor = ConsoleColor.DarkGray;
            string footerText = "Use ↑↓ to navigate, ENTER to select/change, ESC to return";
            int footerX = Math.Max(0, (windowWidth - footerText.Length) / 2);
            Console.SetCursorPosition(footerX, windowHeight - 3);
            Console.WriteLine(footerText);

            // Draw current key mappings
            DrawKeyMappings(windowWidth, windowHeight);
        }

        /// <summary>
        /// Handles user input for the control settings dialog.
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
                0 => _settings.ControlSettings.ControlScheme.ToString(),
                1 => "Press ENTER to customize",
                2 => _settings.ControlSettings.KeyRepeatEnabled ? "ON" : "OFF",
                3 => $"{_settings.ControlSettings.KeyRepeatDelay}ms",
                4 => "Press ENTER to reset",
                _ => string.Empty
            };
        }

        /// <summary>
        /// Draws the current key mappings.
        /// </summary>
        /// <param name="windowWidth">The width of the console window.</param>
        /// <param name="windowHeight">The height of the console window.</param>
        private void DrawKeyMappings(int windowWidth, int windowHeight)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;

            int mappingsY = windowHeight - 12;
            string mappingsTitle = "Current Key Mappings:";
            int titleX = Math.Max(0, (windowWidth - mappingsTitle.Length) / 2);
            Console.SetCursorPosition(titleX, mappingsY);
            Console.WriteLine(mappingsTitle);

            var keyMappings = _settings.ControlSettings.KeyMappings;
            string[] mappingLines = new[]
            {
                $"Move Left: {keyMappings.GetValueOrDefault(GameAction.MoveLeft, ConsoleKey.LeftArrow)}",
                $"Move Right: {keyMappings.GetValueOrDefault(GameAction.MoveRight, ConsoleKey.RightArrow)}",
                $"Soft Drop: {keyMappings.GetValueOrDefault(GameAction.SoftDrop, ConsoleKey.DownArrow)}",
                $"Hard Drop: {keyMappings.GetValueOrDefault(GameAction.HardDrop, ConsoleKey.Spacebar)}",
                $"Rotate: {keyMappings.GetValueOrDefault(GameAction.RotateClockwise, ConsoleKey.UpArrow)}",
                $"Hold: {keyMappings.GetValueOrDefault(GameAction.Hold, ConsoleKey.C)}",
                $"Pause: {keyMappings.GetValueOrDefault(GameAction.Pause, ConsoleKey.P)}",
                $"Menu: {keyMappings.GetValueOrDefault(GameAction.Menu, ConsoleKey.Escape)}"
            };

            for (int i = 0; i < mappingLines.Length; i++)
            {
                string line = mappingLines[i];
                int lineX = Math.Max(0, (windowWidth - line.Length) / 2);
                Console.SetCursorPosition(lineX, mappingsY + 1 + i);
                Console.WriteLine(line);
            }
        }

        /// <summary>
        /// Changes the control scheme.
        /// </summary>
        /// <returns>A task that completes when the control scheme is changed.</returns>
        private async Task ChangeControlScheme()
        {
            Console.Clear();
            Console.ForegroundColor = TitleColor;
            Console.WriteLine("=== SELECT CONTROL SCHEME ===");
            Console.WriteLine();

            var schemes = Enum.GetValues<ControlScheme>();
            int selectedScheme = Array.IndexOf(schemes, _settings.ControlSettings.ControlScheme);

            bool selecting = true;
            while (selecting)
            {
                Console.SetCursorPosition(0, 2);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Choose a control scheme:");
                Console.WriteLine();

                for (int i = 0; i < schemes.Length; i++)
                {
                    Console.ForegroundColor = i == selectedScheme ? SelectedItemColor : NormalItemColor;
                    string indicator = i == selectedScheme ? "> " : "  ";
                    string description = GetControlSchemeDescription(schemes[i]);
                    Console.WriteLine($"{indicator}{schemes[i]} - {description}");
                }

                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine();
                Console.WriteLine("Use ↑↓ to select, ENTER to confirm, ESC to cancel");

                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo key = Console.ReadKey(true);
                    switch (key.Key)
                    {
                        case ConsoleKey.UpArrow:
                            selectedScheme = Math.Max(0, selectedScheme - 1);
                            break;
                        case ConsoleKey.DownArrow:
                            selectedScheme = Math.Min(schemes.Length - 1, selectedScheme + 1);
                            break;
                        case ConsoleKey.Enter:
                            _settings.ControlSettings.ControlScheme = schemes[selectedScheme];
                            ApplyControlScheme(schemes[selectedScheme]);
                            selecting = false;
                            break;
                        case ConsoleKey.Escape:
                            selecting = false;
                            break;
                    }
                }

                await Task.Delay(50);
            }
        }

        /// <summary>
        /// Gets a description for a control scheme.
        /// </summary>
        /// <param name="scheme">The control scheme.</param>
        /// <returns>A description of the control scheme.</returns>
        private string GetControlSchemeDescription(ControlScheme scheme)
        {
            return scheme switch
            {
                ControlScheme.Standard => "Arrow keys for movement and rotation",
                ControlScheme.WASD => "WASD keys for movement and rotation",
                ControlScheme.Custom => "User-defined key mappings",
                _ => "Unknown control scheme"
            };
        }

        /// <summary>
        /// Applies the selected control scheme by updating key mappings.
        /// </summary>
        /// <param name="scheme">The control scheme to apply.</param>
        private void ApplyControlScheme(ControlScheme scheme)
        {
            switch (scheme)
            {
                case ControlScheme.Standard:
                    _settings.ControlSettings.KeyMappings = ControlSettings.GetDefaultKeyMappings();
                    break;
                case ControlScheme.WASD:
                    _settings.ControlSettings.KeyMappings = ControlSettings.GetWASDKeyMappings();
                    break;
                case ControlScheme.Custom:
                    // Keep existing mappings for custom scheme
                    break;
            }
        }

        /// <summary>
        /// Opens the key mapping customization interface.
        /// </summary>
        /// <returns>A task that completes when customization is finished.</returns>
        private async Task CustomizeKeyMappings()
        {
            var keyMappingDialog = new KeyMappingCustomizationDialog(_settings.ControlSettings);
            await keyMappingDialog.ShowAsync();
            _settings.ControlSettings.ControlScheme = ControlScheme.Custom;
        }

        /// <summary>
        /// Toggles the key repeat setting.
        /// </summary>
        /// <returns>A task that completes when the setting is toggled.</returns>
        private async Task ToggleKeyRepeat()
        {
            _settings.ControlSettings.KeyRepeatEnabled = !_settings.ControlSettings.KeyRepeatEnabled;
            await Task.CompletedTask;
        }

        /// <summary>
        /// Adjusts the key repeat delay setting.
        /// </summary>
        /// <returns>A task that completes when the delay is adjusted.</returns>
        private async Task AdjustKeyRepeatDelay()
        {
            Console.Clear();
            Console.ForegroundColor = TitleColor;
            Console.WriteLine("=== ADJUST KEY REPEAT DELAY ===");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"Current delay: {_settings.ControlSettings.KeyRepeatDelay}ms");
            Console.WriteLine();
            Console.WriteLine("Use LEFT/RIGHT arrows to adjust (50-1000ms), ENTER to confirm:");

            bool adjusting = true;
            while (adjusting)
            {
                Console.SetCursorPosition(0, 3);
                Console.ForegroundColor = ValueColor;
                Console.WriteLine($"Key Repeat Delay: {_settings.ControlSettings.KeyRepeatDelay}ms");
                Console.WriteLine($"Rate: {1000.0 / _settings.ControlSettings.KeyRepeatDelay:F1} repeats/second");

                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo key = Console.ReadKey(true);
                    switch (key.Key)
                    {
                        case ConsoleKey.LeftArrow:
                            _settings.ControlSettings.KeyRepeatDelay = Math.Max(50, _settings.ControlSettings.KeyRepeatDelay - 25);
                            break;
                        case ConsoleKey.RightArrow:
                            _settings.ControlSettings.KeyRepeatDelay = Math.Min(1000, _settings.ControlSettings.KeyRepeatDelay + 25);
                            break;
                        case ConsoleKey.Enter:
                        case ConsoleKey.Escape:
                            adjusting = false;
                            break;
                    }
                }

                await Task.Delay(50);
            }
        }

        /// <summary>
        /// Resets the control settings to their default values.
        /// </summary>
        /// <returns>A task that completes when the settings are reset.</returns>
        private async Task ResetControlsToDefaults()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Reset control settings to defaults?");
            Console.WriteLine("This will restore the standard arrow key scheme.");
            Console.WriteLine();
            Console.WriteLine("Press Y to confirm, any other key to cancel...");

            ConsoleKeyInfo key = Console.ReadKey(true);
            if (key.Key == ConsoleKey.Y)
            {
                _settings.ControlSettings = new ControlSettings();
                
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Control settings reset to defaults.");
                await Task.Delay(1500);
            }
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
        /// Represents a menu item in the control settings dialog.
        /// </summary>
        private class ControlMenuItem
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
            /// Initializes a new instance of the ControlMenuItem class.
            /// </summary>
            /// <param name="text">The text to display for this menu item.</param>
            /// <param name="action">The action to execute when this menu item is selected.</param>
            public ControlMenuItem(string text, Func<Task> action)
            {
                Text = text ?? throw new ArgumentNullException(nameof(text));
                Action = action ?? throw new ArgumentNullException(nameof(action));
            }
        }

        #endregion
    }

    /// <summary>
    /// Dialog for customizing individual key mappings.
    /// </summary>
    public class KeyMappingCustomizationDialog
    {
        #region Fields

        private readonly ControlSettings _controlSettings;
        private readonly List<GameAction> _gameActions;
        private int _selectedIndex;
        private bool _isActive;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the KeyMappingCustomizationDialog class.
        /// </summary>
        /// <param name="controlSettings">The control settings to modify.</param>
        public KeyMappingCustomizationDialog(ControlSettings controlSettings)
        {
            _controlSettings = controlSettings ?? throw new ArgumentNullException(nameof(controlSettings));
            _gameActions = Enum.GetValues<GameAction>().ToList();
            _selectedIndex = 0;
            _isActive = false;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Shows the key mapping customization dialog.
        /// </summary>
        /// <returns>A task that completes when the dialog is closed.</returns>
        public async Task ShowAsync()
        {
            Console.CursorVisible = false;
            _isActive = true;

            while (_isActive)
            {
                RenderDialog();
                await HandleInputAsync();
                await Task.Delay(50);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Renders the key mapping customization dialog.
        /// </summary>
        private void RenderDialog()
        {
            Console.Clear();
            Console.BackgroundColor = ConsoleColor.Black;

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("=== CUSTOMIZE KEY MAPPINGS ===");
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Select an action to remap:");
            Console.WriteLine();

            for (int i = 0; i < _gameActions.Count; i++)
            {
                GameAction action = _gameActions[i];
                ConsoleKey currentKey = _controlSettings.KeyMappings.GetValueOrDefault(action, ConsoleKey.Escape);
                
                Console.ForegroundColor = i == _selectedIndex ? ConsoleColor.Yellow : ConsoleColor.White;
                string indicator = i == _selectedIndex ? "> " : "  ";
                Console.WriteLine($"{indicator}{action,-20} : {currentKey}");
            }

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine();
            Console.WriteLine("Use ↑↓ to select, ENTER to remap, ESC to return");
        }

        /// <summary>
        /// Handles user input for the key mapping dialog.
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
                    _selectedIndex = Math.Min(_gameActions.Count - 1, _selectedIndex + 1);
                    break;

                case ConsoleKey.Enter:
                    await RemapSelectedAction();
                    break;

                case ConsoleKey.Escape:
                    _isActive = false;
                    break;
            }
        }

        /// <summary>
        /// Prompts the user to remap the selected action.
        /// </summary>
        /// <returns>A task that completes when the remapping is finished.</returns>
        private async Task RemapSelectedAction()
        {
            GameAction action = _gameActions[_selectedIndex];
            
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"=== REMAP {action.ToString().ToUpper()} ===");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"Press the new key for {action}...");
            Console.WriteLine("(Press ESC to cancel)");

            ConsoleKeyInfo newKey = Console.ReadKey(true);
            
            if (newKey.Key != ConsoleKey.Escape)
            {
                // Check if this key is already mapped to another action
                var existingMapping = _controlSettings.KeyMappings.FirstOrDefault(kvp => kvp.Value == newKey.Key && kvp.Key != action);
                
                if (existingMapping.Key != default)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Key {newKey.Key} is already mapped to {existingMapping.Key}.");
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey(true);
                }
                else
                {
                    _controlSettings.KeyMappings[action] = newKey.Key;
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"{action} remapped to {newKey.Key}");
                    await Task.Delay(1000);
                }
            }
        }

        #endregion
    }
}
