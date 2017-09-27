using System;

namespace Eris.PubSub
{
    public interface IHub
    {
        void Publish<T>(T message, TimeSpan? delay = null) where T : IMessage;

        void Subscribe<T>(Action<T> messageAction) where T : IMessage;

        void Unsubscribe<T>(Action<T> messageAction) where T : IMessage;

        void PassThrough(IHub hub);
    }
}