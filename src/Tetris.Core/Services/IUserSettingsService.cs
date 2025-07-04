using System;
using System.Threading.Tasks;
using Tetris.Core.Models;

namespace Tetris.Core.Services
{
    /// <summary>
    /// Interface for managing user settings including controls, audio, and visual preferences.
    /// Provides methods for loading, saving, and updating user settings with validation.
    /// </summary>
    public interface IUserSettingsService
    {
        #region Events

        /// <summary>
        /// Event raised when settings are successfully updated.
        /// </summary>
        event EventHandler<SettingsUpdatedEventArgs>? SettingsUpdated;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the current user settings.
        /// </summary>
        UserSettings CurrentSettings { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Loads user settings from persistent storage.
        /// If no settings file exists, returns default settings.
        /// </summary>
        /// <returns>A task that returns the loaded user settings.</returns>
        Task<UserSettings> LoadSettingsAsync();

        /// <summary>
        /// Saves the current settings to persistent storage.
        /// </summary>
        /// <param name="settings">The settings to save.</param>
        /// <returns>A task that completes when the settings are saved.</returns>
        Task SaveSettingsAsync(UserSettings settings);

        /// <summary>
        /// Updates a specific setting and saves the changes.
        /// </summary>
        /// <param name="updateAction">An action to update the settings.</param>
        /// <returns>A task that completes when the settings are updated and saved.</returns>
        Task UpdateSettingAsync(Action<UserSettings> updateAction);

        /// <summary>
        /// Resets all settings to their default values.
        /// </summary>
        /// <returns>A task that completes when the settings are reset and saved.</returns>
        Task ResetToDefaultsAsync();

        /// <summary>
        /// Validates the given settings for correctness and consistency.
        /// </summary>
        /// <param name="settings">The settings to validate.</param>
        /// <returns>True if the settings are valid, false otherwise.</returns>
        bool ValidateSettings(UserSettings settings);

        /// <summary>
        /// Applies the current settings to the game engine and UI components.
        /// </summary>
        /// <returns>A task that completes when settings are applied.</returns>
        Task ApplySettingsAsync();

        /// <summary>
        /// Gets the key mapping for a specific game action.
        /// </summary>
        /// <param name="action">The game action to get the key for.</param>
        /// <returns>The console key mapped to the action.</returns>
        ConsoleKey GetKeyForAction(GameAction action);

        /// <summary>
        /// Sets a custom key mapping for a specific game action.
        /// </summary>
        /// <param name="action">The game action to map.</param>
        /// <param name="key">The console key to map to the action.</param>
        /// <returns>A task that completes when the mapping is saved.</returns>
        Task SetKeyMappingAsync(GameAction action, ConsoleKey key);

        /// <summary>
        /// Checks if a specific feature is enabled based on current settings.
        /// </summary>
        /// <param name="feature">The feature to check.</param>
        /// <returns>True if the feature is enabled, false otherwise.</returns>
        bool IsFeatureEnabled(SettingsFeature feature);

        #endregion
    }

    /// <summary>
    /// Event arguments for settings update notifications.
    /// </summary>
    public class SettingsUpdatedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the updated settings.
        /// </summary>
        public UserSettings UpdatedSettings { get; }

        /// <summary>
        /// Gets the type of change that occurred.
        /// </summary>
        public SettingsChangeType ChangeType { get; }

        /// <summary>
        /// Initializes a new instance of the SettingsUpdatedEventArgs class.
        /// </summary>
        /// <param name="updatedSettings">The updated settings.</param>
        /// <param name="changeType">The type of change that occurred.</param>
        public SettingsUpdatedEventArgs(UserSettings updatedSettings, SettingsChangeType changeType)
        {
            UpdatedSettings = updatedSettings ?? throw new ArgumentNullException(nameof(updatedSettings));
            ChangeType = changeType;
        }
    }

    /// <summary>
    /// Represents the types of settings changes that can occur.
    /// </summary>
    public enum SettingsChangeType
    {
        /// <summary>
        /// Settings were loaded from storage.
        /// </summary>
        Loaded,

        /// <summary>
        /// Individual setting was updated.
        /// </summary>
        Updated,

        /// <summary>
        /// Settings were reset to defaults.
        /// </summary>
        Reset,

        /// <summary>
        /// Settings were saved to storage.
        /// </summary>
        Saved
    }

    /// <summary>
    /// Represents features that can be enabled or disabled through settings.
    /// </summary>
    public enum SettingsFeature
    {
        /// <summary>
        /// Sound effects feature.
        /// </summary>
        SoundEffects,

        /// <summary>
        /// Background music feature.
        /// </summary>
        Music,

        /// <summary>
        /// Ghost piece visualization feature.
        /// </summary>
        GhostPiece,

        /// <summary>
        /// Animation effects feature.
        /// </summary>
        Animations,

        /// <summary>
        /// Key repeat functionality.
        /// </summary>
        KeyRepeat
    }
}
