using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LiteNetLib;
using LiteNetLib.Utils;

public class ServerEngine : NetworkEngine {
    NetPeer client;
    Multiplayer multiplayer;
    float connectDeadline;
    float disconnectDeadline;

    public ServerEngine(NetworkManager manager) : base(manager, 4444) {
        Listener.PeerConnectedEvent += OnPeerConnected;
        multiplayer = manager.GameManager.CurrentGame as Multiplayer;
    }

    public override void Dispose() {
        Listener.PeerConnectedEvent -= OnPeerConnected;
        base.Dispose();
    }

    public override void Send(NetworkMessage message, SendOptions options) {
        Send(client, message, options);
    }

    void OnPeerConnected(NetPeer peer) {
        client = peer;
    }

    protected override void OnMessageReceived(NetPeer peer, NetworkMessage message) {
        if (peer != client) return;

        if (message is SynMessage syn) {
            if (syn.GameName == "Pong" && syn.MajorVersion == 1 && syn.MinorVersion == 0) {
                multiplayer.SetRemoteName(syn.PlayerName);

                SynAckMessage synAck = new SynAckMessage();
                synAck.PlayerName = Manager.GameManager.UI.PlayerName;
                Send(synAck, SendOptions.ReliableOrdered);
                ConnectionStatus = ConnectionStatus.Connecting;
                connectDeadline = Time.time + 5;
            }
        } else if (message is AckMessage) {
            if (ConnectionStatus == ConnectionStatus.Connecting) {
                ConnectionStatus = ConnectionStatus.Connected;
                multiplayer.StartGame();
            } else if (ConnectionStatus == ConnectionStatus.Disconnecting) {
                ConnectionStatus = ConnectionStatus.Unconnected;
            }
        } else if (message is FinMessage fin) {
            FinAckMessage finAck = new FinAckMessage();
            Send(finAck, SendOptions.ReliableOrdered);
            disconnectDeadline = Time.time + 5;
            ConnectionStatus = ConnectionStatus.Disconnecting;
        } else if (message is FinAckMessage finAck) {
            AckMessage ack = new AckMessage();
            Send(ack, SendOptions.ReliableOrdered);
            if (ConnectionStatus == ConnectionStatus.Disconnecting) {
                ConnectionStatus = ConnectionStatus.Unconnected;
            }
        } else if (message is PaddleUpdateMessage update) {
            (Manager.GameManager.CurrentGame as Multiplayer).HandlePaddleUpdate(update);
        }
    }

    protected override void Update() {
        if (ConnectionStatus == ConnectionStatus.Connecting && connectDeadline < Time.time) {
            ConnectionStatus = ConnectionStatus.Unconnected;
            NetManager.DisconnectPeer(client);
        }

        if (ConnectionStatus == ConnectionStatus.Disconnecting && disconnectDeadline < Time.time) {
            ConnectionStatus = ConnectionStatus.Unconnected;
            Manager.GameManager.OpenMainMenu();
        }
    }

    public override void Disconnect() {
        Send(client, new FinMessage(), SendOptions.ReliableOrdered);
    }
}
