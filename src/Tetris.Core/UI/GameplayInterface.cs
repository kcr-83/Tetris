using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Tetris.Core.Models;

namespace Tetris.Core.UI
{
    /// <summary>
    /// Provides a graphical user interface for the Tetris gameplay screen,
    /// displaying the game board, score, level, and other game information.
    /// </summary>
    public class GameplayInterface
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

        #endregion

        #region Fields

        private readonly GameEngine _gameEngine;
        private int _windowWidth;
        private int _windowHeight;
        private int _boardStartX;
        private int _boardStartY;
        private int _sidePanelStartX;
        private bool _initialized;

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

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the GameplayInterface class.
        /// </summary>
        /// <param name="gameEngine">The game engine to display.</param>
        public GameplayInterface(GameEngine gameEngine)
        {
            _gameEngine = gameEngine ?? throw new ArgumentNullException(nameof(gameEngine));
            _initialized = false;
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

            // Calculate layout
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

            Console.BackgroundColor = BackgroundColor;
            
            // Render game title
            RenderTitle();
            
            // Create a copy of the board with the current piece added
            var boardWithPiece = new Board(_gameEngine.Board);
            
            // Add the current piece to the display board
            var currentPiece = _gameEngine.CurrentPiece;
            if (currentPiece != null)
            {
                var currentPiecePositions = currentPiece.GetAbsolutePositions();
                foreach (var position in currentPiecePositions)
                {
                    if (boardWithPiece.IsWithinBounds(position.X, position.Y))
                    {
                        boardWithPiece.AddBlock(position.X, position.Y, currentPiece.Id);
                    }
                }
                
                // Add ghost piece if enabled
                if (ShowGhostPiece)
                {
                    RenderGhostPiece(boardWithPiece, currentPiece);
                }
            }
            
            // Render the game board
            RenderBoard(boardWithPiece);
            
            // Render the side panel with game stats
            RenderSidePanel();
            
            // Render controls help
            RenderControlsHelp();
        }

        /// <summary>
        /// Shows a pause overlay on the screen.
        /// </summary>
        public void ShowPauseOverlay()
        {
            int centerX = _boardStartX + (Board.Width * BlockCharacter.Length) / 2 - 10;
            int centerY = _boardStartY + Board.Height / 2 - 2;
            
            Console.ForegroundColor = ConsoleColor.Yellow;
            
            // Draw pause message box
            Console.SetCursorPosition(centerX, centerY);
            Console.WriteLine("┌─────────────────────┐");
            Console.SetCursorPosition(centerX, centerY + 1);
            Console.WriteLine("│       PAUSED        │");
            Console.SetCursorPosition(centerX, centerY + 2);
            Console.WriteLine("│ Press P to continue │");
            Console.SetCursorPosition(centerX, centerY + 3);
            Console.WriteLine("│ Press ESC for menu  │");
            Console.SetCursorPosition(centerX, centerY + 4);
            Console.WriteLine("└─────────────────────┘");
        }

        /// <summary>
        /// Shows the level up animation.
        /// </summary>
        /// <param name="newLevel">The new level reached.</param>
        public async Task ShowLevelUpAnimationAsync(int newLevel)
        {
            int centerX = _boardStartX + (Board.Width * BlockCharacter.Length) / 2 - 8;
            int centerY = _boardStartY + Board.Height / 2 - 1;
            
            Console.ForegroundColor = ConsoleColor.Green;
            
            // Draw level up message box
            Console.SetCursorPosition(centerX, centerY);
            Console.WriteLine("┌────────────────┐");
            Console.SetCursorPosition(centerX, centerY + 1);
            Console.WriteLine($"│   LEVEL {newLevel,-2}!    │");
            Console.SetCursorPosition(centerX, centerY + 2);
            Console.WriteLine("└────────────────┘");
            
            await Task.Delay(1000); // Display for 1 second
            Render(); // Re-render to remove the message
        }

