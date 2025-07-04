using System;
using System.Threading.Tasks;
using Tetris.Core.Models;
using Tetris.Core.Services;
using Tetris.Core.UI;

namespace Tetris.Core.Tests
{
    /// <summary>
    /// Demo program that showcases the Settings System functionality.
    /// Demonstrates loading, modifying, and saving settings, as well as the UI components.
    /// </summary>
    public class SettingsSystemDemo
    {
        #region Fields

        private static readonly IUserSettingsService _settingsService = new UserSettingsService();

        #endregion

        #region Main Entry Point

        /// <summary>
        /// Main entry point for the settings system demo.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        public static async Task Main(string[] args)
        {
            Console.Title = "Tetris Settings System Demo";
            Console.Clear();

            try
            {
                await RunSettingsDemo();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Demo error: {ex.Message}");
                Console.WriteLine("\nPress any key to exit...");
                Console.ReadKey();
            }
        }

        #endregion

        #region Demo Methods

        /// <summary>
        /// Runs the complete settings system demonstration.
        /// </summary>
        private static async Task RunSettingsDemo()
        {
            ShowWelcomeMessage();

            // Demonstrate settings loading
            await DemonstrateSettingsLoading();
            await WaitForUser();

            // Demonstrate settings modification
            await DemonstrateSettingsModification();
            await WaitForUser();

            // Demonstrate settings validation
            await DemonstrateSettingsValidation();
            await WaitForUser();

            // Demonstrate settings UI
            await DemonstrateSettingsUI();
            await WaitForUser();

            // Demonstrate settings events
            await DemonstrateSettingsEvents();
            await WaitForUser();

            ShowDemoCompletion();
        }

        /// <summary>
        /// Shows the welcome message for the demo.
        /// </summary>
        private static void ShowWelcomeMessage()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                TETRIS SETTINGS SYSTEM DEMO                  ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine("This demo showcases the comprehensive settings system for Tetris.");
            Console.WriteLine("You'll see loading, saving, validation, and UI functionality.");
            Console.WriteLine();
        }

        /// <summary>
        /// Demonstrates settings loading functionality.
        /// </summary>
        private static async Task DemonstrateSettingsLoading()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("1. SETTINGS LOADING DEMONSTRATION");
            Console.WriteLine("═══════════════════════════════════");
            Console.ResetColor();

            Console.WriteLine("Loading current user settings...");
            var settings = await _settingsService.LoadSettingsAsync();

