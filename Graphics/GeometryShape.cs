using System.Text;
using NatLib.Arrays;
using NatLib.Core.Structures;
using NatLib.Logging;

namespace QuilartUI.Graphics;

public sealed class GeometryShape
{
    private static ConsoleLogger Logger { get; } = LoggerFactory.Create<GeometryShape>();
    public PointerList<Vertex> VertexArray { get; set; }

    public PointerList<int> IndexArray { get; set; }

    public List<VertexGroup> VertexGroups { get; private set; } = [];
    
    public List<IndexGroup> IndexGroups { get; private set; } = [];

    public GeometryShape(int vertexCount, int indexCount)
    {
        VertexArray = new PointerList<Vertex>(vertexCount);
        IndexArray = new PointerList<int>(indexCount);
    }

    public GeometryShape()
    {
        Logger.LogTrace("Creating new geometry shape...");
        VertexArray = new();
        IndexArray = new();
    }
    
    public VertexGroup AddVertexGroup(int verticesCount)
    {
        // var startVertex = VertexArray.Length;
        // var startIndex = IndexArray.Length;
        //
        // // VertexArray.Resize(group.VerticesCount);
        // // IndexArray.Resize(group.VerticesCount);
        //
        //
        // var group = new VertexGroup(this, verticesCount, null, null);
        //
        // VertexGroups.Add(group);
        //
        // return group;

        return null;
    }

    ~GeometryShape()
    {
        VertexArray.Dispose();
        IndexArray.Dispose();
    }

    public const float FeatheringThickness = 0.8f;



    public static Point2 GetCirclePoint(int index, float radius, int quality)
    {
        var da = 2.0f * MathF.PI / quality;
        var angle = da * index;
        return radius * new Point2(MathF.Cos(angle), MathF.Sin(angle));
    }

    // public static Point2 GetRoundedRectanglePoint(int index, Size2 size, float radius, int quality)
    // {
    //     Console.WriteLine("Got rounded rect for index: " + index + " quality: " + quality);
    //
    //     return 
    // }

    // public static Point2 GetRoundedRectanglePointFeathering(int index, Size2 size, float radius, int quality)
    // {
    //
    // }

