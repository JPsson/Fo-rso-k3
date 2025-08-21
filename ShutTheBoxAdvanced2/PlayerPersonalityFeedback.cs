using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShutTheBoxAdvanced2
{
    public class PlayerPersonalityFeedback : IScoreBehavior<string>
    {
        private readonly OutputHandler _outputHandler;     // The delegate to handle output.
        private readonly IDisplayPersonality _personality; // different personalities give different messages

        public PlayerPersonalityFeedback(OutputHandler outputHandler, IDisplayPersonality personality)
        {
            // 1: DELEGATES
            // 2: Here, 'outputHandler' is a delegate that points to a method taking a string parameter and returning void. 
            //    It's used as a callback to handle messages outside the current class scope, i.e outputting the score results
            // 3: This approach allows the 'PlayerPersonalityFeedback' class to remain decoupled from the specific output mechanism, 
            //    making the class more flexible and reusable. It can output information without knowing where or how that output is handled.
            _outputHandler = outputHandler ?? throw new ArgumentNullException(nameof(outputHandler));
            _personality = personality;
        }

        public string DisplayScore(IBox box)
        {
            string message;
            int tilesLeft = box.UnshutTiles.Count();

            if (tilesLeft <= 3 && tilesLeft > 1)
            {
                message = _personality.OkAttempt;
            }
            else if (tilesLeft == 1)
            {
                message = _personality.GoodAttempt;
            }
            else if (tilesLeft == 0)
            {
                message = _personality.Win;
            }
            else
            {
                message = _personality.BadAttempt;
            }
            return message;
        }
    }
}