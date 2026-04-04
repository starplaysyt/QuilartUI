using NatLib.Logging;
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

    public UIWindow()
    {
        Logger = LoggerFactory.Create(this);

        Logger.LogTrace("Creating UIWindow...");

        Logger.LogTrace("Getting WindowHandlerService...");
        Owner = ServiceController.Get<WindowHandlerService>();

        Logger.LogTrace("Creating SDLWindow...");
        unsafe
        {
            SDLWindow = Logger.LogSDLIfNullPtr(SDL_CreateWindow("Quilart Window", 700, 300,
                SDL_WindowFlags.SDL_WINDOW_RESIZABLE | SDL_WindowFlags.SDL_WINDOW_HIGH_PIXEL_DENSITY));
            Id = (uint)SDL_GetWindowID(SDLWindow);
        }

        Logger.LogTrace("Registering window...");
        Owner.RegisterNewWindow(this);

        Logger.LogTrace("Creating renderer...");
        Renderer = new GraphicsRenderer(this);


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