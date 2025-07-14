
using QuadTrees.QTreeRectF;
using System.Drawing;
using Unity.Mathematics;

namespace UselessFrame.NewRuntime.Entities
{
    public class TransformComponent : Component, IRectFQuadStorable
    {

        public float3 Position;

        public RectangleF Rect => RectangleF.Empty;

    }
}
