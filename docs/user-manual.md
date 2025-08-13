# Tetris Game - User Manual

Welcome to the classic Tetris game! This comprehensive guide will help you master all features and gameplay mechanics.

## Table of Contents

1. [Getting Started](#getting-started)
2. [Game Controls](#game-controls)
3. [Game Modes](#game-modes)
4. [Difficulty Levels](#difficulty-levels)
5. [Scoring System](#scoring-system)
6. [Tetromino Pieces](#tetromino-pieces)
7. [Game Features](#game-features)
8. [Settings and Customization](#settings-and-customization)
9. [Statistics and Progress](#statistics-and-progress)
10. [Save and Load System](#save-and-load-system)
11. [Tips and Strategies](#tips-and-strategies)
12. [Troubleshooting](#troubleshooting)

## Getting Started

### Starting the Game

1. **Launch the Application**: Run `Tetris.Console.Responsive.exe` (Windows) or `./Tetris.Console.Responsive` (Linux/macOS)
2. **Main Menu**: Use the arrow keys and Enter to navigate the menu
3. **New Game**: Select "New Game" to start playing immediately
4. **Load Game**: Select "Load Game" to continue a previously saved session

### System Requirements

- **Operating System**: Windows 10+, macOS 10.15+, or Linux
- **Memory**: 100 MB RAM minimum
- **Storage**: 50 MB available space
- **Display**: Terminal supporting UTF-8 encoding and colors

## Game Controls

### Default Control Scheme

| Action | Default Keys | Alternative Keys | Description |
|--------|-------------|------------------|-------------|
| **Move Left** | `A` | `←` (Left Arrow) | Move falling piece left |
| **Move Right** | `D` | `→` (Right Arrow) | Move falling piece right |
| **Rotate Clockwise** | `W` | `↑` (Up Arrow) | Rotate piece 90° clockwise |
| **Rotate Counter-Clockwise** | `Q` | `Shift + ↑` | Rotate piece 90° counter-clockwise |
| **Soft Drop** | `S` | `↓` (Down Arrow) | Accelerate piece falling |
| **Hard Drop** | `Space` | `Enter` | Instantly drop piece to bottom |
| **Pause/Resume** | `P` | `Esc` | Pause or resume the game |
| **Quit to Menu** | `Escape` | `Q` (in pause) | Return to main menu |

### Control Tips

- **Soft Drop**: Hold down the soft drop key to make pieces fall faster while maintaining control
- **Hard Drop**: Use hard drop to instantly place a piece at the bottom position
- **Rotation**: Try both rotation directions - sometimes counter-clockwise is easier for certain moves
- **Ghost Piece**: The translucent preview shows where your piece will land

## Game Modes

### 🎮 Classic Mode
**Objective**: Play as long as possible and achieve the highest score

- **Gameplay**: Pieces fall continuously with increasing speed
- **Level Progression**: Advance levels every 10 cleared rows
- **Speed Increase**: Each level makes pieces fall faster
- **End Condition**: Game ends when pieces reach the top
- **Best For**: Traditional Tetris experience

### ⏱️ Timed Mode
**Objective**: Score as many points as possible within a time limit

- **Time Limits**:
  - Easy: 3 minutes (180 seconds)
  - Medium: 2 minutes (120 seconds)
  - Hard: 1.5 minutes (90 seconds)
- **Strategy Focus**: Maximize scoring efficiency
- **Speed**: Consistent falling speed throughout
- **End Condition**: Timer reaches zero
- **Best For**: Quick, intense gameplay sessions

### 🎯 Challenge Mode
**Objective**: Clear a specific number of rows to win

- **Row Targets**:
  - Easy: 20 rows
  - Medium: 40 rows
  - Hard: 60 rows
- **Victory Condition**: Clear the target number of rows
- **Progress Tracking**: See remaining rows in the interface
- **Strategy Focus**: Consistent line clearing
- **Best For**: Goal-oriented gameplay

## Difficulty Levels

### 🟢 Easy
- **Initial Speed**: Slower falling (1200ms delay)
- **Speed Progression**: Gradual increase (40ms reduction per level)
- **Minimum Speed**: Not too fast (150ms minimum)
- **Score Multiplier**: 1.0x (standard scoring)
- **Best For**: Beginners and casual play

### 🟡 Medium
- **Initial Speed**: Standard falling (1000ms delay)
- **Speed Progression**: Normal increase (50ms reduction per level)
- **Minimum Speed**: Fast but manageable (100ms minimum)
- **Score Multiplier**: 1.5x (50% bonus)
- **Best For**: Intermediate players

### 🔴 Hard
- **Initial Speed**: Fast falling (800ms delay)
- **Speed Progression**: Rapid increase (60ms reduction per level)
- **Minimum Speed**: Very fast (80ms minimum)
- **Score Multiplier**: 2.0x (double points)
- **Best For**: Expert players seeking challenge

## Scoring System

### Base Scoring

| Lines Cleared | Base Points | Special Name |
|--------------|-------------|--------------|
| 1 line | 100 points | Single |
| 2 lines | 300 points | Double |
| 3 lines | 500 points | Triple |
| 4 lines | 800 points | **Tetris** |

### Score Calculation Formula
```
Final Score = Base Points × Level × Difficulty Multiplier
```

### Scoring Examples

**Example 1 - Single line clear on Level 3 (Medium difficulty)**:
- Base Points: 100
- Level Multiplier: 3
- Difficulty Multiplier: 1.5
- **Final Score: 100 × 3 × 1.5 = 450 points**

**Example 2 - Tetris on Level 5 (Hard difficulty)**:
- Base Points: 800
- Level Multiplier: 5
- Difficulty Multiplier: 2.0
- **Final Score: 800 × 5 × 2.0 = 8,000 points**

### Bonus Points

- **Soft Drop**: +1 point per cell dropped
- **Hard Drop**: +2 points per cell dropped
- **Level Completion**: +100 points × level number

## Tetromino Pieces

Learn the seven classic Tetris pieces and their rotation patterns:

### I-Piece (Cyan) - "Line Piece"
```
████  or  █
          █
          █
          █
```
- **Special**: Only piece that can clear 4 lines at once (Tetris)
- **Strategy**: Save for clearing multiple lines simultaneously
- **Rotation**: 2 states (horizontal/vertical)

### O-Piece (Yellow) - "Square"
```
██
██
```
- **Special**: Doesn't rotate (always same shape)
- **Strategy**: Useful for filling gaps and building stable foundations
- **Rotation**: 1 state (no rotation)

### T-Piece (Purple) - "T-Shape"
```
 █   █     █    █
███  ██   ███   ██
     █          █
```
- **Special**: Central piece in many strategies
- **Strategy**: Versatile for creating T-spin setups
- **Rotation**: 4 states

### S-Piece (Green) - "S-Shape"
```
 ██   █
██    ██
       █
```
- **Strategy**: Creates curves, can be tricky to place
- **Tip**: Plan ahead to avoid creating difficult gaps
- **Rotation**: 2 states

### Z-Piece (Red) - "Z-Shape"
```
██     █
 ██   ██
      █
```
- **Strategy**: Mirror of S-piece, similar placement challenges
- **Tip**: Use with S-pieces to create balanced structures
- **Rotation**: 2 states

### J-Piece (Blue) - "Reverse L"
```
█     ██   ███   █
███   █      █  ██
      █
```
- **Strategy**: Good for corner placements and clearing edges
- **Rotation**: 4 states

### L-Piece (Orange) - "L-Shape"
```
  █   █    ███  ██
███   █      █   █
      ██         █
```
- **Strategy**: Mirror of J-piece, complements J-piece placements
- **Rotation**: 4 states

## Game Features

### 👻 Ghost Piece
- **Description**: Translucent preview showing where the active piece will land
- **Purpose**: Helps plan piece placement and timing
- **Toggle**: Can be enabled/disabled in settings
- **Visual**: Shows in dimmed color at landing position

### ⏸️ Pause System
- **Activation**: Press `P` or `Esc` during gameplay
- **Features**: 
  - Game timer stops
  - Pieces freeze in position
  - Clear pause overlay display
  - Resume with same key
- **Tip**: Use strategically to plan complex moves

### 📊 Real-time Statistics
- **Current Score**: Updated immediately
- **Level**: Shows current level and progress to next
- **Lines Cleared**: Total rows cleared this session
- **Next Piece**: Preview of upcoming tetromino
- **Time Played**: Duration of current session

### 🎨 Visual Feedback
- **Line Clear Animation**: Special effects when clearing rows
- **Level Up**: Visual celebration when advancing levels
- **Game Over**: Clear game over screen with final statistics
- **Piece Placement**: Subtle feedback when pieces lock in place

## Settings and Customization

### Control Settings

#### Predefined Schemes
1. **WASD Scheme** (Default)
   - W: Rotate Clockwise
   - A: Move Left
   - S: Soft Drop
   - D: Move Right
   - Q: Rotate Counter-Clockwise
   - Space: Hard Drop

2. **Arrow Keys Scheme**
   - ↑: Rotate Clockwise
   - ←: Move Left
   - ↓: Soft Drop
   - →: Move Right
   - Shift+↑: Rotate Counter-Clockwise
   - Enter: Hard Drop

3. **Custom Scheme**
   - Define your own key mappings
   - All actions can be remapped
   - Prevents duplicate key assignments

#### Key Mapping Options
- **Move Left/Right**: Any letter key or arrow
- **Rotate**: Any key combination
- **Drop Actions**: Space, Enter, or custom keys
- **System Actions**: Pause, quit, menu navigation

### Audio Settings

#### Sound Effects
- **Master Volume**: 0-100% overall volume control
- **Sound Effects**: Enable/disable game sound effects
  - Piece movement sounds
  - Line clear effects
  - Level up notifications
  - Game over sound
- **Individual Volume**: Separate control for effects

#### Music Settings
- **Background Music**: Enable/disable game music
- **Music Volume**: 0-100% music-specific volume
- **Music Tracks**: Different tracks for different game states
  - Menu music
  - Gameplay music
  - Game over music

### Visual Settings

#### Color Themes
1. **Classic Theme**
   - Traditional Tetris colors
   - High contrast for visibility
   - Nostalgic appearance

2. **Dark Theme**
   - Dark background with bright pieces
   - Reduced eye strain
   - Modern appearance

3. **High Contrast Theme**
   - Maximum visibility
   - Accessibility optimized
   - Clear piece differentiation

4. **Neon Theme**
   - Bright, glowing colors
   - Vibrant appearance
   - Eye-catching design

5. **Monochrome Theme**
   - Black and white only
   - Pattern-based piece differentiation
   - Minimalist design

#### Animation Settings
- **None**: Disable all animations for maximum performance
- **Minimal**: Basic animations only
- **Normal**: Standard animation level (default)
- **Enhanced**: Full animations with extra effects

#### Display Options
- **Show Ghost Piece**: Toggle piece landing preview
- **Show Grid**: Toggle board grid lines
- **Particle Effects**: Enable/disable special effects
- **Screen Adaptation**: Automatic sizing for different terminals

### Gameplay Settings

#### Auto-Save
- **Enable/Disable**: Automatic save functionality
- **Save Frequency**: How often the game saves automatically
- **Quick Save**: Manual save option during gameplay

#### Default Preferences
- **Default Difficulty**: Starting difficulty for new games
- **Default Game Mode**: Preferred game mode
- **Starting Level**: Begin at specific level (Classic mode)

## Statistics and Progress

### Game Statistics

#### Current Session
- **Score**: Points earned this game
- **Lines Cleared**: Rows completed this session
- **Level**: Current difficulty level
- **Time Played**: Duration of current game
- **Pieces Placed**: Number of tetrominoes used

#### All-Time Statistics
- **Highest Score**: Personal best score achieved
- **Total Games Played**: Lifetime game count
- **Total Lines Cleared**: Cumulative rows cleared
- **Total Time Played**: Combined gameplay duration
- **Average Score**: Mean score across all games

#### Advanced Statistics
- **Games Won**: Successful Challenge/Timed mode completions
- **Win Percentage**: Success rate in goal-based modes
- **Best Time**: Fastest Challenge mode completion
- **Lines Per Minute**: Efficiency metric
- **Favorite Difficulty**: Most-played difficulty level
- **Favorite Game Mode**: Most-used game mode

### Statistics Reset
- **Individual Reset**: Reset specific statistics
- **Complete Reset**: Clear all statistics and start fresh
- **Confirmation**: Prevents accidental resets
- **Backup**: Statistics backed up before reset

## Save and Load System

### Automatic Saving

#### Quick Save
- **When**: Automatically when pausing or exiting
- **What**: Complete game state including:
  - Current board configuration
  - Active and next pieces
  - Score and statistics
  - Game mode and settings
  - Time remaining (Timed mode)
  - Progress (Challenge mode)

#### Auto-Save Frequency
- **Every 30 seconds**: During active gameplay
- **Before level advancement**: Preserve progress
- **After significant events**: Line clears, high scores
- **Configurable**: Can adjust frequency in settings

### Manual Save System

#### Save Slots
- **Multiple Saves**: Up to 10 named save slots
- **Custom Names**: Descriptive names for each save
- **Save Preview**: Shows game state summary
- **Overwrite Protection**: Confirmation before replacing saves

#### Save File Information
Each save file contains:
- **Date and Time**: When the game was saved
- **Game Mode**: Classic, Timed, or Challenge
- **Difficulty Level**: Easy, Medium, or Hard
- **Current Score**: Points earned
- **Level**: Current game level
- **Lines Cleared**: Progress information
- **Board State**: Exact piece positions
- **Next Pieces**: Upcoming tetromino queue

### Loading Games

#### Load Options
- **Continue Last Game**: Resume most recent session
- **Load Specific Save**: Choose from saved games list
- **Quick Load**: Shortcut key during gameplay
- **Load from Menu**: Full save management interface

#### Load Verification
- **Compatibility Check**: Ensures save file is valid
- **Version Check**: Handles saves from different game versions
- **Corruption Detection**: Identifies damaged save files
- **Recovery Mode**: Attempts to restore corrupted saves

## Tips and Strategies

### Beginner Tips

#### Basic Strategy
1. **Keep the Stack Low**: Always clear lines to prevent stacking too high
2. **Plan Ahead**: Look at the next piece preview and plan placement
3. **Use the Ghost Piece**: The translucent preview helps with accurate placement
4. **Learn Piece Rotations**: Practice rotating pieces to fit different gaps
5. **Start with Easy**: Build confidence before increasing difficulty

#### Fundamental Techniques
- **Flat Top**: Keep the top of your stack relatively flat
- **No Gaps**: Avoid creating holes that are hard to fill
- **Side Clearing**: Clear lines from left or right edges first
- **T-Piece Versatility**: T-pieces can fill many different gap types

### Intermediate Strategies

#### Stack Management
- **Well Strategy**: Create a column on one side for I-pieces
- **Pyramid Building**: Build higher in the center, lower on sides
- **Gap Planning**: Create intentional gaps for specific pieces
- **Emergency Clearing**: Know how to clear lines quickly when stacking high

#### Scoring Optimization
- **Tetris Focus**: Prioritize 4-line clears for maximum points
- **Level Timing**: Plan line clears to maximize level bonus
- **Combo Building**: Clear multiple lines in succession
- **Efficiency**: Balance speed with accuracy for optimal scoring

### Advanced Techniques

#### T-Spins
- **Setup**: Create T-shaped slots for T-pieces
- **Recognition**: Identify T-spin opportunities
- **Execution**: Rotate T-pieces into slots for bonus points
- **Types**: Learn different T-spin setups (TST, TSD, TSS)

#### Speed Techniques
- **Finesse**: Optimal key sequences for piece placement
- **Lookahead**: Plan multiple pieces in advance
- **Fast Stacking**: Build quickly while maintaining structure
- **Recovery**: Quickly fix dangerous stack situations

#### Mode-Specific Strategies

**Classic Mode**:
- Focus on consistent line clearing
- Build sustainable stacking patterns
- Prepare for increasing speed
- Plan for long-term survival

**Timed Mode**:
- Prioritize high-scoring moves
- Take calculated risks for big points
- Use hard drops for speed
- Focus on Tetrises and multi-line clears

**Challenge Mode**:
- Consistent, steady line clearing
- Avoid risky moves that might cause game over
- Build stable, clearable patterns
- Focus on efficiency over high scores

## Troubleshooting

### Common Issues

#### Performance Issues

**Problem**: Game runs slowly or choppy
**Solutions**:
- Reduce animation settings to "Minimal" or "None"
- Close other applications to free memory
- Use a smaller terminal window
- Disable particle effects in visual settings

**Problem**: High memory usage
**Solutions**:
- Restart the game periodically during long sessions
- Clear old save files
- Disable auto-save if experiencing issues
- Check for system memory leaks

#### Display Issues

**Problem**: Colors not showing correctly
**Solutions**:
- Use Windows Terminal or PowerShell Core on Windows
- Verify terminal supports 256-color mode
- Try different color themes in settings
- Check terminal color settings

**Problem**: Characters not displaying properly
**Solutions**:
- Ensure terminal supports UTF-8 encoding
- Install required fonts for Unicode characters
- Try different terminal applications
- Check system locale settings

#### Control Issues

**Problem**: Keys not responding
**Solutions**:
- Check if game is paused
- Verify key mappings in settings
- Try default control scheme
- Restart the game application

**Problem**: Delayed input response
**Solutions**:
- Check system performance
- Close background applications
- Reduce graphics settings
- Use wired keyboard for gaming

#### Save/Load Issues

**Problem**: Cannot save game
**Solutions**:
- Check file system permissions
- Verify available disk space
- Try different save location
- Run game as administrator (Windows)

**Problem**: Save files corrupted
**Solutions**:
- Use backup saves if available
- Try loading different save slot
- Reset to factory settings if needed
- Check disk for errors

### Getting Help

#### In-Game Help
- **F1 Key**: Quick help overlay (if available)
- **Settings Menu**: Access help sections
- **Controls Screen**: Review key mappings

#### External Resources
- **GitHub Repository**: https://github.com/kcr-83/Tetris
- **Issue Tracker**: Report bugs and request features
- **Community**: Share strategies and get help
- **Documentation**: This manual and technical docs

#### Reporting Issues
When reporting problems, include:
- Operating system and version
- Game version information
- Steps to reproduce the issue
- Error messages (if any)
- Screenshots of the problem (if applicable)

---

## Quick Reference Card

### Essential Controls
| Action | Key | Description |
|--------|-----|-------------|
| Move | A/D | Left/Right movement |
| Rotate | W/Q | Clockwise/Counter-clockwise |
| Drop | S/Space | Soft/Hard drop |
| Pause | P | Pause/Resume game |

### Game Modes
- **Classic**: Endless play until game over
- **Timed**: Score within time limit
- **Challenge**: Clear target number of rows

### Scoring Quick Reference
- 1 line = 100 pts, 2 lines = 300 pts
- 3 lines = 500 pts, 4 lines (Tetris) = 800 pts
- Score = Base × Level × Difficulty multiplier

### Difficulty Multipliers
- Easy: 1.0x, Medium: 1.5x, Hard: 2.0x

---

*Enjoy playing Tetris! Remember, practice makes perfect. Start with easier difficulties and work your way up to become a Tetris master!*

**Version**: 1.0.0  
**Last Updated**: August 13, 2025
