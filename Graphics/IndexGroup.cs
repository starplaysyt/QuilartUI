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

    public static IndexGroup ConnectFan(GeometryShape owner, VertexGroup vertGroup)
    {
        Logger.LogTrace(
            $"Connecting self-fan for vertex group: {vertGroup.StartIndex} - {vertGroup.StartIndex + vertGroup.Length}");
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
        owner.IndexGroups.Add(indexGroup);

        return indexGroup;
    }

    public static IndexGroup ConnectStrip(GeometryShape owner, VertexGroup group1, VertexGroup group2)
    {
        var (vert1Index, vert1Length) = (group1.StartIndex, group1.Length);
        var (vert2Index, vert2Length) = (group2.StartIndex, group2.Length);
        
        var indexCount = (vert1Length + vert2Length - 2) * 3;
        Span<int> indices = stackalloc int[indexCount];
        var lastIndex = 0;
        var triangleCount = vert1Length + vert2Length - 2;

        var step1 = 1f / (vert1Length - 1);
        var step2 = 1f / (vert2Length - 1);
        
        float norm1 = 0f, norm2 = 0f;
        int currI1 = 0, currI2 = 0;
        
        Logger.LogTrace($"Connecting strip for vertex groups: {vert1Index} - {vert1Index + vert1Length} " +
                        $"with {vert2Index} - {vert2Index + vert2Length}");
        
        for (var step = 0; step < triangleCount; step++)
        {
            if (norm1 <= norm2 && currI1 < vert1Length - 1)
            {
                indices[lastIndex++] = vert1Index + currI1;
                indices[lastIndex++] = vert1Index + currI1 + 1;
                indices[lastIndex++] = vert2Index + currI2;
        
                currI1++;
                norm1 += step1;
            }
            else
            {
                indices[lastIndex++] = vert1Index + currI1;
                indices[lastIndex++] = vert2Index + currI2 + 1;
                indices[lastIndex++] = vert2Index + currI2;
        
                currI2++;
                norm2 += step2;
            }
        }
        
        var indexGroup = new IndexGroup(owner)
        {
            Length = indexCount,
            StartIndex = owner.IndexArray.Length
        };
        
        owner.IndexArray.AddSeveral(indices);
        owner.IndexGroups.Add(indexGroup);

        return indexGroup;
    }
}