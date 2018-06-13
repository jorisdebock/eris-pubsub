# Eris PubSub

[![](https://img.shields.io/nuget/v/Eris.PubSub.svg)](https://www.nuget.org/packages/Eris.PubSub)
[![](https://joris.visualstudio.com/_apis/public/build/definitions/b5bf31cd-d10a-4ddb-afc6-e9746c2c9c31/13/badge)](https://github.com/wazowsk1/eris-pubsub)

A simple pubsub implementation

```c#
class Message : IMessage { }

IHub hub = new Hub();

// subscribe to "Message"
hub.Subscribe<Message>(message => {});

// subscribe to "IMessage" interface and receive all messages derived from the interface, same thing can be done with a derived class
hub.Subscribe<IMessage>(message => {});

// publish "Message"
hub.Publish(new Message());

// publish "Message" with a delay
hub.Publish(new Message(), TimeSpan.FromSeconds(1));

// pass through the messages from one hub to another
// now "hub2" will receive all messages that are being send to "hub"
IHub hub2 = new Hub();
hub.PassThrough(hub2);
```
