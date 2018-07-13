using System;
using System.Net.Sockets;

namespace InvertedTomato.Net {
    public static class SocketExtensions { // http://snipplr.com/view/54476/ https://social.msdn.microsoft.com/Forums/de-DE/d5b6ae25-eac8-4e3d-9782-53059de04628/tcp-keepalive-settings-problem?forum=netfxnetcom
        private const Int32 bytesperlong = 4; // 32 / 8
        private const Int32 bitsperbyte = 8;

        /// <summary>
		/// Sets the keep-alive interval for the socket.
		/// </summary>
		/// <param name="target">The socket.</param>
		/// <param name="sendInterval">Time between two keep alive "pings".</param>
		/// <param name="requiredReceiveInterval">Time between two keep alive "pings" when first one fails.</param>
		/// <returns>If the keep alive infos were succefully modified.</returns>
        public static void SetKeepAlive(this Socket target, TimeSpan sendInterval, TimeSpan requiredReceiveInterval) {
            if(null == target) {
                throw new ArgumentNullException(nameof(target));
            }

            var enabled = sendInterval.Ticks != 0 && requiredReceiveInterval.Ticks != 0;

            // Put input arguments in input array
            var input1 = new UInt64[3];
            input1[0] = enabled ? 0UL : 1UL;
            input1[1] = (UInt64)sendInterval.TotalMilliseconds; // time millis
            input1[2] = (UInt64)requiredReceiveInterval.TotalMilliseconds; // interval millis

            // Pack input into byte struct
            var input2 = new Byte[3 * bytesperlong];
            for(var i = 0; i < input1.Length; i++) {
                input2[i * bytesperlong + 3] = (Byte)(input1[i] >> ((bytesperlong - 1) * bitsperbyte) & 0xff);
                input2[i * bytesperlong + 2] = (Byte)(input1[i] >> ((bytesperlong - 2) * bitsperbyte) & 0xff);
                input2[i * bytesperlong + 1] = (Byte)(input1[i] >> ((bytesperlong - 3) * bitsperbyte) & 0xff);
                input2[i * bytesperlong + 0] = (Byte)(input1[i] >> ((bytesperlong - 4) * bitsperbyte) & 0xff);
            }

            // write SIO_VALS to Socket IOControl
            target.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.KeepAlive, enabled);
            target.IOControl(IOControlCode.KeepAliveValues, input2, null);
        }

        public static void Send(this Socket target, ArraySegment<byte> payload) {
            target.Send(payload.Array, payload.Offset, payload.Count, SocketFlags.None);
        }


    }
}
