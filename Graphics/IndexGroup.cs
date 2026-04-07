namespace QuilartUI.Graphics;

public class IndexGroup
{
    public unsafe int* StartIndex { get; set; } = null;

    public int Length { get; set; } = 0;

    public GeometryShape Owner { get; set; }

    public static int GetIndexCount(int vertCount) => 3 * (vertCount - 2);

    public IndexGroup(GeometryShape owner)
    {
        Owner = owner;
    }

    public void ConnectSelfFan(int vertIndex, int vertLength)
    {
        var indexCount = GetIndexCount(vertLength);
        Length = indexCount;

        unsafe
        {
            var startIndex = Owner.AllocateIndex(indexCount);
            StartIndex = startIndex;

            for (var i = 0; i < vertLength - 2; i++)
            {
                startIndex[i * 3] = vertIndex;
                startIndex[i * 3 + 1] = vertIndex + i + 1;
                startIndex[i * 3 + 2] = vertIndex + i + 2;
            }
        }
    }

    public void ConnectSelfStrip(int vertIndex, int vertLength)
    {
        var indexCount = GetIndexCount(vertLength);
        Length = indexCount;

        unsafe
        {
            var startIndex = Owner.AllocateIndex(indexCount);
            StartIndex = startIndex;
            
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
        var length = int.Min(firstLength, secondLength);
        var lastIndex = 0;
        
        unsafe
        {
            var pointer = StartIndex;
            for (var i = 0; i < length - 1; i++)
            {
                pointer[lastIndex++] = firstIndex + i;
                pointer[lastIndex++] = firstIndex + i + 1;
                pointer[lastIndex++] = secondIndex + i + 1;

                pointer[lastIndex++] = secondIndex + i;
                pointer[lastIndex++] = firstIndex + i;
                pointer[lastIndex++] = secondIndex + i + 1;
            }
        }
    }
}