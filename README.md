# XFixedPoint — Fixed-Point Math & Physics Simulation Library

[TOC]

[中文版说明 | 中文文档](README.cn.md)

[![.NET Standard 2.1](https://img.shields.io/badge/.NET-Standard_2.1-blue)](https://docs.microsoft.com/dotnet/standard/net-standard) [![License: MIT](https://img.shields.io/badge/License-MIT-green)](LICENSE)

## Introduction

**XFixedPoint** is a production-grade fixed-point math and physics simulation library targeting [.NET Standard 2.1]. It aims to deliver:

- **Determinism** 
- **High Performance** 
- **Feature Completeness** 
- **Ease of Use** 

---

## Features

- **Core type `XFixed`** — 64.32 fixed-point representation with + − × ÷, comparison, conversion  
- **Math functions `XFixedMath`** — Newton Sqrt, CORDIC Sin/Cos/Tan/Atan2, Exp/Log, Lerp/Clamp/Min/Max  
- **Vectors & Matrices** — `FixedVector2/3/4`, `FixedQuaternion`, `FixedMatrix4x4` for transforms & interpolation  
- **Physics Simulation** — `FixedRigidbody` integration + AABB/Sphere/OBB SAT collision + elastic impulse response  
- **Networking** — `Snapshot`, `InputBuffer`, `RollbackSystem` for deterministic roll-back and input replay  
- **Utilities** — LCG fixed-point RNG, `FixedTime` manager, `FixedDebugger` error statistics  
- **Full Test Coverage** — all modules verified with xUnit  

---

# Project Structure

```text
XFixedPoint/                         ← project root
│
├─ Core/                              ← core fixed-point types & algorithms
│   ├─ XFixedConstants.cs             ← SHIFT, ONE, EPS constants
│   ├─ XFixed.cs                      ← `XFixed` struct & conversions
│   ├─ XFixedArithmetic.cs            ← + − × ÷ implementations
│   ├─ XFixedComparison.cs            ← comparison operators & interfaces
│   └─ XFixedMath.cs                  ← Sqrt, Sin/Cos (CORDIC), Exp/Log, helpers
│
├─ Vectors/                           ← fixed-point vector types
│   ├─ XFixedVector2.cs
│   ├─ XFixedVector3.cs
│   └─ XFixedVector4.cs
│
├─ Quaternions/                       ← fixed-point quaternion
│   ├─ XFixedQuaternion.cs
│   └─ XFixedQuaternionOps.cs
│
├─ Matrices/                          ← 4×4 fixed-point matrix
│   └─ XFixedMatrix4x4.cs
│
├─ Physics/                           ← physics simulation
│   ├─ FixedRigidbody.cs
│   ├─ FixedCollider.cs
│   ├─ Collision/
│   │   ├─ AABBCollider.cs
│   │   ├─ SphereCollider.cs
│   │   └─ OBBCollider.cs
│   └─ PhysicsSystem.cs
│
├─ Networking/                        ← snapshot & rollback systems
│   ├─ Snapshot.cs
│   ├─ InputBuffer.cs
│   └─ RollbackSystem.cs
│
├─ Utilities/                         ← helper utilities
│   ├─ XFixedRandom.cs
│   ├─ XFixedTime.cs
│   └─ XFixedDebugger.cs
│
└─ Tests/                             ← xUnit unit tests
    ├─ CoreTests/
    ├─ VectorTests/
    ├─ QuaternionTests/
    ├─ MatrixTests/
    ├─ PhysicsTests/
    ├─ NetworkingTests/
    └─ UtilitiesTests/

```

## Modules Overview

- **Core**: `XFixed` + `XFixedArithmetic` + `XFixedComparison` + `XFixedConstants` + `XFixedMath`
- **Vectors**: 2D/3D/4D vector operations
- **Quaternions**: quaternion rotation, SLERP, Euler ↔ Quat, matrix conversion
- **Matrices**: 4×4 matrix multiplication & point/vector transform
- **Physics**: rigid-body integration, AABB/Sphere/OBB SAT collision, impulse response
- **Networking**: deterministic `Snapshot`, `InputBuffer`, `RollbackSystem`
- **Utilities**: LCG RNG, fixed-point timekeeping, debugging helpers

------

## Quick Start

```c#
using XFixedPoint.Core;
using XFixedPoint.Vectors;
using XFixedPoint.Quaternions;
using XFixedPoint.Physics;

// Core arithmetic
var a = XFixed.FromDouble(1.25);
var b = XFixed.FromDouble(2.5);
var c = a * b;                     // 3.125
Console.WriteLine(c);

// Vector rotation
var v = new XFixedVector3(a, XFixed.Zero, XFixed.One);
var axis = new XFixedVector3(XFixed.Zero, XFixed.Zero, XFixed.One);
var q = XFixedQuaternion.FromAxisAngle(axis, XFixed.FromDouble(Math.PI/2));
var rotated = q.Rotate(v);

// Physics simulation
var physics = new PhysicsSystem { Gravity = FixedVector3.FromFloat(0, -9.81f, 0) };
var body    = new FixedRigidbody { Mass = XFixed.FromDouble(1.0) };
var sphere  = new XFixedPoint.Physics.Collision.SphereCollider(XFixed.FromDouble(0.5));
physics.AddBody(body, sphere);
physics.Step(XFixed.FromDouble(1.0/60.0));
```

------

## Sample Projects

See the `Samples/` folder (planned) for Unity and console examples.

------

We welcome performance improvements, pure-fixed implementations, new collision shapes, documentation enhancements, and more!