using InvertedTomato.IO.Messages;
using InvertedTomato.Net.Feather;
using System;
using System.Net;

namespace NetLibrarySample {
    class Program {
        static void Main(string[] args) {
            Console.WriteLine("What is your name?");
            var myName = Console.ReadLine();

            // TCP SERVER
            using (var server = new FeatherTcpServer<GenericMessage>()) {
                // Setup receive handler
                server.OnMessageReceived += (endpoint, message) => {
                    var param1 = message.ReadString();
                    var param2 = message.ReadString();
                    // ...repeat for as many paramaters are expected

                    Console.WriteLine($"{param1}> {param2}");

                    // Send reply message
                    var replyMessage = new GenericMessage();
                    replyMessage.WriteBoolean(true);
                    server.SendTo(endpoint, replyMessage);
                };

                // Listen for inbound connections
                server.Listen(12345);
                
                using (var client = new FeatherTcpClient<GenericMessage>()) {
                    // Setup receive handler
                    client.OnMessageReceived += (message) => {
                        var param1 = message.ReadString();
                        var param2 = message.ReadString();
                        // ...repeat for as many paramaters are expected

                        Console.WriteLine($"{param1}> {param2}");
                    };

                    // Connect to server
                    client.Connect("127.0.0.1", 12345);

                    // Loop sending messages
                    Console.WriteLine("Type message to send.");
                    while (true) {
                        var content = Console.ReadLine();
                        if (content == string.Empty) {
                            break;
                        }

                        // Create message
                        var message = new GenericMessage();
                        message.WriteString(myName);
                        message.WriteString(content);

                        // Send message to broadcast address
                        client.Send(message);
                    }
                }
            }

            Console.WriteLine("Done.");

            // UDP PEER
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
                while (true) {
                    var content = Console.ReadLine();
                    if (content == string.Empty) {
                        break;
                    }

                    // Create message
                    var message = new GenericMessage();
                    message.WriteString(myName);
                    message.WriteString(content);

                    // Send message to broadcast address
                    peer.SendTo(new IPEndPoint(IPAddress.Broadcast, 12345), message);
                }
            }

            Console.WriteLine("Done.");
        }
    }
}
