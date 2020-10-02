
namespace BLIT64.Toolkit.Gui
{
    public class CheckBox : Container
    {
        public const int DefaultWidth = 20;
        public const int DefaultHeight = 20;

        public bool Checked { get; private set; }

        public string Label
        {
            get => _label.Text;
            set => _label.Text = value;
        }


        protected readonly Label _label;

        public CheckBox(string id, int width = DefaultWidth, int height = DefaultHeight, string label = "Check") : base(id, width, height)
        {
            CanHaveInputFocus = true;

            _label = new Label(id + "_label", label)
            {
                BubbleEventsToParent = true
            };

            Add(_label);

            _label.Height = this.Height;

            _label.X = Width + 10;

        }


        public override void OnMouseDown(MouseButton button)
        {
            OffsetY = 1;
        }

        public override void OnMouseUp(MouseButton button)
        {
            Toggle();

            OffsetY = 0;
        }

        private void Toggle()
        {
            Checked = !Checked;
        }

        public override void OnKeyUp(Key key)
        {
            if (!HasInputFocus)
            {
                return;
            }

            if (key == Key.Space)
            {
                OffsetY = 0;
                Checked =  !Checked;
            }
        }

        public override void OnKeyDown(Key key)
        {
            if (!HasInputFocus)
            {
                return;
            }

            if (key == Key.Space)
            {
                OffsetY = 1;
            }
        }

        public override void Draw(Blitter blitter, Theme theme)
        {
            theme.DrawCheckbox(blitter, this);

            DrawChildren(blitter, theme);
        }
    }
}
