
namespace BLIT64.Toolkit.Gui
{
    public class WindowCloseButton : Button
    {
        public WindowCloseButton(string id, string label, int width = DefaultWidth, int height = DefaultHeight) : base(id, label, width, height)
        {
        }

        public override void OnMouseDown(MouseButton button)
        {
            _label.OffsetY = 1;
        }

        public override void OnMouseUp(MouseButton button)
        {
            _label.OffsetY = 0;
        }

        public override void Draw(Blitter blitter, Theme theme)
        {
            theme.DrawWindowCloseButton(blitter, this);

            DrawChildren(blitter, theme);
        }
    }

    public class Window : Container
    {
        public string Title
        {
            get => _label.Text;
            set => _label.Text = value;
        }


        protected readonly WindowCloseButton _close_button;
        protected Label _label;

        public Window(string id, int width, int height, string title = "Window") : base(id, width, height)
        {
            _close_button = new WindowCloseButton(id + "_close_button", "X", 30, 30)
            {
                CanHaveInputFocus = false
            };

            _label = new Label(id + "_label", title)
            {
                Width = this.Width, 
                Height = 30,
                IgnoreInput = true
            };

            Draggable = true;


            Add(_close_button);
            Add(_label);

            _close_button.X = Width - _close_button.Width;

            _close_button.OnClick += OnCloseClick;

        }

        public override void OnKeyDown(Key key)
        {
            if (key == Key.Escape)
            {
                Ui.SetVisible(this, false);
            }
        }

        private void OnCloseClick()
        {
            Ui.SetVisible(this, false);
        }

        public override void Draw(Blitter blitter, Theme theme)
        {
            theme.DrawWindow(blitter, this);

            base.Draw(blitter, theme);
        }
    }
}
