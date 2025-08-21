using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Threading;

namespace ShutTheBoxAdvanced2
{
    public class DiceAnimator<T> : IDiceAnimator<T> where T : IDisplayable
    {
        int time;
        int dice1Heigth;
        int dice1Width;
        int dice2Heigth;
        int dice2Width;
        int GridWidth;
        int GridHeigth;
        int reductionFactor = 1;
        T dice1;
        T dice2;
        T floor;
        T space;
        string? grid = null;

        public DiceAnimator(int time)
        {
            this.time = time;
            MultiCastEnd = AcNewLine + AcDisplayGrid + AcWait;
            MultiCast = AcClear;
        }
        public void Animate(T Dice1, T Dice2, T Floor, T Space)
        {
            dice1 = Dice1;
            dice2 = Dice2;
            floor = Floor;
            space = Space;

            // check what the current size of the console window is. Use this to calculate the size of the grid and dice position
            GridWidth = Console.WindowWidth / 2;
            GridHeigth = Console.WindowHeight / 2;
            Random random = new Random();
            dice1Heigth = random.Next(3, GridHeigth);
            dice2Heigth = random.Next(3, GridHeigth);
            dice1Width = random.Next(0, GridWidth / 2);
            dice2Width = random.Next(dice1Heigth + 1, GridWidth);

            int maxDiceHeight = Math.Max(dice1Heigth, dice2Heigth);
            IEnumerable<int> d1BounceInt = BouncingIntegers(dice1Heigth, reductionFactor);
            IEnumerable<int> d2BounceInt = BouncingIntegers(dice2Heigth, reductionFactor);
            IEnumerator<int> d1enumerator = d1BounceInt.GetEnumerator();
            IEnumerator<int> d2enumerator = d2BounceInt.GetEnumerator();

           
            while (d1enumerator.MoveNext() | d2enumerator.MoveNext())
            {
                // reset last frame
                grid = "";
                MultiCast = AcClear;

                dice1Heigth = d1enumerator.Current;
                dice2Heigth = d2enumerator.Current;

                GenerateGrid(GridWidth, GridHeigth);

                MultiCast += MultiCastEnd;
                MultiCast();
            }
        }

        void Clear() => Console.Clear();
        void DisplayGrid() => Console.Write(grid);
        void Wait() => Thread.Sleep(time);
        void DisplayD1() => grid += dice1.Display();
        void DisplayD2() => grid += dice2.Display();
        void DisplaySpace() => grid += space.Display();
        void DisplayFloor() => grid += floor.Display();
        void DisplayNewLine() => grid += "\n";

        private Action AcClear => Clear;
        private Action AcDisplayGrid => DisplayGrid;
        private Action AcWait => Wait;
        private Action AcD1 => DisplayD1;
        private Action AcD2 => DisplayD2;
        private Action AcSpace => DisplaySpace;
        private Action AcNewLine => DisplayNewLine;
        private Action AcFloor => DisplayFloor;
        private Action MultiCast;
        private Action MultiCastEnd;

        private void GenerateGrid(int width, int height)
        {
            // Rows
            for (int i = height - 1; i >= 0; i--)
            {
                MultiCast += AcNewLine;
                // Columns
                for (int j = 0; j < width; j++)
                {
                    if (i == dice1Heigth && j == dice1Width)
                    {
                        MultiCast += AcD1;
                    }
                    else if (i == dice2Heigth && j == dice2Width)
                    {
                        MultiCast += AcD2;
                    }
                    else if (i == 0)
                    {
                        MultiCast += AcFloor;
                    }
                    else
                    {
                        MultiCast += AcSpace;
                    }
                }
            }
        }
        public static IEnumerable<int> BouncingIntegers(int start, int reductionFactor)
        {
            int current = start;
            bool isDecreasing = true;

            // 1: YIELD AND LAZY EVALUATION
            // 2: Here we use yield to create a lazy evaluation of the sequence of integers
            // 3: This was helpful since we dont know the dice starting position 
            //    and we want to animate the dice bouncing until the end every time

            while (current > 0)
            {
                if (isDecreasing)
                {
                    for (int i = current; i >= 1; i--)
                    {
                        yield return i;
                    }
                    isDecreasing = false;
                }
                else
                {
                    for (int i = 2; i <= current; i++)
                    {
                        yield return i;
                    }
                    isDecreasing = true;
                }

                current -= reductionFactor;
            }
        }

    }
}
