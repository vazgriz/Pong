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
        Listener.PeerDisconnectedEvent += OnDisconnected;
        NetManager.Start(port);
    }

    public virtual void Dispose() {
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

    public abstract void Send(NetworkMessage message, SendOptions options);

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
    protected void FinishDisconnect() {
        if ((Manager.GameManager.CurrentGame as Multiplayer).GameRunning) {
            Manager.GameManager.UI.Message.SetText("Disconnected");
            Manager.GameManager.UI.Message.Show();
        }
        (Manager.GameManager.CurrentGame as Multiplayer).StopGame();
        Manager.EndNetworkConnection();
    }

    void OnDisconnected(NetPeer peer, DisconnectInfo disconnectInfo) {
        if (disconnectInfo.Reason == DisconnectReason.Timeout || disconnectInfo.Reason == DisconnectReason.DisconnectPeerCalled || disconnectInfo.Reason == DisconnectReason.RemoteConnectionClose) {
            FinishDisconnect();
        }
    }
}
