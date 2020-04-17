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

        public static (int X, int Y) GetMousePos()
        {
            SDL_GetMouseState(out int x, out int y);

            return (x, y);
        }

        private static MouseButton TranslatePlatformMouseButton(byte button)
        {
            switch (button)
            {
                case 1: return MouseButton.Left;
                case 2: return MouseButton.Middle;
                case 3: return MouseButton.Right;
            }

            return MouseButton.None;
        }

        private static void SetMouseButtonState(byte sdl_button, bool down)
        {
            var button = TranslatePlatformMouseButton(sdl_button);
            _mouse_state[button] = down;
        }
    }
}
