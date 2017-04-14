using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace InvertedTomato.Testable {
    public class SocketReal : ISocket {
        private readonly Socket Underlying;

        public SocketReal(Socket underlying) {
            Underlying = underlying ?? throw new ArgumentNullException("underlying");
        }

        public int Available { get { return Underlying.Available; } }
        public bool Blocking { get { return Underlying.Blocking; } set { Underlying.Blocking = value; } }

        public bool Connected { get { return Underlying.Connected; } }

        public bool DontFragment { get { return Underlying.DontFragment; } set { Underlying.DontFragment = value; } }
        public bool DualMode { get { return Underlying.DualMode; } set { Underlying.DualMode = value; } }
        public bool EnableBroadcast { get { return Underlying.EnableBroadcast; } set { Underlying.EnableBroadcast = value; } }
        public bool ExclusiveAddressUse { get { return Underlying.ExclusiveAddressUse; } set { Underlying.ExclusiveAddressUse = value; } }

        public bool IsBound { get { return Underlying.IsBound; } }

        public EndPoint LocalEndPoint { get { return Underlying.LocalEndPoint; } }

        public AddressFamily AddressFamily { get { return Underlying.AddressFamily; } }

        public bool MulticastLoopback { get { return Underlying.MulticastLoopback; } set { Underlying.MulticastLoopback = value; } }
        public bool NoDelay { get { return Underlying.NoDelay; } set { Underlying.NoDelay = value; } }

        public ProtocolType ProtocolType { get { return Underlying.ProtocolType; } }

        public int ReceiveBufferSize { get { return Underlying.ReceiveBufferSize; } set { Underlying.ReceiveBufferSize = value; } }
        public int ReceiveTimeout { get { return Underlying.ReceiveTimeout; } set { Underlying.ReceiveTimeout = value; } }

        public EndPoint RemoteEndPoint { get { return Underlying.RemoteEndPoint; } }

        public int SendBufferSize { get { return Underlying.SendBufferSize; } set { Underlying.SendBufferSize = value; } }
        public int SendTimeout { get { return Underlying.SendTimeout; } set { Underlying.SendTimeout = value; } }
        public LingerOption LingerState { get { return Underlying.LingerState; } set { Underlying.LingerState = value; } }

        public SocketType SocketType { get { return Underlying.SocketType; } }

        public short Ttl { get { return Underlying.Ttl; } set { Underlying.Ttl = value; } }

        public Socket Accept() { return Underlying.Accept(); }

        public bool AcceptAsync(SocketAsyncEventArgs e) { return Underlying.AcceptAsync(e); }

        public void Bind(EndPoint localEP) { Bind(localEP); }

        public void Connect(IPAddress[] addresses, int port) { Connect(addresses, port); }

        public void Connect(string host, int port) { Connect(host, port); }

        public void Connect(EndPoint remoteEP) { Connect(remoteEP); }

        public void Connect(IPAddress address, int port) { Connect(address, port); }

        public bool ConnectAsync(SocketAsyncEventArgs e) { return ConnectAsync(e); }

        public void Dispose() { Underlying.Dispose(); }

        public void GetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, byte[] optionValue) { Underlying.GetSocketOption(optionLevel, optionName, optionValue); }

        public byte[] GetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, int optionLength) { return Underlying.GetSocketOption(optionLevel, optionName, optionLength); }

        public object GetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName) { return Underlying.GetSocketOption(optionLevel, optionName); }

        public int IOControl(int ioControlCode, byte[] optionInValue, byte[] optionOutValue) { return IOControl(ioControlCode, optionInValue, optionOutValue); }

        public int IOControl(IOControlCode ioControlCode, byte[] optionInValue, byte[] optionOutValue) { return Underlying.IOControl(ioControlCode, optionInValue, optionOutValue); }

        public void Listen(int backlog) { Underlying.Listen(backlog); }

        public bool Poll(int microSeconds, SelectMode mode) { return Underlying.Poll(microSeconds, mode); }

        public int Receive(byte[] buffer, int size, SocketFlags socketFlags) { return Underlying.Receive(buffer, size, socketFlags); }

        public int Receive(byte[] buffer, SocketFlags socketFlags) { return Underlying.Receive(buffer, socketFlags); }

        public int Receive(byte[] buffer, int offset, int size, SocketFlags socketFlags, out SocketError errorCode) { return Underlying.Receive(buffer, offset, size, socketFlags, out errorCode); }

        public int Receive(IList<ArraySegment<byte>> buffers, SocketFlags socketFlags, out SocketError errorCode) { return Underlying.Receive(buffers, socketFlags, out errorCode); }

        public int Receive(IList<ArraySegment<byte>> buffers, SocketFlags socketFlags) { return Underlying.Receive(buffers, socketFlags); }

        public int Receive(IList<ArraySegment<byte>> buffers) { return Underlying.Receive(buffers); }

        public int Receive(byte[] buffer, int offset, int size, SocketFlags socketFlags) { return Underlying.Receive(buffer, offset, size, socketFlags); }

        public int Receive(byte[] buffer) { return Underlying.Receive(buffer); }

        public bool ReceiveAsync(SocketAsyncEventArgs e) { return Underlying.ReceiveAsync(e); }

        public int ReceiveFrom(byte[] buffer, SocketFlags socketFlags, ref EndPoint remoteEP) { return Underlying.ReceiveFrom(buffer, socketFlags, ref remoteEP); }

        public int ReceiveFrom(byte[] buffer, ref EndPoint remoteEP) { return Underlying.ReceiveFrom(buffer, ref remoteEP); }

        public int ReceiveFrom(byte[] buffer, int size, SocketFlags socketFlags, ref EndPoint remoteEP) { return Underlying.ReceiveFrom(buffer, size, socketFlags, ref remoteEP); }

        public int ReceiveFrom(byte[] buffer, int offset, int size, SocketFlags socketFlags, ref EndPoint remoteEP) { return Underlying.ReceiveFrom(buffer, offset, size, socketFlags, ref remoteEP); }

        public bool ReceiveFromAsync(SocketAsyncEventArgs e) { return Underlying.ReceiveFromAsync(e); }

        public int ReceiveMessageFrom(byte[] buffer, int offset, int size, ref SocketFlags socketFlags, ref EndPoint remoteEP, out IPPacketInformation ipPacketInformation) { return Underlying.ReceiveMessageFrom(buffer, offset, size, ref socketFlags, ref remoteEP, out ipPacketInformation); }

        public bool ReceiveMessageFromAsync(SocketAsyncEventArgs e) { return Underlying.ReceiveMessageFromAsync(e); }

        public int Send(byte[] buffer, int offset, int size, SocketFlags socketFlags) { return Underlying.Send(buffer, offset, size, socketFlags); }

        public int Send(byte[] buffer) { return Underlying.Send(buffer);        }

        public int Send(byte[] buffer, int offset, int size, SocketFlags socketFlags, out SocketError errorCode) { return Underlying.Send(buffer, offset, size, socketFlags, out errorCode); }

        public int Send(byte[] buffer, int size, SocketFlags socketFlags) { return Underlying.Send(buffer, size, socketFlags); }

        public int Send(byte[] buffer, SocketFlags socketFlags) { return Underlying.Send(buffer, socketFlags); }

        public int Send(IList<ArraySegment<byte>> buffers) { return Underlying.Send(buffers); }

        public int Send(IList<ArraySegment<byte>> buffers, SocketFlags socketFlags, out SocketError errorCode) { return Underlying.Send(buffers, socketFlags, out errorCode); }

        public int Send(IList<ArraySegment<byte>> buffers, SocketFlags socketFlags) { return Underlying.Send(buffers, socketFlags); }

        public bool SendAsync(SocketAsyncEventArgs e) { return Underlying.SendAsync(e); }

        public bool SendPacketsAsync(SocketAsyncEventArgs e) { return Underlying.SendPacketsAsync(e); }

        public int SendTo(byte[] buffer, SocketFlags socketFlags, EndPoint remoteEP) { return Underlying.SendTo(buffer, socketFlags, remoteEP); }

        public int SendTo(byte[] buffer, EndPoint remoteEP) { return Underlying.SendTo(buffer, remoteEP); }

        public int SendTo(byte[] buffer, int size, SocketFlags socketFlags, EndPoint remoteEP) { return Underlying.SendTo(buffer, size, socketFlags, remoteEP); }

        public int SendTo(byte[] buffer, int offset, int size, SocketFlags socketFlags, EndPoint remoteEP) { return Underlying.SendTo(buffer, offset, size, socketFlags, remoteEP); }

        public bool SendToAsync(SocketAsyncEventArgs e) { return Underlying.SendToAsync(e); }

        public void SetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, bool optionValue) { Underlying.SetSocketOption(optionLevel, optionName, optionValue); }

        public void SetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, byte[] optionValue) { Underlying.SetSocketOption(optionLevel, optionName, optionValue); }

        public void SetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, int optionValue) { Underlying.SetSocketOption(optionLevel, optionName, optionValue); }

        public void SetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, object optionValue) { Underlying.SetSocketOption(optionLevel, optionName, optionValue); }
    }
}
