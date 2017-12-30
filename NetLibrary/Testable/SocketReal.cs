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

        public Int32 Available { get { return Underlying.Available; } }
        public Boolean Blocking { get { return Underlying.Blocking; } set { Underlying.Blocking = value; } }

        public Boolean Connected { get { return Underlying.Connected; } }

        public Boolean DontFragment { get { return Underlying.DontFragment; } set { Underlying.DontFragment = value; } }
        public Boolean DualMode { get { return Underlying.DualMode; } set { Underlying.DualMode = value; } }
        public Boolean EnableBroadcast { get { return Underlying.EnableBroadcast; } set { Underlying.EnableBroadcast = value; } }
        public Boolean ExclusiveAddressUse { get { return Underlying.ExclusiveAddressUse; } set { Underlying.ExclusiveAddressUse = value; } }

        public Boolean IsBound { get { return Underlying.IsBound; } }

        public EndPoint LocalEndPoint { get { return Underlying.LocalEndPoint; } }

        public AddressFamily AddressFamily { get { return Underlying.AddressFamily; } }

        public Boolean MulticastLoopback { get { return Underlying.MulticastLoopback; } set { Underlying.MulticastLoopback = value; } }
        public Boolean NoDelay { get { return Underlying.NoDelay; } set { Underlying.NoDelay = value; } }

        public ProtocolType ProtocolType { get { return Underlying.ProtocolType; } }

        public Int32 ReceiveBufferSize { get { return Underlying.ReceiveBufferSize; } set { Underlying.ReceiveBufferSize = value; } }
        public Int32 ReceiveTimeout { get { return Underlying.ReceiveTimeout; } set { Underlying.ReceiveTimeout = value; } }

        public EndPoint RemoteEndPoint { get { return Underlying.RemoteEndPoint; } }

        public Int32 SendBufferSize { get { return Underlying.SendBufferSize; } set { Underlying.SendBufferSize = value; } }
        public Int32 SendTimeout { get { return Underlying.SendTimeout; } set { Underlying.SendTimeout = value; } }
        public LingerOption LingerState { get { return Underlying.LingerState; } set { Underlying.LingerState = value; } }

        public SocketType SocketType { get { return Underlying.SocketType; } }

        public Int16 Ttl { get { return Underlying.Ttl; } set { Underlying.Ttl = value; } }

        public Socket Accept() { return Underlying.Accept(); }

        public Boolean AcceptAsync(SocketAsyncEventArgs e) { return Underlying.AcceptAsync(e); }

        public void Bind(EndPoint localEP) { Bind(localEP); }

        public void Connect(IPAddress[] addresses, Int32 port) { Connect(addresses, port); }

        public void Connect(String host, Int32 port) { Connect(host, port); }

        public void Connect(EndPoint remoteEP) { Connect(remoteEP); }

        public void Connect(IPAddress address, Int32 port) { Connect(address, port); }

        public Boolean ConnectAsync(SocketAsyncEventArgs e) { return ConnectAsync(e); }

        public void Dispose() { Underlying.Dispose(); }

        public void GetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, Byte[] optionValue) { Underlying.GetSocketOption(optionLevel, optionName, optionValue); }

        public Byte[] GetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, Int32 optionLength) { return Underlying.GetSocketOption(optionLevel, optionName, optionLength); }

        public Object GetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName) { return Underlying.GetSocketOption(optionLevel, optionName); }

        public Int32 IOControl(Int32 ioControlCode, Byte[] optionInValue, Byte[] optionOutValue) { return IOControl(ioControlCode, optionInValue, optionOutValue); }

        public Int32 IOControl(IOControlCode ioControlCode, Byte[] optionInValue, Byte[] optionOutValue) { return Underlying.IOControl(ioControlCode, optionInValue, optionOutValue); }

        public void Listen(Int32 backlog) { Underlying.Listen(backlog); }

        public Boolean Poll(Int32 microSeconds, SelectMode mode) { return Underlying.Poll(microSeconds, mode); }

        public Int32 Receive(Byte[] buffer, Int32 size, SocketFlags socketFlags) { return Underlying.Receive(buffer, size, socketFlags); }

        public Int32 Receive(Byte[] buffer, SocketFlags socketFlags) { return Underlying.Receive(buffer, socketFlags); }

        public Int32 Receive(Byte[] buffer, Int32 offset, Int32 size, SocketFlags socketFlags, out SocketError errorCode) { return Underlying.Receive(buffer, offset, size, socketFlags, out errorCode); }

        public Int32 Receive(IList<ArraySegment<Byte>> buffers, SocketFlags socketFlags, out SocketError errorCode) { return Underlying.Receive(buffers, socketFlags, out errorCode); }

        public Int32 Receive(IList<ArraySegment<Byte>> buffers, SocketFlags socketFlags) { return Underlying.Receive(buffers, socketFlags); }

        public Int32 Receive(IList<ArraySegment<Byte>> buffers) { return Underlying.Receive(buffers); }

        public Int32 Receive(Byte[] buffer, Int32 offset, Int32 size, SocketFlags socketFlags) { return Underlying.Receive(buffer, offset, size, socketFlags); }

        public Int32 Receive(Byte[] buffer) { return Underlying.Receive(buffer); }

        public Boolean ReceiveAsync(SocketAsyncEventArgs e) { return Underlying.ReceiveAsync(e); }

        public Int32 ReceiveFrom(Byte[] buffer, SocketFlags socketFlags, ref EndPoint remoteEP) { return Underlying.ReceiveFrom(buffer, socketFlags, ref remoteEP); }

        public Int32 ReceiveFrom(Byte[] buffer, ref EndPoint remoteEP) { return Underlying.ReceiveFrom(buffer, ref remoteEP); }

        public Int32 ReceiveFrom(Byte[] buffer, Int32 size, SocketFlags socketFlags, ref EndPoint remoteEP) { return Underlying.ReceiveFrom(buffer, size, socketFlags, ref remoteEP); }

        public Int32 ReceiveFrom(Byte[] buffer, Int32 offset, Int32 size, SocketFlags socketFlags, ref EndPoint remoteEP) { return Underlying.ReceiveFrom(buffer, offset, size, socketFlags, ref remoteEP); }

        public Boolean ReceiveFromAsync(SocketAsyncEventArgs e) { return Underlying.ReceiveFromAsync(e); }

        public Int32 ReceiveMessageFrom(Byte[] buffer, Int32 offset, Int32 size, ref SocketFlags socketFlags, ref EndPoint remoteEP, out IPPacketInformation ipPacketInformation) { return Underlying.ReceiveMessageFrom(buffer, offset, size, ref socketFlags, ref remoteEP, out ipPacketInformation); }

        public Boolean ReceiveMessageFromAsync(SocketAsyncEventArgs e) { return Underlying.ReceiveMessageFromAsync(e); }

        public Int32 Send(Byte[] buffer, Int32 offset, Int32 size, SocketFlags socketFlags) { return Underlying.Send(buffer, offset, size, socketFlags); }

        public Int32 Send(Byte[] buffer) { return Underlying.Send(buffer);        }

        public Int32 Send(Byte[] buffer, Int32 offset, Int32 size, SocketFlags socketFlags, out SocketError errorCode) { return Underlying.Send(buffer, offset, size, socketFlags, out errorCode); }

        public Int32 Send(Byte[] buffer, Int32 size, SocketFlags socketFlags) { return Underlying.Send(buffer, size, socketFlags); }

        public Int32 Send(Byte[] buffer, SocketFlags socketFlags) { return Underlying.Send(buffer, socketFlags); }

        public Int32 Send(IList<ArraySegment<Byte>> buffers) { return Underlying.Send(buffers); }

        public Int32 Send(IList<ArraySegment<Byte>> buffers, SocketFlags socketFlags, out SocketError errorCode) { return Underlying.Send(buffers, socketFlags, out errorCode); }

        public Int32 Send(IList<ArraySegment<Byte>> buffers, SocketFlags socketFlags) { return Underlying.Send(buffers, socketFlags); }

        public Boolean SendAsync(SocketAsyncEventArgs e) { return Underlying.SendAsync(e); }

        public Boolean SendPacketsAsync(SocketAsyncEventArgs e) { return Underlying.SendPacketsAsync(e); }

        public Int32 SendTo(Byte[] buffer, SocketFlags socketFlags, EndPoint remoteEP) { return Underlying.SendTo(buffer, socketFlags, remoteEP); }

        public Int32 SendTo(Byte[] buffer, EndPoint remoteEP) { return Underlying.SendTo(buffer, remoteEP); }

        public Int32 SendTo(Byte[] buffer, Int32 size, SocketFlags socketFlags, EndPoint remoteEP) { return Underlying.SendTo(buffer, size, socketFlags, remoteEP); }

        public Int32 SendTo(Byte[] buffer, Int32 offset, Int32 size, SocketFlags socketFlags, EndPoint remoteEP) { return Underlying.SendTo(buffer, offset, size, socketFlags, remoteEP); }

        public Boolean SendToAsync(SocketAsyncEventArgs e) { return Underlying.SendToAsync(e); }

        public void SetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, Boolean optionValue) { Underlying.SetSocketOption(optionLevel, optionName, optionValue); }

        public void SetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, Byte[] optionValue) { Underlying.SetSocketOption(optionLevel, optionName, optionValue); }

        public void SetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, Int32 optionValue) { Underlying.SetSocketOption(optionLevel, optionName, optionValue); }

        public void SetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, Object optionValue) { Underlying.SetSocketOption(optionLevel, optionName, optionValue); }
    }
}
