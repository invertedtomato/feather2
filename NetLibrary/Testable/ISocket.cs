using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace InvertedTomato.Testable {
    public interface ISocket {
        Int32 Available { get; }
        Boolean Blocking { get; set; }
        Boolean Connected { get; }
        Boolean DontFragment { get; set; }
        Boolean DualMode { get; set; }
        Boolean EnableBroadcast { get; set; }
        Boolean ExclusiveAddressUse { get; set; }
        Boolean IsBound { get; }
        EndPoint LocalEndPoint { get; }
        AddressFamily AddressFamily { get; }
        Boolean MulticastLoopback { get; set; }
        Boolean NoDelay { get; set; }
        ProtocolType ProtocolType { get; }
        Int32 ReceiveBufferSize { get; set; }
        Int32 ReceiveTimeout { get; set; }
        EndPoint RemoteEndPoint { get; }
        Int32 SendBufferSize { get; set; }
        Int32 SendTimeout { get; set; }
        LingerOption LingerState { get; set; }
        SocketType SocketType { get; }
        Int16 Ttl { get; set; }
        Socket Accept();
        Boolean AcceptAsync(SocketAsyncEventArgs e);
        void Bind(EndPoint localEP);
        void Connect(IPAddress[] addresses, Int32 port);
        void Connect(String host, Int32 port);
        void Connect(EndPoint remoteEP);
        void Connect(IPAddress address, Int32 port);
        Boolean ConnectAsync(SocketAsyncEventArgs e);
        void Dispose();
        void GetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, Byte[] optionValue);
        Byte[] GetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, Int32 optionLength);
        Object GetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName);
        Int32 IOControl(Int32 ioControlCode, Byte[] optionInValue, Byte[] optionOutValue);
        Int32 IOControl(IOControlCode ioControlCode, Byte[] optionInValue, Byte[] optionOutValue);
        void Listen(Int32 backlog);
        Boolean Poll(Int32 microSeconds, SelectMode mode);
        Int32 Receive(Byte[] buffer, Int32 size, SocketFlags socketFlags);
        Int32 Receive(Byte[] buffer, SocketFlags socketFlags);
        Int32 Receive(Byte[] buffer, Int32 offset, Int32 size, SocketFlags socketFlags, out SocketError errorCode);
        Int32 Receive(IList<ArraySegment<Byte>> buffers, SocketFlags socketFlags, out SocketError errorCode);
        Int32 Receive(IList<ArraySegment<Byte>> buffers, SocketFlags socketFlags);
        Int32 Receive(IList<ArraySegment<Byte>> buffers);
        Int32 Receive(Byte[] buffer, Int32 offset, Int32 size, SocketFlags socketFlags);
        Int32 Receive(Byte[] buffer);
        Boolean ReceiveAsync(SocketAsyncEventArgs e);
        Int32 ReceiveFrom(Byte[] buffer, SocketFlags socketFlags, ref EndPoint remoteEP);
        Int32 ReceiveFrom(Byte[] buffer, ref EndPoint remoteEP);
        Int32 ReceiveFrom(Byte[] buffer, Int32 size, SocketFlags socketFlags, ref EndPoint remoteEP);
        Int32 ReceiveFrom(Byte[] buffer, Int32 offset, Int32 size, SocketFlags socketFlags, ref EndPoint remoteEP);
        Boolean ReceiveFromAsync(SocketAsyncEventArgs e);
        Int32 ReceiveMessageFrom(Byte[] buffer, Int32 offset, Int32 size, ref SocketFlags socketFlags, ref EndPoint remoteEP, out IPPacketInformation ipPacketInformation);
        Boolean ReceiveMessageFromAsync(SocketAsyncEventArgs e);
        Int32 Send(Byte[] buffer, Int32 offset, Int32 size, SocketFlags socketFlags);
        Int32 Send(Byte[] buffer);
        Int32 Send(Byte[] buffer, Int32 offset, Int32 size, SocketFlags socketFlags, out SocketError errorCode);
        Int32 Send(Byte[] buffer, Int32 size, SocketFlags socketFlags);
        Int32 Send(Byte[] buffer, SocketFlags socketFlags);
        Int32 Send(IList<ArraySegment<Byte>> buffers);
        Int32 Send(IList<ArraySegment<Byte>> buffers, SocketFlags socketFlags, out SocketError errorCode);
        Int32 Send(IList<ArraySegment<Byte>> buffers, SocketFlags socketFlags);
        Boolean SendAsync(SocketAsyncEventArgs e);
        Boolean SendPacketsAsync(SocketAsyncEventArgs e);
        Int32 SendTo(Byte[] buffer, SocketFlags socketFlags, EndPoint remoteEP);
        Int32 SendTo(Byte[] buffer, EndPoint remoteEP);
        Int32 SendTo(Byte[] buffer, Int32 size, SocketFlags socketFlags, EndPoint remoteEP);
        Int32 SendTo(Byte[] buffer, Int32 offset, Int32 size, SocketFlags socketFlags, EndPoint remoteEP);
        Boolean SendToAsync(SocketAsyncEventArgs e);
        void SetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, Boolean optionValue);
        void SetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, Byte[] optionValue);
        void SetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, Int32 optionValue);
        void SetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, Object optionValue);
    }
}
