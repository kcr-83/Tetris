using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Tetris.Core.Models
{
    /// <summary>
    /// Represents user customizable settings for the Tetris game.
    /// This includes control mappings, audio preferences, and visual themes.
    /// </summary>
    public class UserSettings
    {
        #region Properties

        /// <summary>
        /// Gets or sets the unique identifier for these settings.
        /// </summary>
        [JsonPropertyName("settingsId")]
        public string SettingsId { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Gets or sets the control scheme configuration.
        /// </summary>
        [JsonPropertyName("controlSettings")]
        public ControlSettings ControlSettings { get; set; } = new();

        /// <summary>
        /// Gets or sets whether sound effects are enabled.
        /// </summary>
        [JsonPropertyName("soundEffectsEnabled")]
        public bool SoundEffectsEnabled { get; set; } = true;

        /// <summary>
        /// Gets or sets whether background music is enabled.
        /// </summary>
        [JsonPropertyName("musicEnabled")]
        public bool MusicEnabled { get; set; } = true;

        /// <summary>
        /// Gets or sets the color theme for the game.
        /// </summary>
        [JsonPropertyName("colorTheme")]
        public ColorTheme ColorTheme { get; set; } = ColorTheme.Classic;

        /// <summary>
        /// Gets or sets the master volume level (0.0 to 1.0).
        /// </summary>
        [JsonPropertyName("masterVolume")]
        public float MasterVolume { get; set; } = 0.8f;

        /// <summary>
        /// Gets or sets whether the ghost piece is shown.
        /// </summary>
        [JsonPropertyName("showGhostPiece")]
        public bool ShowGhostPiece { get; set; } = true;

        /// <summary>
        /// Gets or sets the display mode for animations.
        /// </summary>
        [JsonPropertyName("animationMode")]
        public AnimationMode AnimationMode { get; set; } = AnimationMode.Normal;

        /// <summary>
        /// Gets or sets the date and time when these settings were created.
        /// </summary>
        [JsonPropertyName("dateCreated")]
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the date and time when these settings were last modified.
        /// </summary>
        [JsonPropertyName("dateModified")]
        public DateTime DateModified { get; set; } = DateTime.UtcNow;

        #endregion

        #region Methods

        /// <summary>
        /// Creates a deep copy of the current settings.
        /// </summary>
        /// <returns>A new UserSettings instance with the same values.</returns>
        public UserSettings Clone()
        {
            return new UserSettings
            {
                SettingsId = SettingsId,
                ControlSettings = ControlSettings.Clone(),
                SoundEffectsEnabled = SoundEffectsEnabled,
                MusicEnabled = MusicEnabled,
                ColorTheme = ColorTheme,
                MasterVolume = MasterVolume,
                ShowGhostPiece = ShowGhostPiece,
                AnimationMode = AnimationMode,
                DateCreated = DateCreated,
                DateModified = DateTime.UtcNow
            };
        }

        /// <summary>
        /// Gets the default user settings configuration.
        /// </summary>
        /// <returns>A UserSettings instance with default values.</returns>
        public static UserSettings GetDefaultSettings()
        {
            return new UserSettings();
        }

        #endregion
    }

    /// <summary>
    /// Represents control mapping settings for the game.
    /// </summary>
    public class ControlSettings
    {
        #region Properties

        /// <summary>
        /// Gets or sets the control scheme type.
        /// </summary>
        [JsonPropertyName("controlScheme")]
        public ControlScheme ControlScheme { get; set; } = ControlScheme.Standard;

        /// <summary>
        /// Gets or sets the custom key mappings for game actions.
        /// </summary>
        [JsonPropertyName("keyMappings")]
        public Dictionary<GameAction, ConsoleKey> KeyMappings { get; set; } = GetDefaultKeyMappings();

        /// <summary>
        /// Gets or sets whether key repeat is enabled for movement.
        /// </summary>
        [JsonPropertyName("keyRepeatEnabled")]
        public bool KeyRepeatEnabled { get; set; } = true;

        /// <summary>
        /// Gets or sets the key repeat delay in milliseconds.
        /// </summary>
        [JsonPropertyName("keyRepeatDelay")]
        public int KeyRepeatDelay { get; set; } = 150;

        #endregion

        #region Methods

        /// <summary>
        /// Creates a deep copy of the current control settings.
        /// </summary>
        /// <returns>A new ControlSettings instance with the same values.</returns>
        public ControlSettings Clone()
        {
            return new ControlSettings
            {
                ControlScheme = ControlScheme,
                KeyMappings = new Dictionary<GameAction, ConsoleKey>(KeyMappings),
                KeyRepeatEnabled = KeyRepeatEnabled,
                KeyRepeatDelay = KeyRepeatDelay
            };
        }

        /// <summary>
        /// Gets the default key mappings for the standard control scheme.
        /// </summary>
        /// <returns>A dictionary with default key mappings.</returns>
        public static Dictionary<GameAction, ConsoleKey> GetDefaultKeyMappings()
        {
            return new Dictionary<GameAction, ConsoleKey>
            {
                { GameAction.MoveLeft, ConsoleKey.LeftArrow },
                { GameAction.MoveRight, ConsoleKey.RightArrow },
                { GameAction.SoftDrop, ConsoleKey.DownArrow },
                { GameAction.HardDrop, ConsoleKey.Spacebar },
                { GameAction.RotateClockwise, ConsoleKey.UpArrow },
                { GameAction.RotateCounterclockwise, ConsoleKey.Z },
                { GameAction.Hold, ConsoleKey.C },
                { GameAction.Pause, ConsoleKey.P },
                { GameAction.Menu, ConsoleKey.Escape }
            };
        }

        /// <summary>
        /// Gets the alternative key mappings for the WASD control scheme.
        /// </summary>
        /// <returns>A dictionary with WASD key mappings.</returns>
        public static Dictionary<GameAction, ConsoleKey> GetWASDKeyMappings()
        {
            return new Dictionary<GameAction, ConsoleKey>
            {
                { GameAction.MoveLeft, ConsoleKey.A },
                { GameAction.MoveRight, ConsoleKey.D },
                { GameAction.SoftDrop, ConsoleKey.S },
                { GameAction.HardDrop, ConsoleKey.Spacebar },
                { GameAction.RotateClockwise, ConsoleKey.W },
                { GameAction.RotateCounterclockwise, ConsoleKey.Q },
                { GameAction.Hold, ConsoleKey.E },
                { GameAction.Pause, ConsoleKey.P },
                { GameAction.Menu, ConsoleKey.Escape }
            };
        }

        #endregion
    }

    /// <summary>
    /// Represents the available control schemes.
    /// </summary>
    public enum ControlScheme
    {
        /// <summary>
        /// Standard arrow key control scheme.
        /// </summary>
        Standard,

        /// <summary>
        /// WASD key control scheme.
        /// </summary>
        WASD,

        /// <summary>
        /// Custom user-defined control scheme.
        /// </summary>
        Custom
    }

    /// <summary>
    /// Represents the available game actions that can be mapped to keys.
    /// </summary>
    public enum GameAction
    {
        /// <summary>
        /// Move the current piece to the left.
        /// </summary>
        MoveLeft,

        /// <summary>
        /// Move the current piece to the right.
        /// </summary>
        MoveRight,

        /// <summary>
        /// Accelerate the piece's downward movement.
        /// </summary>
        SoftDrop,

        /// <summary>
        /// Instantly drop the piece to the bottom.
        /// </summary>
        HardDrop,

        /// <summary>
        /// Rotate the piece clockwise.
        /// </summary>
        RotateClockwise,

        /// <summary>
        /// Rotate the piece counterclockwise.
        /// </summary>
        RotateCounterclockwise,

        /// <summary>
        /// Hold the current piece for later use.
        /// </summary>
        Hold,

        /// <summary>
        /// Pause or unpause the game.
        /// </summary>
        Pause,

        /// <summary>
        /// Open the menu or exit to menu.
        /// </summary>
        Menu
    }

    /// <summary>
    /// Represents the available color themes for the game.
    /// </summary>
    public enum ColorTheme
    {
        /// <summary>
        /// Classic Tetris colors.
        /// </summary>
        Classic,

        /// <summary>
        /// Dark theme with muted colors.
        /// </summary>
        Dark,

        /// <summary>
        /// High contrast theme for accessibility.
        /// </summary>
        HighContrast,

        /// <summary>
        /// Neon theme with bright colors.
        /// </summary>
        Neon,

        /// <summary>
        /// Monochrome theme using only white and gray.
        /// </summary>
        Monochrome
    }

    /// <summary>
    /// Represents the available animation modes.
    /// </summary>
    public enum AnimationMode
    {
        /// <summary>
        /// No animations, fastest performance.
        /// </summary>
        None,

        /// <summary>
        /// Minimal animations for essential feedback.
        /// </summary>
        Minimal,

        /// <summary>
        /// Normal animation level.
        /// </summary>
        Normal,

        /// <summary>
        /// Enhanced animations with additional effects.
        /// </summary>
        Enhanced
    }
}
