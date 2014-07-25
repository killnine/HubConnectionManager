##HubConnectionManager##

###Overview###

HubConnectionManager is a simple wrapper class around the [HubConnection](http://www.asp.net/signalr/overview/signalr-20/hubs-api/hubs-api-guide-net-client) class SignalR uses to establish a connection with a server.

###NuGet Installation###
`> Install-Package HubConnectionManager`

https://www.nuget.org/packages/HubConnectionManager/

###Background###

Prior to `HubConnectionManager`, if I happened to launch a client applicaiton when its corresponding SignalR service was unavailable, I'd have to restart the whole application for it to recover. Now, with `HubConnectionManager`, I can ensure that the client will attempt to re-establish a connection with the server. And it works!

Furthermore, wrapping the HubConnection in an `IHubConnectionManager` interface means that mocking connection state is easy. Hooray for comprehensive unit testing!

###Usage###

1. Create an instance of HubConnectionManager through the static `GetHubConnectionManager` method, providing the url of your SignalR endpoint:

```c#
HubConnectionManager connectionManager = HubConnectionManager.GetHubConnectionManager("http://localhost:6789");
```

2. Create you `IHubProxy` via the `CreateHubProxy` method, passing in the name of your Hub:

```c#
IHubProxy clientProxy = connectionManager.CreateHubProxy("ClientHub");
```

3. Initialize the Manager via `Initialize()` method:

```c#
connectionManager.Initialize();
```

That's it! As long as you have a reference to the `HubConnectionManager` like you'd have a reference to your `HubConnection` in the past, it should work just the same.

###Additional Functionality###

I also expose many of the events and properties of HubConnection:

####Events####
* Error
* Received
* Closed
* Reconnecting
* Reconnected
* ConnectionSlow
* StateChanged

####Properties####
* State