        /// <summary>
        /// Shows a message when rows are cleared, with animations for special clears.
        /// </summary>
        /// <param name="rowsCleared">The number of rows cleared.</param>
        /// <param name="score">The score gained from this clear.</param>
        public async Task ShowRowsClearedAnimationAsync(int rowsCleared, int score)
        {
            if (rowsCleared == 0)
                return;
                
            int centerX = _boardStartX + (Board.Width * BlockCharacter.Length) / 2 - 10;
            int centerY = _boardStartY + Board.Height / 2;
            
            string message;
            ConsoleColor color;
            
            switch (rowsCleared)
            {
                case 1:
                    message = "SINGLE";
                    color = ConsoleColor.White;
                    break;
                case 2:
                    message = "DOUBLE";
                    color = ConsoleColor.Yellow;
                    break;
                case 3:
                    message = "TRIPLE";
                    color = ConsoleColor.Magenta;
                    break;
                case 4:
                    message = "TETRIS!";
                    color = ConsoleColor.Cyan;
                    break;
                default:
                    message = $"{rowsCleared} ROWS!";
                    color = ConsoleColor.Red;
                    break;
            }
            
            Console.ForegroundColor = color;
            
            // Draw row clear message box
            Console.SetCursorPosition(centerX, centerY);
            Console.WriteLine($"╔════════════════════╗");
            Console.SetCursorPosition(centerX, centerY + 1);
            Console.WriteLine($"║      {message,-7}      ║");
            Console.SetCursorPosition(centerX, centerY + 2);
            Console.WriteLine($"║    +{score,-6} pts    ║");
            Console.SetCursorPosition(centerX, centerY + 3);
            Console.WriteLine($"╚════════════════════╝");
            
            await Task.Delay(600); // Display briefly
            Render(); // Re-render to remove the message
        }

