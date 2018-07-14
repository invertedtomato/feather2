using InvertedTomato.IO.Messages;
using InvertedTomato.Net.Feather;
using System;

namespace NetLibraryTcpServerSample {
    class Program {
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
    }
}
