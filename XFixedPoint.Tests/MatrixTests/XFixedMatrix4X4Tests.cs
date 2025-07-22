using XFixedPoint.Core;
using XFixedPoint.Matrices;
using XFixedPoint.Vectors;

namespace XFixedPoint.Tests.MatrixTests;

    public class XFixedMatrix4X4Tests
    {
        private const double Tolerance = 1e-5;

        [Fact]
        public void Identity_Matrix_IsNoOp_ForPointAndVector()
        {
            var I = XFixedMatrix4x4.Identity;
            var v3 = new XFixedVector3(XFixed.FromDouble(1.2), XFixed.FromDouble(-3.4), XFixed.FromDouble(5.6));
            
            var pTransformed = I.MultiplyPoint(v3);
            Assert.InRange(pTransformed.X.ToDouble(), v3.X.ToDouble() - Tolerance, v3.X.ToDouble() + Tolerance);
            Assert.InRange(pTransformed.Y.ToDouble(), v3.Y.ToDouble() - Tolerance, v3.Y.ToDouble() + Tolerance);
            Assert.InRange(pTransformed.Z.ToDouble(), v3.Z.ToDouble() - Tolerance, v3.Z.ToDouble() + Tolerance);

            var vTransformed = I.MultiplyVector(v3);
            Assert.InRange(vTransformed.X.ToDouble(), v3.X.ToDouble() - Tolerance, v3.X.ToDouble() + Tolerance);
            Assert.InRange(vTransformed.Y.ToDouble(), v3.Y.ToDouble() - Tolerance, v3.Y.ToDouble() + Tolerance);
            Assert.InRange(vTransformed.Z.ToDouble(), v3.Z.ToDouble() - Tolerance, v3.Z.ToDouble() + Tolerance);
        }

        [Fact]
        public void TranslationMatrix_TranslatesPoint_ButNotVector()
        {
            // 构造平移矩阵：tx=2, ty=-3, tz=5
            var tx = XFixed.FromDouble(2.0);
            var ty = XFixed.FromDouble(-3.0);
            var tz = XFixed.FromDouble(5.0);
            var T = new XFixedMatrix4x4(
                XFixed.One,  XFixed.Zero, XFixed.Zero, tx,
                XFixed.Zero, XFixed.One,  XFixed.Zero, ty,
                XFixed.Zero, XFixed.Zero, XFixed.One,  tz,
                XFixed.Zero, XFixed.Zero, XFixed.Zero, XFixed.One
            );

            var p = new XFixedVector3(XFixed.FromDouble(1.0), XFixed.FromDouble(2.0), XFixed.FromDouble(3.0));
            var expectedPoint = new XFixedVector3(
                XFixed.FromDouble(1.0 + 2.0),
                XFixed.FromDouble(2.0 - 3.0),
                XFixed.FromDouble(3.0 + 5.0)
            );

            var pOut = T.MultiplyPoint(p);
            Assert.InRange(pOut.X.ToDouble(), expectedPoint.X.ToDouble() - Tolerance, expectedPoint.X.ToDouble() + Tolerance);
            Assert.InRange(pOut.Y.ToDouble(), expectedPoint.Y.ToDouble() - Tolerance, expectedPoint.Y.ToDouble() + Tolerance);
            Assert.InRange(pOut.Z.ToDouble(), expectedPoint.Z.ToDouble() - Tolerance, expectedPoint.Z.ToDouble() + Tolerance);

            // 向量变换不受平移影响
            var v = new XFixedVector3(XFixed.FromDouble(-1.0), XFixed.FromDouble(0.5), XFixed.FromDouble(4.0));
            var vOut = T.MultiplyVector(v);
            Assert.InRange(vOut.X.ToDouble(), v.X.ToDouble() - Tolerance, v.X.ToDouble() + Tolerance);
            Assert.InRange(vOut.Y.ToDouble(), v.Y.ToDouble() - Tolerance, v.Y.ToDouble() + Tolerance);
            Assert.InRange(vOut.Z.ToDouble(), v.Z.ToDouble() - Tolerance, v.Z.ToDouble() + Tolerance);
        }

        [Fact]
        public void ScalingMatrix_ScalesVector_AndPointEqually()
        {
            // 构造缩放矩阵：sx=2, sy=3, sz=-1
            var sx = XFixed.FromDouble(2.0);
            var sy = XFixed.FromDouble(3.0);
            var sz = XFixed.FromDouble(-1.0);
            var S = new XFixedMatrix4x4(
                sx,        XFixed.Zero, XFixed.Zero, XFixed.Zero,
                XFixed.Zero, sy,        XFixed.Zero, XFixed.Zero,
                XFixed.Zero, XFixed.Zero, sz,        XFixed.Zero,
                XFixed.Zero, XFixed.Zero, XFixed.Zero, XFixed.One
            );

            var p = new XFixedVector3(XFixed.FromDouble(1.5), XFixed.FromDouble(-2.0), XFixed.FromDouble(4.0));
            var expected = new XFixedVector3(
                XFixed.FromDouble(1.5 * 2.0),
                XFixed.FromDouble(-2.0 * 3.0),
                XFixed.FromDouble(4.0 * -1.0)
            );

            var pOut = S.MultiplyPoint(p);
            Assert.InRange(pOut.X.ToDouble(), expected.X.ToDouble() - Tolerance, expected.X.ToDouble() + Tolerance);
            Assert.InRange(pOut.Y.ToDouble(), expected.Y.ToDouble() - Tolerance, expected.Y.ToDouble() + Tolerance);
            Assert.InRange(pOut.Z.ToDouble(), expected.Z.ToDouble() - Tolerance, expected.Z.ToDouble() + Tolerance);

            var vOut = S.MultiplyVector(p);
            Assert.InRange(vOut.X.ToDouble(), expected.X.ToDouble() - Tolerance, expected.X.ToDouble() + Tolerance);
            Assert.InRange(vOut.Y.ToDouble(), expected.Y.ToDouble() - Tolerance, expected.Y.ToDouble() + Tolerance);
            Assert.InRange(vOut.Z.ToDouble(), expected.Z.ToDouble() - Tolerance, expected.Z.ToDouble() + Tolerance);
        }

        [Fact]
        public void MatrixMultiplication_ComposesTransformations()
        {
            // 创建平移 Tx(1,0,0) 和平移 Ty(0,2,0)
            var T1 = new XFixedMatrix4x4(
                XFixed.One, XFixed.Zero, XFixed.Zero, XFixed.FromDouble(1),
                XFixed.Zero, XFixed.One, XFixed.Zero, XFixed.Zero,
                XFixed.Zero, XFixed.Zero, XFixed.One, XFixed.Zero,
                XFixed.Zero, XFixed.Zero, XFixed.Zero, XFixed.One
            );
            var T2 = new XFixedMatrix4x4(
                XFixed.One, XFixed.Zero, XFixed.Zero, XFixed.Zero,
                XFixed.Zero, XFixed.One, XFixed.Zero, XFixed.FromDouble(2),
                XFixed.Zero, XFixed.Zero, XFixed.One,  XFixed.Zero,
                XFixed.Zero, XFixed.Zero, XFixed.Zero, XFixed.One
            );

            // 先 T1 再 T2，相当于平移 (1,2,0)
            var T12 = T2 * T1;
            var p = new XFixedVector3(XFixed.Zero, XFixed.Zero, XFixed.Zero);
            var pOut = T12.MultiplyPoint(p);

            Assert.InRange(pOut.X.ToDouble(), 1 - Tolerance, 1 + Tolerance);
            Assert.InRange(pOut.Y.ToDouble(), 2 - Tolerance, 2 + Tolerance);
            Assert.InRange(pOut.Z.ToDouble(), 0 - Tolerance, 0 + Tolerance);
        }
    }