using System.Text;
using NatLib.Arrays;
using NatLib.Core.Structures;
using NatLib.Logging;

namespace QuilartUI.Graphics;

public sealed class GeometryShape
{
    private static ConsoleLogger Logger { get; } = LoggerFactory.Create<GeometryShape>();
    
    internal Point2 Position { get; set; }
    
    internal Size2 Size { get; set; }
    
    internal PointerList<Vertex> VertexArray { get; set; }

    internal PointerList<int> IndexArray { get; set; }

    public List<VertexGroup> VertexGroups { get; private set; } = [];
    
    public List<IndexGroup> IndexGroups { get; private set; } = [];

    internal GeometryShape()
    {
        Logger.LogTrace("Creating new geometry shape...");
        VertexArray = new PointerList<Vertex>();
        IndexArray = new PointerList<int>();
    }

    public static GeometryShape CreateRectangle(Point2 position, Size2 size, Color backgroundColor, float radius, float outlineThickness, Color outlineColor)
    {
        var shape = new GeometryShape();
        
        Logger.LogTrace($"Started creating geometry shape (rectangle)...");

        var outlineThicknessFactor = outlineThickness / 2;

        var rectangleBase = VertexGroup.CreateRoundedRectangle(shape, position, size, backgroundColor, radius);
        var rectangleOutlineInner = VertexGroup.CreateRoundedRectangle(shape, position + outlineThicknessFactor, size - outlineThickness, outlineColor, radius - outlineThicknessFactor);
        var rectangleOutlineOuter = VertexGroup.CreateRoundedRectangle(shape, position - outlineThicknessFactor, size + outlineThickness, outlineColor, radius);
        var rectangleOutlineInnerFeathering = VertexGroup.CreateRoundedRectangle(shape, position + outlineThicknessFactor + 0.8f, size - outlineThickness - 1.6f, outlineColor.WithAlpha(0), radius - outlineThicknessFactor);
        var rectangleOutlineOuterFeathering = VertexGroup.CreateRoundedRectangle(shape, position - outlineThicknessFactor - 0.8f, size + outlineThickness + 1.6f, outlineColor.WithAlpha(0), radius);

        IndexGroup.ConnectFan(shape, rectangleBase);
        IndexGroup.ConnectStrip(shape, rectangleOutlineInner, rectangleOutlineOuter);
        IndexGroup.ConnectStrip(shape, rectangleOutlineInner, rectangleOutlineInnerFeathering);
        IndexGroup.ConnectStrip(shape, rectangleOutlineOuter, rectangleOutlineOuterFeathering);
        
        Logger.LogTrace($"Geometry shape (rectangle) created.");
        Logger.LogTrace($"- Total vertex count: {shape.VertexArray.Length} in {shape.VertexGroups.Count} groups.");
        Logger.LogTrace($"- Total index count: {shape.IndexArray.Length} in {shape.IndexGroups.Count} groups.");
        
        return shape;
    }

    public static GeometryShape CreateMultipleRectangles()
    {
        var shape = new GeometryShape();
        
        Logger.LogTrace($"Started creating geometry shape (multiple rectangles)...");

        var size = new Size2(300, 50);
        var backgroundColor = Color.FromHex("1c323c");
        var radius = 20;
        var outlineThickness = 5;
        var outlineColor = Color.FromHex("f8af40");
            
        var outlineThicknessFactor = outlineThickness / 2;
        
        for (int i = 0; i < 20; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                var position = new Point2(10 + j * 310, 10 + i * 60); 
                var rectangleBase = VertexGroup.CreateRoundedRectangle(shape, position, size, backgroundColor, radius);
                var rectangleOutlineInner = VertexGroup.CreateRoundedRectangle(shape, position + outlineThicknessFactor, size - outlineThickness, outlineColor, radius - outlineThicknessFactor);
                var rectangleOutlineOuter = VertexGroup.CreateRoundedRectangle(shape, position - outlineThicknessFactor, size + outlineThickness, outlineColor, radius);
                var rectangleOutlineInnerFeathering = VertexGroup.CreateRoundedRectangle(shape, position + outlineThicknessFactor + 0.8f, size - outlineThickness - 1.6f, outlineColor.WithAlpha(0), radius - outlineThicknessFactor);
                var rectangleOutlineOuterFeathering = VertexGroup.CreateRoundedRectangle(shape, position - outlineThicknessFactor - 0.8f, size + outlineThickness + 1.6f, outlineColor.WithAlpha(0), radius);

                IndexGroup.ConnectFan(shape, rectangleBase);
                IndexGroup.ConnectStrip(shape, rectangleOutlineInner, rectangleOutlineOuter);
                IndexGroup.ConnectStrip(shape, rectangleOutlineInner, rectangleOutlineInnerFeathering);
                IndexGroup.ConnectStrip(shape, rectangleOutlineOuter, rectangleOutlineOuterFeathering);
            }
        }
        
        Logger.LogTrace($"Geometry shape (multiple rectangles) created.");
        Logger.LogTrace($"- Total vertex count: {shape.VertexArray.Length} in {shape.VertexGroups.Count} groups.");
        Logger.LogTrace($"- Total index count: {shape.IndexArray.Length} in {shape.IndexGroups.Count} groups.");
        
        return shape;
    }

    ~GeometryShape()
    {
        VertexArray.Dispose();
        IndexArray.Dispose();
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