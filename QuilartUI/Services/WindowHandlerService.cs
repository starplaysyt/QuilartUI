using QuilartUI.Interfaces;
using static SDL.SDL3;
using NatLib.Logging;
using QuilartUI.Controllers;
using QuilartUI.Elements;
using QuilartUI.Exceptions;
using SDL;


namespace QuilartUI.Services;

public class WindowHandlerService : IQuilartService
{
    internal bool IsRunning = true;
    private Dictionary<nint, UIWindow> WindowEvents { get; set; } = [];
    private List<UIWindow> Windows { get; set; } = [];
    public static ConsoleLogger Logger { get; } = new("WindowsService");
    
    public void Initialize()
    {
        Logger.LogTrace("Initializing Window Handler Service...");

        Logger.LogTrace("Initializing modules...");
        
        Logger.LogTrace("Initializing SDL...");
        if (!SDL_Init(SDL_InitFlags.SDL_INIT_VIDEO))
            Logger.LogErrorAndThrow("Failed to initialize SDL", new InitializationException());

        Logger.LogTrace("Initializing TTF...");
        if (!SDL3_ttf.TTF_Init())
            Logger.LogErrorAndThrow("Failed to initialize TTF", new InitializationException());

        Logger.LogTrace("Initializing MIX...");
        if (!SDL3_mixer.MIX_Init())
            Logger.LogErrorAndThrow("Failed to initialize MIX", new InitializationException());

        Logger.LogTrace("Window Handler initialized successfully.");
    }

    internal void RegisterNewWindow(UIWindow window)
    {
        Logger.LogTrace("Registering new window...");

        Windows.Add(window);
        WindowEvents.Add(window.WindowPtr, window);
        Logger.LogTrace($"Registered new window with pointer {window.WindowPtr}");
    }

    internal void UnregisterWindow(UIWindow window)
    {
        Logger.LogTrace("Unregistering window...");
        
        Windows.Remove(window);
        WindowEvents.Remove(window.WindowPtr);
        Logger.LogTrace($"Unregistered new window with pointer {window.WindowPtr}");
    }

    internal void QuitAllWindows()
    {
        Logger.LogTrace("Quitting all windows...");
        Windows.ForEach(wind => wind.QuitWindow());
    }

    internal void Run()
    {
        Logger.LogTrace("Window cycle started...");
        SDL_Event e;
        
        unsafe
        {
            while (Windows.Count > 0)
            {
                
                if (!IsRunning) 
                { 
                    QuitAllWindows();
                    break;
                }
                
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
            }
        }
        
        Logger.LogTrace("Window cycle stopped.");
        ExecuteCleanup();
    }

    public void ExecuteCleanup()
    {
        Logger.LogTrace("Cleaning up WindowHandlerService...");
        
        SDL_Quit();
        SDL3_ttf.TTF_Quit();
        SDL3_mixer.MIX_Quit();
    }

    public void Exit()
    {
        Logger.LogTrace("Service exit called...");
        
        QuitAllWindows();
        ExecuteCleanup();
        
        Logger.LogTrace("Service exit complete.");
    }
}