using System;
using System.IO;
using System.Threading.Tasks;
using Tetris.Core.Models;
using Tetris.Core.Services;
using Xunit;

namespace Tetris.Core.Tests
{
    /// <summary>
    /// Unit tests for the UserSettingsService class.
    /// Tests settings persistence, validation, and event handling.
    /// </summary>
    public class UserSettingsServiceTests : IDisposable
    {
        #region Fields

        private readonly string _testSettingsDirectory;
        private readonly UserSettingsService _settingsService;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the UserSettingsServiceTests class.
        /// Sets up a temporary directory for testing settings persistence.
        /// </summary>
        public UserSettingsServiceTests()
        {
            _testSettingsDirectory = Path.Combine(Path.GetTempPath(), $"TetrisTests_{Guid.NewGuid()}");
            Directory.CreateDirectory(_testSettingsDirectory);
            _settingsService = new UserSettingsService(_testSettingsDirectory);
        }

        #endregion

        #region Cleanup

        /// <summary>
        /// Cleans up test resources.
        /// </summary>
        public void Dispose()
        {
            if (Directory.Exists(_testSettingsDirectory))
            {
                Directory.Delete(_testSettingsDirectory, true);
            }
        }

        #endregion

        #region LoadSettingsAsync Tests

        [Fact]
        public async Task LoadSettingsAsync_NoExistingFile_ReturnsDefaultSettings()
        {
            // Act
            UserSettings settings = await _settingsService.LoadSettingsAsync();

            // Assert
            Assert.NotNull(settings);
            Assert.True(settings.SoundEffectsEnabled);
            Assert.True(settings.MusicEnabled);
            Assert.Equal(ColorTheme.Classic, settings.ColorTheme);
            Assert.Equal(0.8f, settings.MasterVolume);
            Assert.True(settings.ShowGhostPiece);
            Assert.Equal(AnimationMode.Normal, settings.AnimationMode);
        }

        [Fact]
        public async Task LoadSettingsAsync_ExistingValidFile_ReturnsStoredSettings()
        {
            // Arrange
            UserSettings originalSettings = new UserSettings
            {
                SoundEffectsEnabled = false,
                MusicEnabled = false,
                ColorTheme = ColorTheme.Dark,
                MasterVolume = 0.5f,
                ShowGhostPiece = false,
                AnimationMode = AnimationMode.Minimal
            };
            await _settingsService.SaveSettingsAsync(originalSettings);

            // Act
            UserSettings loadedSettings = await _settingsService.LoadSettingsAsync();

            // Assert
            Assert.False(loadedSettings.SoundEffectsEnabled);
            Assert.False(loadedSettings.MusicEnabled);
            Assert.Equal(ColorTheme.Dark, loadedSettings.ColorTheme);
            Assert.Equal(0.5f, loadedSettings.MasterVolume);
            Assert.False(loadedSettings.ShowGhostPiece);
            Assert.Equal(AnimationMode.Minimal, loadedSettings.AnimationMode);
        }

        [Fact]
        public async Task LoadSettingsAsync_CorruptedFile_ReturnsDefaultSettings()
        {
            // Arrange
            string settingsPath = Path.Combine(_testSettingsDirectory, "settings.json");
            await File.WriteAllTextAsync(settingsPath, "invalid json content");

            // Act
            UserSettings settings = await _settingsService.LoadSettingsAsync();

            // Assert
            Assert.NotNull(settings);
            Assert.True(settings.SoundEffectsEnabled); // Should use defaults
            Assert.True(settings.MusicEnabled);
            Assert.Equal(ColorTheme.Classic, settings.ColorTheme);
        }

        #endregion

        #region SaveSettingsAsync Tests

        [Fact]
        public async Task SaveSettingsAsync_ValidSettings_CreatesFile()
        {
            // Arrange
            UserSettings settings = new UserSettings
            {
                SoundEffectsEnabled = false,
                MusicEnabled = true,
                ColorTheme = ColorTheme.Neon,
                MasterVolume = 0.7f
            };

            // Act
            await _settingsService.SaveSettingsAsync(settings);

            // Assert
            string settingsPath = Path.Combine(_testSettingsDirectory, "settings.json");
            Assert.True(File.Exists(settingsPath));

            // Verify content by loading again
            UserSettings loadedSettings = await _settingsService.LoadSettingsAsync();
            Assert.False(loadedSettings.SoundEffectsEnabled);
            Assert.True(loadedSettings.MusicEnabled);
            Assert.Equal(ColorTheme.Neon, loadedSettings.ColorTheme);
            Assert.Equal(0.7f, loadedSettings.MasterVolume);
        }

