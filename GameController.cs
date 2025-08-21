using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShutTheBoxAdvanced2;

namespace ShutTheBoxAdvanced2
{
    public class GameController<T>
    {
        private readonly IBox _box;
        private readonly IDiceRoller _diceRoller;
        private readonly GameEventNotifier _notifier;



        private event Action<double, DateTime> _onRollEvent;
        private event Action _onNewGameEvent;
        private event Action _onGameEndEvent;
       
       
        // 1: GENERIC TYPES
        // 2: Here, 'T' can be any type (like int, string, etc.), allowing this class to use different scoring systems.
        // 3: This makes our GameController more flexible. It can work with any type of scoring without needing changes.
        private readonly IScoreBehavior<T> _scoreBehavior;

        private OutputHandler _outputHandler;

        // 1: COMPOSITION OVER INHERITANCE.
        // 2: Here, 'GameController' is composed of several objects that give it functionality ('IBox', 'IDiceRoller', 'GameEventNotifier', and 'IScoreBehavior'). 
        //    It doesn't inherit from them but rather gets their functionality by holding and using instances of these objects.
        // 3: This approach is used to avoid the tight coupling that comes with inheritance, allowing for more modular, maintainable, and testable code. 
        //    It makes 'GameController' flexible and independent of the implementations of its composed objects, 
        //    as the actual implementations can be swapped without changing the 'GameController' class itself.
        public GameController(IBox box, IDiceRoller diceRoller, GameEventNotifier notifier, IScoreBehavior<T> scoreBehavior, GameStatsHandler gameStatsHandler) // Dependency injection
        {
            // 1: DEPENDENCY INJECTION
            // 2: In this specific case, the constructor is taking in interfaces/abstractions rather than concrete classes. 
            //    Each of these dependencies (IBox, IDiceRoller, GameEventNotifier, IScoreBehavior) is necessary for the GameController's functionality, 
            //    but the GameController does not need to know the details of their implementations. 
            //    This design allows different implementations of these abstractions to be "injected" when the GameController is created, 
            //    promoting loose coupling and high cohesion.
            // 3: The motivation behind using this concept here is to promote a design that is flexible, modular, and testable. 
            //    By depending on abstractions and not specific implementations, we can easily swap out, 
            //    mock, or change underlying functionality without altering the GameController class. This approach adheres to the SOLID principles, 
            //    particularly the Dependency Inversion Principle, which stipulates that classes should depend upon abstractions and not upon concrete details.
            _box = box;
            _diceRoller = diceRoller;
            _notifier = notifier;
            _scoreBehavior = scoreBehavior;

            // 1: OBSERVER PATTERN (subscription).
            // 2: Here, the GameController is subscribing to the GameEvent event using a method (OnGameEvent). 
            //    When the GameEventNotifier triggers the GameEvent, the OnGameEvent method will be called.
            // 3: This pattern is used here to allow GameController to react to game events. 
            //    The subscription model lets multiple different handlers (in this or other classes) be attached, 
            //    promoting low coupling for greater flexibility and easier expansion or modification.
            _notifier.GameEvent += OnGameEvent;

            // 1: MULTICAST DELEGATES
            // 2: Here, we use multicast delegates to allow multiple methods to subscribe and respond to game events.
            //    This is used to generate event-driven updates to game statistics and achievements in the GameStatsHandler object.
            //    The "core" of these multicast delegates is created in the GameStatsHandler class,
            //    but we also let the Notifier wish the player good luck at the start of each game.
            // 3: We use this to demonstrate event-driven programming, where multiple methods can be notified when a game event occurs.
            //    It allows us to easily add new kinds of statistics and achievements without changing the GameController class.
            //    We simply need to provde another IGameStatsHandler implementation to change how the statistics and achievements work.
            //    Other classes like the GameEventNotifier can also subscribe to these events to provide additional functionality.
            _onNewGameEvent = gameStatsHandler.OnNewGameEvent;
            _onNewGameEvent += () => {_notifier.Notify("Good luck!!");};
            _onRollEvent = gameStatsHandler.OnRollEvent;
            _onGameEndEvent = gameStatsHandler.OnGameEndEvent;
            
        }

