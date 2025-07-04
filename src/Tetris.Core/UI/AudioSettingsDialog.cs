using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tetris.Core.Models;

namespace Tetris.Core.UI
{
    /// <summary>
    /// Dialog for configuring audio settings including sound effects, music, and volume levels.
    /// </summary>
    public class AudioSettingsDialog
    {
        #region Constants

        private const string Title = "AUDIO SETTINGS";
        private const string SubTitle = "Configure sound and music preferences";

        #endregion

        #region Fields

        private readonly UserSettings _settings;
        private readonly List<AudioMenuItem> _menuItems;
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
        /// Initializes a new instance of the AudioSettingsDialog class.
        /// </summary>
        /// <param name="settings">The user settings to modify.</param>
        public AudioSettingsDialog(UserSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _selectedIndex = 0;
            _isActive = false;

            _menuItems = new List<AudioMenuItem>
            {
                new("Sound Effects", () => ToggleSoundEffects()),
                new("Background Music", () => ToggleMusic()),
                new("Master Volume", () => AdjustMasterVolume()),
                new("Return", () => ReturnToPrevious())
            };
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Shows the audio settings dialog and handles user interaction.
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
        /// Renders the audio settings dialog.
        /// </summary>
        private void RenderDialog()
        {
            Console.Clear();
            Console.BackgroundColor = ConsoleColor.Black;

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
            int maxItemWidth = Math.Max(40, _menuItems.Max(item => item.Text.Length) + 30);
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
                    itemText = $"{itemText,-25} : {valueDisplay}";
                }

                Console.WriteLine(itemText);
            }

            // Draw footer
            Console.ForegroundColor = ConsoleColor.DarkGray;
            string footerText = "Use ↑↓ to navigate, ENTER/SPACE to change, ESC to return";
            int footerX = Math.Max(0, (windowWidth - footerText.Length) / 2);
            Console.SetCursorPosition(footerX, windowHeight - 3);
            Console.WriteLine(footerText);

            // Draw current settings info
            DrawSettingsInfo(windowWidth, windowHeight);
        }

        /// <summary>
        /// Handles user input for the audio settings dialog.
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
                0 => _settings.SoundEffectsEnabled ? "ON" : "OFF",
                1 => _settings.MusicEnabled ? "ON" : "OFF",
                2 => $"{_settings.MasterVolume:P0}",
                _ => string.Empty
            };
        }

        /// <summary>
        /// Draws additional settings information.
        /// </summary>
        /// <param name="windowWidth">The width of the console window.</param>
        /// <param name="windowHeight">The height of the console window.</param>
        private void DrawSettingsInfo(int windowWidth, int windowHeight)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;

            int infoY = windowHeight - 8;
            string[] infoLines = new[]
            {
                "Audio Settings:",
                $"Sound Effects: {(_settings.SoundEffectsEnabled ? "Enabled" : "Disabled")}",
                $"Background Music: {(_settings.MusicEnabled ? "Enabled" : "Disabled")}",
                $"Master Volume: {_settings.MasterVolume:P0}"
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
        /// Toggles the sound effects setting.
        /// </summary>
        /// <returns>A task that completes when the setting is toggled.</returns>
        private async Task ToggleSoundEffects()
        {
            _settings.SoundEffectsEnabled = !_settings.SoundEffectsEnabled;
            await Task.CompletedTask;
        }

        /// <summary>
        /// Toggles the background music setting.
        /// </summary>
        /// <returns>A task that completes when the setting is toggled.</returns>
        private async Task ToggleMusic()
        {
            _settings.MusicEnabled = !_settings.MusicEnabled;
            await Task.CompletedTask;
        }

        /// <summary>
        /// Adjusts the master volume setting.
        /// </summary>
        /// <returns>A task that completes when the volume is adjusted.</returns>
        private async Task AdjustMasterVolume()
        {
            Console.Clear();
            Console.ForegroundColor = TitleColor;
            Console.WriteLine("=== ADJUST MASTER VOLUME ===");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"Current volume: {_settings.MasterVolume:P0}");
            Console.WriteLine();
            Console.WriteLine("Use LEFT/RIGHT arrows to adjust, ENTER to confirm:");

            bool adjusting = true;
            while (adjusting)
            {
                Console.SetCursorPosition(0, 3);
                Console.ForegroundColor = ValueColor;
                string volumeBar = GetVolumeBar(_settings.MasterVolume);
                Console.WriteLine($"Volume: {_settings.MasterVolume:P0} {volumeBar}");

                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo key = Console.ReadKey(true);
                    switch (key.Key)
                    {
                        case ConsoleKey.LeftArrow:
                            _settings.MasterVolume = Math.Max(0.0f, _settings.MasterVolume - 0.1f);
                            break;
                        case ConsoleKey.RightArrow:
                            _settings.MasterVolume = Math.Min(1.0f, _settings.MasterVolume + 0.1f);
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
        /// Gets a visual representation of the volume level.
        /// </summary>
        /// <param name="volume">The volume level (0.0 to 1.0).</param>
        /// <returns>A string representing the volume bar.</returns>
        private string GetVolumeBar(float volume)
        {
            int barLength = 20;
            int filledLength = (int)(volume * barLength);
            string filled = new string('█', filledLength);
            string empty = new string('░', barLength - filledLength);
            return $"[{filled}{empty}]";
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
        /// Represents a menu item in the audio settings dialog.
        /// </summary>
        private class AudioMenuItem
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
            /// Initializes a new instance of the AudioMenuItem class.
            /// </summary>
            /// <param name="text">The text to display for this menu item.</param>
            /// <param name="action">The action to execute when this menu item is selected.</param>
            public AudioMenuItem(string text, Func<Task> action)
            {
                Text = text ?? throw new ArgumentNullException(nameof(text));
                Action = action ?? throw new ArgumentNullException(nameof(action));
            }
        }

        #endregion
    }
}
