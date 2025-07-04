using System;
using System.Threading.Tasks;
using Tetris.Core.Models;
using Tetris.Core.Services;

namespace Tetris.Core.UI
{
    /// <summary>
    /// Service that applies user settings to game components at runtime.
    /// Handles theme changes, control mapping, and feature toggles during gameplay.
    /// </summary>
    public class SettingsApplicator
    {
        #region Fields

        private readonly IUserSettingsService _settingsService;
        private readonly GameEngine _gameEngine;
        private UserSettings _currentSettings;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the SettingsApplicator class.
        /// </summary>
        /// <param name="settingsService">The settings service to monitor for changes.</param>
        /// <param name="gameEngine">The game engine to apply settings to.</param>
        public SettingsApplicator(IUserSettingsService settingsService, GameEngine gameEngine)
        {
            _settingsService = settingsService ?? throw new ArgumentNullException(nameof(settingsService));
            _gameEngine = gameEngine ?? throw new ArgumentNullException(nameof(gameEngine));
            _currentSettings = _settingsService.CurrentSettings;

            // Subscribe to settings changes
            _settingsService.SettingsUpdated += OnSettingsUpdated;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Applies current settings to all game components.
        /// </summary>
        public async Task ApplySettingsAsync()
        {
            await ApplyAudioSettingsAsync();
            ApplyVisualSettings();
            ApplyControlSettings();
            ApplyGameplaySettings();
        }

        /// <summary>
        /// Processes a key input based on current control settings.
        /// </summary>
        /// <param name="key">The key that was pressed.</param>
        /// <returns>True if the key was handled, false otherwise.</returns>
        public bool ProcessKeyInput(ConsoleKey key)
        {
            // Check if this key is mapped to any action
            foreach (GameAction action in Enum.GetValues<GameAction>())
            {
                if (_settingsService.GetKeyForAction(action) == key)
                {
                    return ExecuteGameAction(action);
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the current color scheme based on the selected theme.
        /// </summary>
        /// <returns>A ColorScheme object with the appropriate colors.</returns>
        public ColorScheme GetCurrentColorScheme()
        {
            return _currentSettings.ColorTheme switch
            {
                ColorTheme.Classic => new ColorScheme
                {
                    Background = ConsoleColor.Black,
                    Text = ConsoleColor.White,
                    Accent = ConsoleColor.Cyan,
                    Highlight = ConsoleColor.Yellow,
                    Success = ConsoleColor.Green,
                    Warning = ConsoleColor.DarkYellow,
                    Error = ConsoleColor.Red
                },
                ColorTheme.Dark => new ColorScheme
                {
                    Background = ConsoleColor.Black,
                    Text = ConsoleColor.Gray,
                    Accent = ConsoleColor.Blue,
                    Highlight = ConsoleColor.White,
                    Success = ConsoleColor.DarkGreen,
                    Warning = ConsoleColor.DarkYellow,
                    Error = ConsoleColor.DarkRed
                },
                ColorTheme.HighContrast => new ColorScheme
                {
                    Background = ConsoleColor.Black,
                    Text = ConsoleColor.White,
                    Accent = ConsoleColor.Yellow,
                    Highlight = ConsoleColor.White,
                    Success = ConsoleColor.Green,
                    Warning = ConsoleColor.Yellow,
                    Error = ConsoleColor.Red
                },
                ColorTheme.Neon => new ColorScheme
                {
                    Background = ConsoleColor.Black,
                    Text = ConsoleColor.Green,
                    Accent = ConsoleColor.Magenta,
                    Highlight = ConsoleColor.Cyan,
                    Success = ConsoleColor.Green,
                    Warning = ConsoleColor.Yellow,
                    Error = ConsoleColor.Red
                },
                ColorTheme.Monochrome => new ColorScheme
                {
                    Background = ConsoleColor.Black,
                    Text = ConsoleColor.White,
                    Accent = ConsoleColor.Gray,
                    Highlight = ConsoleColor.White,
                    Success = ConsoleColor.White,
                    Warning = ConsoleColor.Gray,
                    Error = ConsoleColor.White
                },
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        /// <summary>
        /// Checks if a specific feature is enabled in the current settings.
        /// </summary>
        /// <param name="feature">The feature to check.</param>
        /// <returns>True if the feature is enabled, false otherwise.</returns>
        public bool IsFeatureEnabled(SettingsFeature feature)
        {
            return _settingsService.IsFeatureEnabled(feature);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Handles settings update events.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private async void OnSettingsUpdated(object? sender, SettingsUpdatedEventArgs e)
        {
            _currentSettings = e.UpdatedSettings;
            await ApplySettingsAsync();
        }

        /// <summary>
        /// Applies audio settings to the game.
        /// </summary>
        private async Task ApplyAudioSettingsAsync()
        {
            // In a real implementation, this would configure audio systems
            // For this console-based game, we'll simulate audio settings
            
            if (_currentSettings.SoundEffectsEnabled)
            {
                // Enable sound effects
                await Task.Run(() =>
                {
                    // Configure sound effects system
                    // Set volume levels, enable audio processing
                });
            }
            else
            {
                // Disable sound effects
                await Task.Run(() =>
                {
                    // Mute or disable sound effects
                });
            }

            if (_currentSettings.MusicEnabled)
            {
                // Enable background music
                await Task.Run(() =>
                {
                    // Start background music playback
                    // Apply volume settings
                });
            }
            else
            {
                // Disable background music
                await Task.Run(() =>
                {
                    // Stop background music
                });
            }
        }

        /// <summary>
        /// Applies visual settings to the game interface.
        /// </summary>
        private void ApplyVisualSettings()
        {
            // Apply color theme
            var colorScheme = GetCurrentColorScheme();
            Console.BackgroundColor = colorScheme.Background;
            Console.ForegroundColor = colorScheme.Text;

            // Configure animation settings
            switch (_currentSettings.AnimationMode)
            {
                case AnimationMode.None:
                    // Disable all animations
                    break;
                case AnimationMode.Minimal:
                    // Enable only essential animations
                    break;
                case AnimationMode.Normal:
                    // Enable standard animations
                    break;
                case AnimationMode.Enhanced:
                    // Enable all animations with enhanced effects
                    break;
            }
        }

        /// <summary>
        /// Applies control settings to the input system.
        /// </summary>
        private void ApplyControlSettings()
        {
            // Control settings are applied through the ProcessKeyInput method
            // This method can be used to configure additional control-related settings
            
            // Apply control scheme specific settings
            switch (_currentSettings.ControlSettings.ControlScheme)
            {
                case ControlScheme.Standard:
                    // Configure standard control responsiveness
                    break;
                case ControlScheme.WASD:
                    // Configure WASD-specific settings
                    break;
                case ControlScheme.Custom:
                    // Apply custom control configurations
                    break;
            }

            // Apply key repeat settings
            // Configure input sensitivity based on user preferences
        }

        /// <summary>
        /// Applies gameplay settings to the game engine.
        /// </summary>
        private void ApplyGameplaySettings()
        {
            // Enable or disable ghost piece based on settings
            if (_gameEngine != null)
            {
                // In a full implementation, the GameEngine would have properties for these features
                // For now, we'll store the settings for use by other components
                
                // Configure ghost piece visibility
                // Configure auto-pause behavior
                // Configure difficulty adjustments
            }
        }

        /// <summary>
        /// Executes a game action based on the mapped input.
        /// </summary>
        /// <param name="action">The game action to execute.</param>
        /// <returns>True if the action was executed, false otherwise.</returns>
        private bool ExecuteGameAction(GameAction action)
        {
            switch (action)
            {
                case GameAction.MoveLeft:
                    _gameEngine.MovePieceLeft();
                    return true;
                case GameAction.MoveRight:
                    _gameEngine.MovePieceRight();
                    return true;
                case GameAction.RotateClockwise:
                    _gameEngine.RotatePieceClockwise();
                    return true;
                case GameAction.RotateCounterclockwise:
                    _gameEngine.RotatePieceCounterClockwise();
                    return true;
                case GameAction.SoftDrop:
                    _gameEngine.ActivateFastDrop();
                    return true;
                case GameAction.HardDrop:
                    _gameEngine.HardDrop();
                    return true;
                case GameAction.Hold:
                    // Implement hold functionality if available
                    return true;
                case GameAction.Pause:
                    // Implement pause functionality
                    return true;
                default:
                    return false;
            }
        }

        #endregion
    }

    /// <summary>
    /// Represents a color scheme for the game interface.
    /// </summary>
    public class ColorScheme
    {
        /// <summary>
        /// Gets or sets the background color.
        /// </summary>
        public ConsoleColor Background { get; set; } = ConsoleColor.Black;

        /// <summary>
        /// Gets or sets the primary text color.
        /// </summary>
        public ConsoleColor Text { get; set; } = ConsoleColor.White;

        /// <summary>
        /// Gets or sets the accent color for highlights.
        /// </summary>
        public ConsoleColor Accent { get; set; } = ConsoleColor.Cyan;

        /// <summary>
        /// Gets or sets the highlight color for selected items.
        /// </summary>
        public ConsoleColor Highlight { get; set; } = ConsoleColor.Yellow;

        /// <summary>
        /// Gets or sets the color for success messages.
        /// </summary>
        public ConsoleColor Success { get; set; } = ConsoleColor.Green;

        /// <summary>
        /// Gets or sets the color for warning messages.
        /// </summary>
        public ConsoleColor Warning { get; set; } = ConsoleColor.DarkYellow;

        /// <summary>
        /// Gets or sets the color for error messages.
        /// </summary>
        public ConsoleColor Error { get; set; } = ConsoleColor.Red;
    }
}
