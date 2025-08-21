using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShutTheBoxAdvanced2
{
    public class Box : IBox
    {
        private readonly List<Tile> tiles;

        public Box()
        {
            // Mapping the specific characters to the tile numbers.
            var tileCharacters = new char[] { '*', 'J', 'A', 'C', 'K', 'P', 'O', 'T', '*' };

            tiles = Enumerable.Range(1, 9)
                          .Select(i => new Tile(i, tileCharacters[i - 1]))
                          .ToList();
        }

        public IEnumerable<int> UnshutTiles
        {
            // 1: ENUMERABLE
            // 2: The 'UnshutTiles' property is an IEnumerable<int> that returns a list of all the tile numbers that are not shut.
            //    This property is used to check if the game is won or notand to display the board.
            // 3: This approach is used to avoid exposing the internal list of tiles, which would allow the caller to modify it.

            // 1: LAMBDA EXPRESSION
            // 2: The lambda is used to define a condition within a LINQ query.
            // 3: It simplifies how we can write code for filtering and selecting specific items in a collection.
            get { return tiles.Where(tile => !tile.IsShut).Select(tile => tile.Number); }
        }

        public void ShutTile(int tileNumber)
        {
            var tile = tiles.FirstOrDefault(t => t.Number == tileNumber && !t.IsShut);
            tile?.Shut();
        }

        public bool IsTileShut(int tileNumber)
        {
            return tiles.Any(t => t.Number == tileNumber && t.IsShut);
        }


        public void DisplayBoard()
        {
            // Create a representation of the board as strings, using LINQ.
            var boardDisplay = tiles.Select(tile =>
            {
                if (tile.IsShut)
                {
                    return $"[{tile.HiddenChar}]";
                }
                else
                {
                    return $"[{tile.Number}]";
                }
            });

            foreach (var tileRepresentation in boardDisplay)
            {
                bool isNumber = tileRepresentation.Trim(new char[] { '[', ']' }).All(c => char.IsDigit(c));

                // Numbers are RED, Shut tiles are GREEN
                Console.ForegroundColor = isNumber ? ConsoleColor.Red : ConsoleColor.Green;
                Console.Write(tileRepresentation);
                Console.ResetColor();
            }
            Console.WriteLine();
        }
    }
}