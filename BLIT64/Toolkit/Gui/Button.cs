
namespace BLIT64.Toolkit.Gui
{
    public class Button : Container
    {
        public const int DefaultWidth = 80;
        public const int DefaultHeight = 30;

        public string Label
        {
            get => _label.Text;
            set => _label.Text = value;
        }

        protected readonly Label _label;

        public Button(string id, string label, int width = DefaultWidth, int height = DefaultHeight) : base(id, width, height)
        {
            CanHaveInputFocus = true;

            _label = new Label(id + "_label", label)
            {
                Width = this.Width, 
                Height = this.Height,
                IgnoreInput = true
            };

            Add(_label);
        }

        public override void OnMouseDown(MouseButton button)
        {
            OffsetY = 1;
            _label.OffsetY = OffsetY;
        }

        public override void OnMouseUp(MouseButton button)
        {
            OffsetY = 0;
            _label.OffsetY = OffsetY;
        }

        public override void Draw(Blitter blitter, Theme theme)
        {
            theme.DrawButton(blitter, this);

            DrawChildren(blitter, theme);
        }
    }
}
