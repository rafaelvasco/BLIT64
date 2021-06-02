using static SDL2.SDL;

namespace BLIT64
{
    internal static partial class Platform
    {
        private static MouseState _mouse_state;
        
        public static event MouseButtonCallback OnMouseDown;
        
        public static event MouseButtonCallback OnMouseUp;

        public static event MouseMoveCallback OnMouseMove;

        public static event KeyCallBack OnKeyDown;

        public static event KeyCallBack OnKeyUp;

        public static ref readonly MouseState GetMouseState()
        {
            return ref _mouse_state;
        }

        public static void ProcessMouseEvent(SDL_Event ev)
        {
            switch (ev.type)
            {
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
            }
        }

        public static (int X, int Y) GetMousePos()
        {
            _ = SDL_GetMouseState(out int x, out int y);

            return (x, y);
        }

        private static MouseButton TranslatePlatformMouseButton(byte button)
        {
            return button switch
            {
                1 => MouseButton.Left,
                2 => MouseButton.Middle,
                3 => MouseButton.Right,
                _ => MouseButton.None,
            };
        }

        private static void SetMouseButtonState(byte sdl_button, bool down)
        {
            var button = TranslatePlatformMouseButton(sdl_button);
            _mouse_state[button] = down;
        }
    }
}
