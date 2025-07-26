// using XFixedPoint.Networking;
//
// namespace XFixedPoint.Tests.NetworkingTests;
//
// public class InputBufferTests
// {
//     [Fact]
//     public void AddTryGetGet_RemoveOld_Behavior()
//     {
//         var buf = new InputBuffer<string>();
//
//         // 添加并覆盖
//         buf.AddInput(1, "one");
//         buf.AddInput(2, "two");
//         buf.AddInput(1, "uno"); // 覆盖 tick=1
//         Assert.True(buf.TryGetInput(1, out var val1));
//         Assert.Equal("uno", val1);
//         Assert.True(buf.TryGetInput(2, out var val2));
//         Assert.Equal("two", val2);
//
//         // GetInput 抛出异常
//         Assert.Throws<KeyNotFoundException>(() => buf.GetInput(3));
//
//         // 移除早于 tick=2 的输入（移除 tick=1）
//         buf.RemoveOld(2);
//         Assert.False(buf.TryGetInput(1, out _));
//         Assert.True(buf.TryGetInput(2, out _));
//
//         // Ticks 集合应只包含 2
//         Assert.Single(buf.Ticks);
//         Assert.Equal(2, buf.Ticks.First());
//     }
// }