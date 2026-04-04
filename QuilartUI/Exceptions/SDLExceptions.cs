using NatLib.Core.Utils;

namespace QuilartUI.Exceptions;

public class SDLOperationException()
    : Exception($"SDL function execution failed. Got SDL error: {SDL.SDL3.SDL_GetError().IfNullOrEmpty("No exception")}");

public class SDLInitializationException(string name) 
    : Exception($"Failed to initialize {name} from SDL. Got SDL error: {SDL.SDL3.SDL_GetError().IfNullOrEmpty("No exception")}");