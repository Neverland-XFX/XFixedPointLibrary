[中文版本](README.cn.md)

# Demo

A minimal fixed-point library example demonstrating client–server real-time movement frame-sync based on fixed-point math and KCP rollback.

---

## Overview

- **Fixed-point physics** (.NET Standard 2.1) ensures bit-identical results across all platforms  
- **KCP over UDP** provides reliable, in-order packet delivery  
- **Rollback system** snapshots each frame and replays late inputs for deterministic simulation

---

## Components

1. **Fixed-Point Library**  
   - Core types: `XFixed`, `XFixedVector3`, `PhysicsSystem`, etc.  
2. **KCP Session**  
   - `KcpSession` wraps UDP + KCP  
   - Offers `OnSend` and `OnReceive` callbacks  
3. **Server**  
   - Listens on UDP port 4000  
   - Manages `Room` objects pairing two players  
   - Forwards matchmaking and movement messages  
4. **Client (Unity)**  
   - `Joystick` UI for directional input  
   - `BattleManager` initializes rollback and drives simulation  
   - `RollbackSystem<MoveOp>` resets velocities, applies multiple inputs per tick, and steps physics

---

## Architecture & Flow

1. **Matchmaking**  
   - Client sends `CMatch` request  
   - Server queues sessions, pairs two players, and replies `SMatch` with spawn info  

2. **Game Start**  
   - Clients load battle scene and spawn two characters  
   - Create `PhysicsSystem` and `RollbackSystem<MoveOp>`, bind two rigid bodies  

3. **Per-Tick Logic**  
   - **Collect Input**: joystick → `MoveOp(tick, playerIndex, rawX, rawZ)`  
   - **Submit & Send**: local `SubmitInput` + send to server  
   - **Advance Simulation** `AdvanceTo(tick)`:  
     1. Roll back if any late inputs  
     2. Save snapshot  
     3. Reset all rigid body velocities to zero  
     4. Apply all inputs for this tick  
     5. Call `PhysicsSystem.Step(dt)`  
   - **Render Update**: copy each body’s position to the GameObject Transform

---

## TODO
