using NatLib.Logging;

namespace QuilartUI.Graphics;

public class IndexGroup
{
    public static ConsoleLogger Logger { get; } = LoggerFactory.Create<IndexGroup>();
    
    public int StartIndex { get; set; } = 0;

    public int Length { get; set; } = 0;

    public GeometryShape Owner { get; set; }
    
    private IndexGroup(GeometryShape owner)
    {
        Owner = owner;
    }
    
    public static int GetIndexCount(int vertCount) => 3 * (vertCount - 2);

    public static IndexGroup ConnectSelfFan(GeometryShape owner, VertexGroup vertGroup)
    {
        Logger.LogTrace($"Connecting self-fan for vertex group: {vertGroup.StartIndex} - {vertGroup.StartIndex + vertGroup.Length}");
        var indexGroup = new IndexGroup(owner);
        var indexCount = GetIndexCount(vertGroup.Length);
        
        indexGroup.Length = indexCount;
        
        Span<int> indices = stackalloc int[indexCount];
        for (var i = 0; i < vertGroup.Length - 2; i++)
        {
            indices[i * 3] = vertGroup.StartIndex;
            indices[i * 3 + 1] = vertGroup.StartIndex + i + 1;
            indices[i * 3 + 2] = vertGroup.StartIndex + i + 2;
        }
        
        indexGroup.StartIndex = owner.IndexArray.Length;
        owner.IndexArray.AddSeveral(indices);

        return indexGroup;
    }

    public static IndexGroup ConnectSelfStrip(GeometryShape owner, VertexGroup vertGroup)
    {
        var vertLength = vertGroup.Length;
        var vertIndex = vertGroup.StartIndex;
        
        Logger.LogTrace($"Connecting self-strip for vertex group: {vertLength} - {vertIndex + vertLength}");
        
        var indexGroup = new IndexGroup(owner);
        var indexCount = GetIndexCount(vertGroup.Length);
        
        indexGroup.Length = indexCount;
        
        Span<int> indices = stackalloc int[indexCount];
        
        for (var i = 0; i < vertLength - 2; i++)
        {
            indices[i * 3 + 2] = vertIndex + i + 2;

            if (i % 2 == 0)
            {
                indices[i * 3] = vertIndex + i;
                indices[i * 3 + 1] = vertIndex + i + 1;
            }
            else
            {
                indices[i * 3] = vertLength + i + 1;
                indices[i * 3 + 1] = vertLength + i;
            }
        }

        indexGroup.StartIndex = owner.IndexArray.Length;
        owner.IndexArray.AddSeveral(indices);
        
        return indexGroup;
    }

    public static IndexGroup ConnectStrip(GeometryShape owner, VertexGroup group1, VertexGroup group2)
    {
        var vert1Length = group1.Length; var vert1Index = group1.StartIndex;
        var vert2Length = group2.Length; var vert2Index = group2.StartIndex;
        
        Logger.LogTrace($"Connecting strip for vertex groups: {vert1Index} - {vert1Index + vert1Length} with {vert2Index} - {vert2Length}");

        var indexGroup = new IndexGroup(owner);
        var minVertLength = Math.Min(vert1Length, vert2Length);
        var indexCount = GetIndexCount(minVertLength * 2);
        
        indexGroup.Length = indexCount;
        
        Span<int> indices = stackalloc int[indexCount];

        Logger.LogTrace($"INFO: indexCount: {indexCount} vertLength: {minVertLength}");
        var lastIndex = 0;
        for (var i = 0; i < minVertLength - 1; i++)
        {
            indices[lastIndex++] = vert1Index + i;
            indices[lastIndex++] = vert1Index + i + 1;
            indices[lastIndex++] = vert2Index + i + 1;
        
            indices[lastIndex++] = vert2Index + i;
            indices[lastIndex++] = vert1Index + i;
            indices[lastIndex++] = vert2Index + i + 1;
        }
        
        indexGroup.StartIndex = owner.IndexArray.Length;
        owner.IndexArray.AddSeveral(indices);
        
        return indexGroup;
    }

    
}