# InvertedTomato.Feather2
Feather is extremely fast and lightweight network messaging socket. Kinda like WCF, without the nonsense and scolding fast speeds. Great for applications communicating over a network when webAPI is too slow or inefficient. SSL encryption is optional.

Feather2 is still under development. At the moment the UDP implementation is available. See below for an example implementation.

## UDP Peer
```c#
	static void Main(string[] args) {
        Console.WriteLine("What is your name?");
        var myName = Console.ReadLine();

        using (var peer = new FeatherUdpPeer<GenericMessage>()) {
            // Setup receive handler
            peer.OnMessageReceived += (endpoint, message) => {
                var name = message.ReadString();
                var content = message.ReadString();

                Console.WriteLine($"{name}> {content}");
            };

            // Bind socket to receive messages
            peer.Bind(12345);

            // Loop sending messages
            Console.WriteLine("Type message to send.");
            while(true) {
                var content = Console.ReadLine();
                if (content == string.Empty) {
                    break;
                }

                // Create message
                var message = new GenericMessage();
                message.WriteString(myName);
                message.WriteString(content);

                // Send message to broadcast address
                peer.SendToSync(new IPEndPoint(IPAddress.Broadcast, 12345), message);
            }
        }

        Console.WriteLine("Done.");
    }
```
