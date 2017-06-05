using System;

namespace Stickler.Conduit
{
    public interface IEventAggregator : IDisposable
    {
        void Publish<TEvent>(TEvent message);
        Guid Subscribe<TEvent>(EventSubscription<TEvent> subscription);
        void Unsubscribe<TEvent>(Guid token);
    }
}
