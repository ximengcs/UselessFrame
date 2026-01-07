
using UnityEngine;
using UselessFrame.NewRuntime.ECS;
using UselessFrameCommon.Entities;

namespace TestGame
{
    [ComponentOf(typeof(ColorComponent))]
    public class ColorView : EntityComponent
    {
        public SpriteRenderer Render;
    }
}
