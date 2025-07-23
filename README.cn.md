# XFixedPoint 定点数学与物理仿真库

[TOC]

[English Documentation | English Version](README.md)

[![.NET Standard 2.1](https://img.shields.io/badge/.NET-Standard_2.1-blue)](https://docs.microsoft.com/dotnet/standard/net-standard) [![License: MIT](https://img.shields.io/badge/License-MIT-green)](LICENSE)

## 项目简介

**XFixedPoint** 是一个基于 [.NET Standard 2.1](https://docs.microsoft.com/dotnet/standard/net-standard) 的生产级定点数数学与物理仿真库，致力于：

- **确定性**
- **高性能**
- **功能完备**
- **易用性**

---

## 特性

- **核心类型 `XFixed`**：64.32 位定点表示，支持加/减/乘/除、比较、转换  
- **数学函数 `XFixedMath`**：Newton 迭代开方、CORDIC 三角、Exp/Log、Lerp/Clamp/Min/Max  
- **向量与矩阵**：`FixedVector2/3/4`、`FixedQuaternion`、`FixedMatrix4x4` 全面支持变换与插值  
- **物理仿真**：`FixedRigidbody` 积分 + AABB/Sphere/OBB 碰撞（SAT）+ 弹性冲量响应  
- **网络同步**：`Snapshot`、`InputBuffer`、`RollbackSystem` 实现确定性回滚  
- **辅助工具**：定点随机数、`FixedTime` 管理、`FixedDebugger` 误差统计  
- **全面测试**：xUnit 单元测试覆盖所有核心功能  



# 项目结构
```text
XFixedPoint/                         ← 仓库根目录
├─ XFixedPoint.csproj                ← .NET Standard 2.1 项目
├─ README.md                         ← 文档
├─ LICENSE                           ← MIT 许可证
├─ Properties/AssemblyInfo.cs        ← 程序集元数据
│
├─ Core/                              ← 核心定点类型与算术
│   ├─ XFixedConstants.cs             ← 位宽、ONE、EPS 等常量
│   ├─ XFixed.cs                      ← `XFixed` 结构体与基本转换
│   ├─ XFixedArithmetic.cs            ← 加/减/乘/除 实现
│   ├─ XFixedComparison.cs            ← 比较运算、接口实现
│   └─ XFixedMath.cs                  ← Sqrt、Sin/Cos/CORDIC、Exp/Log、辅助函数
│
├─ Vectors/                           ← 定点向量
│   ├─ XFixedVector2.cs
│   ├─ XFixedVector3.cs
│   └─ XFixedVector4.cs
│
├─ Quaternions/                       ← 定点四元数
│   ├─ XFixedQuaternion.cs
│   └─ XFixedQuaternionOps.cs
│
├─ Matrices/                          ← 4×4 定点矩阵
│   └─ XFixedMatrix4x4.cs
│
├─ Physics/                           ← 物理仿真
│   ├─ FixedRigidbody.cs
│   ├─ FixedCollider.cs
│   ├─ Collision/
│   │   ├─ AABBCollider.cs
│   │   ├─ SphereCollider.cs
│   │   └─ OBBCollider.cs
│   └─ PhysicsSystem.cs
│
├─ Networking/                        ← 快照与回滚
│   ├─ Snapshot.cs
│   ├─ InputBuffer.cs
│   └─ RollbackSystem.cs
│
├─ Utilities/                         ← 辅助工具
│   ├─ XFixedRandom.cs
│   ├─ XFixedTime.cs
│   └─ XFixedDebugger.cs
│
└─ Tests/                             ← xUnit 单元测试
    ├─ CoreTests/
    ├─ VectorTests/
    ├─ QuaternionTests/
    ├─ MatrixTests/
    ├─ PhysicsTests/
    ├─ NetworkingTests/
    └─ UtilitiesTests/
```

## 模块概览

- **Core**：64.32 位定点 `XFixed`，`XFixedArithmetic`、`XFixedComparison`、`XFixedConstants`、`XFixedMath`
- **Vectors**：2D/3D/4D 向量运算
- **Quaternions**：四元数旋转、SLERP、欧拉角互转、矩阵
- **Matrices**：4×4 矩阵变换
- **Physics**：刚体积分、AABB/Sphere/OBB 碰撞、冲量响应
- **Networking**：Snapshot、InputBuffer、RollbackSystem
- **Utilities**：LCG 随机数、定点时间、误差调试

------

# 快速开始

```c#
using XFixedPoint.Core;
using XFixedPoint.Vectors;
using XFixedPoint.Quaternions;
using XFixedPoint.Physics;

// 核心算术
var a = XFixed.FromDouble(1.25);
var b = XFixed.FromDouble(2.5);
var c = a * b;                     // 3.125
Console.WriteLine(c);              // "3.125"

// 向量旋转
var v = new XFixedVector3(a, XFixed.Zero, XFixed.One);
var axis = new XFixedVector3(XFixed.Zero, XFixed.Zero, XFixed.One);
var q = XFixedQuaternion.FromAxisAngle(axis, XFixed.FromDouble(Math.PI/2));
var rotated = q.Rotate(v);

// 物理仿真
var phys = new PhysicsSystem { Gravity = FixedVector3.FromFloat(0, -9.81f, 0) };
var body = new FixedRigidbody { Mass = XFixed.FromDouble(1.0) };
var sphere = new XFixedPoint.Physics.Collision.SphereCollider(XFixed.FromDouble(0.5));
phys.AddBody(body, sphere);
phys.Step(XFixed.FromDouble(1.0/60.0));
```

## 示例项目

请参阅 `Samples/`（计划中）获取 Unity 与控制台示例。

------

欢迎性能优化、纯定点算法、新碰撞形状、文档改进等各类贡献！