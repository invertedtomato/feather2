using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace InvertedTomato.Testable {
    public interface ISocket {
        int Available { get; }
        bool Blocking { get; set; }
        bool Connected { get; }
        bool DontFragment { get; set; }
        bool DualMode { get; set; }
        bool EnableBroadcast { get; set; }
        bool ExclusiveAddressUse { get; set; }
        bool IsBound { get; }
        EndPoint LocalEndPoint { get; }
        AddressFamily AddressFamily { get; }
        bool MulticastLoopback { get; set; }
        bool NoDelay { get; set; }
        ProtocolType ProtocolType { get; }
        int ReceiveBufferSize { get; set; }
        int ReceiveTimeout { get; set; }
        EndPoint RemoteEndPoint { get; }
        int SendBufferSize { get; set; }
        int SendTimeout { get; set; }
        LingerOption LingerState { get; set; }
        SocketType SocketType { get; }
        short Ttl { get; set; }
        Socket Accept();
        bool AcceptAsync(SocketAsyncEventArgs e);
        void Bind(EndPoint localEP);
        void Connect(IPAddress[] addresses, int port);
        void Connect(string host, int port);
        void Connect(EndPoint remoteEP);
        void Connect(IPAddress address, int port);
        bool ConnectAsync(SocketAsyncEventArgs e);
        void Dispose();
        void GetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, byte[] optionValue);
        byte[] GetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, int optionLength);
        object GetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName);
        int IOControl(int ioControlCode, byte[] optionInValue, byte[] optionOutValue);
        int IOControl(IOControlCode ioControlCode, byte[] optionInValue, byte[] optionOutValue);
        void Listen(int backlog);
        bool Poll(int microSeconds, SelectMode mode);
        int Receive(byte[] buffer, int size, SocketFlags socketFlags);
        int Receive(byte[] buffer, SocketFlags socketFlags);
        int Receive(byte[] buffer, int offset, int size, SocketFlags socketFlags, out SocketError errorCode);
        int Receive(IList<ArraySegment<byte>> buffers, SocketFlags socketFlags, out SocketError errorCode);
        int Receive(IList<ArraySegment<byte>> buffers, SocketFlags socketFlags);
        int Receive(IList<ArraySegment<byte>> buffers);
        int Receive(byte[] buffer, int offset, int size, SocketFlags socketFlags);
        int Receive(byte[] buffer);
        bool ReceiveAsync(SocketAsyncEventArgs e);
        int ReceiveFrom(byte[] buffer, SocketFlags socketFlags, ref EndPoint remoteEP);
        int ReceiveFrom(byte[] buffer, ref EndPoint remoteEP);
        int ReceiveFrom(byte[] buffer, int size, SocketFlags socketFlags, ref EndPoint remoteEP);
        int ReceiveFrom(byte[] buffer, int offset, int size, SocketFlags socketFlags, ref EndPoint remoteEP);
        bool ReceiveFromAsync(SocketAsyncEventArgs e);
        int ReceiveMessageFrom(byte[] buffer, int offset, int size, ref SocketFlags socketFlags, ref EndPoint remoteEP, out IPPacketInformation ipPacketInformation);
        bool ReceiveMessageFromAsync(SocketAsyncEventArgs e);
        int Send(byte[] buffer, int offset, int size, SocketFlags socketFlags);
        int Send(byte[] buffer);
        int Send(byte[] buffer, int offset, int size, SocketFlags socketFlags, out SocketError errorCode);
        int Send(byte[] buffer, int size, SocketFlags socketFlags);
        int Send(byte[] buffer, SocketFlags socketFlags);
        int Send(IList<ArraySegment<byte>> buffers);
        int Send(IList<ArraySegment<byte>> buffers, SocketFlags socketFlags, out SocketError errorCode);
        int Send(IList<ArraySegment<byte>> buffers, SocketFlags socketFlags);
        bool SendAsync(SocketAsyncEventArgs e);
        bool SendPacketsAsync(SocketAsyncEventArgs e);
        int SendTo(byte[] buffer, SocketFlags socketFlags, EndPoint remoteEP);
        int SendTo(byte[] buffer, EndPoint remoteEP);
        int SendTo(byte[] buffer, int size, SocketFlags socketFlags, EndPoint remoteEP);
        int SendTo(byte[] buffer, int offset, int size, SocketFlags socketFlags, EndPoint remoteEP);
        bool SendToAsync(SocketAsyncEventArgs e);
        void SetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, bool optionValue);
        void SetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, byte[] optionValue);
        void SetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, int optionValue);
        void SetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, object optionValue);
    }
}
