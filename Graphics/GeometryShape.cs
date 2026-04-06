using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using NatLib.Arrays;
using NatLib.Core.Structures;

namespace QuilartUI.Graphics;

public sealed class GeometryShape
{
    public PointerArray<Vertex> VertexArray { get; set; }

    public PointerArray<int> IndexArray { get; set; }

    public List<VertexGroup> VertexGroups { get; private set; } = [];

    public GeometryShape()
    {
        VertexArray = new PointerArray<Vertex>();
        IndexArray = new PointerArray<int>();
    }

    public VertexGroup AddVertexGroup(int verticesCount)
    {
        var startVertex = VertexArray.Length;
        var startIndex = IndexArray.Length;

        // VertexArray.Resize(group.VerticesCount);
        // IndexArray.Resize(group.VerticesCount);


        var group = new VertexGroup(this, verticesCount, null, null);

        VertexGroups.Add(group);

        return group;
    }

    ~GeometryShape()
    {
        VertexArray.Dispose();
        IndexArray.Dispose();
    }

    public const float FeatheringThickness = 0.8f;

    public static int GetIndicesLength(int vertsCount) => 3 * (vertsCount - 2);

    public static Point2 GetCirclePoint(int index, float radius, int quality)
    {
        var da = 2.0f * MathF.PI / quality;
        var angle = da * index;
        return radius * new Point2(MathF.Cos(angle), MathF.Sin(angle));
    }

    public static Point2 GetRoundedRectanglePoint(int index, Size2 size, float radius, int quality)
    {
        Console.WriteLine("Got rounded rect for index: " + index + " quality: " + quality);

        Span<Point2> centersSpan = stackalloc Point2[5];
        centersSpan[0] = new Point2(size.Width - radius, size.Height - radius);
        centersSpan[1] = new Point2(radius, size.Height - radius);
        centersSpan[2] = new Point2(radius, radius);
        centersSpan[3] = new Point2(size.Width - radius, radius);
        centersSpan[4] = new Point2(size.Width - radius, size.Height - radius);

        var arcQuality = quality / 4;

        var cornerIdx = index / arcQuality;

        return centersSpan[cornerIdx] + GetCirclePoint(index - cornerIdx, radius, quality - 4);
    }

    public static Point2 GetRoundedRectanglePointFeathering(int index, Size2 size, float radius, int quality)
    {
        Span<Point2> centersSpan = stackalloc Point2[5];
        centersSpan[0] = new Point2(size.Width - radius, size.Height - radius);
        centersSpan[1] = new Point2(radius, size.Height - radius);
        centersSpan[2] = new Point2(radius, radius);
        centersSpan[3] = new Point2(size.Width - radius, radius);
        centersSpan[4] = new Point2(size.Width - radius, size.Height - radius);

        var arcQuality = quality / 4;

        var cornerIdx = index / arcQuality;

        return centersSpan[cornerIdx] + GetCirclePoint(index - cornerIdx, radius + FeatheringThickness, quality - 4);
    }

    public static void GenerateRoundedRectangle(Span<Vertex> vertices, Point2 position, Size2 size, Color color,
        float radius,
        int quality)
    {
        for (int i = 0; i <= quality; i++)
        {
            vertices[i].Position = position + GetRoundedRectanglePoint(i, size, radius, quality);
            vertices[i].Color = color;
        }
    }

    public static void GenerateRoundedRectangleFeathering(Span<Vertex> vertices, Point2 position, Size2 size,
        Color color, float radius,
        int quality)
    {
        for (var i = 0; i <= quality; i++)
        {
            vertices[i].Position = position + GetRoundedRectanglePointFeathering(i, size, radius, quality);
            vertices[i].Color = color.WithAlpha(0);
        }
    }

    public static void GenerateCircleFeathering(Span<Vertex> vertices, Color color, Point2 position, float radius,
        int quality)
    {
        var da = 2.0f * MathF.PI / quality;
        var featheringRadius = radius + FeatheringThickness;
        for (var i = 0; i <= quality; i++)
        {
            var angle = da * i;
            vertices[i] = new Vertex(
                new Point2(
                    position.X + featheringRadius * MathF.Cos(angle),
                    position.Y + featheringRadius * MathF.Sin(angle)),
                color.WithAlpha(0)
            );
        }
    }

