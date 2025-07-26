using System;

public static class KcpMessageSerializer
{
    public static byte[] Pack(RequestType type, object payload)
    {
        switch (type)
        {
            case RequestType.CMatch:    return PackCMatch((CMatch)payload);
            case RequestType.SMatch:    return PackSMatch((SMatch)payload);
            case RequestType.CBattleOp: return PackMoveOp((MoveOp)payload);
            default: throw new ArgumentException($"Unknown {type}");
        }
    }

    static byte[] PackCMatch(CMatch m)
    {
        var buf = new byte[8];
        BitConverter.GetBytes((int)RequestType.CMatch).CopyTo(buf, 0);
        BitConverter.GetBytes(m.PlayerId).CopyTo(buf, 4);
        return buf;
    }

    static byte[] PackSMatch(SMatch m)
    {
        var buf = new byte[16];
        int off = 0;
        BitConverter.GetBytes((int)RequestType.SMatch).CopyTo(buf, off); off += 4;
        BitConverter.GetBytes(m.PlayerIndex).CopyTo(buf, off);            off += 4;
        BitConverter.GetBytes(m.SpawnX).CopyTo(buf, off);                 off += 4;
        BitConverter.GetBytes(m.SpawnZ).CopyTo(buf, off);
        return buf;
    }

    static byte[] PackMoveOp(MoveOp m)
    {
        var buf = new byte[20];
        int off = 0;
        BitConverter.GetBytes((int)RequestType.CBattleOp).CopyTo(buf, off); off += 4;
        BitConverter.GetBytes(m.Tick).CopyTo(buf, off);                    off += 4;
        BitConverter.GetBytes(m.PlayerIndex).CopyTo(buf, off);            off += 4;
        BitConverter.GetBytes(m.RawX).CopyTo(buf, off);                   off += 4;
        BitConverter.GetBytes(m.RawZ).CopyTo(buf, off);
        return buf;
    }

    public static (RequestType type, object payload) Unpack(byte[] d)
    {
        var type = (RequestType)BitConverter.ToInt32(d, 0);
        switch (type)
        {
            case RequestType.CMatch:
                return (type, new CMatch {
                    PlayerId = BitConverter.ToInt32(d, 4)
                });
            case RequestType.SMatch:
                return (type, new SMatch {
                    PlayerIndex = BitConverter.ToInt32(d, 4),
                    SpawnX      = BitConverter.ToSingle(d, 8),
                    SpawnZ      = BitConverter.ToSingle(d, 12)
                });
            case RequestType.CBattleOp:
                return (type, new MoveOp {
                    Tick        = BitConverter.ToInt32(d, 4),
                    PlayerIndex = BitConverter.ToInt32(d, 8),
                    RawX        = BitConverter.ToSingle(d, 12),
                    RawZ        = BitConverter.ToSingle(d, 16)
                });
            default:
                throw new ArgumentException($"Unknown {type}");
        }
    }
}
