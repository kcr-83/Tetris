using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tetris.Core.Models;

namespace Tetris.Core.UI
{
    /// <summary>
    /// Dialog for configuring visual settings including color themes and animation preferences.
    /// </summary>
    public class VisualSettingsDialog
    {
        #region Constants

        private const string Title = "VISUAL SETTINGS";
        private const string SubTitle = "Configure appearance and visual effects";

        #endregion

        #region Fields

        private readonly UserSettings _settings;
        private readonly List<VisualMenuItem> _menuItems;
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
        /// Initializes a new instance of the VisualSettingsDialog class.
        /// </summary>
        /// <param name="settings">The user settings to modify.</param>
        public VisualSettingsDialog(UserSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _selectedIndex = 0;
            _isActive = false;

            _menuItems = new List<VisualMenuItem>
            {
                new("Color Theme", () => ChangeColorTheme()),
                new("Animation Mode", () => ChangeAnimationMode()),
                new("Preview Theme", () => PreviewCurrentTheme()),
                new("Reset to Defaults", () => ResetVisualsToDefaults()),
                new("Return", () => ReturnToPrevious())
            };
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Shows the visual settings dialog and handles user interaction.
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
        /// Renders the visual settings dialog.
        /// </summary>
        private void RenderDialog()
        {
            Console.Clear();
            ApplyCurrentTheme();

            // Calculate positions for centered display
            int windowWidth = Console.WindowWidth;
            int windowHeight = Console.WindowHeight;

            int titleX = Math.Max(0, (windowWidth - Title.Length) / 2);
            int titleY = Math.Max(0, windowHeight / 8);

            // Draw title
            Console.ForegroundColor = GetThemedColor(ThemeElement.Title);
            Console.SetCursorPosition(titleX, titleY);
            Console.WriteLine(Title);

            // Draw subtitle
            Console.ForegroundColor = GetThemedColor(ThemeElement.Subtitle);
            int subtitleX = Math.Max(0, (windowWidth - SubTitle.Length) / 2);
            Console.SetCursorPosition(subtitleX, titleY + 1);
            Console.WriteLine(SubTitle);

            // Draw menu items
            int menuStartY = titleY + 4;
            int maxItemWidth = Math.Max(40, _menuItems.Max(item => item.Text.Length) + 30);
            int menuX = Math.Max(0, (windowWidth - maxItemWidth) / 2);

            for (int i = 0; i < _menuItems.Count; i++)
            {
                Console.ForegroundColor = i == _selectedIndex 
                    ? GetThemedColor(ThemeElement.SelectedItem) 
                    : GetThemedColor(ThemeElement.NormalItem);
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
            Console.ForegroundColor = GetThemedColor(ThemeElement.Footer);
            string footerText = "Use ↑↓ to navigate, ENTER to select/change, ESC to return";
            int footerX = Math.Max(0, (windowWidth - footerText.Length) / 2);
            Console.SetCursorPosition(footerX, windowHeight - 3);
            Console.WriteLine(footerText);

            // Draw theme preview
            DrawThemePreview(windowWidth, windowHeight);
        }

        /// <summary>
        /// Applies the current color theme to the console.
        /// </summary>
        private void ApplyCurrentTheme()
        {
            Console.BackgroundColor = GetThemedBackgroundColor();
        }

        /// <summary>
        /// Gets a themed color for a specific UI element.
        /// </summary>
        /// <param name="element">The UI element to get the color for.</param>
        /// <returns>The appropriate console color for the element.</returns>
        private ConsoleColor GetThemedColor(ThemeElement element)
        {
            return _settings.ColorTheme switch
            {
                ColorTheme.Classic => element switch
                {
                    ThemeElement.Title => ConsoleColor.Cyan,
                    ThemeElement.Subtitle => ConsoleColor.Gray,
                    ThemeElement.SelectedItem => ConsoleColor.Yellow,
                    ThemeElement.NormalItem => ConsoleColor.White,
                    ThemeElement.Value => ConsoleColor.Green,
                    ThemeElement.Footer => ConsoleColor.DarkGray,
                    _ => ConsoleColor.White
                },
                ColorTheme.Dark => element switch
                {
                    ThemeElement.Title => ConsoleColor.DarkCyan,
                    ThemeElement.Subtitle => ConsoleColor.DarkGray,
                    ThemeElement.SelectedItem => ConsoleColor.DarkYellow,
                    ThemeElement.NormalItem => ConsoleColor.Gray,
                    ThemeElement.Value => ConsoleColor.DarkGreen,
                    ThemeElement.Footer => ConsoleColor.DarkGray,
                    _ => ConsoleColor.Gray
                },
                ColorTheme.HighContrast => element switch
                {
                    ThemeElement.Title => ConsoleColor.White,
                    ThemeElement.Subtitle => ConsoleColor.White,
                    ThemeElement.SelectedItem => ConsoleColor.Black,
                    ThemeElement.NormalItem => ConsoleColor.White,
                    ThemeElement.Value => ConsoleColor.White,
                    ThemeElement.Footer => ConsoleColor.White,
                    _ => ConsoleColor.White
                },
                ColorTheme.Neon => element switch
                {
                    ThemeElement.Title => ConsoleColor.Magenta,
                    ThemeElement.Subtitle => ConsoleColor.DarkMagenta,
                    ThemeElement.SelectedItem => ConsoleColor.Green,
                    ThemeElement.NormalItem => ConsoleColor.Cyan,
                    ThemeElement.Value => ConsoleColor.Yellow,
                    ThemeElement.Footer => ConsoleColor.DarkGray,
                    _ => ConsoleColor.Cyan
                },
                ColorTheme.Monochrome => element switch
                {
                    ThemeElement.Title => ConsoleColor.White,
                    ThemeElement.Subtitle => ConsoleColor.Gray,
                    ThemeElement.SelectedItem => ConsoleColor.White,
                    ThemeElement.NormalItem => ConsoleColor.Gray,
                    ThemeElement.Value => ConsoleColor.White,
                    ThemeElement.Footer => ConsoleColor.DarkGray,
                    _ => ConsoleColor.Gray
                },
                _ => ConsoleColor.White
            };
        }

        /// <summary>
        /// Gets the themed background color.
        /// </summary>
        /// <returns>The appropriate background color for the current theme.</returns>
        private ConsoleColor GetThemedBackgroundColor()
        {
            return _settings.ColorTheme switch
            {
                ColorTheme.Classic => ConsoleColor.Black,
                ColorTheme.Dark => ConsoleColor.Black,
                ColorTheme.HighContrast => ConsoleColor.Black,
                ColorTheme.Neon => ConsoleColor.Black,
                ColorTheme.Monochrome => ConsoleColor.Black,
                _ => ConsoleColor.Black
            };
        }

        /// <summary>
        /// Handles user input for the visual settings dialog.
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
                0 => _settings.ColorTheme.ToString(),
                1 => _settings.AnimationMode.ToString(),
                2 => "Press ENTER to preview",
                3 => "Press ENTER to reset",
                _ => string.Empty
            };
        }