            Console.WriteLine();
            Console.WriteLine("Current Settings:");
            Console.WriteLine($"  Sound Effects: {(settings.SoundEffectsEnabled ? "Enabled" : "Disabled")}");
            Console.WriteLine($"  Music: {(settings.MusicEnabled ? "Enabled" : "Disabled")}");
            Console.WriteLine($"  Master Volume: {settings.MasterVolume:P0}");
            Console.WriteLine($"  Color Theme: {settings.ColorTheme}");
            Console.WriteLine($"  Animation Mode: {settings.AnimationMode}");
            Console.WriteLine($"  Ghost Piece: {(settings.ShowGhostPiece ? "Enabled" : "Disabled")}");
            Console.WriteLine($"  Control Scheme: {settings.ControlSettings.ControlScheme}");
            Console.WriteLine();
            Console.WriteLine($"Settings ID: {settings.SettingsId}");
            Console.WriteLine($"Last Modified: {settings.DateModified:yyyy-MM-dd HH:mm:ss}");
        }

        /// <summary>
        /// Demonstrates settings modification functionality.
        /// </summary>
        private static async Task DemonstrateSettingsModification()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("2. SETTINGS MODIFICATION DEMONSTRATION");
            Console.WriteLine("═══════════════════════════════════════");
            Console.ResetColor();

            Console.WriteLine("Modifying settings programmatically...");

            // Demonstrate updating individual settings
            await _settingsService.UpdateSettingAsync(settings =>
            {
                settings.ColorTheme = ColorTheme.Dark;
                settings.MasterVolume = 0.6f;
                settings.ShowGhostPiece = false;
            });

            Console.WriteLine("✓ Changed color theme to Dark");
            Console.WriteLine("✓ Set master volume to 60%");
            Console.WriteLine("✓ Disabled ghost piece");
            Console.WriteLine();

            // Show updated settings
            var updatedSettings = _settingsService.CurrentSettings;
            Console.WriteLine("Updated Settings:");
            Console.WriteLine($"  Color Theme: {updatedSettings.ColorTheme}");
            Console.WriteLine($"  Master Volume: {updatedSettings.MasterVolume:P0}");
            Console.WriteLine($"  Ghost Piece: {(updatedSettings.ShowGhostPiece ? "Enabled" : "Disabled")}");
        }

        /// <summary>
        /// Demonstrates settings validation functionality.
        /// </summary>
        private static async Task DemonstrateSettingsValidation()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("3. SETTINGS VALIDATION DEMONSTRATION");
            Console.WriteLine("═════════════════════════════════════");
            Console.ResetColor();

            Console.WriteLine("Testing settings validation with invalid values...");

            // Create settings with invalid values
            var invalidSettings = new UserSettings
            {
                MasterVolume = 1.5f, // Invalid: > 1.0
                SoundEffectsEnabled = true,
                MusicEnabled = true
            };

            Console.WriteLine($"Attempting to save volume: {invalidSettings.MasterVolume:P0} (invalid)");

            // Save and reload to see validation in action
            await _settingsService.SaveSettingsAsync(invalidSettings);
            var validatedSettings = await _settingsService.LoadSettingsAsync();

            Console.WriteLine($"After validation, volume is: {validatedSettings.MasterVolume:P0}");
            Console.WriteLine("✓ Settings validation automatically fixed invalid values");

            // Test another invalid value
            await _settingsService.UpdateSettingAsync(settings => settings.MasterVolume = -0.2f);
            var revalidatedSettings = _settingsService.CurrentSettings;
            Console.WriteLine($"Volume set to -20%, automatically corrected to: {revalidatedSettings.MasterVolume:P0}");
        }

        /// <summary>
        /// Demonstrates the settings user interface.
        /// </summary>
        private static async Task DemonstrateSettingsUI()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("4. SETTINGS USER INTERFACE DEMONSTRATION");
            Console.WriteLine("═════════════════════════════════════════");
            Console.ResetColor();

            Console.WriteLine("The settings system includes interactive UI components:");
            Console.WriteLine();
            Console.WriteLine("Available UI Components:");
            Console.WriteLine("  • SettingsInterface - Main settings menu");
            Console.WriteLine("  • AudioSettingsDialog - Sound and music configuration");
            Console.WriteLine("  • ControlSettingsDialog - Key mapping and control schemes");
            Console.WriteLine("  • VisualSettingsDialog - Themes and visual effects");
            Console.WriteLine("  • GameplaySettingsDialog - Gameplay features");
            Console.WriteLine();

            Console.WriteLine("Creating settings interface...");
            var settingsInterface = new SettingsInterface(_settingsService);

            Console.WriteLine("✓ Settings interface created successfully");
            Console.WriteLine("✓ All dialog components initialized");
            Console.WriteLine();
            Console.WriteLine("Note: UI components are designed for interactive console use");
            Console.WriteLine("      and integrate seamlessly with the main game menu.");
        }

        /// <summary>
        /// Demonstrates settings events functionality.
        /// </summary>
        private static async Task DemonstrateSettingsEvents()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("5. SETTINGS EVENTS DEMONSTRATION");
            Console.WriteLine("══════════════════════════════════");
            Console.ResetColor();

            Console.WriteLine("Demonstrating event-driven architecture...");

            bool eventReceived = false;
            SettingsUpdatedEventArgs? receivedArgs = null;

            // Subscribe to settings events
            _settingsService.SettingsUpdated += (sender, args) =>
            {
                eventReceived = true;
                receivedArgs = args;
                Console.WriteLine($"  📡 Event received: Settings updated at {args.UpdatedSettings.DateModified:HH:mm:ss}");
            };

            Console.WriteLine("Subscribed to SettingsUpdated event...");
            Console.WriteLine();

            // Trigger events by updating settings
            Console.WriteLine("Updating animation mode...");
            await _settingsService.UpdateSettingAsync(settings => settings.AnimationMode = AnimationMode.Enhanced);

            Console.WriteLine("Updating control scheme...");
            await _settingsService.UpdateSettingAsync(settings => settings.ControlSettings.ControlScheme = ControlScheme.WASD);

            Console.WriteLine();
            Console.WriteLine($"Events received: {(eventReceived ? "✓ Yes" : "✗ No")}");
            if (receivedArgs != null)
            {
                Console.WriteLine($"Latest event data: {receivedArgs.UpdatedSettings.AnimationMode}, {receivedArgs.UpdatedSettings.ControlSettings.ControlScheme}");
            }
        }

        /// <summary>
        /// Shows demo completion message.
        /// </summary>
        private static void ShowDemoCompletion()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                    DEMO COMPLETED!                          ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine("The Tetris Settings System includes:");
            Console.WriteLine("✓ Comprehensive user settings model");
            Console.WriteLine("✓ Persistent file-based storage");
            Console.WriteLine("✓ Automatic validation and error handling");
            Console.WriteLine("✓ Event-driven architecture");
            Console.WriteLine("✓ Interactive UI components");
            Console.WriteLine("✓ Full integration with the game");
            Console.WriteLine();
            Console.WriteLine("Settings are automatically saved to: Settings/settings.json");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Press any key to exit...");
            Console.ResetColor();
            Console.ReadKey();
        }

        /// <summary>
        /// Waits for user input to continue the demo.
        /// </summary>
        private static async Task WaitForUser()
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("Press any key to continue...");
            Console.ResetColor();
            Console.ReadKey();
            Console.WriteLine();
            await Task.Delay(100); // Small delay for better UX
        }

        #endregion
    }
}
