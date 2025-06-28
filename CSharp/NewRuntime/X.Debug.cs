
using System;
using System.Text;
using System.Threading.Tasks;
using UselessFrame.Net;
using UselessFrame.NewRuntime.Net;

namespace UselessFrame.NewRuntime
{
    public static partial class X
    {
        private static void PrintSystemException(object sender, UnhandledExceptionEventArgs e)
        {
            _logManager.Error($"system error happen");
            Exception ex = e.ExceptionObject as Exception;
            if (ex != null)
            {
                _logManager.Exception(ex);
            }
            else
            {
                _logManager.Error($"system error happen, but exception is null.");
            }
        }

        private static void PrintTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            _logManager.Error($"async task error happen");
            _logManager.Exception(e.Exception);
            e.SetObserved();
        }

        private static void PrintUniTaskException(Exception e)
        {
            _logManager.Error($"async unitask error happen");
            _logManager.Exception(e);
        }

        public static string GetDebugInfo()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine("============Pool Start============");
            //ShowNetPoolDebugInfo(sb, NetPoolUtility.CreateMessage, "MessageResultPool");
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
