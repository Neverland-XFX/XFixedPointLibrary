[English Version](README.md)

# Demo

一个定点库的极简示例，展示了基于定点数学和 KCP 回滚的客户端–服务端实时移动帧同步。

---

## 概述

- **定点物理**（.NET Standard 2.1）保证全平台一致的数值结果  
- **KCP over UDP** 提供可靠、顺序包交付  
- **回滚系统** 保存每帧快照，在收到延迟输入时回滚重演

---

## 组件说明

1. **定点库**  
   - 核心类型：`XFixed`、`XFixedVector3`、`PhysicsSystem` 等  
2. **KCP 会话**  
   - `KcpSession` 简单封装 UDP 与 KCP  
   - 提供 `OnSend` 与 `OnReceive` 回调  
3. **服务器**  
   - 监听 UDP 4000 端口  
   - 管理 `Room` 房间，将两个玩家配对  
   - 转发匹配与移动操作消息  
4. **客户端（Unity）**  
   - `Joystick` UI 获取玩家方向输入  
   - `BattleManager` 初始化回滚系统并驱动仿真  
   - `RollbackSystem<MoveOp>` 每帧重置速度、应用多条输入、推进物理

---

## 架构与流程

1. **匹配阶段**  
   - 客户端发送 `CMatch`（请求匹配）  
   - 服务端将请求排队，凑齐两人后创建房间并发送 `SMatch`（匹配成功与出生点信息）

2. **进入战斗**  
   - 客户端加载战斗场景，实例化两名角色并设置初始位置  
   - 构建 `PhysicsSystem` 与 `RollbackSystem<MoveOp>`，绑定两个刚体

3. **每帧逻辑**  
   - **采集输入**：摇杆 → `MoveOp(tick, playerIndex, rawX, rawZ)`  
   - **提交 & 发送**：本地 `SubmitInput` + 发往服务端  
   - **推进仿真** `AdvanceTo(tick)`：  
     1. 若有延迟输入，回滚到最早帧  
     2. 保存快照  
     3. 重置所有刚体速度  
     4. 应用本帧所有输入  
     5. 调用 `PhysicsSystem.Step(dt)`  
   - **渲染更新**：角色位置同步至游戏对象 Transform

---

## TODO
