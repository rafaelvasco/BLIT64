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
            int render_surface_width, 
            int render_surface_height, 
            bool fullscreen)
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
            InitGraphics(render_surface_width, render_surface_height);
            InitKeyboard();
        }

        public static void Shutdown()
        {
            DestroyWindow();
            ShutdownGraphics();
            SDL_Quit();
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
                        var key_code = (int) ev.key.keysym.sym;
                        AddKey(key_code);
                        if (OnKeyDown != null)
                        {
                            var key = TranslatePlatformKey(key_code);
                            if (_last_key_down != key && key != Key.None)
                            {
                                _last_key_down = key;
                                OnKeyDown(key);
                            }
                        }
                        
                        break;

                    case SDL_EventType.SDL_KEYUP:
                        var key_code_up = (int) ev.key.keysym.sym;
                        RemoveKey(key_code_up);
                        _last_key_down = Key.None;
                        if (OnKeyUp != null)
                        {
                            var key = TranslatePlatformKey(key_code_up);
                            if (key == Key.None) return;
                            OnKeyUp(key);
                        }
                        break;

                    case SDL_EventType.SDL_MOUSEBUTTONDOWN:
                        SetMouseButtonState(ev.button.button, true);
                        if (OnMouseDown != null)
                        {
                            var button = TranslatePlatformMouseButton(ev.button.button);
                            OnMouseDown.Invoke(button);
                        }
                        break;

                    case SDL_EventType.SDL_MOUSEBUTTONUP:
                        SetMouseButtonState(ev.button.button, false);
                        if (OnMouseUp != null)
                        {
                            var button = TranslatePlatformMouseButton(ev.button.button);
                            OnMouseUp.Invoke(button);
                        }
                        break;

                    case SDL_EventType.SDL_MOUSEMOTION:
                        OnMouseMove?.Invoke();
                        break;

                    case SDL_EventType.SDL_WINDOWEVENT:

                        switch (ev.window.windowEvent)
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
