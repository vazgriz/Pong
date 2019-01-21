using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LiteNetLib;
using LiteNetLib.Utils;

public class ClientEngine : NetworkEngine {
    float connectDeadline;
    float disconnectDeadline;
    NetPeer server;
    Multiplayer multiplayer;

    public ClientEngine(NetworkManager manager) : base(manager) {
        Listener.NetworkReceiveUnconnectedEvent += OnDiscoveryResponse;
        multiplayer = manager.GameManager.CurrentGame as Multiplayer;
    }

    public override void Dispose() {
        Listener.NetworkReceiveUnconnectedEvent -= OnDiscoveryResponse;
        base.Dispose();
    }

    public override void Send(NetworkMessage message, SendOptions options) {
        Send(server, message, options);
    }

    public void FindServers() {
        DiscoverRequestMessage message = new DiscoverRequestMessage();
        message.GameName = "Pong";
        message.MajorVersion = 1;
        message.MinorVersion = 0;

        NetDataWriter writer = new NetDataWriter();
        writer.Put((ushort)message.Type);
        message.Write(writer);

        NetManager.SendDiscoveryRequest(writer, 4444);
        NetManager.Flush();
    }

    void OnDiscoveryResponse(NetEndPoint remoteEndPoint, NetDataReader reader, UnconnectedMessageType messageType) {
        if (messageType != UnconnectedMessageType.DiscoveryResponse) return;

        MessageType type = (MessageType)reader.GetUShort();
        if (type != MessageType.DiscoverResponse) return;

        DiscoverResponseMessage message = new DiscoverResponseMessage();
        message.Read(reader);

        Manager.AddServer(remoteEndPoint, message.ServerName);
    }

    public void Connect(string address) {
        server = NetManager.Connect(address, 4444);

        SynMessage syn = new SynMessage();
        syn.PlayerName = Manager.GameManager.UI.PlayerName;
        syn.GameName = "Pong";
        syn.MajorVersion = 1;
        syn.MinorVersion = 0;

        Send(syn, SendOptions.ReliableOrdered);
        NetManager.Flush();
        ConnectionStatus = ConnectionStatus.Connecting;
        connectDeadline = Time.time + 5;
    }

    protected override void OnMessageReceived(NetPeer peer, NetworkMessage message) {
        if (peer != server) return;

        if (message is SynAckMessage synAck) {
            (Manager.GameManager.CurrentGame as Multiplayer).SetRemoteName(synAck.PlayerName);

            AckMessage ack = new AckMessage();
            Send(ack, SendOptions.ReliableOrdered);
            ConnectionStatus = ConnectionStatus.Connected;
            Manager.GameManager.UI.Message.Hide();
        } else if (message is AckMessage) {
            if (ConnectionStatus == ConnectionStatus.Disconnecting) {
                ConnectionStatus = ConnectionStatus.Unconnected;
                FinishDisconnect();
            }
        } else if (message is FinMessage fin) {
            FinAckMessage finAck = new FinAckMessage();
            Send(finAck, SendOptions.ReliableOrdered);
            ConnectionStatus = ConnectionStatus.Disconnecting;
            disconnectDeadline = Time.time + 5;
        } else if (message is FinAckMessage finAck) {
            AckMessage ack = new AckMessage();
            Send(ack, SendOptions.ReliableOrdered);

            if (ConnectionStatus == ConnectionStatus.Disconnecting) {
                ConnectionStatus = ConnectionStatus.Unconnected;
                FinishDisconnect();
            }
        } else if (message is StartMessage) {
            (Manager.GameManager.CurrentGame as Multiplayer).StartGame();
        } else if (message is PaddleUpdateMessage paddleUpdate) {
            (Manager.GameManager.CurrentGame as Multiplayer).HandlePaddleUpdate(paddleUpdate);
        } else if (message is BallUpdateMessage ballUpdate) {
            (Manager.GameManager.CurrentGame as Multiplayer).HandleBallUpdate(ballUpdate);
        } else if (message is LaunchBallMessage launch) {
            (Manager.GameManager.CurrentGame as Multiplayer).HandleBallLaunch(launch);
        }
    }

    protected override void Update() {
        if (ConnectionStatus == ConnectionStatus.Connecting && connectDeadline < Time.time) {
            ConnectionStatus = ConnectionStatus.Unconnected;
            Manager.GameManager.UI.Message.SetText("Failed to connect");
            Manager.GameManager.UI.Message.Show();
        }

        if (ConnectionStatus == ConnectionStatus.Disconnecting && disconnectDeadline < Time.time) {
            ConnectionStatus = ConnectionStatus.Unconnected;
            FinishDisconnect();
        }
    }

    public override void Disconnect() {
        Send(new FinMessage(), SendOptions.ReliableOrdered);
        ConnectionStatus = ConnectionStatus.Disconnecting;
        disconnectDeadline = Time.time + 5;
    }
}
