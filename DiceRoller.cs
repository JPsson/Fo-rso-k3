using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShutTheBoxAdvanced2
{
    public class DiceRoller : IDiceRoller
    {
        private readonly Random _random = new Random();

        public (int, int) RollDice()
        {
            return (_random.Next(1, 7), _random.Next(1, 7));
        }

        public string VisualizeDice(int number)
        {
            return number switch
            {
                1 => "[     ]\n[  *  ]\n[     ]\n",
                2 => "[*    ]\n[     ]\n[    *]\n",
                3 => "[*    ]\n[  *  ]\n[    *]\n",
                4 => "[*   *]\n[     ]\n[*   *]\n",
                5 => "[*   *]\n[  *  ]\n[*   *]\n",
                6 => "[*   *]\n[*   *]\n[*   *]\n",
                _ => "[ ???? ]\n[ ???? ]\n[ ???? ]\n"
            };
        }
    }
}