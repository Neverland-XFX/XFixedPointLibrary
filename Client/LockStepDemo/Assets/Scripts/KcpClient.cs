using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KcpClient : MonoBehaviour
{
    [Header("Server")] public string serverIp = "127.0.0.1";
    [Header("Server")] public int serverPort = 4000;
    public uint conv;

    UdpClient _udp;
    KcpSession _session;
    CancellationTokenSource _cts;
    SynchronizationContext _syncCtx;

    public static KcpClient Instance { get; private set; }
    public int PlayerIndex { get; private set; } = -1;

    public event Action<int> OnMatch;
    public event Action<MoveOp> OnMoveOpReceived;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        _syncCtx = SynchronizationContext.Current;
        Debug.Log($"[Client] conv={conv}");
    }

    private void Start()
    {
        _udp = new UdpClient(0);
        var ep = new IPEndPoint(IPAddress.Parse(serverIp), serverPort);

        _session = new KcpSession(conv);
        _session.Init(_udp, ep, OnRawReceive);

        _cts = new CancellationTokenSource();
        _ = ReceiveLoop(_cts.Token);

        // send match
        var buf = KcpMessageSerializer.Pack(
            RequestType.CMatch, new CMatch { PlayerId = 0 });
        _session.Send(buf);
        Debug.Log("[Client] Sent CMatch");
    }

    async Task ReceiveLoop(CancellationToken t)
    {
        try
        {
            while (!t.IsCancellationRequested)
            {
                var res = await _udp.ReceiveAsync();
                _session.UpdateRemoteEndPoint(res.RemoteEndPoint);
                _session.Input(res.Buffer);
            }
        }
        catch
        {
        }
    }

    void OnRawReceive(byte[] raw)
    {
        var (type, payload) = KcpMessageSerializer.Unpack(raw);
        switch (type)
        {
            case RequestType.SMatch:
                var sm = (SMatch)payload;
                PlayerIndex = sm.PlayerIndex;
                _syncCtx.Post(_ =>
                {
                    Debug.Log(
                        $"[Client] Matched as {PlayerIndex}, " +
                        $"spawn=({sm.SpawnX:F2},{sm.SpawnZ:F2})"
                    );
                    OnMatch?.Invoke(PlayerIndex);
                    SceneManager.LoadScene("BattleScene");
                }, null);
                break;

            case RequestType.CBattleOp:
                var op = (MoveOp)payload;
                _syncCtx.Post(_ =>
                {
                    Debug.Log(
                        $"[RecvOp] Tick={op.Tick} " +
                        $"P{op.PlayerIndex} Raw=({op.RawX:F2},{op.RawZ:F2})"
                    );
                    OnMoveOpReceived?.Invoke(op);
                }, null);
                break;
        }
    }

    public void SendMoveOp(MoveOp op)
    {
        var buf = KcpMessageSerializer.Pack(RequestType.CBattleOp, op);
        Debug.Log(
            $"[SendOp] Tick={op.Tick} P{op.PlayerIndex} " +
            $"Raw=({op.RawX:F2},{op.RawZ:F2})"
        );
        _session.Send(buf);
    }

    void OnDestroy()
    {
        _cts.Cancel();
        _session.Dispose();
        _udp.Close();
    }
}