using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShutTheBoxAdvanced2
{
    public class NormalDisplayPersonality : IDisplayPersonality
    {
        public string BadAttempt => "Poor attempt! Try again!";
        public string OkAttempt => "Fair attempt! Try again!";
        public string GoodAttempt => "Soooo close! Try again!";
        public string Win => "Congratulations, you have shut the box!";
    }
}