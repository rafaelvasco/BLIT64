
namespace BLIT64.Toolkit.Gui
{
    public class CheckBox : Container
    {
        public const int DefaultWidth = 20;
        public const int DefaultHeight = 20;

        public string Label
        {
            get => _label.Text;
            set => _label.Text = value;
        }


        protected readonly Label _label;

        public CheckBox(string id, int width = DefaultWidth, int height = DefaultHeight, string label = "Check") : base(id, width, height)
        {

            CanHaveInputFocus = true;

            Toggable = true;

            _label = new Label(id + "_label", label)
            {
                BubbleEventsToParent = true
            };

            Add(_label);

            _label.Height = _height;

            _label.X = _width + 10;

        }


        public override void OnMouseDown(MouseButton button, int x, int y)
        {
            OffsetY = 1;
        }

        public override void OnMouseUp(MouseButton button, int x, int y)
        {
            OffsetY = 0;
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

                if (ToggleGroup == null)
                {
                    On = !On;
                }
                else
                {
                    On = true;
                    Ui.UpdateToggleGroup(this);
                }
                
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

        public override void Draw(Canvas canvas, IGuiDrawer drawer)
        {
            drawer.DrawCheckbox(canvas, this);

            DrawChildren(canvas, drawer);
        }
    }
}
