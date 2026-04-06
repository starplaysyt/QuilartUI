using SDL;
using QuilartUI.Elements;
using NatLib.Core.Structures;
using static SDL.SDL3;

namespace QuilartUI.Graphics;

public sealed class GraphicsRenderer
{
    private unsafe SDL_Renderer* Renderer { get; }

    internal unsafe nint RendererPtr => (nint)Renderer;


    public Color RendererColor
    {
        get;
        set
        {
            unsafe
            {
                field = value;
                SDL_SetRenderDrawColorFloat(Renderer, value.R, value.G, value.B, value.A);
            }
        }
    }

    internal GraphicsRenderer(UIWindow window)
    {
        unsafe
        {
            Renderer = SDL_CreateRenderer((SDL_Window*)window.WindowPtr, (byte*)null);

            SDL_SetRenderDrawBlendMode(Renderer, SDL_BlendMode.SDL_BLENDMODE_BLEND);

            SDL_SetRenderVSync(Renderer, 30);
        }
    }

    public unsafe void RenderClear() => SDL_RenderClear(Renderer);

    public unsafe void RenderPresent() => SDL_RenderPresent(Renderer);

    public void DrawGeometryShape(GeometryShape shape)
    {
        var vertexArray = shape.VertexArray;
        var indexArray = shape.IndexArray;
        unsafe
        {
            SDL_RenderGeometry(Renderer, null,
                (SDL_Vertex*)vertexArray.AsPointer(), vertexArray.Length,
                indexArray.AsPointer(), indexArray.Length);
        }
    }
}