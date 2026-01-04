
using System;

namespace UselessFrame.UIElements
{
    public interface IUIManager
    {
        IUIHandle OpenUIAsync(Type uiType, string groupName, long id, object userData = null);

        IUIHandle OpenUIAsync<T>(string groupName, long id, object userData = null) where T : IUI;

        IUIHandle CloseUIAsync(Type uiType, long id, bool destroy = true);

        IUIHandle CloseUIAsync<T>(long id, bool destroy = true) where T : IUI;
    }
}
