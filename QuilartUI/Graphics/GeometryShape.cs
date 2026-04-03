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
}