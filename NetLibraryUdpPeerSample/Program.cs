using InvertedTomato.IO.Messages;
using InvertedTomato.Net.Feather;
using System;
using System.Net;

namespace NetLibraryUdpPeerSample {
    class Program {
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
    }
}
