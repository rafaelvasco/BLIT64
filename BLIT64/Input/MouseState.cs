
namespace BLIT64
{
    public struct MouseState
    {
        private MouseButton _button_state;

        public bool this[MouseButton button]
        {
            get => (_button_state & button) == button;
            set
            {
                if (value)
                {
                    _button_state |= button;
                }
                else
                {
                    _button_state &= ~button;
                }
            }
        }
    }
}
