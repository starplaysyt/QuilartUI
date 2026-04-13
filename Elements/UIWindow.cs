using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using NatLib.Logging;
using NatLib.Core.Structures;
using QuilartUI.Controllers;
using QuilartUI.Extensions;
using QuilartUI.Graphics;
using QuilartUI.Services;
using SDL;
using static SDL.SDL3;

namespace QuilartUI.Elements;

public sealed class UIWindow
{
    private ConsoleLogger Logger { get; }
    private unsafe SDL_Window* SDLWindow { get; }
    public uint Id { get; }

    public bool IsAlive { get; private set; } = true;

    internal unsafe nint WindowPtr => (nint)SDLWindow;

    private WindowHandlerService Owner { get; }

    public GraphicsRenderer Renderer { get; }

    //TODO: Debug only element
    public List<GeometryShape> Shapes { get; } = new();

    public FrameCounterService FrameCounter { get; }

    public UIWindow()
    {
        Logger = LoggerFactory.Create(this);

        Logger.LogTrace("Creating UIWindow...");

        Logger.LogTrace("Getting WindowHandlerService...");
        Owner = ServiceController.Get<WindowHandlerService>();
        FrameCounter = ServiceController.Get<FrameCounterService>();

        Logger.LogTrace("Creating SDLWindow...");
        unsafe
        {
            SDLWindow = Logger.LogSDLIfNullPtr(SDL_CreateWindow("Quilart Window", 1200, 700,
                SDL_WindowFlags.SDL_WINDOW_RESIZABLE));
            Id = (uint)SDL_GetWindowID(SDLWindow);
        }

        Logger.LogTrace("Registering window...");
        Owner.RegisterNewWindow(this);

        Logger.LogTrace("Creating renderer...");
        Renderer = new GraphicsRenderer(this)
        {
            //TODO: Debug only element
            // DrawingShape = GeometryShape.CreateRoundedRectangle(
            //     new Point2(100, 100),
            //     new Size2(200, 400),
            //     Color.White,
            //     50);
            RendererColor = Color.FromHex("2c2c2c")
        };


        var color1 = Color.FromHex("f8af40");
        var color2 = Color.FromHex("1c323c");

        Console.WriteLine("BEFORE ACQUIRING BUFFERS");
        Console.ReadKey();

        for (int i = 0; i < 20; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                var shape = GeometryShape.CreateRectangle(new Point2(10 + j * 310, 10 + i * 60), new Size2(300, 50),
                    color2, 20,
                    5, color1);
                Shapes.Add(shape);
                Console.WriteLine(shape.IndexArray.Length);
            }
        }

        //Shapes.Add(GeometryShape.CreateMultipleRectangles());

        Console.WriteLine("AFTER ACQUIRING BUFFERS");
        Console.ReadKey();

        //GeometryShape.CreateRectangleBackgroundShaded(new Point2(200, 200), new Size2(600, 500), color2, color1, 100, 20); 

        //GeometryShape.CreateRectangleOutline(new Point2(200, 200), new Size2(600, 500), color1, 20, 20); 

        //GeometryShape.CreateRectangle(new Point2(200, 200), new Size2(200, 100), color2, 20);

        //GeometryShape.CreateCircleBackground(new Point2(300, 300), color1.WithAlpha(0.6f), color2.WithAlpha(0.6f), 200, 10);

        // var circleColor = Color.Magenta;
        // DrawingShape2 = GeometryShape.CreateCircle(
        //     new Point2(300, 300),
        //     circleColor.WithAlpha(0.5f),
        //     100);

        Logger.LogTrace("UIWindow created successfully.");
    }

    internal unsafe void UpdateEvents(SDL_Event* e)
    {
        switch (e->Type)
        {
            case SDL_EventType.SDL_EVENT_QUIT:
                QuitWindow();
                break;

            case SDL_EventType.SDL_EVENT_KEY_DOWN:
                if (e->key.key == SDL_Keycode.SDLK_ESCAPE)
                    QuitWindow();

                if (e->key.key == SDL_Keycode.SDLK_RETURN)
                {
                    var window = new UIWindow();
                }

                if (e->key.key == SDL_Keycode.SDLK_Q)
                    Owner.RequestQuit();
                break;
        }
    }

    internal unsafe void RenderWindow()
    {
        // Console.WriteLine("BEFORE RENDERING");
        // Console.ReadKey();
        
        bool isUpdated = FrameCounter.Tick();

        Renderer.RenderClear();
        // Renderer.DrawGeometryShape(Shapes[0]);

        foreach (var shape in Shapes)
        {
            Renderer.DrawGeometryShape(shape);
        }
        // Renderer.DrawGeometryShape(DrawingShape2);
        
        // Console.WriteLine("AFTER DRAW GEOMETRY SHAPE");
        // Console.ReadKey();

        unsafe
        {
            SDL_RenderDebugText((SDL_Renderer*)Renderer.RendererPtr, 10, 10, FrameCounter.CurrentFps.ToString());
        }

        Renderer.RenderPresent();
        
        // Console.WriteLine("AFTER RENDERING");
        // Console.ReadKey();
    }

    public void QuitWindow()
    {
        unsafe
        {
            Logger.LogTrace("Quitting Window...");

            Owner.UnregisterWindow(this);

            // TODO: SDL_DestroyRenderer, or call it as renderer wrapper function 
            SDL_DestroyWindow(SDLWindow);

            IsAlive = false;

            Logger.LogTrace("Window quit successfully.");
        }
    }
}