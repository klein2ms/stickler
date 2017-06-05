using System;

namespace Stickler.Conduit
{
    public class EventSubscription<TEvent>
    {
        public Action<TEvent> Action { get; }

        public EventSubscription(Action<TEvent> action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            Action = action;
        } 
    }
}
