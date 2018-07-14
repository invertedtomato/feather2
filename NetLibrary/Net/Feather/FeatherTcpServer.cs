﻿using InvertedTomato.IO.Messages;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace InvertedTomato.Net.Feather {
    public class FeatherTcpServer<TMessage> : IDisposable where TMessage : IImportableMessage, IExportableMessage, new() {
        private readonly Socket Underlying = new Socket(SocketType.Stream, ProtocolType.Tcp);
        private readonly ConcurrentDictionary<EndPoint, Client> Clients = new ConcurrentDictionary<EndPoint, Client>();
        private readonly Object Sync = new Object();

        public event Action<EndPoint, TMessage> OnMessageReceived;
        public event Action<EndPoint> OnClientConnected;
        public event Action<EndPoint, DisconnectionType> OnClientDisconnected;

        public bool NoDelay { get { return Underlying.NoDelay; } set { Underlying.NoDelay = value; } }
        public bool IsDisposed { get; private set; }

        public void Listen(Int32 port) {
            Listen(new IPEndPoint(IPAddress.Any, port));
        }
        public void Listen(IPEndPoint localEndPoint) {
            lock (Sync) {
                Underlying.Bind(localEndPoint);
                Underlying.Listen(100);

                AcceptStart();
            }
        }
        public void Disconnect(EndPoint address) {
            if (null == address) {
                throw new ArgumentNullException(nameof(address));
            }

            if (!Clients.TryRemove(address, out var client)) {
                throw new KeyNotFoundException();
            }

            try {
                client.Socket.Shutdown(SocketShutdown.Both);
            } catch (SocketException) { };
            client.Socket.Dispose();
        }

        public void SendTo(EndPoint address, TMessage message) {
            if (null == address) {
                throw new ArgumentNullException(nameof(address));
            }
            if (null == message) {
                throw new ArgumentNullException(nameof(message));
            }

            // Get target client
            if (!Clients.TryGetValue(address, out var client)) {
                throw new KeyNotFoundException();
            }

            // Extract payload from message
            var payload = message.Export();

            // Check the payload is not too large
            if (payload.Count > UInt16.MaxValue) {
                throw new ArgumentOutOfRangeException(nameof(message), "Message must encode to a payload of 65KB or less");
            }

            // Convert length to header
            var lengthBytes = BitConverter.GetBytes((UInt16)payload.Count);

            lock (Sync) {
                // Send length header, followed by payload
                client.Socket.Send(lengthBytes);
                client.Socket.Send(payload);
            }
        }

        protected virtual void Dispose(bool disposing) {
            if (IsDisposed) {
                return;
            }
            IsDisposed = true;

            if (disposing) {
                // Dispose managed state (managed objects)
                try {
                    Underlying.Shutdown(SocketShutdown.Both);
                } catch (SocketException) { }
                Underlying.Dispose();

                foreach (var item in Clients) {
                    try {
                        item.Value.Socket.Shutdown(SocketShutdown.Both);
                    } catch (SocketException) { }
                    item.Value.Socket.Dispose();
                }
            }
        }

        public void Dispose() {
            Dispose(true);
        }


        private void AcceptStart() {
            try {
                // Prepare arguments
                var args = new SocketAsyncEventArgs();
                args.Completed += (sender, e) => {
                    AcceptEnd(e);
                };

                // Start accept - note that this will not call Completed and return false if it completes synchronously
                if (!Underlying.AcceptAsync(args)) {
                    AcceptEnd(args);
                }
            } catch (ObjectDisposedException) { }
        }

        private void AcceptEnd(SocketAsyncEventArgs args) {
            // Get endpoint
            var endPoint = args.AcceptSocket.RemoteEndPoint;

            // Facilitate shutdown
            if (null == endPoint) {
                return;
            }

            // Create client record
            var client = Clients[endPoint] = new Client() {
                Socket = args.AcceptSocket,
                LengthBuffer = new Byte[2],
                LengthCount = 0
            };

            // Copy NoDelay
            client.Socket.NoDelay = Underlying.NoDelay;

            // Raise connected event
            OnClientConnected?.Invoke(endPoint);

            // Start receiving
            ReceiveLengthStart(client);

            // Start accepting next request
            AcceptStart();
        }

        private void ReceiveLengthStart(Client client) {
            try {
                var args = new SocketAsyncEventArgs();
                args.RemoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
                args.SocketFlags = SocketFlags.None;
                args.SetBuffer(client.LengthBuffer, client.LengthCount, 2 - client.LengthCount);
                args.Completed += (sender, e) => { ReceiveLenghtEnd(client, e); };

                if (!client.Socket.ReceiveAsync(args)) {
                    ReceiveLenghtEnd(client, args);
                }
            } catch (ObjectDisposedException) { };
        }

        private void ReceiveLenghtEnd(Client client, SocketAsyncEventArgs args) {
            // Update byte received count with what was just received
            client.LengthCount += args.BytesTransferred;

            if (client.LengthCount < 2) {
                // Not all length received - get more
                ReceiveLengthStart(client);
            } else {
                // Compute length
                var length = BitConverter.ToUInt16(client.LengthBuffer, 0);
                client.PayloadBuffer = new byte[length];

                // Reset state
                client.LengthCount = 0;

                // Receive payload now
                ReceivePayloadStart(client);
            }
        }


        private void ReceivePayloadStart(Client client) {
            try {
                var args = new SocketAsyncEventArgs();
                args.RemoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
                args.SocketFlags = SocketFlags.None;
                args.SetBuffer(client.PayloadBuffer, client.PayloadCount, client.PayloadBuffer.Length - client.PayloadCount);
                args.Completed += (sender, e) => {
                    ReceivePayloadEnd(client, e);
                };

                if (!client.Socket.ReceiveAsync(args)) {
                    ReceivePayloadEnd(client, args);
                }
            } catch (ObjectDisposedException) { };
        }

        private void ReceivePayloadEnd(Client client, SocketAsyncEventArgs args) {
            if (args.BytesTransferred < 0) {
                throw new Exception(); // TODO----------------------------------------- disconnection logic
            }

            // Update received count
            client.PayloadCount += args.BytesTransferred;

            if (client.PayloadCount < client.PayloadBuffer.Length) {
                ReceivePayloadStart(client);
            } else {
                // Instantiate message
                var message = new TMessage();
                message.Import(new ArraySegment<byte>(client.PayloadBuffer, 0, client.PayloadBuffer.Length));
                
                // Reset state
                client.PayloadCount = 0;

                // Raise received event
                OnMessageReceived?.Invoke(args.RemoteEndPoint, message);

                ReceiveLengthStart(client);
            }
        }
    }

    class Client {
        public Socket Socket;

        public Byte[] LengthBuffer;
        public Int32 LengthCount;

        public Byte[] PayloadBuffer;
        public Int32 PayloadCount;

    }
}
