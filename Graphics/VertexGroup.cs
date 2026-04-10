using NatLib.Core.Structures;

namespace QuilartUI.Graphics;

public class VertexGroup
{
    public int StartIndex { get; private set; }

    public int Length { get; private set; }

    public GeometryShape Owner { get; private set; }

    public VertexGroup(GeometryShape owner)
    {
        Owner = owner;
    }

    public static VertexGroup CreateRoundedRectangle(GeometryShape owner, Point2 location, Size2 size, Color color,
        float radius)
    {
        var group = new VertexGroup(owner);
        radius = float.Clamp(radius, 0, float.Min(size.Width, size.Height) / 2);
        var quality = (int)float.Clamp(radius * 4 * 0.8f, 16, float.MaxValue);
        if (quality % 4 != 0) quality += 4 - quality % 4;

        var startIndex = owner.VertexArray.Length;
        group.StartIndex = startIndex;
        group.Length = quality + 1;

        Span<Point2> centersSpan = stackalloc Point2[5];
        centersSpan[0] = new Point2(size.Width - radius, size.Height - radius);
        centersSpan[1] = new Point2(radius, size.Height - radius);
        centersSpan[2] = new Point2(radius, radius);
        centersSpan[3] = new Point2(size.Width - radius, radius);
        centersSpan[4] = new Point2(size.Width - radius, size.Height - radius);

        Span<Vertex> pointer = stackalloc Vertex[quality + 1];

        var arcQuality = quality / 4;
        var da = 2.0f * MathF.PI / (quality - 4);

        for (var i = 0; i <= quality; i++)
        {
            var cornerIdx = i / arcQuality;
            var currentCenter = centersSpan[cornerIdx];
            var angle = da * (i - cornerIdx);

            var (sin, cos) = MathF.SinCos(angle);

            pointer[i].Position = new Point2(
                location.X + currentCenter.X + radius * cos,
                location.Y + currentCenter.Y + radius * sin);

            pointer[i].Color = color;
        }

        owner.VertexArray.AddSeveral(pointer);
        owner.VertexGroups.Add(group);
        return group;
    }
}