        /// <summary>
        /// Draws a preview of the current theme.
        /// </summary>
        /// <param name="windowWidth">The width of the console window.</param>
        /// <param name="windowHeight">The height of the console window.</param>
        private void DrawThemePreview(int windowWidth, int windowHeight)
        {
            Console.ForegroundColor = GetThemedColor(ThemeElement.Footer);

            int previewY = windowHeight - 12;
            string previewTitle = $"Theme Preview: {_settings.ColorTheme}";
            int titleX = Math.Max(0, (windowWidth - previewTitle.Length) / 2);
            Console.SetCursorPosition(titleX, previewY);
            Console.WriteLine(previewTitle);

            // Draw sample Tetris pieces using theme colors
            int pieceY = previewY + 2;
            int pieceX = Math.Max(0, (windowWidth - 40) / 2);

            Console.SetCursorPosition(pieceX, pieceY);
            Console.ForegroundColor = GetPieceColor(0); // I piece
            Console.Write("████ ");
            Console.ForegroundColor = GetPieceColor(1); // J piece
            Console.Write("███ ");
            Console.ForegroundColor = GetPieceColor(2); // L piece
            Console.Write("███ ");
            Console.ForegroundColor = GetPieceColor(3); // O piece
            Console.Write("██");

            Console.SetCursorPosition(pieceX, pieceY + 1);
            Console.ForegroundColor = GetPieceColor(4); // S piece
            Console.Write(" ██ ");
            Console.ForegroundColor = GetPieceColor(5); // T piece
            Console.Write("███ ");
            Console.ForegroundColor = GetPieceColor(6); // Z piece
            Console.Write("██ ");
            Console.ForegroundColor = GetPieceColor(3); // O piece
            Console.Write(" ██");

            // Animation mode info
            Console.ForegroundColor = GetThemedColor(ThemeElement.Footer);
            string animationInfo = $"Animation Mode: {_settings.AnimationMode} - {GetAnimationDescription()}";
            int animationX = Math.Max(0, (windowWidth - animationInfo.Length) / 2);
            Console.SetCursorPosition(animationX, pieceY + 3);
            Console.WriteLine(animationInfo);
        }

