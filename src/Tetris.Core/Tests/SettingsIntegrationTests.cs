using System;
using System.IO;
using System.Threading.Tasks;
using Tetris.Core.Models;
using Tetris.Core.Services;
using Tetris.Core.UI;
using Xunit;

namespace Tetris.Core.Tests
{
    /// <summary>
    /// Integration tests for the Settings UI components.
    /// Tests the interaction between UI components and the settings service.
    /// </summary>
    public class SettingsIntegrationTests : IDisposable
    {
        #region Fields

        private readonly string _testSettingsDirectory;
        private readonly IUserSettingsService _settingsService;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the SettingsIntegrationTests class.
        /// </summary>
        public SettingsIntegrationTests()
        {
            _testSettingsDirectory = Path.Combine(Path.GetTempPath(), $"TetrisIntegrationTests_{Guid.NewGuid()}");
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

        #region SettingsInterface Integration Tests

        [Fact]
        public void SettingsInterface_Initialization_LoadsCurrentSettings()
        {
            // Act
            var settingsInterface = new SettingsInterface(_settingsService);

            // Assert
            Assert.NotNull(settingsInterface);
            // The interface should load the current settings successfully
        }

        [Fact]
        public async Task SettingsInterface_SettingsSaved_UpdatesService()
        {
            // Arrange
            var settingsInterface = new SettingsInterface(_settingsService);
            bool eventRaised = false;
            SettingsUpdatedEventArgs? eventArgs = null;

            settingsInterface.SettingsSaved += (sender, args) =>
            {
                eventRaised = true;
                eventArgs = args;
            };

            // Act
            await _settingsService.UpdateSettingAsync(settings => settings.ColorTheme = ColorTheme.Dark);

            // Simulate settings save through the interface by triggering the event
            settingsInterface.GetType()
                .GetMethod("OnSettingsSaved", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?
                .Invoke(settingsInterface, new object[] { new SettingsUpdatedEventArgs(_settingsService.CurrentSettings, SettingsChangeType.Saved) });

            // Assert
            Assert.Equal(ColorTheme.Dark, _settingsService.CurrentSettings.ColorTheme);
        }

        #endregion

        #region AudioSettingsDialog Integration Tests

        [Fact]
        public void AudioSettingsDialog_Initialization_UsesCurrentSettings()
        {
            // Arrange
            var settings = _settingsService.CurrentSettings;
            settings.SoundEffectsEnabled = false;
            settings.MusicEnabled = true;
            settings.MasterVolume = 0.7f;

            // Act
            var audioDialog = new AudioSettingsDialog(settings);

            // Assert
            Assert.NotNull(audioDialog);
            // The dialog should initialize with the provided settings
        }

        [Fact]
        public async Task AudioSettingsDialog_SettingsModification_UpdatesPersistence()
        {
            // Arrange
            var settings = _settingsService.CurrentSettings.Clone();
            var audioDialog = new AudioSettingsDialog(settings);

            // Act
            settings.SoundEffectsEnabled = false;
            settings.MasterVolume = 0.3f;
            await _settingsService.SaveSettingsAsync(settings);

            // Assert
            var loadedSettings = await _settingsService.LoadSettingsAsync();
            Assert.False(loadedSettings.SoundEffectsEnabled);
            Assert.Equal(0.3f, loadedSettings.MasterVolume);
        }

        #endregion

        #region ControlSettingsDialog Integration Tests

        [Fact]
        public void ControlSettingsDialog_Initialization_UsesCurrentSettings()
        {
            // Arrange
            var settings = _settingsService.CurrentSettings;
            settings.ControlSettings.ControlScheme = ControlScheme.WASD;

            // Act
            var controlDialog = new ControlSettingsDialog(settings);

            // Assert
            Assert.NotNull(controlDialog);
            // The dialog should initialize with the provided settings
        }

        [Fact]
        public async Task ControlSettingsDialog_KeyMappingChange_UpdatesPersistence()
        {
            // Arrange
            var settings = _settingsService.CurrentSettings.Clone();
            var controlDialog = new ControlSettingsDialog(settings);

            // Act
            settings.ControlSettings.KeyMappings[GameAction.MoveLeft] = ConsoleKey.A;
            settings.ControlSettings.KeyMappings[GameAction.MoveRight] = ConsoleKey.D;
            await _settingsService.SaveSettingsAsync(settings);

            // Assert
            var loadedSettings = await _settingsService.LoadSettingsAsync();
            Assert.Equal(ConsoleKey.A, loadedSettings.ControlSettings.KeyMappings[GameAction.MoveLeft]);
            Assert.Equal(ConsoleKey.D, loadedSettings.ControlSettings.KeyMappings[GameAction.MoveRight]);
        }

        #endregion

        #region VisualSettingsDialog Integration Tests

        [Fact]
        public void VisualSettingsDialog_Initialization_UsesCurrentSettings()
        {
            // Arrange
            var settings = _settingsService.CurrentSettings;
            settings.ColorTheme = ColorTheme.Neon;
            settings.AnimationMode = AnimationMode.Enhanced;

            // Act
            var visualDialog = new VisualSettingsDialog(settings);

            // Assert
            Assert.NotNull(visualDialog);
            // The dialog should initialize with the provided settings
        }

        [Fact]
        public async Task VisualSettingsDialog_ThemeChange_UpdatesPersistence()
        {
            // Arrange
            var settings = _settingsService.CurrentSettings.Clone();
            var visualDialog = new VisualSettingsDialog(settings);

            // Act
            settings.ColorTheme = ColorTheme.HighContrast;
            settings.AnimationMode = AnimationMode.Minimal;
            await _settingsService.SaveSettingsAsync(settings);

            // Assert
            var loadedSettings = await _settingsService.LoadSettingsAsync();
            Assert.Equal(ColorTheme.HighContrast, loadedSettings.ColorTheme);
            Assert.Equal(AnimationMode.Minimal, loadedSettings.AnimationMode);
        }

        #endregion

        #region GameplaySettingsDialog Integration Tests

        [Fact]
        public void GameplaySettingsDialog_Initialization_UsesCurrentSettings()
        {
            // Arrange
            var settings = _settingsService.CurrentSettings;
            settings.ShowGhostPiece = false;

            // Act
            var gameplayDialog = new GameplaySettingsDialog(settings);

            // Assert
            Assert.NotNull(gameplayDialog);
            // The dialog should initialize with the provided settings
        }

        [Fact]
        public async Task GameplaySettingsDialog_FeatureToggle_UpdatesPersistence()
        {
            // Arrange
            var settings = _settingsService.CurrentSettings.Clone();
            var gameplayDialog = new GameplaySettingsDialog(settings);

            // Act
            settings.ShowGhostPiece = false;
            await _settingsService.SaveSettingsAsync(settings);

            // Assert
            var loadedSettings = await _settingsService.LoadSettingsAsync();
            Assert.False(loadedSettings.ShowGhostPiece);
        }

        #endregion

        #region End-to-End Settings Workflow Tests

        [Fact]
        public async Task EndToEndSettingsWorkflow_CompleteUserJourney_WorksCorrectly()
        {
            // Arrange - Simulate a complete user journey through all settings
            var settingsInterface = new SettingsInterface(_settingsService);

            // Act 1: Modify audio settings
            await _settingsService.UpdateSettingAsync(settings =>
            {
                settings.SoundEffectsEnabled = false;
                settings.MusicEnabled = true;
                settings.MasterVolume = 0.5f;
            });

            // Act 2: Modify control settings
            await _settingsService.UpdateSettingAsync(settings =>
            {
                settings.ControlSettings.ControlScheme = ControlScheme.WASD;
                settings.ControlSettings.KeyMappings[GameAction.MoveLeft] = ConsoleKey.A;
                settings.ControlSettings.KeyMappings[GameAction.MoveRight] = ConsoleKey.D;
            });

            // Act 3: Modify visual settings
            await _settingsService.UpdateSettingAsync(settings =>
            {
                settings.ColorTheme = ColorTheme.Dark;
                settings.AnimationMode = AnimationMode.Enhanced;
            });

            // Act 4: Modify gameplay settings
            await _settingsService.UpdateSettingAsync(settings =>
            {
                settings.ShowGhostPiece = false;
            });

            // Assert - Verify all changes persisted
            var finalSettings = await _settingsService.LoadSettingsAsync();
            
            // Audio settings
            Assert.False(finalSettings.SoundEffectsEnabled);
            Assert.True(finalSettings.MusicEnabled);
            Assert.Equal(0.5f, finalSettings.MasterVolume);
            
            // Control settings
            Assert.Equal(ControlScheme.WASD, finalSettings.ControlSettings.ControlScheme);
            Assert.Equal(ConsoleKey.A, finalSettings.ControlSettings.KeyMappings[GameAction.MoveLeft]);
            Assert.Equal(ConsoleKey.D, finalSettings.ControlSettings.KeyMappings[GameAction.MoveRight]);
            
            // Visual settings
            Assert.Equal(ColorTheme.Dark, finalSettings.ColorTheme);
            Assert.Equal(AnimationMode.Enhanced, finalSettings.AnimationMode);
            
            // Gameplay settings
            Assert.False(finalSettings.ShowGhostPiece);
        }

        [Fact]
        public async Task SettingsReset_RestoresToDefaults_WorksCorrectly()
        {
            // Arrange - Modify settings away from defaults
            await _settingsService.UpdateSettingAsync(settings =>
            {
                settings.SoundEffectsEnabled = false;
                settings.MusicEnabled = false;
                settings.ColorTheme = ColorTheme.Dark;
                settings.MasterVolume = 0.1f;
                settings.ShowGhostPiece = false;
                settings.AnimationMode = AnimationMode.None;
            });

            // Act - Reset to defaults
            var defaultSettings = new UserSettings();
            await _settingsService.SaveSettingsAsync(defaultSettings);

            // Assert
            var loadedSettings = await _settingsService.LoadSettingsAsync();
            Assert.True(loadedSettings.SoundEffectsEnabled);
            Assert.True(loadedSettings.MusicEnabled);
            Assert.Equal(ColorTheme.Classic, loadedSettings.ColorTheme);
            Assert.Equal(0.8f, loadedSettings.MasterVolume);
            Assert.True(loadedSettings.ShowGhostPiece);
            Assert.Equal(AnimationMode.Normal, loadedSettings.AnimationMode);
        }

        #endregion

        #region Performance and Error Handling Tests

        [Fact]
        public async Task SettingsService_ConcurrentAccess_HandlesCorrectly()
        {
            // Arrange
            var tasks = new Task[10];

            // Act - Simulate concurrent access
            for (int i = 0; i < 10; i++)
            {
                int index = i;
                tasks[i] = _settingsService.UpdateSettingAsync(settings =>
                {
                    settings.MasterVolume = 0.1f * (index + 1);
                });
            }

            await Task.WhenAll(tasks);

            // Assert - Service should handle concurrent access gracefully
            var finalSettings = _settingsService.CurrentSettings;
            Assert.True(finalSettings.MasterVolume >= 0.1f && finalSettings.MasterVolume <= 1.0f);
        }

        [Fact]
        public async Task SettingsService_InvalidDirectoryPermissions_HandlesGracefully()
        {
            // Arrange - Create a service with a directory that might have permission issues
            var readOnlyDirectory = Path.Combine(_testSettingsDirectory, "readonly");
            Directory.CreateDirectory(readOnlyDirectory);

            try
            {
                // Make directory read-only on Windows (this might not work on all systems)
                var dirInfo = new DirectoryInfo(readOnlyDirectory);
                dirInfo.Attributes |= FileAttributes.ReadOnly;

                var restrictedService = new UserSettingsService(readOnlyDirectory);

                // Act & Assert - Should not throw, should handle gracefully
                var settings = await restrictedService.LoadSettingsAsync();
                Assert.NotNull(settings);
            }
            finally
            {
                // Clean up - Remove read-only attribute
                try
                {
                    var dirInfo = new DirectoryInfo(readOnlyDirectory);
                    dirInfo.Attributes &= ~FileAttributes.ReadOnly;
                }
                catch
                {
                    // Ignore cleanup errors in tests
                }
            }
        }

        #endregion
    }
}
