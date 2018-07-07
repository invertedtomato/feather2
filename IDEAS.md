== Server
```C#
public class Session : SessionBase {
	// SessionBase has Address in it, but you can add whatever else you want here for the session's context
	
	public void OnMessageReceived(Message msg){
	
	}
}

using (var server = new FeatherSocket<Session>()) {
	// Setup handler for when client connects - a new instance of Session will be created
	server.OnClientConnected += (session)= > {
		Console.WriteLine($"{session.Address} has connected.");
		
		// Send a message
		var msg = new GenericMessage(); // See https://github.com/invertedtomato/messages/blob/master/Library/IO/Messages/GenericMessage.cs for details, but basically what you're use to already, just without the opcode - so you can use whatever opcode data type you'd like
		msg.WriteUnsignedInteger(5);
		server.SendMessage(session.Address, ...);
	};

	// Setup handler for when client disconnects
	server.OnClientDisconnected += (session) => {
		Console.WriteLine($"{session.Address} has disconnected.");
	};

	// Setup handler for inbound messages
	server.OnMessageReceived += (session, message) => {
		// You could globally handle messages here, or to be like Feather1 we could do this to handle it in the session's context
		session.OnMessageReceived(msg);
	};

	// Start listening for connections on TCP 777 - could equally have been UDP, also possible to listen on multiple ports/protocols at once
	server.Listen(777, Protocol.TCP)

	// Keep running until stopped
	Console.WriteLine("Server running. Press any key to terminate.");
	Console.ReadKey(true);
}
```
