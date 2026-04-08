using NatLib.Arrays;
using NatLib.Core.Structures;
using SDL;

namespace QuilartUI.Graphics;

public class VertexGroup
{
    public int StartIndex { get; private set; } = 0;

    public int Length { get; private set; } = 0;

    public GeometryShape Owner { get; private set; }

    public unsafe VertexGroup(GeometryShape owner)
    {
        Owner = owner;
    }

    public static VertexGroup CreateCircle(GeometryShape owner, Point2 location, Color color, int radius)
    {
        var group = new VertexGroup(owner);
        var quality = (int)(radius * 0.8f);
        var da = 2.0f * MathF.PI / quality;
        
        var startIndex = owner.VertexArray.Length;
        Span<Vertex> pointer = stackalloc Vertex[quality + 1];
        
        group.StartIndex = startIndex;
        group.Length = quality;
        
        for (var i = 0; i <= quality; i++)
        {
            var angle = da * i;
            pointer[i].Position = new Point2(
                location.X + radius * MathF.Cos(angle),
                location.Y + radius * MathF.Sin(angle));
            pointer[i].Color = color;
        }
        
        owner.VertexArray.AddSeveral(pointer);

        return group;
    }
}