    public static void GenerateCircle(Span<Vertex> vertices, Color color, Point2 position, float radius, int quality)
    {
        var da = 2.0f * MathF.PI / quality;
        for (var i = 0; i <= quality; i++)
        {
            var angle = da * i;
            vertices[i].Position = new Point2(
                position.X + radius * MathF.Cos(angle),
                position.Y + radius * MathF.Sin(angle));
            vertices[i].Color = color;
        }
    }

    public static unsafe void FillTriangleStrip(Span<int> indices, int vertCount)
    {
        for (var i = 0; i < vertCount - 2; i++)
        {
            indices[i * 3 + 2] = i + 2;

            if (i % 2 == 0)
            {
                indices[i * 3] = i;
                indices[i * 3 + 1] = i + 1;
            }
            else
            {
                indices[i * 3] = i + 1;
                indices[i * 3 + 1] = i;
            }
        }
    }

    public static void FillTriangleStripAlign(Span<int> indices, int vertCount)
    {
        //TODO: change that to MemoryMarshal.GetReference to suppress bound checks for every record  
        var alignment = vertCount / 2;
        var lastIndex = 0;
        for (var i = 0; i < alignment - 1; i++)
        {
            indices[lastIndex++] = i;
            indices[lastIndex++] = i + 1;
            indices[lastIndex++] = i + alignment;

            indices[lastIndex++] = i + 1;
            indices[lastIndex++] = i + alignment;
            indices[lastIndex++] = i + alignment + 1;
        }
    }

    public static void FillTriangleFan(Span<int> indices, int vertCount)
    {
        for (var i = 0; i < vertCount - 2; i++)
        {
            indices[i * 3] = 0;
            indices[i * 3 + 1] = i + 1;
            indices[i * 3 + 2] = i + 2;
        }
    }


    public static GeometryShape CreateCircle(Point2 location, Color color, int radius)
    {
        const int quality = 33;
        var shape = new GeometryShape();

        var mainGroup = shape.AddVertexGroup(quality);

        GenerateCircle(mainGroup.GetVertexArea(), color, location, radius, quality);


        // //quality = 5, vertices = 12
        // var quality = 32;
        // var indicesFanCount = GetIndicesLength(quality);
        // var indicesStripCount = GetIndicesLength(quality * 2) + 6;
        // var verticesCount = quality * 2 + 2;
        // // * 2 - feathering cycle around base vertices,
        // // + 2 - strip end vertices(to make feathering logically connected
        //
        // var shape = new GeometryShape(verticesCount, indicesFanCount + indicesStripCount);
        //
        // GenerateCircle(shape.VertexArray.AsSpan(), color, location, radius, quality);
        // FillTriangleFan(shape.IndexArray.AsSpan(), quality);
        // GenerateCircleFeathering(shape.VertexArray.AsSpan()[(quality + 1)..], color, location, radius, quality);
        // FillTriangleStripAlign(shape.IndexArray.AsSpan()[indicesFanCount..], verticesCount);
        //
        // return shape;
    }

    public static GeometryShape CreateRoundedRectangle(Point2 location, Size2 size, Color color, int radius)
    {
        const int quality = 32;
        var indicesFanCount = GetIndicesLength(quality);
        var indicesStripCount = GetIndicesLength(quality * 2) + 6;
        var verticesCount = quality * 2 + 2;

        var shape = new GeometryShape(verticesCount, indicesFanCount + indicesStripCount);

        GenerateRoundedRectangle(shape.VertexArray.AsSpan(), location, size, color, radius, quality);
        FillTriangleFan(shape.IndexArray.AsSpan(), quality);
        GenerateRoundedRectangleFeathering(shape.VertexArray.AsSpan()[(quality + 1)..], location, size, color, radius,
            quality);
        FillTriangleStripAlign(shape.IndexArray.AsSpan()[indicesFanCount..], verticesCount);

        Console.WriteLine(shape.ToString());


        return shape;
    }

    public override string ToString()
    {
        var bld = new StringBuilder();

        bld.AppendLine($"indices: {string.Join(',', IndexArray)}");
        for (int i = 0; i < VertexArray.Length; i++)
        {
            bld.AppendLine($"vertex {i}: {VertexArray[i]}");
        }

        return bld.ToString();
    }
}