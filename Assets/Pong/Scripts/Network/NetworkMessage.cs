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
    FinAck,
    StartGame,
    LaunchBall,
    PaddleUpdate,
    Goal,
    EndGame,
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
            case MessageType.StartGame: return new StartMessage();
            case MessageType.LaunchBall: return new LaunchBallMessage();
            case MessageType.PaddleUpdate: return new PaddleUpdateMessage();
            case MessageType.Goal: return new GoalMessage();
            case MessageType.EndGame: return new EndGameMessage();
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

public class StartMessage : NetworkMessage {
    public StartMessage() : base(MessageType.StartGame) {

    }

    public override void Read(NetDataReader reader) {
        //nothing
    }

    public override void Write(NetDataWriter writer) {
        //nothing
    }
}

public class LaunchBallMessage : NetworkMessage {
    public float Angle { get; set; }
    public float Time { get; set; }

    public LaunchBallMessage() : base(MessageType.LaunchBall) {

    }

    public override void Read(NetDataReader reader) {
        Angle = reader.GetFloat();
        Time = reader.GetFloat();
    }

    public override void Write(NetDataWriter writer) {
        writer.Put(Angle);
        writer.Put(Time);
    }
}

public class PaddleUpdateMessage : NetworkMessage {
    public float Position { get; set; }
    public float Velocity { get; set; }
    public float Input { get; set; }

    public PaddleUpdateMessage() : base(MessageType.PaddleUpdate) {

    }

    public override void Read(NetDataReader reader) {
        Position = reader.GetFloat();
        Velocity = reader.GetFloat();
        Input = reader.GetFloat();
    }

    public override void Write(NetDataWriter writer) {
        writer.Put(Position);
        writer.Put(Velocity);
        writer.Put(Input);
    }
}

public class GoalMessage : NetworkMessage {
    public Player Player { get; set; }
    public float Time { get; set; }

    public GoalMessage() : base(MessageType.Goal) {

    }

    public override void Read(NetDataReader reader) {
        Player = (Player)reader.GetByte();
        Time = reader.GetFloat();
    }

    public override void Write(NetDataWriter writer) {
        writer.Put((byte)Player);
        writer.Put(Time);
    }
}

public class EndGameMessage : NetworkMessage {
    public Player Winner { get; set; }

    public EndGameMessage() : base(MessageType.EndGame) {

    }

    public override void Read(NetDataReader reader) {
        Winner = (Player)reader.GetByte();
    }

    public override void Write(NetDataWriter writer) {
        writer.Put((byte)Winner);
    }
}