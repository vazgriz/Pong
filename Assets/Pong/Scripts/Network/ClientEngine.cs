using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LiteNetLib;
using LiteNetLib.Utils;

public class ClientEngine : NetworkEngine {
    public ClientEngine(NetworkManager manager) : base(manager) {
        Listener.NetworkReceiveUnconnectedEvent += OnDiscoveryResponse;
    }

    public override void Dispose() {
        Listener.NetworkReceiveUnconnectedEvent -= OnDiscoveryResponse;
        base.Dispose();
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
}
