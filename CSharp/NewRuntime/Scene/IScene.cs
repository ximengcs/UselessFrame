using System;
using System.Collections.Generic;
using System.Text;
using UselessFrame.NewRuntime.Entity;

namespace UselessFrame.NewRuntime.Scene
{
    public interface IScene
    {
        IEntityManager Entity { get; }

        IRouter Router { get; }
    }
}
