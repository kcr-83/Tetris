using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Tetris.Core.Models;

namespace Tetris.Core.Services
{
    /// <summary>
    /// Service for managing user settings including controls, audio, and visual preferences.
    /// Handles loading, saving, and validation of user settings with file-based persistence.
    /// </summary>
    public class UserSettingsService : IUserSettingsService
    {
        #region Constants

        /// <summary>
        /// The default filename for user settings.
        /// </summary>
        private const string SettingsFileName = "settings.json";

        /// <summary>
        /// The directory where settings are stored.
        /// </summary>
        private const string SettingsDirectory = "Settings";

        #endregion

        #region Fields

        private UserSettings _currentSettings;
        private readonly string _settingsFilePath;
        private readonly JsonSerializerOptions _jsonOptions;

        #endregion

        #region Events

        /// <summary>
        /// Event raised when settings are successfully updated.
        /// </summary>
        public event EventHandler<SettingsUpdatedEventArgs>? SettingsUpdated;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the current user settings.
        /// </summary>
        public UserSettings CurrentSettings => _currentSettings;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the UserSettingsService class.
        /// </summary>
        public UserSettingsService()
        {
            _currentSettings = UserSettings.GetDefaultSettings();
            _settingsFilePath = Path.Combine(SettingsDirectory, SettingsFileName);
            
            _jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
            };

            EnsureSettingsDirectoryExists();
        }

        /// <summary>
        /// Initializes a new instance of the UserSettingsService class with a custom settings directory.
        /// This constructor is primarily used for testing.
        /// </summary>
        /// <param name="customSettingsDirectory">The custom directory to store settings in.</param>
        public UserSettingsService(string customSettingsDirectory)
        {
            _currentSettings = UserSettings.GetDefaultSettings();
            _settingsFilePath = Path.Combine(customSettingsDirectory, SettingsFileName);
            
            _jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
            };

            EnsureSettingsDirectoryExists();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Loads user settings from persistent storage.
        /// If no settings file exists, returns default settings.
        /// </summary>
        /// <returns>A task that returns the loaded user settings.</returns>
        public async Task<UserSettings> LoadSettingsAsync()
        {
            try
            {
                if (!File.Exists(_settingsFilePath))
                {
                    _currentSettings = UserSettings.GetDefaultSettings();
                    await SaveSettingsAsync(_currentSettings);
                    return _currentSettings;
                }

                string jsonContent = await File.ReadAllTextAsync(_settingsFilePath);
                
                if (string.IsNullOrWhiteSpace(jsonContent))
                {
                    _currentSettings = UserSettings.GetDefaultSettings();
                    return _currentSettings;
                }

                UserSettings? loadedSettings = JsonSerializer.Deserialize<UserSettings>(jsonContent, _jsonOptions);
                
                if (loadedSettings != null && ValidateSettings(loadedSettings))
                {
                    _currentSettings = loadedSettings;
                }
                else
                {
                    _currentSettings = UserSettings.GetDefaultSettings();
                    // Save corrected settings
                    await SaveSettingsAsync(_currentSettings);
                }

                SettingsUpdated?.Invoke(this, new SettingsUpdatedEventArgs(_currentSettings, SettingsChangeType.Loaded));
                return _currentSettings;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading settings: {ex.Message}");
                _currentSettings = UserSettings.GetDefaultSettings();
                return _currentSettings;
            }
        }

