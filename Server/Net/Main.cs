using System.Net.Sockets;
using GameServer;

const int ListenPort = 4000;

var udp = new UdpClient(ListenPort);
var sessions = new Dictionary<uint, KcpSession>();
var waitQ = new Queue<KcpSession>();
var rooms = new List<Room>();
int nextId = 1;
object lockObj = new();

Console.WriteLine($"[Server] Listening on :{ListenPort}");

while (true)
{
    var res = udp.ReceiveAsync().Result;
    var buf = res.Buffer;
    var ep = res.RemoteEndPoint;
    uint conv = BitConverter.ToUInt32(buf, 0);

    if (!sessions.TryGetValue(conv, out var sess))
    {
        sess = new KcpSession(conv);
        sess.Init(udp, ep, raw => HandleMsg(raw, sess));
        sessions[conv] = sess;
        Console.WriteLine($"[Conn] conv={conv} from {ep}");
    }
    else
    {
        sess.UpdateRemoteEndPoint(ep);
    }

    sess.Input(buf);
}

void HandleMsg(byte[] raw, KcpSession s)
{
    var (type, payload) = KcpMessageSerializer.Unpack(raw);
    switch (type)
    {
        case RequestType.CMatch:
            lock (lockObj)
            {
                waitQ.Enqueue(s);
                if (waitQ.Count >= 2)
                {
                    var a = waitQ.Dequeue();
                    var b = waitQ.Dequeue();
                    var room = new Room(a, b, nextId++);
                    rooms.Add(room);
                }
            }

            break;

        case RequestType.CBattleOp:
            lock (lockObj)
            {
                foreach (var r in rooms)
                    if (r.Contains(s))
                    {
                        r.ForwardOp(raw);
                        break;
                    }
            }

            break;
    }
}