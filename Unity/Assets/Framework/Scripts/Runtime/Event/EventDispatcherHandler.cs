
namespace UselessFrame.NewRuntime.Events
{
    /// <summary>
    /// 事件处理委托
    /// </summary>
    /// <param name="e">事件</param>
    public delegate void XEventHandler(XEvent e);

    /// <summary>
    /// 事件处理委托
    /// </summary>
    /// <param name="e">事件</param>
    /// <returns>返回true时移除此监听</returns>
    public delegate bool XEventHandler2(XEvent e);
}
