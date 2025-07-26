namespace GameServer
{
    /// <summary>
    /// 两人房间：下发初始位置，只转发 MoveOp
    /// </summary>
    class Room
    {
        public int Id { get; }
        const float SpawnOffset = 2f;

        readonly KcpSession _a, _b;

        public Room(KcpSession a, KcpSession b, int id)
        {
            Id = id;
            _a = a;
            _b = b;

            // P0 @ (-SpawnOffset,0), P1 @ (+SpawnOffset,0)
            var m0 = new SMatch { PlayerIndex = 0, SpawnX = -SpawnOffset, SpawnZ = 0f };
            var m1 = new SMatch { PlayerIndex = 1, SpawnX =  SpawnOffset, SpawnZ = 0f };

            _a.Send(KcpMessageSerializer.Pack(RequestType.SMatch, m0));
            _b.Send(KcpMessageSerializer.Pack(RequestType.SMatch, m1));

            Console.WriteLine(
                $"[Room{Id}] Created. " +
                $"P0 spawn=({m0.SpawnX:F2},{m0.SpawnZ:F2})  " +
                $"P1 spawn=({m1.SpawnX:F2},{m1.SpawnZ:F2})"
            );
        }

        public bool Contains(KcpSession s) => s == _a || s == _b;

        public void ForwardOp(byte[] raw)
        {
            var (type, payload) = KcpMessageSerializer.Unpack(raw);
            if (type == RequestType.CBattleOp)
            {
                var op = (MoveOp)payload;
                Console.WriteLine(
                    $"[Room{Id}|RecvOp] Tick={op.Tick}  " +
                    $"P{op.PlayerIndex} Raw=({op.RawX:F2},{op.RawZ:F2})"
                );
            }
            _a.Send(raw);
            _b.Send(raw);
        }
    }
}