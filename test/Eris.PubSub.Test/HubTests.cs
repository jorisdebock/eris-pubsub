using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Eris.PubSub.Test
{
    public class HubTests
    {
        private readonly IHub _hub = new Hub();

        private class Message : IMessage
        {
        }

        private class Message2 : IMessage
        {
        }

        private class Message3 : Message
        {
        }

        public interface IMessageHandler
        {
            void Handle(IMessage message);
        }

        [Fact]
        public async Task Publish_Message_Should_Call_Subsriber_Onces()
        {
            var messageHandlerMock = new Mock<IMessageHandler>();

            _hub.Subscribe<Message>(messageHandlerMock.Object.Handle);
            _hub.Publish(new Message());

            // make sure publish has enough time to execute the task
            await Task.Delay(50).ConfigureAwait(false);

            messageHandlerMock.Verify(x => x.Handle(It.IsAny<IMessage>()), Times.Once);
        }

        [Fact]
        public async Task Publish_Message_Should_Call_Subsriber_Twices()
        {
            var messageHandlerMock = new Mock<IMessageHandler>();

            _hub.Subscribe<Message>(messageHandlerMock.Object.Handle);
            _hub.Subscribe<Message>(messageHandlerMock.Object.Handle);

            _hub.Publish(new Message());

            // make sure publish has enough time to execute the task
            await Task.Delay(50).ConfigureAwait(false);

            messageHandlerMock.Verify(x => x.Handle(It.IsAny<IMessage>()), Times.Exactly(2));
        }

        [Fact]
        public async Task Publish_Message_Twices_Should_Call_Subsriber_Twice()
        {
            var messageHandlerMock = new Mock<IMessageHandler>();

            _hub.Subscribe<Message>(messageHandlerMock.Object.Handle);

            _hub.Publish(new Message());
            _hub.Publish(new Message());

            // make sure publish has enough time to execute the task
            await Task.Delay(50).ConfigureAwait(false);

            messageHandlerMock.Verify(x => x.Handle(It.IsAny<IMessage>()), Times.Exactly(2));
        }

        [Fact]
        public async Task Subscribe_Twices_And_Unsubscribe_Ones_Should_Call_Subsriber_Ones()
        {
            var messageHandlerMock = new Mock<IMessageHandler>();

            _hub.Subscribe<Message>(messageHandlerMock.Object.Handle);
            _hub.Subscribe<Message>(messageHandlerMock.Object.Handle);

            _hub.Unsubscribe<Message>(messageHandlerMock.Object.Handle);

            _hub.Publish(new Message());

            // make sure publish has enough time to execute the task
            await Task.Delay(50).ConfigureAwait(false);

            messageHandlerMock.Verify(x => x.Handle(It.IsAny<IMessage>()), Times.Once);
        }

        [Fact]
        public async Task Subscribe_Twices_And_Unsubscribe_Twices_Should_Call_Subsriber_Never()
        {
            var messageHandlerMock = new Mock<IMessageHandler>();

            _hub.Subscribe<Message>(messageHandlerMock.Object.Handle);
            _hub.Subscribe<Message>(messageHandlerMock.Object.Handle);

            _hub.Unsubscribe<Message>(messageHandlerMock.Object.Handle);
            _hub.Unsubscribe<Message>(messageHandlerMock.Object.Handle);

            _hub.Publish(new Message());

            // make sure publish has enough time to execute the task
            await Task.Delay(50).ConfigureAwait(false);

            messageHandlerMock.Verify(x => x.Handle(It.IsAny<IMessage>()), Times.Never);
        }

        [Fact]
        public async Task Publish_Two_Messages_Subscribe_To_Interface_Should_Be_Called_Twices()
        {
            var messageHandlerMock = new Mock<IMessageHandler>();

            _hub.Subscribe<IMessage>(messageHandlerMock.Object.Handle);
            _hub.Publish(new Message());
            _hub.Publish(new Message2());

            // make sure publish has enough time to execute the task
            await Task.Delay(50).ConfigureAwait(false);

            messageHandlerMock.Verify(x => x.Handle(It.IsAny<IMessage>()), Times.Exactly(2));
        }

        [Fact]
        public async Task Publish_Two_Messages_Subscribe_To_Message_Should_Be_Called_Onces()
        {
            var messageHandlerMock = new Mock<IMessageHandler>();

            _hub.Subscribe<Message>(messageHandlerMock.Object.Handle);
            _hub.Publish(new Message());
            _hub.Publish(new Message2());

            // make sure publish has enough time to execute the task
            await Task.Delay(50).ConfigureAwait(false);

            messageHandlerMock.Verify(x => x.Handle(It.IsAny<IMessage>()), Times.Once);
        }

        [Fact]
        public async Task Publish_Messages_With_BaseType_Subscribe_To_Message_BaseType_Should_Be_Called_Onces()
        {
            var messageHandlerMock = new Mock<IMessageHandler>();

            _hub.Subscribe<Message>(messageHandlerMock.Object.Handle);
            _hub.Publish(new Message3());

            // make sure publish has enough time to execute the task
            await Task.Delay(50).ConfigureAwait(false);

            messageHandlerMock.Verify(x => x.Handle(It.IsAny<IMessage>()), Times.Once);
        }

        [Fact]
        public async Task Publish_Messages_With_BaseType_Subscribe_To_Message_Interface_Should_Be_Called_Onces()
        {
            var messageHandlerMock = new Mock<IMessageHandler>();

            _hub.Subscribe<IMessage>(messageHandlerMock.Object.Handle);
            _hub.Publish(new Message3());

            // make sure publish has enough time to execute the task
            await Task.Delay(50).ConfigureAwait(false);

            messageHandlerMock.Verify(x => x.Handle(It.IsAny<IMessage>()), Times.Once);
        }

        [Fact]
        public async Task Subscribe_To_Interface_Should_Be_Called_Onces()
        {
            var messageHandlerMock = new Mock<IMessageHandler>();

            _hub.Subscribe<IMessage>(messageHandlerMock.Object.Handle);
            _hub.Publish(new Message());

            // make sure publish has enough time to execute the task
            await Task.Delay(50).ConfigureAwait(false);

            messageHandlerMock.Verify(x => x.Handle(It.IsAny<IMessage>()), Times.Once);
        }

        [Fact]
        public async Task Publish_With_Delay_Should_Not_Call_Subscriber_Before_Delay()
        {
            var messageHandlerMock = new Mock<IMessageHandler>();

            _hub.Subscribe<Message>(messageHandlerMock.Object.Handle);

            _hub.Publish(new Message(), TimeSpan.FromMilliseconds(250));

            // make sure publish has enough time to execute the task
            await Task.Delay(50).ConfigureAwait(false);

            messageHandlerMock.Verify(x => x.Handle(It.IsAny<IMessage>()), Times.Never);
        }

        [Fact]
        public async Task Publish_With_Delay_Should_Call_Subscriber_After_Delay()
        {
            var messageHandlerMock = new Mock<IMessageHandler>();

            _hub.Subscribe<Message>(messageHandlerMock.Object.Handle);

            _hub.Publish(new Message(), TimeSpan.FromMilliseconds(250));

            // make sure publish has enough time to execute the task
            await Task.Delay(50).ConfigureAwait(false);

            // wait for the publish delay
            await Task.Delay(250).ConfigureAwait(false);

            messageHandlerMock.Verify(x => x.Handle(It.IsAny<IMessage>()), Times.Once);
        }

        [Fact]
        public async Task PassThrough_Messages_To_Other_Hub()
        {
            var localHub = new Hub();
            _hub.PassThrough(localHub);

            var messageHandlerMock = new Mock<IMessageHandler>();

            localHub.Subscribe<IMessage>(messageHandlerMock.Object.Handle);
            _hub.Publish(new Message());

            // make sure publish has enough time to execute the task
            await Task.Delay(50).ConfigureAwait(false);

            messageHandlerMock.Verify(x => x.Handle(It.IsAny<IMessage>()), Times.Once);
        }
    }
}