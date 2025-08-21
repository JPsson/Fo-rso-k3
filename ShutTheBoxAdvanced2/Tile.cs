using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShutTheBoxAdvanced2
{
    public class Tile
    {
        public int Number { get; }
        public bool IsShut { get; private set; }
        public char HiddenChar { get; }

        public Tile(int number, char hiddenChar)
        {
            Number = number;
            IsShut = false;
            HiddenChar = hiddenChar;
        }

        public void Shut()
        {
            IsShut = true;
        }
    }
}