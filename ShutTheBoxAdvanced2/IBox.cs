using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShutTheBoxAdvanced2
{
    public interface IBox
    {
        void ShutTile(int tile);
        bool IsTileShut(int tile);
        void DisplayBoard();
        IEnumerable<int> UnshutTiles { get; }
    }
}