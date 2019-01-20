using System;
using System.Collections.Generic;
using UnityEngine;
using LiteNetLib;
using LiteNetLib.Utils;

public enum ConnectionStatus {
    Unconnected,
    Connecting,
    Connected,
    Disconnecting
}

public abstract class NetworkEngine : IDisposable {
    protected NetworkManager Manager { get; private set; }
    public EventBasedNetListener Listener { get; private set; }
    public NetManager NetManager { get; private set; }
    NetDataWriter writer;

    public ConnectionStatus ConnectionStatus { get; protected set; }

    protected NetworkEngine(NetworkManager manager) : this(manager, 0) { }

    protected NetworkEngine(NetworkManager manager, int port) {
        Manager = manager;
        Listener = new EventBasedNetListener();
        NetManager = new NetManager(Listener, "Pong");
        NetManager.UnconnectedMessagesEnabled = true;
        NetManager.DiscoveryEnabled = true;
        writer = new NetDataWriter();

        Listener.NetworkReceiveEvent += OnMessageReceived;
        NetManager.Start(port);
    }

    public virtual void Dispose() {
        Listener.NetworkReceiveEvent -= OnMessageReceived;
        NetManager.Stop();
    }

    public void InternalUpdate() {
        NetManager.PollEvents();
        Update();
        NetManager.Flush();
    }

    protected void Send(NetPeer peer, NetworkMessage message, SendOptions options) {
        writer.Reset();
        writer.Put((ushort)message.Type);
        message.Write(writer);
        peer.Send(writer, options);
    }

    void OnMessageReceived(NetPeer peer, NetDataReader reader) {
        MessageType type = (MessageType)reader.GetUShort();
        NetworkMessage message = NetworkMessage.Decode(type);

        if (message != null) {
            message.Read(reader);
            OnMessageReceived(peer, message);
        }
    }

    protected abstract void OnMessageReceived(NetPeer peer, NetworkMessage message);
    protected abstract void Update();
    public abstract void Disconnect();
}
