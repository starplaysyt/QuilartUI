namespace QuilartUI.Exceptions;

public class SDLException() : Exception(SDL.SDL3.SDL_GetError());