using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.DependencyInjection;

namespace ShutTheBoxAdvanced2
{
    // Define a delegate for handling text output.
    public delegate void OutputHandler(string message);

    class Program
    {
        static void Main(string[] args)
        {
            // Standard emojis:
            IDisplayable dice1Displayable = new Emoji("🎲");
            IDisplayable dice2Displayable = new Emoji("🎲");
            IDisplayable groundDisplayable = new Emoji("🟩");
            IDisplayable spaceDisplayable = new Emoji("🟦");
            while (true) // Keep the application running until the user decides to exit
            {
                // 1: STRATEGY PATTERN
                // 2: The user is allowed to specify what vibe the comments given from the program has
                // 3: To easily change the behavior of the comments at run-time
                Console.Clear();
                Console.WriteLine("Welcome to Shut the Box!\n");
                Console.WriteLine("Please select a personality:\n1: Normal\n2: Friendly\n3: Evil\n");
                Console.WriteLine("Any other input will result in the default personality.\n");

                int input1 = 1;
                bool validInput = int.TryParse(Console.ReadLine(), out input1);
                IDisplayPersonality personality = new NormalDisplayPersonality();

                switch (input1)
                {
                    case 1:
                        personality = new NormalDisplayPersonality();
                        break;
                    case 2:
                        personality = new FriendlyDisplayPersonality();
                        break;
                    case 3:
                        personality = new EvilDisplayPersonality();
                        break;
                }


                Console.Clear();
                Console.WriteLine("Do you want to play with emojis or chars?\n1: Emojis\n2: Characters");
                Console.WriteLine("Any other input will result in standard emojis.\n");

                validInput = int.TryParse(Console.ReadLine(), out input1);

                if (input1 == 1)
                {
                    Console.Clear();
                    Console.WriteLine("You have chosen to play with emojis.");
                    System.Console.WriteLine("Do you want option 1 or 2 for dice 1?\n1: 🎲\n2: 🎱 ");
                    System.Console.WriteLine("Any other input will result in standard emojis.\n");
                    validInput = int.TryParse(Console.ReadLine(), out input1);

                    if (input1 == 2)
                    {
                        dice1Displayable = new Emoji("🎱");
                    }
                    else
                    {
                        dice1Displayable = new Emoji("🎲");
                    }
                    Console.Clear();
                    System.Console.WriteLine("Do you want option 1 or 2 for dice 2?\n1: 🎲\n2: 🎱 ");
                    System.Console.WriteLine("Any other input will result in standard emojis.\n");
                    validInput = int.TryParse(Console.ReadLine(), out input1);
                    if (input1 == 2)
                    {
                        dice2Displayable = new Emoji("🎱");
                    }
                    else
                    {
                        dice2Displayable = new Emoji("🎲");
                    }
                    Console.Clear();
                    System.Console.WriteLine("Do you want option 1 or 2 for the ground?\n1: 🟩\n2: 🟨 ");
                    System.Console.WriteLine("Any other input will result in standard emojis.\n");
                    validInput = int.TryParse(Console.ReadLine(), out input1);
                    if (input1 == 2)
                    {
                        groundDisplayable = new Emoji("🟨");
                    }
                    else
                    {
                        groundDisplayable = new Emoji("🟩");
                    }
                    Console.Clear();
                    System.Console.WriteLine("Do you want option 1 or 2 for the space?\n1: 🟦\n2: 🟪 ");
                    System.Console.WriteLine("Any other input will result in standard emojis.\n");
                    validInput = int.TryParse(Console.ReadLine(), out input1);
                    if (input1 == 2)
                    {
                        spaceDisplayable = new Emoji("🟪");
                    }
                    else
                    {
                        spaceDisplayable = new Emoji("🟦");
                    }
                }
                else if (input1 == 2)
                {
                    Console.Clear();
                    Console.WriteLine("You have chosen to play with characters.");
                    System.Console.WriteLine("please enter a char for dice 1\n");
                    char input2 = '*';
                    bool validInput2 = char.TryParse(Console.ReadLine(), out input2);
                    dice1Displayable = new Symbol(input2);
                    System.Console.WriteLine("please enter a char for dice 2\n");
                    validInput2 = char.TryParse(Console.ReadLine(), out input2);
                    dice2Displayable = new Symbol(input2);
                    System.Console.WriteLine("please enter a char for the ground\n");
                    validInput2 = char.TryParse(Console.ReadLine(), out input2);
                    groundDisplayable = new Symbol(input2);
                    System.Console.WriteLine("please enter a char for the space\n");
                    validInput2 = char.TryParse(Console.ReadLine(), out input2);
                    spaceDisplayable = new Symbol(input2);
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("You have chosen to play with the standard emojis.");
                }

                // Set up the dependency injection and services
                var serviceProvider = new ServiceCollection()
                    .AddSingleton<IBox, Box>()
                    .AddSingleton<IDiceRoller, DiceRoller>()
                    .AddSingleton<IScoreBehavior<string>, PlayerPersonalityFeedback>(provider =>
                        new PlayerPersonalityFeedback(Console.WriteLine, personality))
                    .AddSingleton<GameEventNotifier>()
                    .AddSingleton<GameController<string>>()
                    .AddSingleton<GameStatsHandler>()
                    .BuildServiceProvider();

                // Retrieve the GameController and start the game
                var gameController = serviceProvider.GetService<GameController<string>>();
                gameController.SetOutputHandler(Console.WriteLine);
                bool playAgain = gameController.StartGame(dice1Displayable, dice2Displayable, groundDisplayable, spaceDisplayable);

                if (!playAgain)
                {
                    break;
                }
            }
        }
    }


}

