using InvertedTomato.IO.Messages;
using InvertedTomato.Net.Feather;
using System;

namespace NetLibraryTcpClientSample {
    class Program {
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
    }
}
