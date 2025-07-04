using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tetris.Core.Models;
using Tetris.Core.Services;

namespace Tetris.Core.UI
{
    /// <summary>
    /// User interface for managing game settings including controls, audio, and visual preferences.
    /// Provides an interactive menu for users to customize their gameplay experience.
    /// </summary>
    public class SettingsInterface
    {
        #region Constants

        private const string Title = "GAME SETTINGS";
        private const string SubTitle = "Customize your Tetris experience";

        #endregion

        #region Fields

        private readonly IUserSettingsService _settingsService;
        private readonly List<SettingsMenuItem> _menuItems;
        private int _selectedIndex;
        private bool _isActive;
        private UserSettings _workingSettings;

        #endregion

        #region Events

        /// <summary>
        /// Event raised when the user requests to return to the main menu.
        /// </summary>
        public event EventHandler? ReturnToMenuRequested;

        /// <summary>
        /// Event raised when settings are successfully saved.
        /// </summary>
        public event EventHandler<SettingsUpdatedEventArgs>? SettingsSaved;

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

        /// <summary>
        /// Gets or sets the color used for the background.
        /// </summary>
        public ConsoleColor BackgroundColor { get; set; } = ConsoleColor.Black;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the SettingsInterface class.
        /// </summary>
        /// <param name="settingsService">The settings service to use for managing settings.</param>
        public SettingsInterface(IUserSettingsService settingsService)
        {
            _settingsService = settingsService ?? throw new ArgumentNullException(nameof(settingsService));
            _workingSettings = _settingsService.CurrentSettings.Clone();
            _selectedIndex = 0;
            _isActive = false;

            _menuItems = new List<SettingsMenuItem>
            {
                new("Audio Settings", ShowAudioSettings),
                new("Control Settings", ShowControlSettings),
                new("Visual Settings", ShowVisualSettings),
                new("Gameplay Settings", ShowGameplaySettings),
                new("Reset to Defaults", ResetToDefaults),
                new("Save Settings", SaveSettings),
                new("Cancel", CancelSettings)
            };
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Shows the settings interface and handles user interaction.
        /// </summary>
        /// <returns>A task that completes when the user exits the settings.</returns>
        public async Task ShowAsync()
        {
            await InitializeAsync();
            
            _isActive = true;
            
            while (_isActive)
            {
                RenderInterface();
                await HandleInputAsync();
                await Task.Delay(50); // Prevent high CPU usage
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Initializes the settings interface.
        /// </summary>
        /// <returns>A task that completes when initialization is finished.</returns>
        private async Task InitializeAsync()
        {
            Console.CursorVisible = false;
            Console.BackgroundColor = BackgroundColor;
            
            // Load current settings
            _workingSettings = (await _settingsService.LoadSettingsAsync()).Clone();
        }

        /// <summary>
        /// Renders the settings interface.
        /// </summary>
        private void RenderInterface()
        {
            Console.Clear();
            Console.BackgroundColor = BackgroundColor;

            // Calculate positions for centered display
            int windowWidth = Console.WindowWidth;
            int windowHeight = Console.WindowHeight;
            
            int titleX = Math.Max(0, (windowWidth - Title.Length) / 2);
            int titleY = Math.Max(0, windowHeight / 6);

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
            int maxItemWidth = _menuItems.Max(item => item.Text.Length) + 20;
            int menuX = Math.Max(0, (windowWidth - maxItemWidth) / 2);

            for (int i = 0; i < _menuItems.Count; i++)
            {
                Console.ForegroundColor = i == _selectedIndex ? SelectedItemColor : NormalItemColor;
                Console.SetCursorPosition(menuX, menuStartY + i);

                string selectionIndicator = i == _selectedIndex ? "> " : "  ";
                string itemText = $"{selectionIndicator}{_menuItems[i].Text}";
                
                // Add value display for certain settings
                string valueDisplay = GetValueDisplayForMenuItem(i);
                if (!string.IsNullOrEmpty(valueDisplay))
                {
                    itemText += $" - {valueDisplay}";
                }
                
                Console.WriteLine(itemText);
            }

            // Draw footer
            Console.ForegroundColor = ConsoleColor.DarkGray;
            string footerText = "Use ↑↓ to navigate, ENTER to select, ESC to cancel";
            int footerX = Math.Max(0, (windowWidth - footerText.Length) / 2);
            Console.SetCursorPosition(footerX, windowHeight - 3);
            Console.WriteLine(footerText);

            // Draw current settings summary
            DrawSettingsSummary(windowWidth, windowHeight);
        }

        /// <summary>
        /// Handles user input for the settings interface.
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
                    await ExecuteSelectedMenuItemAsync();
                    break;

                case ConsoleKey.Escape:
                    await CancelSettings();
                    break;
            }
        }

        /// <summary>
        /// Executes the currently selected menu item.
        /// </summary>
        /// <returns>A task that completes when the menu item is executed.</returns>
        private async Task ExecuteSelectedMenuItemAsync()
        {
            if (_selectedIndex >= 0 && _selectedIndex < _menuItems.Count)
            {
                await _menuItems[_selectedIndex].Action();
            }
        }

        /// <summary>
        /// Gets the value display text for a menu item.
        /// </summary>
        /// <param name="itemIndex">The index of the menu item.</param>
        /// <returns>The value display text, or empty string if no value to display.</returns>
        private string GetValueDisplayForMenuItem(int itemIndex)
        {
            return itemIndex switch
            {
                0 => $"Sound: {(_workingSettings.SoundEffectsEnabled ? "ON" : "OFF")}, Music: {(_workingSettings.MusicEnabled ? "ON" : "OFF")}",
                1 => $"Scheme: {_workingSettings.ControlSettings.ControlScheme}",
                2 => $"Theme: {_workingSettings.ColorTheme}",
                3 => $"Ghost: {(_workingSettings.ShowGhostPiece ? "ON" : "OFF")}",
                _ => string.Empty
            };
        }

        /// <summary>
        /// Draws a summary of current settings.
        /// </summary>
        /// <param name="windowWidth">The width of the console window.</param>
        /// <param name="windowHeight">The height of the console window.</param>
        private void DrawSettingsSummary(int windowWidth, int windowHeight)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            
            int summaryY = windowHeight - 8;
            string[] summaryLines = new[]
            {
                "Current Settings:",
                $"Audio: Sound {(_workingSettings.SoundEffectsEnabled ? "ON" : "OFF")}, Music {(_workingSettings.MusicEnabled ? "ON" : "OFF")}, Volume {_workingSettings.MasterVolume:P0}",
                $"Controls: {_workingSettings.ControlSettings.ControlScheme} scheme, Repeat {(_workingSettings.ControlSettings.KeyRepeatEnabled ? "ON" : "OFF")}",
                $"Visual: {_workingSettings.ColorTheme} theme, Ghost piece {(_workingSettings.ShowGhostPiece ? "ON" : "OFF")}",
                $"Animations: {_workingSettings.AnimationMode}"
            };

            for (int i = 0; i < summaryLines.Length; i++)
            {
                string line = summaryLines[i];
                int lineX = Math.Max(0, (windowWidth - line.Length) / 2);
                Console.SetCursorPosition(lineX, summaryY + i);
                Console.WriteLine(line);
            }
        }

