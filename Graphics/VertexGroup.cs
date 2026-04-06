using NatLib.Arrays;
using NatLib.Core.Structures;
using SDL;

namespace QuilartUI.Graphics;

public class VertexGroup
{
    public unsafe Vertex* ZeroVertex { get; private set; }
    public unsafe int* ZeroIndex { get; private set; }

    public int VerticesCount { get; private set; }
    public int IndicesCount { get; private set; }

    public GeometryShape Owner { get; private set; }

    public unsafe VertexGroup(GeometryShape owner, int verticesCount, Vertex* zeroVertexId, int* zereIndexId)
    {
        VerticesCount = verticesCount;
        IndicesCount = 3 * (verticesCount - 2);

        ZeroVertex = zeroVertexId;
        ZeroIndex = zereIndexId;
    }

    public unsafe Span<Vertex> GetVertexArea() => new(ZeroVertex, VerticesCount);

    public unsafe Span<int> GetIndexArea() => new(ZeroIndex, IndicesCount);

    public static void ConnectSelfFan(VertexGroup group)
    {
    }
}