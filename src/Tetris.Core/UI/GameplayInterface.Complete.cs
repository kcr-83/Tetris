using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Tetris.Core.Models;

namespace Tetris.Core.UI
{
    /// <summary>
    /// Improved implementation of GameplayInterface with better responsiveness and
    /// adaptive layout for different console window sizes.
    /// </summary>
    public partial class GameplayInterfaceComplete
    {
        #region Constants

        /// <summary>
        /// Width of the side panel in characters.
        /// </summary>
        private const int SidePanelWidth = 20;

        /// <summary>
        /// The character used to represent filled blocks on the board.
        /// </summary>
        private const string BlockCharacter = "██";

        /// <summary>
        /// The character used to represent empty spaces on the board.
        /// </summary>
        private const string EmptyCharacter = "··";
        
        /// <summary>
        /// The character used to represent filled blocks in compact mode.
        /// </summary>
        private const string CompactBlockCharacter = "█";
        
        /// <summary>
        /// The character used to represent empty spaces in compact mode.
        /// </summary>
        private const string CompactEmptyCharacter = "·";

        // Minimum requirements for the game display
        private const int MinimumWindowWidth = 60;
        private const int MinimumWindowHeight = 25;
        
        // Compact mode thresholds
        private const int CompactModeWidth = 50;
        private const int CompactModeHeight = 20;

        #endregion

        #region Fields

        private readonly GameEngine _gameEngine;
        private int _windowWidth;
        private int _windowHeight;
        private int _boardStartX;
        private int _boardStartY;
        private int _sidePanelStartX;
        private bool _initialized;
        private int _lastWindowWidth;
        private int _lastWindowHeight;
        private bool _isWindowTooSmall;
        private bool _isCompactMode;
        private int _lastFrameTime;
        private bool _useDoubleBuffering;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the title displayed at the top of the gameplay screen.
        /// </summary>
        public string GameTitle { get; set; } = "TETRIS";

        /// <summary>
        /// Gets or sets the color of the game title.
        /// </summary>
        public ConsoleColor TitleColor { get; set; } = ConsoleColor.Cyan;

        /// <summary>
        /// Gets or sets the color of the game board border.
        /// </summary>
        public ConsoleColor BorderColor { get; set; } = ConsoleColor.White;

        /// <summary>
        /// Gets or sets the color of the score and statistics text.
        /// </summary>
        public ConsoleColor StatsColor { get; set; } = ConsoleColor.Yellow;

        /// <summary>
        /// Gets or sets the color of the help text.
        /// </summary>
        public ConsoleColor HelpTextColor { get; set; } = ConsoleColor.Gray;

        /// <summary>
        /// Gets or sets the background color of the gameplay screen.
        /// </summary>
        public ConsoleColor BackgroundColor { get; set; } = ConsoleColor.Black;

        /// <summary>
        /// Gets or sets whether to show a ghost piece that indicates where the current piece will land.
        /// </summary>
        public bool ShowGhostPiece { get; set; } = true;

        /// <summary>
        /// Gets or sets the color of the ghost piece.
        /// </summary>
        public ConsoleColor GhostPieceColor { get; set; } = ConsoleColor.DarkGray;
        
