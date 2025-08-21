using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShutTheBoxAdvanced2
{
    public class EvilDisplayPersonality : IDisplayPersonality
    {
        public string BadAttempt => "Ha! You're terrible at this!";
        public string OkAttempt => "Really, thats all you've got?";
        public string GoodAttempt => "Lol, I don't think you will ever win this game!";
        public string Win => "You may have won this time, but I'm not that impresseed tbh!";
    }
}