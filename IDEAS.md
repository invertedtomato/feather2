# Server
```C#
// Like the old 'Connection' in Feather 1, but contains session state, not the actual connection
public class Session : SessionBase {
	// SessionBase has Address and Socket in it, but you can add whatever else you want here for the session's context
	
	// This isn't manditory here (it can be handled globally later), but here for Feather 1 consistancy
	public void OnMessageReceived(Message msg){
		// Read opcode
		var opcode = msg.ReadUnsignedInteger();
		
		// etc.. read other data the same as Feather 1
	}
	
	// This isn't manditory either, just added so as to be consistant with Feather 1
	public void SendMessage(Message msg){
		Socket.Send(Address, msg);
	}
}

class Program {
    static void Main(string[] args) {
	using (var server = new FeatherSocket<Session, GenericMessage>()) { // Session = your session context (class above) 
										// GenericMessage = your message type. I've written a
										// few already for different projects. Have a look here
										// https://github.com/invertedtomato/messages/tree/master/Library/IO/Messages
										// Basically there's a StringMessage if you want to send
										// strings, or BinaryMessage for basic binary payloads.
										// GenericMessage is like Feather 1, but with some basic
										// compression. More details below on that...
		// Setup handler for when client connects - a new instance of Session will be created
		server.OnClientConnected += (session)= > {
			Console.WriteLine($"{session.Address} has connected.");
		
			// Compose a message
			var msg = new GenericMessage(); 
							// See https://github.com/invertedtomato/messages/blob/master/Library/IO/Messages/GenericMessage.cs
							// Note that there's no Opcode. Just use the first field for the same 
							// purpose. You can use whatever datatype you'd like for that (heck string if
							// you'd prefer).
							
			// Write the first field
			msg.WriteUnsignedInteger(5);	// As above, in GenericMessage fields are compressed. The integer '5' will be
							// sent as a single byte. Larger values will use more. Here's a crash course on
							// VLQ if you need: https://en.wikipedia.org/wiki/Variable-length_quantity
							// I chose VLQ as it's really fast and great for integers. It's useless for
							// floats and strings though - so they're uncompressed in GenericMessage.
			
			// ... write other fields
			
			// Send the message
			server.SendMessage(session.Address, msg);
		};

		// Setup handler for when client disconnects
		server.OnClientDisconnected += (session) => {
			Console.WriteLine($"{session.Address} has disconnected.");
		};

		// Setup handler for inbound messages
		server.OnMessageReceived += (session, msg) => {
			// You could globally handle messages here, or to be like Feather 1 we could do this to handle it in the session's context like this:
			session.OnMessageReceived(msg);
		};

	// Start listening for connections on TCP 777 - could equally have been UDP, also possible to listen on multiple ports/protocols at once
	server.ListenTcp(777) // Alternatively ListenTcpSsl(777, <reference to SSL cert>)
				/// alternatively ListenUdp(777)

	// Keep running until stopped
	Console.WriteLine("Server running. Press any key to terminate.");
	Console.ReadKey(true);
}
```

# Client
```c#
class Program {
    static void Main(string[] args) {
        // Connect to server
        using (var client = new FeatherSocket<GenericMessage>()){ // Since I didn't give a session context type it will use SessionBase
									// automatically. On a client it's a little redundant
		// Setup handler for received messages
		client.OnMessageRecieved += (session, msg) => {
			// Read the message
			var opcode = msg.ReadUnsignedInteger();
			// etc.
		};
		
		// Connect to a remote server
		client.ConnectTcp("localhost", 777);
		
		// Get user's name
		Console.WriteLine("What's your name?");
		var userName = Console.ReadLine();

		// Go in a loop, sending any message the user types
		Console.WriteLine("Ready. Type your messages to send.");
		while (true) {
			Console.Write(userName + "> ");
			var message =  Console.ReadLine();
			if (!string.IsNullOrEmpty(message)) {
				var msg = new GenericMessage();
				// ...
				
				// Send message - since we're only going to be connected to one server, send to all
				client.SendAll(msg);
			}
            }
        }
    }
}
```