        /// <summary>
        /// Resizes the interface based on the current console window size.
        /// </summary>
        public void HandleResize()
        {
            Console.Clear();
            CalculateLayout();
            Render();
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
            
            // Calculate board position (centered horizontally, with room for side panel)
            int totalBoardWidth = (Board.Width * BlockCharacter.Length) + 2; // +2 for borders
            int totalContentWidth = totalBoardWidth + SidePanelWidth + 4; // +4 for spacing
            
            _boardStartX = Math.Max(2, (_windowWidth - totalContentWidth) / 2);
            _boardStartY = Math.Max(3, (_windowHeight - Board.Height) / 2);
            
            // Side panel starts right after the board with a small gap
            _sidePanelStartX = _boardStartX + totalBoardWidth + 2;
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
        /// <param name="currentPiece">The current falling piece.</param>
        private void RenderGhostPiece(Board board, Tetromino currentPiece)
        {
            // Create a copy of the current piece to find its landing position
            var ghostPiece = currentPiece.Clone();
            int ghostY = ghostPiece.Position.Y;
            
            // Find the landing position
            while (board.CanTetrominoMoveDown(ghostPiece))
            {
                ghostY++;
                ghostPiece.Position = new System.Drawing.Point(ghostPiece.Position.X, ghostY);
            }
            
            // Only show the ghost if it's not at the same position as the current piece
            if (ghostY > currentPiece.Position.Y)
            {
                // Get the positions of the ghost piece
                var ghostPositions = ghostPiece.GetAbsolutePositions();
                
                // Add the ghost piece to the board with a special ID for coloring
                foreach (var position in ghostPositions)
                {
                    // Only add the ghost block if the position is empty
                    if (board.IsWithinBounds(position.X, position.Y) && !board.Grid[position.X, position.Y].HasValue)
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
            
            // Next Piece Preview
            Console.ForegroundColor = StatsColor;
            Console.SetCursorPosition(_sidePanelStartX, currentY);
            Console.WriteLine("NEXT PIECE:");
            currentY += 1;
            
            // Draw the next piece preview in a box
            RenderNextPiecePreview(indentX, currentY);
            currentY += 6; // Space for the preview box
            
            // Game Statistics
            Console.ForegroundColor = StatsColor;
            Console.SetCursorPosition(_sidePanelStartX, currentY);
            Console.WriteLine("STATISTICS");
            currentY += 1;
            
            Console.SetCursorPosition(indentX, currentY);
            Console.WriteLine($"Score: {_gameEngine.Score}");
            currentY += 1;
            
            Console.SetCursorPosition(indentX, currentY);
            Console.WriteLine($"Level: {_gameEngine.Level}");
            currentY += 1;
            
            Console.SetCursorPosition(indentX, currentY);
            Console.WriteLine($"Lines: {_gameEngine.Board.RowsCleared}");
            currentY += 1;
            
            Console.SetCursorPosition(indentX, currentY);
            Console.WriteLine($"Speed: {GetSpeedText()}");
            currentY += 2;
            
            // Line Statistics
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
            
            // Adding a difficulty indicator
            Console.SetCursorPosition(_sidePanelStartX, currentY);
            Console.WriteLine("DIFFICULTY");
            currentY += 1;
            
            Console.SetCursorPosition(indentX, currentY);
            Console.WriteLine(GetDifficultyText());
        }

        /// <summary>
        /// Renders the next piece preview in a box.
        /// </summary>
        /// <param name="x">The starting X position.</param>
        /// <param name="y">The starting Y position.</param>
        private void RenderNextPiecePreview(int x, int y)
        {
            var nextPiece = _gameEngine.NextPiece;
            if (nextPiece == null)
                return;
                
            // Get the piece's shape
            var blocks = nextPiece.Blocks;
            
            // Determine the bounding box
            int minX = blocks.Min(p => p.X);
            int minY = blocks.Min(p => p.Y);
            int maxX = blocks.Max(p => p.X);
            int maxY = blocks.Max(p => p.Y);
            
            int width = maxX - minX + 1;
            int height = maxY - minY + 1;
            
            // Create a 2D array to represent the piece
            bool[,] pieceShape = new bool[width, height];
            
            // Fill in the shape
            foreach (var pos in blocks)
            {
                pieceShape[pos.X - minX, pos.Y - minY] = true;
            }
            
            // Draw a box around the preview
            Console.ForegroundColor = BorderColor;
            Console.SetCursorPosition(x, y);
            Console.WriteLine("┌─────┐");
            
            for (int previewY = 0; previewY < 4; previewY++)
            {
                Console.SetCursorPosition(x, y + previewY + 1);
                Console.Write("│");
                
                for (int previewX = 0; previewX < 4; previewX++)
                {
                    bool isFilled = false;
                    
                    // Check if this position contains a block from the next piece
                    if (previewY < height && previewX < width)
                    {
                        isFilled = pieceShape[previewX, previewY];
                    }
                    
                    if (isFilled)
                    {
                        SetTetrominoColor(nextPiece.Id);
                        Console.Write("█");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write(" ");
                    }
                }
                
                Console.ForegroundColor = BorderColor;
                Console.WriteLine(" │");
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
            string helpText = "←→: Move   ↑: Rotate   ↓: Fast Drop   Space: Hard Drop   P: Pause   ESC: Menu";
            int helpX = _windowWidth / 2 - helpText.Length / 2;
            int helpY = _boardStartY + Board.Height + 3;
            
            Console.SetCursorPosition(helpX, helpY);
            Console.WriteLine(helpText);
        }

        /// <summary>
        /// Sets the console color based on the tetromino ID.
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
                case 1: // I piece
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    break;
                case 2: // J piece
                    Console.ForegroundColor = ConsoleColor.Blue;
                    break;
                case 3: // L piece
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    break;
                case 4: // O piece
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case 5: // S piece
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case 6: // T piece
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    break;
                case 7: // Z piece
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
            }
        }

        /// <summary>
        /// Gets a text description of the current game speed.
        /// </summary>
        /// <returns>A string representing the game speed.</returns>
        private string GetSpeedText()
        {
            int level = _gameEngine.Level;
            
            if (level <= 5)
                return "Normal";
            else if (level <= 10)
                return "Fast";
            else if (level <= 15)
                return "Very Fast";
            else if (level <= 20)
                return "Super Fast";
            else
                return "Extreme";
        }

        /// <summary>
        /// Gets a text description of the current difficulty level.
        /// </summary>
        /// <returns>A string representing the difficulty level.</returns>
        private string GetDifficultyText()
        {
            int level = _gameEngine.Level;
            
            if (level <= 3)
                return "Easy";
            else if (level <= 8)
                return "Medium";
            else if (level <= 15)
                return "Hard";
            else
                return "Expert";
        }

        #endregion
    }
}
