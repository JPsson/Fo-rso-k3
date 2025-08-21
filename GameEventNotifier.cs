using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShutTheBoxAdvanced2
{
    public class GameEventNotifier
    {
        // 1: GENERIC DELEGATES
        // 2: Here, 'T' represents any type that can be passed as event data. 
        //    This generic delegate allows the GameEventHandler to accept various data types, 
        //    enhancing flexibility regarding the kind of data shared with event subscribers.
        // 3: By using a generic delegate, we ensure our GameEventNotifier is versatile and can handle different types of events, 
        //    not limiting it to specific data types. This design promotes reusability and adaptability, 
        //    accommodating unforeseen future requirements.
        public delegate void GameEventHandler<T>(T eventData);
        // 1: EVENTS
        // 2: 'GameEvent' is based on the generic delegate and can attach multiple listeners (observers). 
        //    When the 'Notify' method is called, all attached observers are notified.
        // 3: This allows decoupling the event sender from receivers, promoting a scalable and maintainable code structure.
        public event GameEventHandler<string> GameEvent;

        public void Notify(string message)
        {
            GameEvent?.Invoke(message);
        }
    }
}