        /// <summary>
        /// Shows the audio settings submenu.
        /// </summary>
        /// <returns>A task that completes when the submenu is closed.</returns>
        private async Task ShowAudioSettings()
        {
            var audioDialog = new AudioSettingsDialog(_workingSettings);
            await audioDialog.ShowAsync();
        }

        /// <summary>
        /// Shows the control settings submenu.
        /// </summary>
        /// <returns>A task that completes when the submenu is closed.</returns>
        private async Task ShowControlSettings()
        {
            var controlDialog = new ControlSettingsDialog(_workingSettings);
            await controlDialog.ShowAsync();
        }

        /// <summary>
        /// Shows the visual settings submenu.
        /// </summary>
        /// <returns>A task that completes when the submenu is closed.</returns>
        private async Task ShowVisualSettings()
        {
            var visualDialog = new VisualSettingsDialog(_workingSettings);
            await visualDialog.ShowAsync();
        }

        /// <summary>
        /// Shows the gameplay settings submenu.
        /// </summary>
        /// <returns>A task that completes when the submenu is closed.</returns>
        private async Task ShowGameplaySettings()
        {
            var gameplayDialog = new GameplaySettingsDialog(_workingSettings);
            await gameplayDialog.ShowAsync();
        }

        /// <summary>
        /// Resets all settings to their default values.
        /// </summary>
        /// <returns>A task that completes when settings are reset.</returns>
        private async Task ResetToDefaults()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Reset all settings to defaults?");
            Console.WriteLine("This will lose all your customizations.");
            Console.WriteLine();
            Console.WriteLine("Press Y to confirm, any other key to cancel...");

            ConsoleKeyInfo key = Console.ReadKey(true);
            if (key.Key == ConsoleKey.Y)
            {
                _workingSettings = UserSettings.GetDefaultSettings();
                
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Settings reset to defaults.");
                await Task.Delay(1500);
            }
        }

        /// <summary>
        /// Saves the current settings.
        /// </summary>
        /// <returns>A task that completes when settings are saved.</returns>
        private async Task SaveSettings()
        {
            try
            {
                await _settingsService.SaveSettingsAsync(_workingSettings);
                
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Settings saved successfully!");
                await Task.Delay(1500);
                
                SettingsSaved?.Invoke(this, new SettingsUpdatedEventArgs(_workingSettings, SettingsChangeType.Saved));
                _isActive = false;
                ReturnToMenuRequested?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error saving settings: {ex.Message}");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey(true);
            }
        }

        /// <summary>
        /// Cancels settings changes and returns to the main menu.
        /// </summary>
        /// <returns>A task that completes when the cancel operation is finished.</returns>
        private async Task CancelSettings()
        {
            _isActive = false;
            ReturnToMenuRequested?.Invoke(this, EventArgs.Empty);
            await Task.CompletedTask;
        }

        #endregion

        #region Nested Classes

        /// <summary>
        /// Represents a menu item in the settings interface.
        /// </summary>
        private class SettingsMenuItem
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
            /// Initializes a new instance of the SettingsMenuItem class.
            /// </summary>
            /// <param name="text">The text to display for this menu item.</param>
            /// <param name="action">The action to execute when this menu item is selected.</param>
            public SettingsMenuItem(string text, Func<Task> action)
            {
                Text = text ?? throw new ArgumentNullException(nameof(text));
                Action = action ?? throw new ArgumentNullException(nameof(action));
            }
        }

        #endregion
    }
}
