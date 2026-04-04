using QuilartUI.Interfaces;
using static SDL.SDL3;
using QuilartUI.Controllers;
using QuilartUI.Elements;
using QuilartUI.Exceptions;
using SDL;


namespace QuilartUI.Services;

public sealed class WindowHandlerService : QuilartService
{
    internal bool IsRunning = true;
    private Dictionary<nint, UIWindow> WindowEvents { get; } = [];
    private List<UIWindow> Windows { get; } = [];
    private FrameLimiterService FrameLimiter { get; set; }
    
    public override void Initialize()
    {
        Logger.LogTrace("Initializing Window Handler Service...");

        Logger.LogTrace("Initializing modules...");
        
        Logger.LogTrace("Initializing SDL...");
        if (!SDL_Init(SDL_InitFlags.SDL_INIT_VIDEO))
            Logger.LogFatalAndThrow("Failed to initialize SDL", new SDLInitializationException("SDL"));

        Logger.LogTrace("Initializing TTF...");
        if (!SDL3_ttf.TTF_Init())
            Logger.LogFatalAndThrow("Failed to initialize TTF", new SDLInitializationException("SDL_ttf"));

        Logger.LogTrace("Initializing MIX...");
        if (!SDL3_mixer.MIX_Init())
            Logger.LogFatalAndThrow("Failed to initialize MIX", new SDLInitializationException("SDL_mixer"));
        
        Logger.LogTrace("Initializing FrameLimiter...");
        
        FrameLimiter = ServiceController.Get<FrameLimiterService>();

        Logger.LogTrace("Window Handler initialized successfully.");
    }

    internal void RequestQuit()
    {
        Logger.LogTrace("Requesting quit...");
        IsRunning = false;
    }

    internal void RegisterNewWindow(UIWindow window)
    {
        Logger.LogTrace($"Registering window {window.Id}...");

        Windows.Add(window);
        WindowEvents.Add(window.WindowPtr, window);
        Logger.LogTrace($"Registered window with pointer {window.WindowPtr}");
    }

    internal void UnregisterWindow(UIWindow window)
    {
        Logger.LogTrace($"Unregistering window {window.Id}...");
        
        Windows.Remove(window);
        WindowEvents.Remove(window.WindowPtr);
        
        Logger.LogTrace($"Unregistered window with pointer {window.WindowPtr}");
    }

    internal void Run()
    {
        Logger.LogTrace("Window cycle started...");
        SDL_Event e;
        
        unsafe
        {
            while (Windows.Count > 0)
            {
                if (!IsRunning) break;
                
                while (SDL_PollEvent(&e))
                {
                    var window = (nint)SDL_GetWindowFromEvent(&e);
                    if (window == IntPtr.Zero) continue;
                    
                    WindowEvents[window].UpdateEvents(&e);
                }

                foreach (var t in Windows)
                {
                    t.RenderWindow();
                }
                
                FrameLimiter.Wait();
            }
        }
        
        Logger.LogTrace("Window cycle stopped.");
        ServiceController.Exit();
        Logger.LogTrace("Execution complete. Goodbye.");
    }

    public override void Exit()
    {
        Logger.LogTrace("Service exit called...");
        
        Logger.LogTrace("Quitting all windows...");

        // INFO: Window can unsubscribe itself from service, and cause collection modification during ForEach
        // Solution - copy the list of links to windows to prevent collection modification.
        var list = new List<UIWindow>(Windows);
        list.ForEach(wind => wind.QuitWindow());
        
        Logger.LogTrace("Quitting SDL modules...");
        
        SDL_Quit();
        SDL3_ttf.TTF_Quit();
        SDL3_mixer.MIX_Quit();
        
        Logger.LogTrace("Service exit complete.");
    }
}