    public static void GenerateRoundedRectangle(Span<Vertex> vertices, Point2 position, Size2 size, Color color,
        float radius,
        int quality)
    {
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
            var angle = da * i - cornerIdx;

            vertices[i].Position =
                position + centersSpan[cornerIdx] + GetCirclePoint(i - cornerIdx, radius, quality - 4);
            vertices[i].Color = color;
        }
    }

    public static void GenerateRoundedRectangleFeathering(Span<Vertex> vertices, Point2 position, Size2 size,
        Color color, float radius,
        int quality)
    {
        Span<Point2> centersSpan = stackalloc Point2[5];
        centersSpan[0] = new Point2(size.Width - radius, size.Height - radius);
        centersSpan[1] = new Point2(radius, size.Height - radius);
        centersSpan[2] = new Point2(radius, radius);
        centersSpan[3] = new Point2(size.Width - radius, radius);
        centersSpan[4] = new Point2(size.Width - radius, size.Height - radius);
        
        var arcQuality = quality / 4;
        
        for (var i = 0; i <= quality; i++)
        {
            var cornerIdx = i / arcQuality;
            
            vertices[i].Position = position 
                                   + centersSpan[cornerIdx] 
                                   + GetCirclePoint(i - cornerIdx, radius + FeatheringThickness, quality - 4);
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

    public static int GetIndicesLength(int vertCount) => 3 * (vertCount - 2);

    public static GeometryShape CreateCircle(Point2 center, Color color, float radius)
    {
        Logger.LogTrace("Creating circle shape...");
        var geomShape = new GeometryShape();

        var circleBody = VertexGroup.CreateCircle(geomShape, center, color, radius);
        
        geomShape.VertexGroups.Add(circleBody);

        var circleFill = IndexGroup.ConnectSelfFan(geomShape, circleBody);
        
        geomShape.IndexGroups.Add(circleFill);
        
        return geomShape;
    }

    public static GeometryShape CreateComplexShape(Point2 location, Color color, int radius)
    {
        Logger.LogTrace("Creating complex shape...");
        var geomShape = new GeometryShape();
        //
        // Logger.LogTrace("Creating vertex groups...");
        // var vertGroup1 = VertexGroup.CreateCircle(geomShape, location - new Point2(50, 50), color.WithR(0.8f).WithAlpha(0.5f), radius);
        // var vertGroup2 = VertexGroup.CreateCircle(geomShape, location + new Point2(50, 50), color.WithR(0.1f).WithAlpha(0.5f), radius);
        // var vertGroup3 = VertexGroup.CreateCircle(geomShape, location + new Point2(0, 50), color.WithR(0.9f).WithAlpha(0.5f), radius);
        //
        // geomShape.VertexGroups.Add(vertGroup1);
        // geomShape.VertexGroups.Add(vertGroup2);
        // geomShape.VertexGroups.Add(vertGroup3);
        //
        // Logger.LogTrace("Creating index groups...");
        //
        // var indexGroup1 = IndexGroup.ConnectSelfFan(geomShape, vertGroup1);
        // var indexGroup2 = IndexGroup.ConnectSelfStrip(geomShape, vertGroup2);
        // var indexGroup3 = IndexGroup.ConnectStrip(geomShape, vertGroup3, vertGroup1);
        //
        // geomShape.IndexGroups.Add(indexGroup1);
        // geomShape.IndexGroups.Add(indexGroup2);
        // geomShape.IndexGroups.Add(indexGroup3);

        var outerFeathering = VertexGroup.CreateCircle(geomShape, location, color.WithAlpha(0), radius + 1f);
        var circleOuter = VertexGroup.CreateCircle(geomShape, location, color, radius);
        var circleInner = VertexGroup.CreateCircle(geomShape, location, color, radius - 10);
        var innerFeathering = VertexGroup.CreateCircle(geomShape, location, color.WithAlpha(0), radius - 10 - 1f);
        
        geomShape.VertexGroups.Add(outerFeathering);
        geomShape.VertexGroups.Add(circleOuter);
        geomShape.VertexGroups.Add(circleInner);
        geomShape.VertexGroups.Add(innerFeathering);
        
        // var vertexGroup1 = VertexGroup.CreateCircle(geomShape, location, color, radius);
        // var vertexGroup2 = VertexGroup.CreateCircle(geomShape, location, color.WithAlpha(0), radius + 5);
        // var vertexGroup3 = VertexGroup.CreateRoundedRectangle(geomShape, location, new Size2(500, 500), color, 20);
        //
        // geomShape.VertexGroups.Add(vertexGroup1);
        // geomShape.VertexGroups.Add(vertexGroup2);
        // geomShape.VertexGroups.Add(vertexGroup3);

        var featheringOuterConnection = IndexGroup.ConnectStrip(geomShape, outerFeathering, circleOuter);
        var outerInnerConnection = IndexGroup.ConnectStrip(geomShape, circleOuter, circleInner);
        var innerFeatheringConnection = IndexGroup.ConnectStrip(geomShape, circleInner, innerFeathering);
        
        geomShape.IndexGroups.Add(featheringOuterConnection);
        geomShape.IndexGroups.Add(outerInnerConnection);
        geomShape.IndexGroups.Add(innerFeatheringConnection);
        
        // var indexGroup = IndexGroup.ConnectStrip(geomShape, vertexGroup1, vertexGroup2);
        // var indexGroup2 = IndexGroup.ConnectSelfFan(geomShape, vertexGroup3);
        //
        // geomShape.IndexGroups.Add(indexGroup);
        // geomShape.IndexGroups.Add(indexGroup2);
        
        return geomShape;
    }
    
    
    public static GeometryShape CreateCircle(Point2 location, Color color, int radius)
    {
        // const int quality = 33;
        // var shape = new GeometryShape();
        //
        // var mainGroup = shape.AddVertexGroup(quality);
        //
        // GenerateCircle(mainGroup.GetVertexArea(), color, location, radius, quality);


        //quality = 5, vertices = 12
        var quality = 32;
        var indicesFanCount = GetIndicesLength(quality);
        var indicesStripCount = GetIndicesLength(quality * 2) + 6;
        var verticesCount = quality * 2 + 2;
        // * 2 - feathering cycle around base vertices,
        // + 2 - strip end vertices(to make feathering logically connected
        
        var shape = new GeometryShape(verticesCount, indicesFanCount + indicesStripCount);
        
        GenerateCircle(shape.VertexArray.AsSpan(), color, location, radius, quality);
        FillTriangleFan(shape.IndexArray.AsSpan(), quality);
        GenerateCircleFeathering(shape.VertexArray.AsSpan()[(quality + 1)..], color, location, radius, quality);
        FillTriangleStripAlign(shape.IndexArray.AsSpan()[indicesFanCount..], verticesCount);
        
        return shape;

        return null;
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