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

    public static VertexGroup CreateCircle(GeometryShape owner, Point2 location, Color color, float radius)
    {
        var group = new VertexGroup(owner);
        var quality = 32; //(int)(radius * 0.5f);
        var da = 2.0f * MathF.PI / quality;

        var startIndex = owner.VertexArray.Length;
        Span<Vertex> pointer = stackalloc Vertex[quality + 1];

        group.StartIndex = startIndex;
        group.Length = quality + 1;

        for (var i = 0; i <= quality; i++)
        {
            var angle = da * i;
            pointer[i].Position = new Point2(
                location.X + radius * MathF.Cos(angle),
                location.Y + radius * MathF.Sin(angle));
            pointer[i].Color = color;
        }

        owner.VertexArray.AddSeveral(pointer);
        owner.VertexGroups.Add(group);
        return group;
    }

    public static VertexGroup CreateRoundedRectangle(GeometryShape owner, Point2 location, Size2 size, Color color,
        float radius)
    {
        var group = new VertexGroup(owner);
        const int quality = 32;

        var startIndex = owner.VertexArray.Length;
        Span<Vertex> pointer = stackalloc Vertex[quality + 1];

        group.StartIndex = startIndex;
        group.Length = quality + 1;

        Span<Point2> centersSpan = stackalloc Point2[5];
        centersSpan[0] = new Point2(size.Width - radius, size.Height - radius);
        centersSpan[1] = new Point2(radius, size.Height - radius);
        centersSpan[2] = new Point2(radius, radius);
        centersSpan[3] = new Point2(size.Width - radius, radius);
        centersSpan[4] = new Point2(size.Width - radius, size.Height - radius);

        var arcQuality = quality / 4;
        var da = 2.0f * MathF.PI / (quality - 4);

        for (var i = 0; i <= quality; i++)
        {
            var cornerIdx = i / arcQuality;
            var angle = da * (i - cornerIdx);

            pointer[i].Position =
                location + centersSpan[cornerIdx] + radius * new Point2(MathF.Cos(angle), MathF.Sin(angle));
            pointer[i].Color = color;
        }

        owner.VertexArray.AddSeveral(pointer);
        owner.VertexGroups.Add(group);
        return group;
    }

    public static VertexGroup CreateRoundedRectangleAuto(GeometryShape owner, Point2 location, Size2 size, Color color,
        float radius)
    {
        var group = new VertexGroup(owner);
        var quality = (int)(radius * 2);
        if (quality % 4 != 0) quality += 4 - quality % 4;
        // Console.WriteLine("RESULT QUALITY = {0}", quality);

        var startIndex = owner.VertexArray.Length;
        Span<Vertex> pointer = stackalloc Vertex[quality + 1];

        group.StartIndex = startIndex;
        group.Length = quality + 1;

        Span<Point2> centersSpan = stackalloc Point2[5];
        centersSpan[0] = new Point2(size.Width - radius, size.Height - radius);
        centersSpan[1] = new Point2(radius, size.Height - radius);
        centersSpan[2] = new Point2(radius, radius);
        centersSpan[3] = new Point2(size.Width - radius, radius);
        centersSpan[4] = new Point2(size.Width - radius, size.Height - radius);

        var arcQuality = quality / 4;
        var da = 2.0f * MathF.PI / (quality - 4);

        for (var i = 0; i <= quality; i++)
        {
            var cornerIdx = i / arcQuality;
            var angle = da * (i - cornerIdx);
            
            pointer[i].Position =
                location + centersSpan[cornerIdx] + radius * new Point2(MathF.Cos(angle), MathF.Sin(angle));
            pointer[i].Color = color;
        }

        owner.VertexArray.AddSeveral(pointer);
        owner.VertexGroups.Add(group);
        return group;
    }
    
    public static VertexGroup CreateRoundedRectangleAuto(GeometryShape owner, Point2 location, Size2 size, Color color,
        float radius, int quality)
    {
        var group = new VertexGroup(owner);
        //Console.WriteLine("RESULT QUALITY = {0}", quality);

        var startIndex = owner.VertexArray.Length;
        Span<Vertex> pointer = stackalloc Vertex[quality + 1];

        group.StartIndex = startIndex;
        group.Length = quality + 1;

        Span<Point2> centersSpan = stackalloc Point2[5];
        centersSpan[0] = new Point2(size.Width - radius, size.Height - radius);
        centersSpan[1] = new Point2(radius, size.Height - radius);
        centersSpan[2] = new Point2(radius, radius);
        centersSpan[3] = new Point2(size.Width - radius, radius);
        centersSpan[4] = new Point2(size.Width - radius, size.Height - radius);

        var arcQuality = quality / 4;
        var da = 2.0f * MathF.PI / (quality - 4);

        for (var i = 0; i <= quality; i++)
        {
            var cornerIdx = i / arcQuality;
            var angle = da * (i - cornerIdx);

            pointer[i].Position =
                location + centersSpan[cornerIdx] + radius * new Point2(MathF.Cos(angle), MathF.Sin(angle));
            pointer[i].Color = color;
        }

        owner.VertexArray.AddSeveral(pointer);
        owner.VertexGroups.Add(group);
        return group;
    }
}