using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LiteNetLib;
using LiteNetLib.Utils;

public class ServerEngine : NetworkEngine {
    NetPeer client;
    Multiplayer multiplayer;

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
            }
        } else if (message is FinMessage fin) {
            FinAckMessage finAck = new FinAckMessage();
            Send(finAck, SendOptions.ReliableOrdered);
        } else if (message is FinAckMessage finAck) {
            AckMessage ack = new AckMessage();
            Send(ack, SendOptions.ReliableOrdered);
        }
    }

    protected override void Update() {

    }

    public override void Disconnect() {
        Send(client, new FinMessage(), SendOptions.ReliableOrdered);
    }
}
