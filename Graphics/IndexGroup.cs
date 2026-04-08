using NatLib.Logging;

namespace QuilartUI.Graphics;

public class IndexGroup
{
    public int StartIndex { get; set; } = 0;

    public int Length { get; set; } = 0;

    public GeometryShape Owner { get; set; }

    public static int GetIndexCount(int vertCount) => 3 * (vertCount - 2);

    public IndexGroup(GeometryShape owner)
    {
        Owner = owner;
    }

    public static IndexGroup ConnectSelfFan(GeometryShape owner, VertexGroup vertGroup1)
    {
        var logger = LoggerFactory.Create("ConnectSelfFan");
        logger.LogTrace($"Connecting self-fan for vertex group: {vertGroup1.StartIndex}, {vertGroup1.Length}");
        var indexGroup = new IndexGroup(owner);
        var indexCount = GetIndexCount(vertGroup1.Length);
        
        indexGroup.Length = indexCount;
        
        Span<int> indices = stackalloc int[indexCount];

        unsafe
        {
            for (var i = 0; i < vertGroup1.Length - 2; i++)
            {
                indices[i * 3] = vertGroup1.StartIndex;
                indices[i * 3 + 1] = vertGroup1.StartIndex + i + 1;
                indices[i * 3 + 2] = vertGroup1.StartIndex + i + 2;
            }
        }
        
        indexGroup.StartIndex = owner.IndexArray.Length;
        owner.IndexArray.AddSeveral(indices);

        return indexGroup;
    }

    public void ConnectSelfStrip(int vertIndex, int vertLength)
    {
        var indexCount = GetIndexCount(vertLength);
        Length = indexCount;

        unsafe
        {
            Span<int> startIndex = Span<int>.Empty;// Owner.AllocateIndex(indexCount);
            // StartIndex = startIndex;
            
            for (var i = 0; i < vertLength - 2; i++)
            {
                startIndex[i * 3 + 2] = vertIndex + i + 2;

                if (i % 2 == 0)
                {
                    startIndex[i * 3] = vertIndex + i;
                    startIndex[i * 3 + 1] = vertIndex + i + 1;
                }
                else
                {
                    startIndex[i * 3] = vertLength + i + 1;
                    startIndex[i * 3 + 1] = vertLength + i;
                }
            }
        }
    }

    public void ConnectStrip(int firstIndex, int firstLength, int secondIndex, int secondLength)
    {
        // var length = int.Min(firstLength, secondLength);
        // var lastIndex = 0;
        //
        // unsafe
        // {
        //     var pointer = StartIndex;
        //     for (var i = 0; i < length - 1; i++)
        //     {
        //         pointer[lastIndex++] = firstIndex + i;
        //         pointer[lastIndex++] = firstIndex + i + 1;
        //         pointer[lastIndex++] = secondIndex + i + 1;
        //
        //         pointer[lastIndex++] = secondIndex + i;
        //         pointer[lastIndex++] = firstIndex + i;
        //         pointer[lastIndex++] = secondIndex + i + 1;
        //     }
        // }
    }
}