        /// <summary>
        /// Saves the current settings to persistent storage.
        /// </summary>
        /// <param name="settings">The settings to save.</param>
        /// <returns>A task that completes when the settings are saved.</returns>
        public async Task SaveSettingsAsync(UserSettings settings)
        {
            try
            {
                if (settings == null)
                {
                    throw new ArgumentNullException(nameof(settings));
                }

                if (!ValidateSettings(settings))
                {
                    throw new ArgumentException("Invalid settings provided", nameof(settings));
                }

                settings.DateModified = DateTime.UtcNow;
                
                EnsureSettingsDirectoryExists();
                
                string jsonContent = JsonSerializer.Serialize(settings, _jsonOptions);
                await File.WriteAllTextAsync(_settingsFilePath, jsonContent);
                
                _currentSettings = settings.Clone();
                
                SettingsUpdated?.Invoke(this, new SettingsUpdatedEventArgs(_currentSettings, SettingsChangeType.Saved));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving settings: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Updates a specific setting and saves the changes.
        /// </summary>
        /// <param name="updateAction">An action to update the settings.</param>
        /// <returns>A task that completes when the settings are updated and saved.</returns>
        public async Task UpdateSettingAsync(Action<UserSettings> updateAction)
        {
            try
            {
                if (updateAction == null)
                {
                    throw new ArgumentNullException(nameof(updateAction));
                }

                UserSettings updatedSettings = _currentSettings.Clone();
                updateAction(updatedSettings);
                
                await SaveSettingsAsync(updatedSettings);
                
                SettingsUpdated?.Invoke(this, new SettingsUpdatedEventArgs(_currentSettings, SettingsChangeType.Updated));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating settings: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Resets all settings to their default values.
        /// </summary>
        /// <returns>A task that completes when the settings are reset and saved.</returns>
        public async Task ResetToDefaultsAsync()
        {
            try
            {
                UserSettings defaultSettings = UserSettings.GetDefaultSettings();
                await SaveSettingsAsync(defaultSettings);
                
                SettingsUpdated?.Invoke(this, new SettingsUpdatedEventArgs(_currentSettings, SettingsChangeType.Reset));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error resetting settings: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Validates the given settings for correctness and consistency.
        /// </summary>
        /// <param name="settings">The settings to validate.</param>
        /// <returns>True if the settings are valid, false otherwise.</returns>
        public bool ValidateSettings(UserSettings settings)
        {
            if (settings == null)
            {
                return false;
            }

            try
            {
                // Validate master volume range
                if (settings.MasterVolume < 0.0f || settings.MasterVolume > 1.0f)
                {
                    return false;
                }

                // Validate control settings
                if (settings.ControlSettings == null)
                {
                    return false;
                }

                // Validate key mappings
                if (settings.ControlSettings.KeyMappings == null || settings.ControlSettings.KeyMappings.Count == 0)
                {
                    return false;
                }

                // Check for duplicate key mappings
                HashSet<ConsoleKey> usedKeys = new HashSet<ConsoleKey>();
                foreach (ConsoleKey key in settings.ControlSettings.KeyMappings.Values)
                {
                    if (usedKeys.Contains(key))
                    {
                        return false; // Duplicate key mapping
                    }
                    usedKeys.Add(key);
                }

                // Validate key repeat delay
                if (settings.ControlSettings.KeyRepeatDelay < 50 || settings.ControlSettings.KeyRepeatDelay > 1000)
                {
                    return false;
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Applies the current settings to the game engine and UI components.
        /// </summary>
        /// <returns>A task that completes when settings are applied.</returns>
        public async Task ApplySettingsAsync()
        {
            try
            {
                // Apply settings to game components
                // This would typically involve notifying various game components about setting changes
                
                // For now, we'll just ensure settings are loaded and valid
                if (_currentSettings == null)
                {
                    await LoadSettingsAsync();
                }

                // Future implementations could include:
                // - Updating audio system volume levels
                // - Configuring input handling for new key mappings
                // - Applying color theme to UI components
                // - Setting animation preferences
                
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error applying settings: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Gets the key mapping for a specific game action.
        /// </summary>
        /// <param name="action">The game action to get the key for.</param>
        /// <returns>The console key mapped to the action.</returns>
        public ConsoleKey GetKeyForAction(GameAction action)
        {
            if (_currentSettings?.ControlSettings?.KeyMappings?.ContainsKey(action) == true)
            {
                return _currentSettings.ControlSettings.KeyMappings[action];
            }

            // Fallback to default mappings
            var defaultMappings = ControlSettings.GetDefaultKeyMappings();
            return defaultMappings.ContainsKey(action) ? defaultMappings[action] : ConsoleKey.Escape;
        }

        /// <summary>
        /// Sets a custom key mapping for a specific game action.
        /// </summary>
        /// <param name="action">The game action to map.</param>
        /// <param name="key">The console key to map to the action.</param>
        /// <returns>A task that completes when the mapping is saved.</returns>
        public async Task SetKeyMappingAsync(GameAction action, ConsoleKey key)
        {
            await UpdateSettingAsync(settings =>
            {
                // Check if this key is already mapped to another action
                foreach (var kvp in settings.ControlSettings.KeyMappings)
                {
                    if (kvp.Value == key && kvp.Key != action)
                    {
                        throw new InvalidOperationException($"Key {key} is already mapped to action {kvp.Key}");
                    }
                }

                settings.ControlSettings.KeyMappings[action] = key;
                settings.ControlSettings.ControlScheme = ControlScheme.Custom;
            });
        }

        /// <summary>
        /// Checks if a specific feature is enabled based on current settings.
        /// </summary>
        /// <param name="feature">The feature to check.</param>
        /// <returns>True if the feature is enabled, false otherwise.</returns>
        public bool IsFeatureEnabled(SettingsFeature feature)
        {
            return feature switch
            {
                SettingsFeature.SoundEffects => _currentSettings.SoundEffectsEnabled,
                SettingsFeature.Music => _currentSettings.MusicEnabled,
                SettingsFeature.GhostPiece => _currentSettings.ShowGhostPiece,
                SettingsFeature.Animations => _currentSettings.AnimationMode != AnimationMode.None,
                SettingsFeature.KeyRepeat => _currentSettings.ControlSettings.KeyRepeatEnabled,
                _ => false
            };
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Ensures that the settings directory exists, creating it if necessary.
        /// </summary>
        private void EnsureSettingsDirectoryExists()
        {
            try
            {
                if (!Directory.Exists(SettingsDirectory))
                {
                    Directory.CreateDirectory(SettingsDirectory);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating settings directory: {ex.Message}");
            }
        }

        #endregion
    }
}
