using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using XFixedPoint.Core;
using XFixedPoint.Networking;
using XFixedPoint.Physics;
using XFixedPoint.Quaternions;
using XFixedPoint.Vectors;

[DisallowMultipleComponent]
public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance { get; private set; }

    [Header("参数")] [SerializeField] float speed = 5f;
    [SerializeField] float serverFPS = 15f;

    [Header("Prefab (Resources)")] [SerializeField]
    string heroName = "Hero";

    [SerializeField] string enemyName = "Enemy";

    KcpClient _client;
    int _myIndex;
    PhysicsSystem _physics;
    RollbackSystem<MoveOp> _rollback;
    List<FixedRigidbody> _bodies;
    GameObject[] _players;

    float _tickInterval;
    float _accumTime;
    int _curTick;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    void OnSceneLoaded(Scene s, LoadSceneMode m)
    {
        if (s.name != "BattleScene") return;
        Initialize();
    }

    void Initialize()
    {
        _client = KcpClient.Instance;
        _myIndex = _client.PlayerIndex;
        _tickInterval = 1f / serverFPS;
        _physics = new PhysicsSystem();
        _bodies = new List<FixedRigidbody>();
        _rollback = new RollbackSystem<MoveOp>(_physics, _bodies);

        // 实例化两位玩家
        _players = new GameObject[2];
        for (int i = 0; i < 2; i++)
        {
            bool isLocal = (i == _myIndex);
            string res = isLocal ? heroName : enemyName;
            var prefab = Resources.Load<GameObject>(res);
            var go = Instantiate(prefab, new Vector3(
                (i == 0 ? -2f : 2f), 0f, 0f
            ), Quaternion.identity);
            go.name = isLocal ? "Hero_Local" : "Enemy_Remote";
            _players[i] = go;

            var body = new FixedRigidbody
            {
                Position = new XFixedVector3(
                    XFixed.FromFloat(go.transform.position.x),
                    XFixed.Zero,
                    XFixed.FromFloat(go.transform.position.z)
                ),
                Rotation = XFixedQuaternion.Identity
            };

            _physics.AddBody(body, null);
            _bodies.Add(body);
        }

        _client.OnMoveOpReceived += op =>
        {
            Debug.Log($"[BattleManager #{_client.PlayerIndex}] EnqueueInput P{op.PlayerIndex} Tick={op.Tick}");
            _rollback.SubmitInput(op.Tick, op);
        };
    }

    void Update()
    {
        _accumTime += Time.deltaTime;
        while (_accumTime >= _tickInterval)
        {
            _accumTime -= _tickInterval;

            // 本地输入
            Vector2 v = Joystick.Instance.Value;
            var op = new MoveOp
            {
                Tick = _curTick,
                PlayerIndex = _myIndex,
                RawX = v.x,
                RawZ = v.y
            };

            _rollback.SubmitInput(_curTick, op);
            _client.SendMoveOp(op);

            // 推进仿真
            _rollback.AdvanceTo(
                _curTick,
                XFixed.FromFloat(_tickInterval),
                ApplyInput
            );

            // 更新视图
            for (int i = 0; i < 2; i++)
            {
                var p = _bodies[i].Position;
                _players[i].transform.position = new Vector3(
                    p.X.ToFloat(),
                    0f,
                    p.Z.ToFloat()
                );
            }

            Debug.Log(
                $"[ClientSim] Tick={_curTick}  " +
                $"P0=({_bodies[0].Position.X.ToFloat():F2}," +
                $"{_bodies[0].Position.Z.ToFloat():F2})  " +
                $"P1=({_bodies[1].Position.X.ToFloat():F2}," +
                $"{_bodies[1].Position.Z.ToFloat():F2})"
            );

            _curTick++;
        }
    }

    void ApplyInput(MoveOp op)
    {
        // 1) 限制 RawX/Z 在 [-1,1]
        float rx = Mathf.Clamp(op.RawX, -1f, 1f);
        float rz = Mathf.Clamp(op.RawZ, -1f, 1f);

        // 2) 如果长度超过 1，就归一化
        float mag = Mathf.Sqrt(rx * rx + rz * rz);
        if (mag > 1f)
        {
            rx /= mag;
            rz /= mag;
        }

        // 3) 构造定点方向向量并设置速度
        var dir = new XFixedVector3(
            XFixed.FromFloat(rx),
            XFixed.Zero,
            XFixed.FromFloat(rz)
        );
        _bodies[op.PlayerIndex].Velocity =
            dir * XFixed.FromFloat(speed);
    }
}