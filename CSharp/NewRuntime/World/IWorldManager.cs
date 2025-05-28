using System;
using System.Collections.Generic;
using System.Text;
using UselessFrame.NewRuntime.Scene;

namespace UselessFrame.NewRuntime.World
{
    public interface IWorldManager
    {
        ISceneManager Scene { get; }

        IRouter Router { get; }
    }
}
