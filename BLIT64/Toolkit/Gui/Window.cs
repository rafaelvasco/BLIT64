
namespace BLIT64.Toolkit.Gui
{

    public class Window : Container
    {
        public string Title
        {
            get => _label.Text;
            set => _label.Text = value;
        }


        protected readonly Button _close_button;
        protected Label _label;

        public Window(string id, int width, int height, string title = "Window") : base(id, width, height)
        {
            _close_button = new Button(id + "_close_button", "X", 30, 30)
            {
                CanHaveInputFocus = false
            };

            _label = new Label(id + "_label", title)
            {
                Width = _width, 
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

        public override void Draw(Canvas blitter, IGuiDrawer drawer)
        {
            drawer.DrawWindow(blitter, this);

            DrawChildren(blitter, drawer);
        }
    }
}