        /// <summary>
        /// Gets the color for a specific tetromino piece based on the current theme.
        /// </summary>
        /// <param name="pieceId">The ID of the tetromino piece.</param>
        /// <returns>The appropriate color for the piece.</returns>
        private ConsoleColor GetPieceColor(int pieceId)
        {
            return _settings.ColorTheme switch
            {
                ColorTheme.Classic => pieceId switch
                {
                    0 => ConsoleColor.Cyan,     // I piece
                    1 => ConsoleColor.Blue,     // J piece
                    2 => ConsoleColor.DarkYellow, // L piece
                    3 => ConsoleColor.Yellow,   // O piece
                    4 => ConsoleColor.Green,    // S piece
                    5 => ConsoleColor.Magenta,  // T piece
                    6 => ConsoleColor.Red,      // Z piece
                    _ => ConsoleColor.White
                },
                ColorTheme.Dark => pieceId switch
                {
                    0 => ConsoleColor.DarkCyan,
                    1 => ConsoleColor.DarkBlue,
                    2 => ConsoleColor.DarkYellow,
                    3 => ConsoleColor.DarkYellow,
                    4 => ConsoleColor.DarkGreen,
                    5 => ConsoleColor.DarkMagenta,
                    6 => ConsoleColor.DarkRed,
                    _ => ConsoleColor.DarkGray
                },
                ColorTheme.HighContrast => ConsoleColor.White,
                ColorTheme.Neon => pieceId switch
                {
                    0 => ConsoleColor.Cyan,
                    1 => ConsoleColor.Blue,
                    2 => ConsoleColor.Yellow,
                    3 => ConsoleColor.Yellow,
                    4 => ConsoleColor.Green,
                    5 => ConsoleColor.Magenta,
                    6 => ConsoleColor.Red,
                    _ => ConsoleColor.White
                },
                ColorTheme.Monochrome => ConsoleColor.White,
                _ => ConsoleColor.White
            };
        }

        /// <summary>
        /// Gets a description of the current animation mode.
        /// </summary>
        /// <returns>A description of the animation mode.</returns>
        private string GetAnimationDescription()
        {
            return _settings.AnimationMode switch
            {
                AnimationMode.None => "No animations, best performance",
                AnimationMode.Minimal => "Essential animations only",
                AnimationMode.Normal => "Standard animation effects",
                AnimationMode.Enhanced => "Full effects and transitions",
                _ => "Unknown animation mode"
            };
        }

