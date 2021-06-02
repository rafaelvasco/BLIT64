using System;
using System.Runtime.InteropServices;
using static SDL2.SDL;

namespace BLIT64
{
    public enum RunningPlatform
    {
        Windows,
        Osx,
        Linux,
        Unknown
    }

    internal static partial class Platform
    {
        public static event Action OnQuit;
        public static event Action LostFocus;
        public static event Action GainedFocus;

        private static Key _last_key_down = Key.None;

        private static RunningPlatform? _running_platform;

        public static RunningPlatform RunningPlatform
        {
            get
            {
                if (_running_platform != null)
                {
                    return _running_platform.Value;
                }

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    _running_platform = RunningPlatform.Windows;
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    _running_platform = RunningPlatform.Osx;
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    _running_platform = RunningPlatform.Linux;
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
                {
                    _running_platform = RunningPlatform.Linux;
                }
                else
                {
                    _running_platform = RunningPlatform.Unknown;
                }

                return _running_platform.Value;
            }
        }

        public static void Init(
            string title, 
            int display_width, 
            int display_height, 
            int pixel_size = 1,
            bool fullscreen = false)
        {
            Ensure64BitArchitecture();
            
            SDL_SetHint(SDL_HINT_WINDOWS_DISABLE_THREAD_NAMING, "1");

            if (SDL_Init(SDL_INIT_VIDEO | SDL_INIT_JOYSTICK | SDL_INIT_GAMECONTROLLER | SDL_INIT_HAPTIC) < 0)
            {
                SDL_Quit();
                throw new ApplicationException("Failed to initialize SDL2");
            }

            SDL_DisableScreenSaver();

            CreateWindow(title, display_width, display_height, fullscreen);
            InitGraphics(display_width / pixel_size, display_height / pixel_size);
            InitKeyboard();
        }

        public static void Shutdown()
        {
            DestroyWindow();
            ShutdownGraphics();
            SDL_Quit();
        }

        public static void Delay(uint ms)
        {
            SDL_Delay(ms);
        }

        public static ulong GetPerformanceCounter()
        {
            return SDL_GetPerformanceCounter();
        }

        public static ulong GetPerformanceFrequency()
        {
            return SDL_GetPerformanceFrequency();
        }

        public static void ProcessEvents()
        {
            while (SDL_PollEvent(out var ev) == 1)
            {
                switch (ev.type)
                {
                    case SDL_EventType.SDL_QUIT:
                        OnQuit?.Invoke();
                        break;

                    case SDL_EventType.SDL_KEYDOWN:
                    case SDL_EventType.SDL_KEYUP:
                        ProcessKeyEvent(ev);
                        break;

                    case SDL_EventType.SDL_MOUSEBUTTONDOWN:
                    case SDL_EventType.SDL_MOUSEBUTTONUP:
                    case SDL_EventType.SDL_MOUSEMOTION:
                        ProcessMouseEvent(ev);
                        break;

                    case SDL_EventType.SDL_WINDOWEVENT:
                        ProcessWindowEvent(ev);
                        break;

                    case SDL_EventType.SDL_CONTROLLERDEVICEADDED:
                        break;

                    case SDL_EventType.SDL_CONTROLLERDEVICEREMOVED:
                        break;
                }
            }
        }

        private static void Ensure64BitArchitecture()
        {
            var runtime_architecture = RuntimeInformation.OSArchitecture;
            if (runtime_architecture == Architecture.Arm || runtime_architecture == Architecture.X86)
            {
                 throw new NotSupportedException("32-bit architecture is not supported.");
            }
        }
    }
}