        /// <summary>
        /// Gets or sets whether to enable double buffering for smoother rendering.
        /// </summary>
        public bool EnableDoubleBuffering
        {
            get => _useDoubleBuffering;
            set => _useDoubleBuffering = value;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the GameplayInterface class.
        /// </summary>
        /// <param name="gameEngine">The game engine to display.</param>
        public GameplayInterfaceComplete(GameEngine gameEngine)
        {
            _gameEngine = gameEngine ?? throw new ArgumentNullException(nameof(gameEngine));
            _initialized = false;
            _useDoubleBuffering = true;
            _lastFrameTime = Environment.TickCount;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Initializes the gameplay interface.
        /// </summary>
        public void Initialize()
        {
            Console.CursorVisible = false;
            Console.BackgroundColor = BackgroundColor;
            Console.Clear();

            // Calculate layout based on window size
            CalculateLayout();

            _initialized = true;
        }

        /// <summary>
        /// Renders the full gameplay screen, including the board, current piece, next piece preview,
        /// score, level, and other game information.
        /// </summary>
        public void Render()
        {
            if (!_initialized)
            {
                Initialize();
            }

            // Throttle rendering to maintain performance
            if (Environment.TickCount - _lastFrameTime < 16) // ~60 FPS max
            {
                return;
            }
            _lastFrameTime = Environment.TickCount;

            Console.BackgroundColor = BackgroundColor;
            
            // Check if we need to use compact mode due to window size constraints
            if (_isCompactMode)
            {
                RenderCompactMode();
                return;
            }
            
            // Render game title
            RenderTitle();
            
            // Create a copy of the board with the current piece added
            var boardWithPiece = new Board(_gameEngine.Board);
            
            // Add the current piece to the display board
            var currentPiece = _gameEngine.CurrentPiece;
            if (currentPiece != null)
            {
                foreach (var point in currentPiece.Blocks)
                {
                    int boardX = currentPiece.Position.X + point.X;
                    int boardY = currentPiece.Position.Y + point.Y;
                    
                    if (boardX >= 0 && boardX < Board.Width && boardY >= 0 && boardY < Board.Height)
                    {
                        boardWithPiece.Grid[boardX, boardY] = currentPiece.Id;
                    }
                }
                
                // Add the ghost piece to show where the piece will land
                if (ShowGhostPiece)
                {
                    AddGhostPiece(boardWithPiece, currentPiece);
                }
            }
            
            // Render the board
            RenderBoard(boardWithPiece);
            
            // Render the side panel with stats and next piece
            RenderSidePanel();
            
            // Render controls help at the bottom
            RenderControlsHelp();
        }

        /// <summary>
        /// Shows an overlay with a pause message.
        /// </summary>
        public void ShowPauseOverlay()
        {
            int centerX = Console.WindowWidth / 2 - 10;
            int centerY = Console.WindowHeight / 2;
            
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Yellow;
            
            Console.SetCursorPosition(centerX, centerY - 1);
            Console.WriteLine("┌──────────────┐");
            Console.SetCursorPosition(centerX, centerY);
            Console.WriteLine("│  GAME PAUSED │");
            Console.SetCursorPosition(centerX, centerY + 1);
            Console.WriteLine("└──────────────┘");
            
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        /// <summary>
        /// Shows a message indicating the window is too small.
        /// </summary>
        public void ShowWindowTooSmallMessage()
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Clear();
            
            Console.ForegroundColor = ConsoleColor.Red;
            
            int centerX = Math.Max(0, Console.WindowWidth / 2 - 15);
            int centerY = Math.Max(0, Console.WindowHeight / 2 - 2);
            
            Console.SetCursorPosition(centerX, centerY);
            Console.WriteLine("┌───────────────────────────┐");
            Console.SetCursorPosition(centerX, centerY + 1);
            Console.WriteLine("│   Window size too small   │");
            Console.SetCursorPosition(centerX, centerY + 2);
            Console.WriteLine("│ Please resize your window │");
            Console.SetCursorPosition(centerX, centerY + 3);
            Console.WriteLine("└───────────────────────────┘");
        }

        /// <summary>
        /// Shows an animation when the level increases.
        /// </summary>
        /// <param name="level">The new level.</param>
        /// <returns>A task representing the animation.</returns>
        public async Task ShowLevelUpAnimationAsync(int level)
        {
            int centerX = Console.WindowWidth / 2 - 10;
            int centerY = Console.WindowHeight / 2;
            
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Green;
            
            for (int i = 0; i < 3; i++)
            {
                Console.SetCursorPosition(centerX, centerY - 1);
                Console.WriteLine("┌──────────────┐");
                Console.SetCursorPosition(centerX, centerY);
                Console.WriteLine("│  LEVEL UP!   │");
                Console.SetCursorPosition(centerX, centerY + 1);
                Console.WriteLine($"│  LEVEL {level,-4} │");
                Console.SetCursorPosition(centerX, centerY + 2);
                Console.WriteLine("└──────────────┘");
                
                await Task.Delay(200);
                
                Console.SetCursorPosition(centerX, centerY - 1);
                Console.WriteLine("              ");
                Console.SetCursorPosition(centerX, centerY);
                Console.WriteLine("              ");
                Console.SetCursorPosition(centerX, centerY + 1);
                Console.WriteLine("              ");
                Console.SetCursorPosition(centerX, centerY + 2);
                Console.WriteLine("              ");
                
                await Task.Delay(100);
            }
            
            // Redraw the screen after animation
            Render();
        }

        /// <summary>
        /// Shows an animation when rows are cleared.
        /// </summary>
        /// <param name="rowsCleared">The number of rows cleared.</param>
        /// <param name="scoreGained">The score gained from clearing the rows.</param>
        /// <returns>A task representing the animation.</returns>
        public async Task ShowRowsClearedAnimationAsync(int rowsCleared, int scoreGained)
        {
            // Determine score text based on rows cleared
            string clearText = rowsCleared switch
            {
                1 => "SINGLE",
                2 => "DOUBLE",
                3 => "TRIPLE",
                4 => "TETRIS!",
                _ => $"{rowsCleared} ROWS"
            };
            
            int centerX = Console.WindowWidth / 2 - 7;
            int centerY = Math.Max(5, Console.WindowHeight / 4);
            
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = rowsCleared == 4 ? ConsoleColor.Magenta : ConsoleColor.Cyan;
            
            Console.SetCursorPosition(centerX, centerY);
            Console.WriteLine($"+{scoreGained,6} pts");
            Console.SetCursorPosition(centerX, centerY + 1);
            Console.WriteLine($"  {clearText}!");
            
            await Task.Delay(rowsCleared == 4 ? 600 : 300);
            
            // Clear the message
            Console.SetCursorPosition(centerX, centerY);
            Console.WriteLine("            ");
            Console.SetCursorPosition(centerX, centerY + 1);
            Console.WriteLine("            ");
            
            // Redraw the screen after animation
            Render();
        }

        /// <summary>
        /// Resizes the interface based on the current console window size.
        /// </summary>
        public void HandleResize()
        {
            int newWidth = Console.WindowWidth;
            int newHeight = Console.WindowHeight;
            
            // Only redraw if the size actually changed
            if (newWidth != _windowWidth || newHeight != _windowHeight)
            {
                Console.Clear();
                CalculateLayout();
                
                // Check if the window is too small for the game
                if (_isWindowTooSmall)
                {
                    ShowWindowTooSmallMessage();
                }
                else
                {
                    Render();
                }
            }
        }

        /// <summary>
        /// Checks if the console window has been resized and updates the layout if needed.
        /// </summary>
        /// <returns>True if the window was resized, false otherwise.</returns>
        public bool CheckForResize()
        {
            int currentWidth = Console.WindowWidth;
            int currentHeight = Console.WindowHeight;
            
            if (currentWidth != _lastWindowWidth || currentHeight != _lastWindowHeight)
            {
                // Update cached dimensions
                _lastWindowWidth = currentWidth;
                _lastWindowHeight = currentHeight;
                
                // Handle the resize
                HandleResize();
                return true;
            }
            
            return false;
        }
        
        /// <summary>
        /// Processes user input with improved responsiveness.
        /// </summary>
        /// <param name="key">The key that was pressed.</param>
        /// <returns>True if fast drop is active, false otherwise.</returns>
        public bool ProcessGameInput(ConsoleKeyInfo key)
        {
            bool fastDropActive = false;

            switch (key.Key)
            {
                case ConsoleKey.LeftArrow:
                case ConsoleKey.A:  // Alternative control
                    // Move piece left with smoother response
                    _gameEngine.MovePieceLeft();
                    break;

                case ConsoleKey.RightArrow:
                case ConsoleKey.D:  // Alternative control
                    // Move piece right with smoother response
                    _gameEngine.MovePieceRight();
                    break;

                case ConsoleKey.UpArrow:
                case ConsoleKey.W:  // Alternative control
                    // Rotate piece clockwise
                    _gameEngine.RotatePieceClockwise();
                    break;

                case ConsoleKey.DownArrow:
                case ConsoleKey.S:  // Alternative control
                    // Activate fast drop
                    _gameEngine.ActivateFastDrop();
                    fastDropActive = true;
                    break;

                case ConsoleKey.Spacebar:
                    // Hard drop
                    _gameEngine.HardDrop();
                    break;

                case ConsoleKey.Z:
                    // Alternative rotation (counter-clockwise) if implemented
                    if (_gameEngine.GetType().GetMethod("RotatePieceCounterClockwise") != null)
                    {
                        var method = _gameEngine.GetType().GetMethod("RotatePieceCounterClockwise");
                        method?.Invoke(_gameEngine, null);
                    }
                    break;

                case ConsoleKey.C:
                case ConsoleKey.X:  // Alternative hold key
                    // Hold piece functionality if implemented
                    if (_gameEngine.GetType().GetMethod("HoldCurrentPiece") != null)
                    {
                        var method = _gameEngine.GetType().GetMethod("HoldCurrentPiece");
                        method?.Invoke(_gameEngine, null);
                    }
                    break;
            }

            return fastDropActive;
        }

        #endregion

        #region Private Methods
        
        /// <summary>
        /// Calculates the layout of the interface elements based on the console window size.
        /// </summary>
        private void CalculateLayout()
        {
            _windowWidth = Console.WindowWidth;
            _windowHeight = Console.WindowHeight;
            
            // Determine if we need to use compact mode based on window dimensions
            _isCompactMode = (_windowWidth < CompactModeWidth || _windowHeight < CompactModeHeight) &&
                            _windowWidth >= MinimumWindowWidth && _windowHeight >= MinimumWindowHeight;
            
            // Calculate board position with better adaptability
            int totalBoardWidth = _isCompactMode 
                ? Board.Width + 2  // Single character per cell in compact mode
                : (Board.Width * BlockCharacter.Length) + 2; // Double character per cell in normal mode
                
            int totalContentWidth = totalBoardWidth + SidePanelWidth + 4; // +4 for spacing
            
            // Ensure minimum spacing and adjust for small windows
            if (_windowWidth < totalContentWidth + 4)
            {
                // If window is too narrow, prioritize showing the board
                _boardStartX = 2;
                _sidePanelStartX = Math.Max(_boardStartX + totalBoardWidth + 1, _windowWidth - SidePanelWidth - 2);
            }
            else
            {
                // Normal layout for wider windows
                _boardStartX = Math.Max(2, (_windowWidth - totalContentWidth) / 2);
                _sidePanelStartX = _boardStartX + totalBoardWidth + 2;
            }
            
            // Vertical centering with minimum spacing at top
            _boardStartY = Math.Max(3, (_windowHeight - Board.Height) / 2);
            
            // Store window dimensions for resize detection
            _lastWindowWidth = Console.WindowWidth;
            _lastWindowHeight = Console.WindowHeight;
            
            // Check if the window is too small for the game
            _isWindowTooSmall = (_windowWidth < MinimumWindowWidth || _windowHeight < MinimumWindowHeight);
        }
        
        /// <summary>
        /// Renders the game title at the top of the screen.
        /// </summary>
        private void RenderTitle()
        {
            Console.ForegroundColor = TitleColor;
            int titleX = _windowWidth / 2 - GameTitle.Length / 2;
            Console.SetCursorPosition(titleX, 1);
            Console.WriteLine(GameTitle);
        }
        
        /// <summary>
        /// Renders the game board.
        /// </summary>
        /// <param name="board">The board to render.</param>
        private void RenderBoard(Board board)
        {
            Console.ForegroundColor = BorderColor;
            
            // Draw top border
            Console.SetCursorPosition(_boardStartX, _boardStartY);
            Console.Write("┌" + new string('─', Board.Width * BlockCharacter.Length) + "┐");
            
            // Draw each row of the board
            for (int y = 0; y < Board.Height; y++)
            {
                Console.SetCursorPosition(_boardStartX, _boardStartY + y + 1);
                Console.Write("│");
                
                for (int x = 0; x < Board.Width; x++)
                {
                    var cell = board.Grid[x, y];
                    
                    if (cell.HasValue)
                    {
                        // Set color based on the tetromino type
                        SetTetrominoColor(cell.Value);
                        Console.Write(BlockCharacter);
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write(EmptyCharacter);
                    }
                }
                
                Console.ForegroundColor = BorderColor;
                Console.Write("│");
            }
            
            // Draw bottom border
            Console.SetCursorPosition(_boardStartX, _boardStartY + Board.Height + 1);
            Console.Write("└" + new string('─', Board.Width * BlockCharacter.Length) + "┘");
        }
        
        /// <summary>
        /// Adds a ghost piece to the board that shows where the current piece would land.
        /// </summary>
        /// <param name="board">The board to add the ghost piece to.</param>
        /// <param name="currentPiece">The current tetromino.</param>
        private void AddGhostPiece(Board board, Tetromino currentPiece)
        {
            // Create a shadow piece by cloning the current piece
            var ghostPiece = currentPiece.Clone();
            
            // Drop the ghost piece until it collides
            int ghostY = ghostPiece.Position.Y;
            while (_gameEngine.CanPieceMove(ghostPiece, 0, 1))
            {
                ghostY++;
                ghostPiece.Position = new Point(ghostPiece.Position.X, ghostY);
            }
            
            // Only show the ghost if it's not at the same position as the current piece
            if (ghostY > currentPiece.Position.Y)
            {
                // Get the positions of the ghost piece
                var ghostPositions = ghostPiece.Blocks.Select(point => 
                    new Point(ghostPiece.Position.X + point.X, ghostPiece.Position.Y + point.Y)).ToList();
                
                foreach (var position in ghostPositions)
                {
                    // Only add the ghost block if the position is empty
                    if (_gameEngine.IsValidPosition(position.X, position.Y) && !board.Grid[position.X, position.Y].HasValue)
                    {
                        // Use -1 as a special ID for ghost pieces
                        board.Grid[position.X, position.Y] = -1;
                    }
                }
            }
        }
        
        /// <summary>
        /// Renders the side panel with game statistics and next piece preview.
        /// </summary>
        private void RenderSidePanel()
        {
            int currentY = _boardStartY + 1;
            int indentX = _sidePanelStartX + 2;
            
            Console.ForegroundColor = StatsColor;
            
            // Score
            Console.SetCursorPosition(_sidePanelStartX, currentY);
            Console.WriteLine("SCORE:");
            Console.SetCursorPosition(indentX, currentY + 1);
            Console.WriteLine($"{_gameEngine.Score}");
            
            currentY += 3;
            
            // Level
            Console.SetCursorPosition(_sidePanelStartX, currentY);
            Console.WriteLine("LEVEL:");
            Console.SetCursorPosition(indentX, currentY + 1);
            Console.WriteLine($"{_gameEngine.Level}");
            
            currentY += 3;
            
            // Next Piece Preview
            Console.SetCursorPosition(_sidePanelStartX, currentY);
            Console.WriteLine("NEXT PIECE:");
            currentY += 1;
            RenderNextPiecePreview(indentX, currentY);
            
            currentY += 6;
            
            // Statistics
            Console.SetCursorPosition(_sidePanelStartX, currentY);
            Console.WriteLine("STATISTICS");
            currentY += 1;
            
            Console.SetCursorPosition(indentX, currentY);
            Console.WriteLine($"Speed: {GetSpeedText()}");
            
            currentY += 2;
            
            // Line clears
            Console.SetCursorPosition(_sidePanelStartX, currentY);
            Console.WriteLine("LINE CLEARS");
            currentY += 1;
            
            Console.SetCursorPosition(indentX, currentY);
            Console.WriteLine($"Single: {_gameEngine.SingleRowsCleared}");
            currentY += 1;
            
            Console.SetCursorPosition(indentX, currentY);
            Console.WriteLine($"Double: {_gameEngine.DoubleRowsCleared}");
            currentY += 1;
            
            Console.SetCursorPosition(indentX, currentY);
            Console.WriteLine($"Triple: {_gameEngine.TripleRowsCleared}");
            currentY += 1;
            
            Console.SetCursorPosition(indentX, currentY);
            Console.WriteLine($"Tetris: {_gameEngine.TetrisCleared}");
            
            currentY += 2;
            
            // Difficulty indicator
            Console.SetCursorPosition(_sidePanelStartX, currentY);
            Console.WriteLine("DIFFICULTY");
            currentY += 1;
            
            Console.SetCursorPosition(indentX, currentY);
            Console.WriteLine(GetDifficultyText());
        }
        
        /// <summary>
        /// Renders a preview of the next piece.
        /// </summary>
        /// <param name="x">The X position to start the preview.</param>
        /// <param name="y">The Y position to start the preview.</param>
        private void RenderNextPiecePreview(int x, int y)
        {
            var nextPiece = _gameEngine.NextPiece;
            
            if (nextPiece == null)
            {
                return;
            }
            
            // Determine the bounding box of the piece
            int minX = nextPiece.Blocks.Min(p => p.X);
            int minY = nextPiece.Blocks.Min(p => p.Y);
            int maxX = nextPiece.Blocks.Max(p => p.X);
            int maxY = nextPiece.Blocks.Max(p => p.Y);
            
            int width = maxX - minX + 1;
            int height = maxY - minY + 1;
            
            // Create a 2D array to represent the piece
            bool[,] pieceGrid = new bool[width, height];
            
            // Fill in the shape
            foreach (var block in nextPiece.Blocks)
            {
                int gridX = block.X - minX;
                int gridY = block.Y - minY;
                pieceGrid[gridX, gridY] = true;
            }
            
            // Draw the piece preview
            Console.ForegroundColor = BorderColor;
            Console.SetCursorPosition(x, y);
            Console.WriteLine("┌─────┐");
            
            for (int previewY = 0; previewY < height; previewY++)
            {
                Console.SetCursorPosition(x, y + previewY + 1);
                Console.Write("│");
                
                for (int previewX = 0; previewX < width; previewX++)
                {
                    bool isFilled = pieceGrid[previewX, previewY];
                    
                    if (isFilled)
                    {
                        SetTetrominoColor(nextPiece.Id);
                        Console.Write("█");
                    }
                    else
                    {
                        Console.Write(" ");
                    }
                }
                
                // Fill remaining space to make the preview consistent
                for (int i = width; i < 5; i++)
                {
                    Console.Write(" ");
                }
                
                Console.ForegroundColor = BorderColor;
                Console.Write("│");
            }
            
            // Fill any remaining rows to make the preview box consistent
            for (int i = height; i < 4; i++)
            {
                Console.SetCursorPosition(x, y + i + 1);
                Console.Write("│     │");
            }
            
            Console.SetCursorPosition(x, y + 5);
            Console.WriteLine("└─────┘");
        }
        
        /// <summary>
        /// Renders the controls help at the bottom of the screen.
        /// </summary>
        private void RenderControlsHelp()
        {
            Console.ForegroundColor = HelpTextColor;
            
            // Calculate position for centered help text
            string helpText = "← → ↓: Move   ↑: Rotate   Space: Drop   P: Pause   Esc: Menu";
            int helpX = _windowWidth / 2 - helpText.Length / 2;
            int helpY = _boardStartY + Board.Height + 3;
            
            if (helpY < _windowHeight - 1)
            {
                Console.SetCursorPosition(helpX, helpY);
                Console.WriteLine(helpText);
                
                // Add WASD alternative if there's space
                if (helpY + 1 < _windowHeight - 1)
                {
                    string altHelpText = "WASD: Alternative Movement   Z: Rotate CCW   X/C: Hold (if available)";
                    int altHelpX = _windowWidth / 2 - altHelpText.Length / 2;
                    Console.SetCursorPosition(altHelpX, helpY + 1);
                    Console.WriteLine(altHelpText);
                }
            }
        }
        
        /// <summary>
        /// Sets the console foreground color based on the tetromino ID.
        /// </summary>
        /// <param name="tetrominoId">The ID of the tetromino.</param>
        private void SetTetrominoColor(int tetrominoId)
        {
            // If this is a ghost piece (ID = -1), use the ghost piece color
            if (tetrominoId == -1)
            {
                Console.ForegroundColor = GhostPieceColor;
                return;
            }
            
            // Set colors based on traditional Tetris colors
            switch (tetrominoId)
            {
                case 0: // I piece
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    break;
                    
                case 1: // J piece
                    Console.ForegroundColor = ConsoleColor.Blue;
                    break;
                    
                case 2: // L piece
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    break;
                    
                case 3: // O piece
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                    
                case 4: // S piece
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                    
                case 5: // T piece
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    break;
                    
                case 6: // Z piece
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                    
                default:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
            }
        }
        
        /// <summary>
        /// Gets a text description of the current game speed based on level.
        /// </summary>
        /// <returns>A text description of the game speed.</returns>
        private string GetSpeedText()
        {
            int level = _gameEngine.Level;
            
            if (level <= 5)
            {
                return "Normal";
            }
            else if (level <= 10)
            {
                return "Fast";
            }
            else if (level <= 15)
            {
                return "Very Fast";
            }
            else if (level <= 20)
            {
                return "Super Fast";
            }
            else
            {
                return "Extreme";
            }
        }
        
        /// <summary>
        /// Gets a text description of the current game difficulty based on level.
        /// </summary>
        /// <returns>A text description of the game difficulty.</returns>
        private string GetDifficultyText()
        {
            int level = _gameEngine.Level;
            
            if (level <= 3)
            {
                return "Easy";
            }
            else if (level <= 8)
            {
                return "Medium";
            }
            else if (level <= 15)
            {
                return "Hard";
            }
            else
            {
                return "Expert";
            }
        }
        
        /// <summary>
        /// Renders a simplified view of the game for very small console windows.
        /// </summary>
        private void RenderCompactMode()
        {
            Console.Clear();
            Console.ForegroundColor = TitleColor;
            
            // Shortened title if needed
            string displayTitle = _windowWidth < GameTitle.Length + 4 ? "TETRIS" : GameTitle;
            int titleX = Math.Max(0, _windowWidth / 2 - displayTitle.Length / 2);
            Console.SetCursorPosition(titleX, 0);
            Console.WriteLine(displayTitle);
            
            // Create a copy of the board with the current piece added
            var boardWithPiece = new Board(_gameEngine.Board);
            var currentPiece = _gameEngine.CurrentPiece;
            
            if (currentPiece != null)
            {
                foreach (var point in currentPiece.Blocks)
                {
                    int boardX = currentPiece.Position.X + point.X;
                    int boardY = currentPiece.Position.Y + point.Y;
                    
                    if (boardX >= 0 && boardX < Board.Width && boardY >= 0 && boardY < Board.Height)
                    {
                        boardWithPiece.Grid[boardX, boardY] = currentPiece.Id;
                    }
                }
                
                // Add the ghost piece if enabled and there's room
                if (ShowGhostPiece)
                {
                    AddGhostPiece(boardWithPiece, currentPiece);
                }
            }
            
            // Draw compact board
            RenderCompactBoard(boardWithPiece);
            
            // Show minimal stats (score and level only)
            int statsY = _boardStartY + Board.Height + 2;
            if (statsY + 2 < _windowHeight)
            {
                Console.ForegroundColor = StatsColor;
                Console.SetCursorPosition(1, statsY);
                Console.Write($"Score: {_gameEngine.Score}");
                Console.SetCursorPosition(1, statsY + 1);
                Console.Write($"Level: {_gameEngine.Level}");
                
                // Show next piece if there's room
                if (_windowWidth >= 30 && statsY + 2 < _windowHeight - 5)
                {
                    Console.SetCursorPosition(_windowWidth - 15, statsY);
                    Console.Write("Next:");
                    if (_gameEngine.NextPiece != null)
                    {
                        SetTetrominoColor(_gameEngine.NextPiece.Id);
                        Console.SetCursorPosition(_windowWidth - 15, statsY + 1);
                        Console.Write("##");
                    }
                }
            }
        }
        
        /// <summary>
        /// Renders a more compact version of the game board for small windows.
        /// </summary>
        /// <param name="board">The board to render.</param>
        private void RenderCompactBoard(Board board)
        {
            Console.ForegroundColor = BorderColor;
            
            // Minimal top border
            Console.SetCursorPosition(_boardStartX, _boardStartY);
            Console.Write("┌" + new string('─', Board.Width) + "┐");
            
            // Draw each row of the board using single-width characters
            for (int y = 0; y < Board.Height; y++)
            {
                Console.SetCursorPosition(_boardStartX, _boardStartY + y + 1);
                Console.Write("│");
                
                for (int x = 0; x < Board.Width; x++)
                {
                    var cell = board.Grid[x, y];
                    
                    if (cell.HasValue)
                    {
                        SetTetrominoColor(cell.Value);
                        Console.Write(CompactBlockCharacter);
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write(CompactEmptyCharacter);
                    }
                }
                
                Console.ForegroundColor = BorderColor;
                Console.Write("│");
            }
            
            // Draw bottom border
            Console.SetCursorPosition(_boardStartX, _boardStartY + Board.Height + 1);
            Console.Write("└" + new string('─', Board.Width) + "┘");
        }
        
        #endregion
    }
}
