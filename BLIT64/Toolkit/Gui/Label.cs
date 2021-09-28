
namespace BLIT64.Toolkit.Gui
{
    public class Label : Widget
    {
        public int Color { get; set; }

        public int Scale
        {
            get => _scale;
            set
            {
                _scale = value;
                if (_scale < 1)
                {
                    _scale = 1;
                }
                TextMeasure = Canvas.TextMeasure(_text, _scale);
                _width = TextMeasure.W;
                _height = TextMeasure.H;
            }
        }

        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                if (!string.IsNullOrEmpty(_text))
                {
                    TextMeasure = Canvas.TextMeasure(_text, _scale);
                }
            }
        }

        protected string _text;
        protected int _scale = 1;

        public (int W, int H) TextMeasure { get; private set; }

        public Label(string id, string text, int scale = 1) : base(id, 0, 0)
        {
            Text = text;
            Scale = scale;
        }

        public override void Draw(Canvas canvas, IGuiDrawer drawer)
        {
            drawer.DrawLabel(canvas, this);
        }
    }
}
