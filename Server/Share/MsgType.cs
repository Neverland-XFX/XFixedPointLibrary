// Share/Protocol.cs
using System;

public enum RequestType : int
{
    CMatch = 0,
    SMatch,
    CBattleOp,
}

[Serializable]
public class CMatch
{
    public int PlayerId;
}

[Serializable]
public class SMatch
{
    public int   PlayerIndex; // 0 or 1
    public float SpawnX;
    public float SpawnZ;
}

[Serializable]
public class MoveOp
{
    public int   Tick;
    public int   PlayerIndex;
    public float RawX;
    public float RawZ;
}
