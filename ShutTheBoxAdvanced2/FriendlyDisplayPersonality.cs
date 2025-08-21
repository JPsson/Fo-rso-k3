using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShutTheBoxAdvanced2
{
    public class FriendlyDisplayPersonality : IDisplayPersonality
    {
        public string BadAttempt => "Oh no! You can do better than that!";
        public string OkAttempt => "Not bad! Try again!";
        public string GoodAttempt => "So close! Try again!";
        public string Win => "Congratulations, you have shut the box!";
    }
}