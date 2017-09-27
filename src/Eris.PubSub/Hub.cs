using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Eris.PubSub
{
    public class Hub : IHub
    {
        private readonly ConcurrentDictionary<Type, IList> _subscribers = new ConcurrentDictionary<Type, IList>();
        private readonly ConcurrentBag<IHub> _hubs = new ConcurrentBag<IHub>();

        public void Publish<T>(T message, TimeSpan? delay = null) where T : IMessage
        {
            Task.Run(() => PublishMessage(message, delay));
        }

        private async Task PublishMessage<T>(T message, TimeSpan? delay = null) where T : IMessage
        {
            if (delay.HasValue)
            {
                await Task.Delay((int)delay.Value.TotalMilliseconds).ConfigureAwait(false);
            }

            var type = typeof(T);
            if (_subscribers.TryGetValue(type, out var messageActions))
            {
                InvokeActions(messageActions, message);
            }

            var typeInfo = type.GetTypeInfo();
            if (typeInfo.BaseType != null && _subscribers.TryGetValue(typeInfo.BaseType, out messageActions))
            {
                InvokeActions(messageActions, message);
            }

            foreach (var t in typeInfo.ImplementedInterfaces)
            {
                if (_subscribers.TryGetValue(t, out messageActions))
                {
                    InvokeActions(messageActions, message);
                }
            }

            foreach (var hub in _hubs)
            {
                hub.Publish(message);
            }
        }

        private void InvokeActions<T>(IList messageActions, T message) where T : IMessage
        {
            foreach (Action<T> messageTask in messageActions)
            {
                Task.Run(() => messageTask.Invoke(message));
            }
        }

        public void Subscribe<T>(Action<T> messageAction) where T : IMessage
        {
            _subscribers.AddOrUpdate(typeof(T),
                new List<Action<T>> { messageAction },
                (type, list) =>
                {
                    list.Add(messageAction);
                    return list;
                });
        }

        public void Unsubscribe<T>(Action<T> messageAction) where T : IMessage
        {
            if (_subscribers.TryGetValue(typeof(T), out var messageActions))
            {
                messageActions.Remove(messageAction);
            }
        }

        public void PassThrough(IHub hub) => _hubs.Add(hub);
    }
}