        [Fact]
        public async Task SaveSettingsAsync_NullSettings_ThrowsArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => _settingsService.SaveSettingsAsync(null!));
        }

        [Fact]
        public async Task SaveSettingsAsync_InvalidVolume_ThrowsArgumentException()
        {
            // Arrange
            UserSettings settings = new UserSettings
            {
                MasterVolume = 1.5f // Invalid volume > 1.0
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(
                () => _settingsService.SaveSettingsAsync(settings));
        }

        #endregion

        #region UpdateSettingAsync Tests

        [Fact]
        public async Task UpdateSettingAsync_ValidUpdate_UpdatesAndSaves()
        {
            // Arrange
            bool eventRaised = false;
            _settingsService.SettingsUpdated += (sender, args) => eventRaised = true;

            // Act
            await _settingsService.UpdateSettingAsync(settings => settings.SoundEffectsEnabled = false);

            // Assert
            Assert.False(_settingsService.CurrentSettings.SoundEffectsEnabled);
            Assert.True(eventRaised);

            // Verify persistence
            UserSettings loadedSettings = await _settingsService.LoadSettingsAsync();
            Assert.False(loadedSettings.SoundEffectsEnabled);
        }

        [Fact]
        public async Task UpdateSettingAsync_NullAction_ThrowsArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => _settingsService.UpdateSettingAsync(null!));
        }

        #endregion

        #region Validation Tests

        [Fact]
        public void ValidateSettings_InvalidMasterVolume_ReturnsFalse()
        {
            // Arrange
            UserSettings settings = new UserSettings
            {
                MasterVolume = -0.5f // Invalid negative volume
            };

            // Act
            bool isValid = _settingsService.ValidateSettings(settings);

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void IsFeatureEnabled_ValidFeatures_ReturnsCorrectValues()
        {
            // Arrange
            UserSettings settings = new UserSettings
            {
                SoundEffectsEnabled = true,
                MusicEnabled = false,
                ShowGhostPiece = true
            };

            // Create service with these settings
            UserSettingsService service = new UserSettingsService(_testSettingsDirectory);
            service.GetType()
                .GetField("_currentSettings", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?
                .SetValue(service, settings);

            // Act & Assert
            Assert.True(service.IsFeatureEnabled(SettingsFeature.SoundEffects));
            Assert.False(service.IsFeatureEnabled(SettingsFeature.Music));
            Assert.True(service.IsFeatureEnabled(SettingsFeature.GhostPiece));
        }

        #endregion

        #region Control Settings Tests

        [Fact]
        public async Task GetKeyForAction_ValidAction_ReturnsCorrectKey()
        {
            // Arrange
            UserSettings settings = new UserSettings();
            settings.ControlSettings.KeyMappings[GameAction.MoveLeft] = ConsoleKey.A;

            await _settingsService.SaveSettingsAsync(settings);

            // Act
            ConsoleKey key = _settingsService.GetKeyForAction(GameAction.MoveLeft);

            // Assert
            Assert.Equal(ConsoleKey.A, key);
        }

        [Fact]
        public void GetKeyForAction_InvalidAction_ReturnsDefault()
        {
            // Act
            ConsoleKey key = _settingsService.GetKeyForAction((GameAction)999);

            // Assert
            Assert.Equal(ConsoleKey.Escape, key); // Default fallback
        }

        #endregion

        #region Event Tests

        [Fact]
        public async Task SettingsUpdated_WhenSettingsChange_EventRaised()
        {
            // Arrange
            bool eventRaised = false;
            SettingsUpdatedEventArgs? eventArgs = null;

            _settingsService.SettingsUpdated += (sender, args) =>
            {
                eventRaised = true;
                eventArgs = args;
            };

            // Act
            await _settingsService.UpdateSettingAsync(settings => settings.ColorTheme = ColorTheme.Dark);

            // Assert
            Assert.True(eventRaised);
            Assert.NotNull(eventArgs);
            Assert.Equal(ColorTheme.Dark, eventArgs.UpdatedSettings.ColorTheme);
        }

        #endregion

        #region Integration Tests

        [Fact]
        public async Task FullSettingsWorkflow_SaveLoadUpdate_WorksCorrectly()
        {
            // Arrange
            UserSettings originalSettings = new UserSettings
            {
                SoundEffectsEnabled = false,
                MusicEnabled = true,
                ColorTheme = ColorTheme.HighContrast,
                MasterVolume = 0.6f,
                ShowGhostPiece = false,
                AnimationMode = AnimationMode.Enhanced
            };

            // Act & Assert - Save
            await _settingsService.SaveSettingsAsync(originalSettings);

            // Act & Assert - Load
            UserSettings loadedSettings = await _settingsService.LoadSettingsAsync();
            Assert.False(loadedSettings.SoundEffectsEnabled);
            Assert.True(loadedSettings.MusicEnabled);
            Assert.Equal(ColorTheme.HighContrast, loadedSettings.ColorTheme);
            Assert.Equal(0.6f, loadedSettings.MasterVolume);

            // Act & Assert - Update
            await _settingsService.UpdateSettingAsync(settings => 
            {
                settings.SoundEffectsEnabled = true;
                settings.ColorTheme = ColorTheme.Monochrome;
            });

            UserSettings updatedSettings = _settingsService.CurrentSettings;
            Assert.True(updatedSettings.SoundEffectsEnabled);
            Assert.Equal(ColorTheme.Monochrome, updatedSettings.ColorTheme);
            Assert.True(updatedSettings.MusicEnabled); // Should remain unchanged
        }

        #endregion
    }
}
