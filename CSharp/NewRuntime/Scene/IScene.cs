using System;
using System.Collections.Generic;
using System.Text;
using UselessFrame.NewRuntime.Entity;
using UselessFrame.NewRuntime.Router;

namespace UselessFrame.NewRuntime.Scene
{
    public interface IScene : IRouter
    {
        IEntityManager Entity { get; }
    }
}
