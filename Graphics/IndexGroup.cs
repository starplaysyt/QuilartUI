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
        owner.IndexGroups.Add(indexGroup);

        return indexGroup;
    }

    public static IndexGroup ConnectStrip(GeometryShape owner, VertexGroup group1, VertexGroup group2)
    {
        var vert1Length = group1.Length;
        var vert1Index = group1.StartIndex;
        var vert2Length = group2.Length;
        var vert2Index = group2.StartIndex;

        Logger.LogTrace(
            $"Connecting strip for vertex groups: {vert1Index} - {vert1Index + vert1Length} with {vert2Index} - {vert2Length}");

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
        owner.IndexGroups.Add(indexGroup);

        return indexGroup;
    }

    public static IndexGroup ConnectStripExtra(GeometryShape owner, VertexGroup group1, VertexGroup group2)
    {
        var vert1Length = group1.Length;
        var vert1Index = group1.StartIndex;
        var vert2Length = group2.Length;
        var vert2Index = group2.StartIndex;

        var minVertLength = Math.Max(vert1Length, vert2Length);

        var incrementFactor1 = (float)vert1Length / minVertLength;
        var incrementFactor2 = (float)vert2Length / minVertLength;

        Logger.LogTrace(
            $"Connecting strip for vertex groups: {vert1Index} - {vert1Index + vert1Length} with {vert2Index} - {vert2Index + vert2Length}");
        Logger.LogTrace($"IncrementFactor1 : {incrementFactor1} IncrementFactor2 : {incrementFactor2}");

        var indexGroup = new IndexGroup(owner);

        var indexCount = GetIndexCount(minVertLength * 2);

        indexGroup.Length = indexCount;

        Span<int> indices = stackalloc int[indexCount];

        Logger.LogTrace($"INFO: indexCount: {indexCount} vertLength: {minVertLength}");
        var lastIndex = 0; //in core array

        for (float i1 = 0, i2 = 0;
             i1 < vert1Length - 1 || i2 < vert2Length - 1;
             i1 += incrementFactor1, i2 += incrementFactor2)
        {
            Logger.LogTrace($"INFO: i1: {i1} i2: {i2} lastIndex: {lastIndex}");
            var actI1 = (int)i1;
            var actI2 = (int)i2;

            indices[lastIndex++] = vert1Index + actI1;
            indices[lastIndex++] = vert1Index + actI1 + 1;
            indices[lastIndex++] = vert2Index + actI2 + 1;

            indices[lastIndex++] = vert2Index + actI2;
            indices[lastIndex++] = vert1Index + actI1;
            indices[lastIndex++] = vert2Index + actI2 + 1;
        }

        indexGroup.StartIndex = owner.IndexArray.Length;
        owner.IndexArray.AddSeveral(indices);
        owner.IndexGroups.Add(indexGroup);

        return indexGroup;
    }
}