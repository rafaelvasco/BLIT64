
namespace BLIT64.Toolkit.Gui
{
    public class DefaultTheme : Theme
    {
        public byte TextColor = Palette.WhiteColor;
        public byte TextShadowColor = Palette.BlackColor;

        public byte PanelColor = 38;
        public byte PanelBorderColor = Palette.BlackColor;
        public byte PanelBorderSize = 2;

        public byte WindowColor = 38;
        public int WindowHeaderHeight = 30;
        public byte WindowHeaderColor = 11;
        public byte WindowCloseButtonColor = 12;
        public byte WindowCloseButtonHoverColor = 10;
        public byte WindowCloseButtonActiveColor = 12;

        public byte ButtonColor = 12;
        public byte ButtonHoverColor = 11;
        public byte ButtonActiveColor = 12;

        public byte CheckboxColor = 39;
        public byte CheckboxBorderColor = 40;
        public byte CheckboxIndicatorColor = 11;

        public byte TabHeaderColor = 40;

        public byte ListViewBgColor = 40;
        public byte ListViewRowColor = 39;
        public byte ListViewRowHoverColor = 12;
        public byte ListViewRowSelectedColor = 11;

        public override void DrawButton(Blitter blitter, Button button)
        {
            var color = ButtonColor;

            if (button.Hovered)
            {
                color = ButtonHoverColor;
            }

            if (button.Active)
            {
                color = ButtonActiveColor;

            }

            DrawPanel(blitter, button.DrawX, button.DrawY, button.Width, button.Height, color, PanelBorderColor, PanelBorderSize);
        }

        public override void DrawPanel(Blitter blitter, Panel panel)
        {
            DrawPanel(blitter, panel.DrawX, panel.DrawY, panel.Width, panel.Height, PanelColor, PanelBorderColor, PanelBorderSize);
        }

        public override void DrawLabel(Blitter blitter, Label label)
        {
            DrawShadowedText(blitter, label.Text, label.DrawX, label.DrawY, label.Width, label.Height, label.TextMeasure.W, label.TextMeasure.H, TextColor, TextShadowColor);
        }

        public override void DrawCheckbox(Blitter blitter, CheckBox checkbox)
        {
            DrawInsetPanel(blitter, checkbox.DrawX , checkbox.DrawY, checkbox.Width, checkbox.Height, CheckboxColor, CheckboxBorderColor, 2);

            int indicator_w = checkbox.Width-4;
            int indicator_h = checkbox.Height-4;

            if (checkbox.Checked)
            {
                blitter.Rect(checkbox.DrawX + checkbox.Width/2 - indicator_w/2, checkbox.DrawY + checkbox.Height/2 - indicator_h/2, indicator_w, indicator_h, CheckboxIndicatorColor);
            }
        }

        public override void DrawWindow(Blitter blitter, Window window)
        {
            DrawPanel(blitter, window.DrawX, window.DrawY, window.Width, window.Height, WindowColor, PanelBorderColor, PanelBorderSize);

            blitter.Rect(window.DrawX, window.DrawY, window.Width, WindowHeaderHeight, WindowHeaderColor);
        }

        public override void DrawWindowCloseButton(Blitter blitter, Button button)
        {
            var color = WindowCloseButtonColor;

            if (button.Hovered)
            {
                color = WindowCloseButtonHoverColor;
            }

            if (button.Active)
            {
                color = WindowCloseButtonActiveColor;

            }

            blitter.Rect(button.DrawX, button.DrawY, button.Width, button.Height, color);
        }
        public override void DrawTabHeader(Blitter blitter, TabHeader tab_header)
        {
            DrawPanel(blitter, tab_header.DrawX, tab_header.DrawY, tab_header.Width, tab_header.Height, TabHeaderColor, PanelBorderColor, PanelBorderSize);
        }

        private void DrawPanel(Blitter blitter, int x, int y, int w, int h, byte color, byte border_color, int border_size)
        {
            blitter.Rect(x, y, w, h, color);
            blitter.RectBorder(x, y, w, h, border_color, border_size);
        }

        public override void DrawListView(Blitter blitter, ListView list_view)
        {
            DrawInsetPanel(blitter, list_view.DrawX, list_view.DrawY, list_view.Width, list_view.Height, ListViewBgColor, PanelBorderColor, PanelBorderSize );

            blitter.Clip(list_view.DrawX, list_view.DrawY, list_view.Width, list_view.Height);

            for (int i = 0; i < list_view.Items.Count; ++i)
            {
                var item = list_view.Items[i];

                var row_y = list_view.DrawY + i * list_view.RowHeight - list_view.TranslateY;

                if (i%2 == 0)
                {
                    blitter.Rect(list_view.DrawX, row_y, list_view.Width, list_view.RowHeight, ListViewRowColor);
                }

                if (i == list_view.HoveredIndex)
                {
                    blitter.Rect(list_view.DrawX, row_y, list_view.Width, list_view.RowHeight, ListViewRowHoverColor);
                }

                if (i == list_view.SelectedIndex)
                {
                    blitter.Rect(list_view.DrawX, row_y, list_view.Width, list_view.RowHeight, ListViewRowSelectedColor);
                }
                

                var item_label_measure = blitter.TextMeasure(item.Label);
                blitter.Text(list_view.DrawX + 15, row_y + list_view.RowHeight/2 - item_label_measure.Height/2, item.Label, 1, TextColor);

                if (!list_view.ScrollThumbRect.IsEmpty)
                {
                    var thumb_rect = list_view.ScrollThumbRect;
                    blitter.Rect(thumb_rect.X + list_view.DrawX, thumb_rect.Y + list_view.DrawY + list_view.TranslateY, thumb_rect.W, thumb_rect.H, 10);
                }
            }

            blitter.Clip();
        }

        private void DrawShadowedText(
            Blitter blitter, 
            string text, 
            int x, 
            int y, 
            int w, 
            int h, 
            int text_w, 
            int text_h, 
            byte color, 
            byte shadow_color,
            int scale = 1)
        {
            int draw_x = x + w / 2 - text_w / 2;
            int draw_y = y + h / 2 - text_h / 2;

            blitter.Text(draw_x, draw_y+1, text, scale, shadow_color);
            blitter.Text(draw_x, draw_y, text, scale, color);
        }

        private void DrawInsetPanel(Blitter blitter, int x, int y, int w, int h, byte color, byte border_color, int border_size)
        {
            blitter.Rect(x, y, w, h, color);
            blitter.RectBorder(x, y, w, h, border_color, border_size);
        }
    }
}
