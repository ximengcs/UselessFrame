
using System.Numerics;

namespace Core.Application
{
    public class AppUtility 
    {
        public static readonly Vector4 Red = new Vector4(1.0f, 0.0f, 0.0f, 1.0f);       // 纯红
        public static readonly Vector4 Green = new Vector4(0.0f, 1.0f, 0.0f, 1.0f);     // 纯绿
        public static readonly Vector4 Blue = new Vector4(0.0f, 0.0f, 1.0f, 1.0f);      // 纯蓝
        public static readonly Vector4 Yellow = new Vector4(1.0f, 1.0f, 0.0f, 1.0f);    // 黄色
        public static readonly Vector4 Magenta = new Vector4(1.0f, 0.0f, 1.0f, 1.0f);   // 品红
        public static readonly Vector4 Cyan = new Vector4(0.0f, 1.0f, 1.0f, 1.0f);      // 青色
        public static readonly Vector4 White = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);     // 纯白
        public static readonly Vector4 Black = new Vector4(0.0f, 0.0f, 0.0f, 1.0f);     // 纯黑
        public static readonly Vector4 Gray = new Vector4(0.5f, 0.5f, 0.5f, 1.0f);      // 灰色

        // 柔和颜色
        public static readonly Vector4 LightRed = new Vector4(1.0f, 0.5f, 0.5f, 1.0f);     // 浅红
        public static readonly Vector4 LightGreen = new Vector4(0.5f, 1.0f, 0.5f, 1.0f);   // 浅绿
        public static readonly Vector4 LightBlue = new Vector4(0.5f, 0.5f, 1.0f, 1.0f);    // 浅蓝

        // 深色系
        public static readonly Vector4 DarkRed = new Vector4(0.5f, 0.0f, 0.0f, 1.0f);      // 深红
        public static readonly Vector4 DarkGreen = new Vector4(0.0f, 0.5f, 0.0f, 1.0f);    // 深绿
        public static readonly Vector4 DarkBlue = new Vector4(0.0f, 0.0f, 0.5f, 1.0f);     // 深蓝

        // 透明色（Alpha < 1.0）
        public static readonly Vector4 TransparentRed = new Vector4(1.0f, 0.0f, 0.0f, 0.5f);   // 半透明红
        public static readonly Vector4 TransparentGreen = new Vector4(0.0f, 1.0f, 0.0f, 0.5f); // 半透明绿
    }
}
