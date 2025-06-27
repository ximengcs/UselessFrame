
using System.Text;
using UselessFrame.Net;
using UselessFrame.NewRuntime.Net;

namespace UselessFrame.NewRuntime
{
    public static partial class X
    {

        public static string GetDebugInfo()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine("============Pool Start============");
            ShowNetPoolDebugInfo(sb, NetPoolUtility._messageResultPool, "MessageResultPool");
            sb.AppendLine("============Pool End============");
            sb.AppendLine("============Ref Start============");
            sb.AppendLine();
            foreach (var obj in NetDebugInfo.GetObject())
            {
                sb.AppendLine($"{obj.IsAlive,-8} | {obj.HeadInfo} | {obj.Msg}");
            }
            sb.AppendLine("============Ref End============");
            return sb.ToString();
        }

        private static void ShowNetPoolDebugInfo<T>(StringBuilder sb, NetObjectPool<T> pool, string title) where T : class, new()
        {
            sb.AppendLine($"------------{title} Start------------");
            sb.Append($"|{pool.Count,-4}");
            sb.Append($"|{pool.MaxSize,-4}");
            sb.Append($"|{pool._DEBUG_requireTimes,-10}");
            sb.Append($"|{pool._DEBUG_reuseTimes,-10}");
            sb.Append($"|{pool._DEBUG_newTimes,-10}");
            sb.Append($"|{pool._DEBUG_releaseTimes,-10}");
            sb.Append($"|{pool._DEBUG_toPoolTimes,-10}");
            sb.Append($"|{pool._DEBUG_wasteTimes,-10}|");
            sb.AppendLine();
            sb.AppendLine($"------------{title} End------------");
        }
    }
}
