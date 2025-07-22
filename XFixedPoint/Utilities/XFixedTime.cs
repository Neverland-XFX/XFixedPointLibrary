using XFixedPoint.Core;

namespace XFixedPoint.Utilities
{
    /// <summary>
    /// 定点时间管理，记录帧计数、DeltaTime 与累计时间
    /// </summary>
    public static class XFixedTime
    {
        /// <summary>
        /// 已运行的帧数
        /// </summary>
        public static int FrameCount { get; private set; }

        /// <summary>
        /// 上一帧的定点 DeltaTime
        /// </summary>
        public static XFixed DeltaTime { get; private set; }

        /// <summary>
        /// 从开始累积的定点时间
        /// </summary>
        public static XFixed Time { get; private set; }

        /// <summary>
        /// 刷新一次时间：传入本帧的定点步长（如 Fixed.FromFloat(1f/60f)）
        /// </summary>
        public static void Tick(XFixed deltaTime)
        {
            DeltaTime = deltaTime;
            Time += deltaTime;
            FrameCount++;
        }

        /// <summary>
        /// 重置所有时间与帧计数
        /// </summary>
        public static void Reset()
        {
            FrameCount = 0;
            DeltaTime = XFixed.Zero;
            Time = XFixed.Zero;
        }
    }
}