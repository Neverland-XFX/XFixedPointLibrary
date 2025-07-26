// Server/KcpSession.cs
using System;
using System.Buffers;
using System.Net;
using System.Net.Sockets;
using System.Net.Sockets.Kcp;
using System.Threading;
using System.Threading.Tasks;

public class KcpSession : IDisposable
{
    public uint Conv { get; }
    readonly PoolSegManager.Kcp _kcp;
    readonly CancellationTokenSource _cts = new CancellationTokenSource();
    Task? _loop;
    UdpClient _udp;
    IPEndPoint _remoteEP;

    public Action<byte[]>?              OnReceive;
    public Action<ReadOnlyMemory<byte>> OnSend;

    public KcpSession(uint conv)
    {
        Conv = conv;
        _kcp = new PoolSegManager.Kcp(conv, new InternalCallback(this));
        _kcp.NoDelay(1, 10, 2, 1);
        _kcp.WndSize(128, 128);
    }

    public void Init(UdpClient udp, IPEndPoint ep, Action<byte[]> recvCb)
    {
        _udp      = udp;
        _remoteEP = ep;
        OnReceive = recvCb;
        OnSend    = data => _udp.Send(data.ToArray(), data.Length, _remoteEP);
        Start();
    }

    public void UpdateRemoteEndPoint(IPEndPoint ep) => _remoteEP = ep;

    void Start()
    {
        if (_loop != null) return;
        _loop = Task.Run(async () =>
        {
            var token = _cts.Token;
            while (!token.IsCancellationRequested)
            {
                _kcp.Update(DateTimeOffset.UtcNow);
                while (_kcp.PeekSize() > 0)
                {
                    int sz = _kcp.PeekSize();
                    var buf = new byte[sz];
                    if (_kcp.Recv(buf) >= 0)
                        OnReceive?.Invoke(buf);
                }
                await Task.Delay(10, token).ConfigureAwait(false);
            }
        }, _cts.Token);
    }

    public void Send(ReadOnlyMemory<byte> data)  => _kcp.Send(data.ToArray());
    public void Input(ReadOnlyMemory<byte> packet) => _kcp.Input(packet.ToArray());

    public void Dispose()
    {
        _cts.Cancel();
        try { _loop?.Wait(100); } catch { }
        _cts.Dispose();
    }

    class InternalCallback : IKcpCallback
    {
        readonly KcpSession _owner;
        public InternalCallback(KcpSession o) => _owner = o;
        public void Output(IMemoryOwner<byte> buf, int len)
        {
            _owner.OnSend?.Invoke(buf.Memory.Slice(0, len));
            buf.Dispose();
        }
        public IMemoryOwner<byte> RentBuffer(int length) =>
            MemoryPool<byte>.Shared.Rent(length);
        public void Receive(byte[] buffer) { }
    }
}
