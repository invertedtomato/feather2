using InvertedTomato.IO.Messages;
using InvertedTomato.Net.Feather;
using System;
using System.Net;

namespace NetLibrarySample {
    class Program {
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
    }
}
