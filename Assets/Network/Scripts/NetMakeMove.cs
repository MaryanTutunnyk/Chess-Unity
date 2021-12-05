using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public class NetMakeMove : NetMessage
{
    public int OriginalX;
    public int OriginalY;
    public int DestinationX;
    public int DestinationY;
    public int TeamID;

    public NetMakeMove()
    {
        Code = OpCode.MAKE_MOVE;
    }

    public NetMakeMove(DataStreamReader reader)
    {
        Code = OpCode.MAKE_MOVE;
        Deserialize(reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte((byte)Code);
        writer.WriteInt(OriginalX);
        writer.WriteInt(OriginalY);
        writer.WriteInt(DestinationX);
        writer.WriteInt(DestinationY);
        writer.WriteInt(TeamID);
    }

    public override void Deserialize(DataStreamReader reader)
    {
        OriginalX = reader.ReadInt();
        OriginalY = reader.ReadInt();
        DestinationX = reader.ReadInt();
        DestinationY = reader.ReadInt();
        TeamID = reader.ReadInt();
    }

    public override void ReceivedOnClient()
    {
        NetUtility.C_MAKE_MOVE?.Invoke(this);
    }

    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        NetUtility.S_MAKE_MOVE?.Invoke(this, cnn);
    }
}
