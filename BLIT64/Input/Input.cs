

namespace BLIT64
{
    public static class Input
    {
        private static KeyState _prev_key_state;
        private static KeyState _cur_key_state;
        private static MouseState _prev_mouse_state;
        private static MouseState _cur_mouse_state;


        internal static void Init()
        {

        }

        public static void AddMouseDownListener(MouseButtonCallback action)
        {
            Platform.OnMouseDown += action;
        }

        public static void AddMouseUpListener(MouseButtonCallback action)
        {
            Platform.OnMouseUp += action;
        }

        public static void AddMouseMoveListener(MouseMoveCallback action)
        {
            Platform.OnMouseMove += action;
        }

        public static void AddKeyDownListener(KeyCallBack action)
        {
            Platform.OnKeyDown += action;
        }

        public static void AddKeyUpListener(KeyCallBack action)
        {
            Platform.OnKeyUp += action;
        }

        public static (int X, int Y) MousePos
        {
            get
            {
                var (mouse_x, mouse_y) = Platform.GetMousePos();
                return ((int X, int Y)) (
                    mouse_x / Platform.DisplayScaleFactorX,
                    mouse_y / Platform.DisplayScaleFactorY
                );
            }
        }

        public static bool KeyDown(Key key)
        {
            return _cur_key_state[key];
        }

        public static bool KeyPressed(Key key)
        {
            return _cur_key_state[key] && !_prev_key_state[key];
        }

        public static bool KeyReleased(Key key)
        {
            return !_cur_key_state[key] && _prev_key_state[key];
        }

        public static bool MouseDown(MouseButton button)
        {
            return _cur_mouse_state[button];
        }

        public static bool MousePressed(MouseButton button)
        {
            return _cur_mouse_state[button] && !_prev_mouse_state[button];
        }

        public static bool MouseReleased(MouseButton button)
        {
            return !_cur_mouse_state[button] && _prev_mouse_state[button];
        }

        internal static void Update()
        {
            _prev_key_state = _cur_key_state;
            _cur_key_state = Platform.GetKeyState();
            _prev_mouse_state = _cur_mouse_state;
            _cur_mouse_state = Platform.GetMouseState();
        }
    }
}
