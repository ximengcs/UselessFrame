
using TestIMGUI.Entities;
using UnityEngine;
using UselessFrame.NewRuntime.ECS;

namespace TestGame
{
    [ComponentOf(typeof(ColorComponent))]
    public class ColorView : EntityComponent
    {
        public SpriteRenderer Render;
    }
}
