using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Tetris.Core.UI
{
    /// <summary>
    /// Utility class to fix references to Shape and Type in the GameplayInterface.Complete.cs file.
    /// </summary>
    internal class ShapeFixer
    {
        public static void FixGameplayInterfaceComplete()
        {
            string filePath = @"c:\ProgramData\Comarch IBARD\Sync\f9ac72d3\d955\Praca\Github Copilot\_project\Tetris\src\Tetris.Core\UI\GameplayInterface.Complete.cs";
            string content = File.ReadAllText(filePath);
            
            // Replace Shape with Blocks and Type with Id
            content = Regex.Replace(content, @"currentPiece\.Shape", "currentPiece.Blocks");
            content = Regex.Replace(content, @"ghostPiece\.Shape", "ghostPiece.Blocks");
            content = Regex.Replace(content, @"nextPiece\.Shape", "nextPiece.Blocks");
            
            content = Regex.Replace(content, @"currentPiece\.Type", "currentPiece.Id");
            content = Regex.Replace(content, @"nextPiece\.Type", "nextPiece.Id");
            content = Regex.Replace(content, @"_gameEngine\.NextPiece\.Type", "_gameEngine.NextPiece.Id");
            
            File.WriteAllText(filePath, content);
            
            Console.WriteLine("GameplayInterface.Complete.cs file fixed successfully.");
        }
    }
}
