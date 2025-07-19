
using MemoryPack;
using QuadTrees.QTreeRectF;
using System.Drawing;
using Unity.Mathematics;

namespace UselessFrame.NewRuntime.Entities
{
    [CoreComponent]
    [MemoryPackable]
    public partial class TransformComponent : EntityComponent, IRectFQuadStorable
    {
        private RectangleF _rect;

        public float3 Position;

        public RectangleF Rect
        {
            get => _rect;
            set => _rect = value;
        }
    }
}
