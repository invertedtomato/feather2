using System.ComponentModel;

namespace InvertedTomato.Net.Feather {
    public enum DisconnectionType : byte {
        [Description("The local side closed the connection.")]
        LocalDisconnection = 1,

        [Description("The remote side closed the connection.")]
        RemoteDisconnection = 2,

        [Description("The connection was unexpectedly interrupted.")]
        ConnectionInterupted = 3,

        [Description("The connection was terminated locally due to malformed data arriving.")]
        MalformedPayload = 4
    }
}