        public void SetOutputHandler(OutputHandler outputHandler)
        {
            _outputHandler = outputHandler ?? throw new ArgumentNullException(nameof(outputHandler));
        }

        private void OnGameEvent(string message)
        {
            _outputHandler(message);
        }

        public bool StartGame(IDisplayable dice1Displayable, IDisplayable dice2Displayable, IDisplayable groundSymb, IDisplayable spaceSymb)
        {
            // resets and collects statistics
            // shows welcoming message
            _onNewGameEvent?.Invoke();
            
            System.Threading.Thread.Sleep(2000); // To give the player time to read the message

            while (true) // Game loop
            {
                // DISPLAY DICE ROLL ANIMATION
                DiceAnimator<IDisplayable> anim = new DiceAnimator<IDisplayable>(20);
                anim.Animate(dice1Displayable, dice2Displayable, groundSymb, spaceSymb);

                _box.DisplayBoard();
                var (dice1, dice2) = _diceRoller.RollDice();

                _outputHandler(_diceRoller.VisualizeDice(dice1));
                _outputHandler(_diceRoller.VisualizeDice(dice2));

                int selection = GetPlayerMove(dice1, dice2);
                
                // add roll event to statistics
                _onRollEvent?.Invoke(dice1 + dice2, DateTime.Now);

                if (selection == -1) // The player has no valid moves -> GAME OVER:(
                {
                    var message = _scoreBehavior.DisplayScore(_box);
                    Console.Clear();
                    _outputHandler($"GAME OVER!\n\n{message}");
                    // calculates and displays stats, achievements and shows end screen
                    _onGameEndEvent?.Invoke();
                    break;
                }

                var scoreInfo = _scoreBehavior.DisplayScore(_box);
                _box.ShutTile(selection);

                if (GameWon())
                {
                    _outputHandler("Congratulations, you have shut the box!");
                    // calculates and displays stats, achievements and shows end screen
                    _onGameEndEvent?.Invoke();
                    break;
                }
                // End of game loop
            }

            _outputHandler("Press Enter to play again or type 'exit' to quit.");
            string userAction = Console.ReadLine();
            return string.IsNullOrEmpty(userAction) || !userAction.Equals("exit", StringComparison.OrdinalIgnoreCase);
        }

        private int GetPlayerMove(int dice1, int dice2)
        {
            // 1: COLLECTIONS (HashSet)
            // 2: A HashSet named 'validOptions' is used to store possible unique tile numbers that the player can shut based on the current dice roll. 
            //    It automatically ensures that no duplicates are entered, maintaining a collection of unique valid options.
            // 3: The HashSet is ideal here to avoid any duplicate entries effortlessly, 
            //    which can occur if both dice have the same value or add up to a value of an unshut tile. 
            //    It simplifies ensuring the player's selections are valid and unique, improving the clarity and reliability of the game logic.
            var validOptions = new HashSet<int>();
            int sum = dice1 + dice2;
            if (!_box.IsTileShut(dice1) && dice1 <= 9) validOptions.Add(dice1);
            if (!_box.IsTileShut(dice2) && dice2 <= 9) validOptions.Add(dice2);
            if (sum <= 9 && !_box.IsTileShut(sum)) validOptions.Add(sum);

            // Game over condition
            if (!validOptions.Any())
            {
                return -1;
            }

            // Inform the player of the valid options and ask for input.
            // Continue asking until the player provides a valid input.
            _outputHandler($"Options: {string.Join(", ", validOptions)}. Enter the number of the tile you want to shut:");
            while (true)
            {
                string input = Console.ReadLine();
                if (int.TryParse(input, out int selection) && validOptions.Contains(selection))
                {
                    return selection;
                }
                else
                {
                    _outputHandler("Invalid selection. Please choose one of the available options:");
                }
            }
        }

        private bool GameWon()
        {
            // 1: LINQ   
            // 2: Here, we're asking LINQ to check if any of our 'tiles' are unchecked. 
            // 3: We do this because it saves time and work, especially if we have a lot of tiles.                 
            //    It means we only do the full check when we really need to use the list of unshut tile numbers somewhere else in our program.
            //    Also, it's just nice to work with:)
            return !_box.UnshutTiles.Any();
        }
    }
}