using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Tetris.Core.Models;

namespace Tetris.Core.UI
{
    /// <summary>
    /// Partial class containing improved methods for GameplayInterface that enhance responsiveness
    /// and window resizing capabilities.
    /// </summary>
    public partial class GameplayInterface
    {
        #region Improved Methods

        /// <summary>
        /// Calculates the layout of the interface elements based on the console window size
        /// with better adaptability to different screen dimensions.
        /// </summary>
        private void CalculateLayoutImproved()
        {
            _windowWidth = Console.WindowWidth;
            _windowHeight = Console.WindowHeight;
            
            // Calculate board position with better adaptability
            int totalBoardWidth = (Board.Width * BlockCharacter.Length) + 2; // +2 for borders
            int totalContentWidth = totalBoardWidth + SidePanelWidth + 4; // +4 for spacing
            
            // Determine layout based on available space
            if (_windowWidth < totalContentWidth + 4)
            {
                // For narrow windows, prioritize showing the board
                _boardStartX = 2;
                _sidePanelStartX = Math.Max(_boardStartX + totalBoardWidth + 1, _windowWidth - SidePanelWidth - 2);
                
                // If extremely narrow, we might need to adjust rendering later
                if (_windowWidth < _boardStartX + totalBoardWidth + 2)
                {
                    _isCompactMode = true;
                }
                else
                {
                    _isCompactMode = false;
                }
            }
            else
            {
                // For wider windows, center the layout
                _boardStartX = Math.Max(2, (_windowWidth - totalContentWidth) / 2);
                _sidePanelStartX = _boardStartX + totalBoardWidth + 2;
                _isCompactMode = false;
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
        /// Resizes the interface based on the current console window size with improved
        /// handling of window size changes.
        /// </summary>
        public void HandleResizeImproved()
        {
            int newWidth = Console.WindowWidth;
            int newHeight = Console.WindowHeight;
            
            // Only redraw if the size actually changed
            if (newWidth != _windowWidth || newHeight != _windowHeight)
            {
                Console.Clear();
                CalculateLayoutImproved();
                
                // Check if the window is too small for the game
                if (_isWindowTooSmall)
                {
                    ShowWindowTooSmallMessage();
                }
                else
                {
                    // Use compact mode if needed
                    if (_isCompactMode)
                    {
                        RenderCompactMode();
                    }
                    else
                    {
                        Render();
                    }
                }
            }
        }

        /// <summary>
        /// Checks if the console window has been resized and updates the layout if needed.
        /// More reliable than the original implementation.
        /// </summary>
        /// <returns>True if the window was resized, false otherwise.</returns>
        public bool CheckForResizeImproved()
        {
            int currentWidth = Console.WindowWidth;
            int currentHeight = Console.WindowHeight;
            
            if (currentWidth != _lastWindowWidth || currentHeight != _lastWindowHeight)
            {
                // Update cached dimensions
                _lastWindowWidth = currentWidth;
                _lastWindowHeight = currentHeight;
                
                // Handle the resize
                HandleResizeImproved();
                return true;
            }
            
            return false;
        }

        /// <summary>
        /// Processes user input with improved responsiveness and support for alternative controls.
        /// </summary>
        /// <param name="key">The key that was pressed.</param>
        /// <returns>True if fast drop is active, false otherwise.</returns>
        public bool ProcessGameInputImproved(ConsoleKeyInfo key)
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
                        _gameEngine.RotatePieceCounterClockwise();
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
            }
        }
        
        /// <summary>
        /// Renders a more compact version of the game board for small windows.
        /// </summary>
        /// <param name="board">The board to render.</param>
        private void RenderCompactBoard(Board board)
        {
            const string compactBlockChar = "█"; // Single character block
            
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
                        Console.Write(compactBlockChar);
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write("·");
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
