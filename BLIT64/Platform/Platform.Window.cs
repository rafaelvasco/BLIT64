using System;
using static SDL2.SDL;

namespace BLIT64
{
    internal static partial class Platform
    {
        public static IntPtr WindowHandle { get; private set; }

        public static event Action<(int width, int height)> WindowResized;

        public static void ProcessWindowEvent(SDL_Event ev)
        {
            switch(ev.window.windowEvent)
            {
                case SDL_WindowEventID.SDL_WINDOWEVENT_SIZE_CHANGED:

                    var new_w = ev.window.data1;
                    var new_h = ev.window.data2;
                    WindowResized?.Invoke((new_w, new_h));
                    UpdateDisplayScaleFactor();
                    break;

                case SDL_WindowEventID.SDL_WINDOWEVENT_CLOSE:
                    OnQuit?.Invoke();
                    break;

                case SDL_WindowEventID.SDL_WINDOWEVENT_FOCUS_LOST:
                    LostFocus?.Invoke();
                    break;

                case SDL_WindowEventID.SDL_WINDOWEVENT_FOCUS_GAINED:
                    GainedFocus?.Invoke();
                    break;
            }
        }

        public static void CreateWindow(string title, int width, int height, bool fullscreen)
        {
            var windowFlags = SDL_WindowFlags.SDL_WINDOW_HIDDEN |
                              SDL_WindowFlags.SDL_WINDOW_INPUT_FOCUS |
                              SDL_WindowFlags.SDL_WINDOW_MOUSE_FOCUS |
                              SDL_WindowFlags.SDL_WINDOW_ALLOW_HIGHDPI;

            if (fullscreen)
            {
                windowFlags |= SDL_WindowFlags.SDL_WINDOW_FULLSCREEN_DESKTOP;
            }

            WindowHandle = SDL_CreateWindow(
                title, 
                SDL_WINDOWPOS_CENTERED, 
                SDL_WINDOWPOS_CENTERED, 
                width, 
                height,
                windowFlags);

            if (WindowHandle == IntPtr.Zero)
            {
                throw new ApplicationException("Error while creating SDL2 Window: " + SDL_GetError());
            }
        }

        public static void ShowWindow(bool show)
        {
            if (show)
            {
                SDL_ShowWindow(WindowHandle);
            }
            else
            {
                SDL_HideWindow(WindowHandle);
            }
        }

        public static bool IsFullscreen()
        {
            return GetWindowFlags().HasFlag(WindowFlags.Fullscreen);
        }

        public static void SetWindowFullscreen(bool fullscreen)
        {
            if (IsFullscreen() != fullscreen)
            {
                _ = SDL_SetWindowFullscreen(WindowHandle, (uint)(fullscreen ? WindowFlags.FullscreenDesktop : 0));
            }
        }

        public static void SetWindowSize(int width, int height)
        {
            if (IsFullscreen())
            {
                return;
            }

            SDL_SetWindowSize(WindowHandle, width, height);
            SDL_SetWindowPosition(WindowHandle, SDL_WINDOWPOS_CENTERED, SDL_WINDOWPOS_CENTERED);
        }

        public static (int width, int height) GetWindowSize()
        {
            SDL_GetWindowSize(WindowHandle, out var w, out var h);
            return (w, h);
        }

        public static void SetWindowBorderless(bool borderless)
        {
            SDL_SetWindowBordered(WindowHandle, (SDL_bool) (borderless ? 0 : 1));
        }

        public static void SetWindowTitle(string title)
        {
            SDL_SetWindowTitle(WindowHandle, title);
        }

        public static string GetWindowTitle()
        {
            return SDL_GetWindowTitle(WindowHandle);
        }

        public static void ShowCursor(bool show)
        {
            _ = SDL_ShowCursor(show ? 1 : 0);
        }

        public static bool CursorVisible()
        {
            var state = SDL_ShowCursor(SDL_QUERY);
            return state == SDL_ENABLE;
        }

        private static void DestroyWindow()
        {
            if (WindowHandle != IntPtr.Zero)
            {
                SDL_DestroyWindow(WindowHandle);
            }
        }


        public static WindowFlags GetWindowFlags()
        {
            return (WindowFlags)SDL_GetWindowFlags(WindowHandle);
        }

        [Flags]
        public enum WindowFlags
        {
            Fullscreen = 0x00000001,
            Shown = 0x00000004,
            Hidden = 0x00000008,
            Borderless = 0x00000010,
            Resizable = 0x00000020,
            Minimized = 0x00000040,
            Maximized = 0x00000080,
            InputFocus = 0x00000200,
            MouseFocus = 0x00000400,
            FullscreenDesktop = 0x00001001,
            AllowHighDPI = 0x00002000,
            MouseCapture = 0x00004000
        }
    }
}
