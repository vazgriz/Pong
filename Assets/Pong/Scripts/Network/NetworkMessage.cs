using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LiteNetLib;
using LiteNetLib.Utils;

public enum MessageType : ushort {
    None,
    DiscoverRequest,
    DiscoverResponse,
    Syn,
    SynAck,
    Ack,
    Fin,
    FinAck
}

public abstract class NetworkMessage {
    public MessageType Type { get; private set; }

    protected NetworkMessage(MessageType type) {
        Type = type;
    }

    public abstract void Read(NetDataReader reader);
    public abstract void Write(NetDataWriter writer);

    public static NetworkMessage Decode(MessageType type) {
        switch (type) {
            case MessageType.DiscoverRequest: return new DiscoverRequestMessage();
            case MessageType.DiscoverResponse: return new DiscoverResponseMessage();
            case MessageType.Syn: return new SynMessage();
            case MessageType.SynAck: return new SynAckMessage();
            case MessageType.Ack: return new AckMessage();
            case MessageType.Fin: return new FinMessage();
            case MessageType.FinAck: return new FinAckMessage();
            default: return null;
        }
    }
}

public class DiscoverRequestMessage : NetworkMessage {
    public string GameName { get; set; }
    public int MajorVersion { get; set; }
    public int MinorVersion { get; set; }

    public DiscoverRequestMessage() : base(MessageType.DiscoverRequest) {

    }

    public override void Read(NetDataReader reader) {
        GameName = reader.GetString();
        MajorVersion = reader.GetInt();
        MinorVersion = reader.GetInt();
    }

    public override void Write(NetDataWriter writer) {
        writer.Put(GameName);
        writer.Put(MajorVersion);
        writer.Put(MinorVersion);
    }
}

public class DiscoverResponseMessage : NetworkMessage {
    public string ServerName { get; set; }

    public DiscoverResponseMessage() : base(MessageType.DiscoverResponse) {

    }

    public override void Read(NetDataReader reader) {
        ServerName = reader.GetString();
    }

    public override void Write(NetDataWriter writer) {
        writer.Put(ServerName);
    }
}

public class SynMessage : NetworkMessage {
    public string PlayerName { get; set; }
    public string GameName { get; set; }
    public int MajorVersion { get; set; }
    public int MinorVersion { get; set; }

    public SynMessage() : base(MessageType.Syn) {

    }

    public override void Read(NetDataReader reader) {
        PlayerName = reader.GetString();
        GameName = reader.GetString();
        MajorVersion = reader.GetInt();
        MinorVersion = reader.GetInt();
    }

    public override void Write(NetDataWriter writer) {
        writer.Put(PlayerName);
        writer.Put(GameName);
        writer.Put(MajorVersion);
        writer.Put(MinorVersion);
    }
}

public class SynAckMessage : NetworkMessage {
    public string PlayerName { get; set; }

    public SynAckMessage() : base(MessageType.SynAck) {

    }

    public override void Read(NetDataReader reader) {
        PlayerName = reader.GetString();
    }

    public override void Write(NetDataWriter writer) {
        writer.Put(PlayerName);
    }
}

public class AckMessage : NetworkMessage {
    public AckMessage() : base(MessageType.Ack) {

    }

    public override void Read(NetDataReader reader) {
        //nothing
    }

    public override void Write(NetDataWriter writer) {
        //nothing
    }
}

public class FinMessage : NetworkMessage {
    public FinMessage() : base(MessageType.Fin) {

    }

    public override void Read(NetDataReader reader) {
        //nothing
    }

    public override void Write(NetDataWriter writer) {
        //nothing
    }
}

public class FinAckMessage : NetworkMessage {
    public FinAckMessage() : base(MessageType.FinAck) {

    }

    public override void Read(NetDataReader reader) {
        //nothing
    }

    public override void Write(NetDataWriter writer) {
        //nothing
    }
}
