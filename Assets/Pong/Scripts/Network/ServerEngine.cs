using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LiteNetLib;
using LiteNetLib.Utils;

public class ServerEngine : NetworkEngine {
    public ServerEngine(NetworkManager manager) : base(manager, 4444) {
        Listener.PeerConnectedEvent += OnPeerConnected;
    }

    public override void Dispose() {
        Listener.PeerConnectedEvent -= OnPeerConnected;
        base.Dispose();
    }

    void OnPeerConnected(NetPeer peer) {

    }

    protected override void OnMessageReceived(NetPeer peer, NetworkMessage message) {
        if (message is SynMessage syn) {
            if (syn.GameName == "Pong" && syn.MajorVersion == 1 && syn.MinorVersion == 0) {
                SynAckMessage synAck = new SynAckMessage();
                Send(peer, synAck, SendOptions.ReliableOrdered);
            }
        } else if (message is FinMessage fin) {
            FinAckMessage finAck = new FinAckMessage();
            Send(peer, finAck, SendOptions.ReliableOrdered);
        } else if (message is FinAckMessage finAck) {
            AckMessage ack = new AckMessage();
            Send(peer, ack, SendOptions.ReliableOrdered);
        }
    }
}