        /// <summary>
        /// Changes the color theme.
        /// </summary>
        /// <returns>A task that completes when the theme is changed.</returns>
        private async Task ChangeColorTheme()
        {
            Console.Clear();
            Console.ForegroundColor = TitleColor;
            Console.WriteLine("=== SELECT COLOR THEME ===");
            Console.WriteLine();

            var themes = Enum.GetValues<ColorTheme>();
            int selectedTheme = Array.IndexOf(themes, _settings.ColorTheme);

            bool selecting = true;
            while (selecting)
            {
                Console.SetCursorPosition(0, 2);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Choose a color theme:");
                Console.WriteLine();

                for (int i = 0; i < themes.Length; i++)
                {
                    Console.ForegroundColor = i == selectedTheme ? SelectedItemColor : NormalItemColor;
                    string indicator = i == selectedTheme ? "> " : "  ";
                    string description = GetThemeDescription(themes[i]);
                    Console.WriteLine($"{indicator}{themes[i],-15} - {description}");
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
                            selectedTheme = Math.Max(0, selectedTheme - 1);
                            break;
                        case ConsoleKey.DownArrow:
                            selectedTheme = Math.Min(themes.Length - 1, selectedTheme + 1);
                            break;
                        case ConsoleKey.Enter:
                            _settings.ColorTheme = themes[selectedTheme];
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
        /// Gets a description for a color theme.
        /// </summary>
        /// <param name="theme">The color theme.</param>
        /// <returns>A description of the theme.</returns>
        private string GetThemeDescription(ColorTheme theme)
        {
            return theme switch
            {
                ColorTheme.Classic => "Traditional Tetris colors",
                ColorTheme.Dark => "Darker, muted color palette",
                ColorTheme.HighContrast => "High contrast for accessibility",
                ColorTheme.Neon => "Bright, vibrant colors",
                ColorTheme.Monochrome => "Black and white only",
                _ => "Unknown theme"
            };
        }

        /// <summary>
        /// Changes the animation mode.
        /// </summary>
        /// <returns>A task that completes when the animation mode is changed.</returns>
        private async Task ChangeAnimationMode()
        {
            Console.Clear();
            Console.ForegroundColor = TitleColor;
            Console.WriteLine("=== SELECT ANIMATION MODE ===");
            Console.WriteLine();

            var modes = Enum.GetValues<AnimationMode>();
            int selectedMode = Array.IndexOf(modes, _settings.AnimationMode);

            bool selecting = true;
            while (selecting)
            {
                Console.SetCursorPosition(0, 2);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Choose animation level:");
                Console.WriteLine();

                for (int i = 0; i < modes.Length; i++)
                {
                    Console.ForegroundColor = i == selectedMode ? SelectedItemColor : NormalItemColor;
                    string indicator = i == selectedMode ? "> " : "  ";
                    string description = GetAnimationModeDescription(modes[i]);
                    Console.WriteLine($"{indicator}{modes[i],-10} - {description}");
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
                            selectedMode = Math.Max(0, selectedMode - 1);
                            break;
                        case ConsoleKey.DownArrow:
                            selectedMode = Math.Min(modes.Length - 1, selectedMode + 1);
                            break;
                        case ConsoleKey.Enter:
                            _settings.AnimationMode = modes[selectedMode];
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
        /// Gets a description for an animation mode.
        /// </summary>
        /// <param name="mode">The animation mode.</param>
        /// <returns>A description of the animation mode.</returns>
        private string GetAnimationModeDescription(AnimationMode mode)
        {
            return mode switch
            {
                AnimationMode.None => "No animations, fastest performance",
                AnimationMode.Minimal => "Essential feedback only",
                AnimationMode.Normal => "Standard effects and transitions",
                AnimationMode.Enhanced => "Full effects with extra polish",
                _ => "Unknown animation mode"
            };
        }

        /// <summary>
        /// Shows a preview of the current theme with sample game elements.
        /// </summary>
        /// <returns>A task that completes when the preview is closed.</returns>
        private async Task PreviewCurrentTheme()
        {
            Console.Clear();
            ApplyCurrentTheme();

            Console.ForegroundColor = GetThemedColor(ThemeElement.Title);
            Console.WriteLine("=== THEME PREVIEW ===");
            Console.WriteLine();

            Console.ForegroundColor = GetThemedColor(ThemeElement.Subtitle);
            Console.WriteLine($"Theme: {_settings.ColorTheme}");
            Console.WriteLine($"Animation: {_settings.AnimationMode}");
            Console.WriteLine();

            // Draw a sample game board
            Console.ForegroundColor = GetThemedColor(ThemeElement.NormalItem);
            Console.WriteLine("Sample Game Board:");
            Console.WriteLine("┌────────────┐");

            for (int row = 0; row < 5; row++)
            {
                Console.Write("│");
                for (int col = 0; col < 5; col++)
                {
                    if (row == 4 && col < 3)
                    {
                        Console.ForegroundColor = GetPieceColor(0); // I piece
                        Console.Write("██");
                    }
                    else if (row == 3 && col >= 3)
                    {
                        Console.ForegroundColor = GetPieceColor(3); // O piece
                        Console.Write("██");
                    }
                    else
                    {
                        Console.ForegroundColor = GetThemedColor(ThemeElement.NormalItem);
                        Console.Write("  ");
                    }
                }
                Console.ForegroundColor = GetThemedColor(ThemeElement.NormalItem);
                Console.WriteLine("│");
            }

            Console.WriteLine("└────────────┘");
            Console.WriteLine();

            Console.ForegroundColor = GetThemedColor(ThemeElement.Footer);
            Console.WriteLine("Press any key to return...");
            Console.ReadKey(true);
        }

        /// <summary>
        /// Resets the visual settings to their default values.
        /// </summary>
        /// <returns>A task that completes when the settings are reset.</returns>
        private async Task ResetVisualsToDefaults()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Reset visual settings to defaults?");
            Console.WriteLine("This will restore the classic theme and normal animations.");
            Console.WriteLine();
            Console.WriteLine("Press Y to confirm, any other key to cancel...");

            ConsoleKeyInfo key = Console.ReadKey(true);
            if (key.Key == ConsoleKey.Y)
            {
                _settings.ColorTheme = ColorTheme.Classic;
                _settings.AnimationMode = AnimationMode.Normal;
                
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Visual settings reset to defaults.");
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

        #region Nested Classes and Enums

        /// <summary>
        /// Represents a menu item in the visual settings dialog.
        /// </summary>
        private class VisualMenuItem
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
            /// Initializes a new instance of the VisualMenuItem class.
            /// </summary>
            /// <param name="text">The text to display for this menu item.</param>
            /// <param name="action">The action to execute when this menu item is selected.</param>
            public VisualMenuItem(string text, Func<Task> action)
            {
                Text = text ?? throw new ArgumentNullException(nameof(text));
                Action = action ?? throw new ArgumentNullException(nameof(action));
            }
        }

        /// <summary>
        /// Represents different UI elements that can be themed.
        /// </summary>
        private enum ThemeElement
        {
            Title,
            Subtitle,
            SelectedItem,
            NormalItem,
            Value,
            Footer
        }

        #endregion
    }
}
