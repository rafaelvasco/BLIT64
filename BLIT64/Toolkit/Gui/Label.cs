
namespace BLIT64.Toolkit.Gui
{
    public class Label : Widget
    {
        public byte Color { get; set; }

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
                    TextMeasure = Game.Instance.Blitter.TextMeasure(_text, _scale);
                }
            }
        }

        protected string _text;
        protected int _scale = 1;

        public (int W, int H) TextMeasure { get; private set; }

        public Label(string id, string text) : base(id, 0, 0)
        {
            Text = text;

            Width = TextMeasure.W;
            Height = TextMeasure.H;
        }

        public override void Draw(Blitter blitter, Theme theme)
        {
            theme.DrawLabel(blitter, this);
        }
    }
}
