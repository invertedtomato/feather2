# InvertedTomato.Feather2
Feather2 is extremely fast and lightweight network messaging system. It's kinda like WCF, without the nonsense and 
with scolding fast speeds. It's great for applications communicating over a network when speed, efficiency or the 
one-way nature of traditional web APIs isn't a great fit.

Feather2 is the replacement of Feather1. Feather1 was great, though it's interface was cumbersome in areas, it lacked
flexibility and UDP support and it was limited to .NET Framework. Feather2 addresses all of those issues.

## TLDR, how do I make it go?
### TCP Server
```c#
static void Main(string[] args) {
	using (var server = new FeatherTcpServer<GenericMessage>()) {
		// Watch for when clients connect
		server.OnClientConnected += (endPoint) => {
			Console.WriteLine($"{endPoint} connected.");
		};

		// Watch for when clients disconnect
		server.OnClientDisconnected += (endPoint, reason) => {
			Console.WriteLine($"{endPoint} disconnected.");
		};

		// Watch for messages to arrive
		server.OnMessageReceived += (endPoint, message) => {
			// Read parameters in the same order they were written
			var name = message.ReadString();
			var body = message.ReadString();
			Console.WriteLine($"{name}> {body}");

			// Forward message on to all clients (including the sender, for confirmation)
			foreach (var remoteEndPoint in server.RemoteEndPoints) {
				server.SendTo(remoteEndPoint, message);
			}
		};

		// Listen for inbound connections
		server.Listen(12345);

		// Wait for key press to shutdown
		Console.WriteLine("Server now listening for connections on port 12345. Press any key to halt.");
		Console.ReadKey(true);
	}
}
```

### TCP Client
```c#
static void Main(string[] args) {
    Console.WriteLine("What is your name?");
    var myName = Console.ReadLine();

    using (var client = new FeatherTcpClient<GenericMessage>()) {
        // Watch for messages to arrive
        client.OnMessageReceived += (message) => {
            // Read parameters in the same order they were written
            var name = message.ReadString();
            var body = message.ReadString();
            Console.WriteLine($"{name}> {body}");
        };

        // Watch for disconnetion
        client.OnDisconnected += (reason) => {
            Console.WriteLine($"Disconnected because '{reason}'.");
        };

        // Connect to server
        client.Connect("127.0.0.1", 12345);

        // Loop sending messages
        Console.WriteLine("Connected. Type message to send.");
        while (true) {
            var body = Console.ReadLine();
            if (body == string.Empty) {
                break;
            }

            // Create message
            var message = new GenericMessage();
            message.WriteString(myName);
            message.WriteString(body);

            // Send message to broadcast address
            client.Send(message);
        }
    }
}
```

### UDP Peer
```c#
static void Main(string[] args) {
    Console.WriteLine("What is your name?");
    var myName = Console.ReadLine();
            
    using (var peer = new FeatherUdpPeer<GenericMessage>()) {
        // Watch for messages to arrive
        peer.OnMessageReceived += (endpoint, message) => {
            var name = message.ReadString();
            var body = message.ReadString();
            Console.WriteLine($"{name}> {body}");
        };

        // Bind socket to receive messages
        peer.Bind(12345);

        // Loop sending messages
        Console.WriteLine("Type message to send.");
        while (true) {
            var body = Console.ReadLine();
            if (body == string.Empty) {
                break;
            }

            // Create message
            var message = new GenericMessage();
            message.WriteString(myName);
            message.WriteString(body);

            // Broadcast message on the local network (similarly, we could send to a specific address on the internet instead)
            peer.SendTo(new IPEndPoint(IPAddress.Broadcast, 12345), message);
        }
    }
}
```

## Where do I get it from?
You can [find it on NuGet](https://www.nuget.org/packages/InvertedTomato.Feather2.Net/).