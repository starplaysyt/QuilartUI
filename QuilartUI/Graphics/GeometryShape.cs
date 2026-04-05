using NatLib.Core.Structures;

namespace QuilartUI.Graphics;

public sealed class GeometryShape
{
    public Vertex[] VertexArray { get; set; }

    public int[] IndexArray { get; set; }

    public GeometryShape(int vertexCount, int indexCount)
    {
        VertexArray = new Vertex[vertexCount];
        IndexArray = new int[indexCount];
    }
    
    public static int GetIndicesLength(int vertsCount) => 3 * (vertsCount - 2);

    public static Point2 GetCirclePoint(int index, float radius, int quality)
    {
        var da = 2.0f * MathF.PI / quality;
        var angle = da * index;
        return radius * new Point2(MathF.Cos(angle), MathF.Sin(angle));
    }

    public static Point2 GetCircleFeatheringPoint(int index, float radius, int quality, float featheringMod)
    {
        var da = 2.0f * MathF.PI / quality;
        var angle = da * index;
        return (radius + featheringMod) * new Point2(MathF.Cos(angle), MathF.Sin(angle));
    }

    public static void GenerateCircleFeathering(Span<Vertex> vertices, Color color, Point2 position, float radius, int quality)
    {
        for (int i = 0; i <= quality; i++)
        {
            vertices[i].Position = position + GetCircleFeatheringPoint(i, radius, quality, 2f);
            vertices[i].Color = color.WithAlpha(0);
        }
    }

    public static void GenerateCircle(Span<Vertex> vertices, Color color, Point2 position, float radius, int quality)
    {
        for (int i = 0; i <= quality; i++)
        {
            vertices[i].Position = position + GetCirclePoint(i, radius, quality);
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
        var alignment = vertCount / 2;
        var lastIndex = 0;
        for (var i = 0; i < alignment - 1; i++)
        {
            indices[lastIndex] = i;
            indices[lastIndex + 1] = i + 1;
            indices[lastIndex + 2] = i + alignment;

            indices[lastIndex + 3] = i + 1;
            indices[lastIndex + 4] = i + alignment;
            indices[lastIndex + 5] = i + alignment + 1;

            lastIndex += 6;
        }
    }

    public static void FillTriangleFan(int[] indices, int vertCount)
    {
        for (var i = 0; i < vertCount - 2; i++)
        {
            indices[i * 3] = 0;
            indices[i * 3 + 1] = i + 1;
            indices[i * 3 + 2] = i + 2;
        }
    }

    public static GeometryShape CreateCircle(Point2 location, Color color, int radius, int quality)
    {
        //quality = 5, vertices = 12
        
        var indicesFanCount = GetIndicesLength(quality);
        var indicesStripCount = GetIndicesLength(quality * 2) + 6;
        var verticesCount = quality * 2 + 2; 
        // * 2 - feathering cycle around base vertices,
        // + 2 - strip end vertices(to make feathering logically connected
        
        var shape = new GeometryShape(verticesCount, indicesFanCount + indicesStripCount);

        GenerateCircle(shape.VertexArray, color, location, radius, quality);
        FillTriangleFan(shape.IndexArray, quality);
        GenerateCircleFeathering(shape.VertexArray.AsSpan()[(quality + 1)..], color, location, radius, quality);
        FillTriangleStripAlign(shape.IndexArray.AsSpan()[indicesFanCount..], verticesCount);
        
        return shape;
    }

    public static GeometryShape CreateRoundedRectangle(Point2 location, Size2 size, int radius, float feathering)
    {
        return null;
    }
}