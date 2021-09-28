using System;

namespace BLIT64.Toolkit.Gui
{
    public class DefaultGuiDrawer : IGuiDrawer
    {
        public int TextColor = 35;
        public int TextShadowColor = 28;

        public int PanelColor = 38;
        public int PanelBorderColor = 28;
        public int PanelBorderSize = 2;

        public int WindowColor = 38;
        public int WindowHeaderHeight = 30;
        public int WindowHeaderColor = 11;
        public int WindowCloseButtonColor = 12;
        public int WindowCloseButtonHoverColor = 10;
        public int WindowCloseButtonActiveColor = 12;

        public int ButtonColor = 12;
        public int ButtonHoverColor = 11;
        public int ButtonActiveColor = 12;

        public int CheckboxColor = 39;
        public int CheckboxBorderColor = 40;
        public int CheckboxIndicatorColor = 11;

        public int TabHeaderColor = 40;

        public int ListViewBgColor = 40;
        public int ListViewRowColor = 39;
        public int ListViewRowHoverColor = 12;
        public int ListViewRowSelectedColor = 11;

        public void DrawButton(Canvas blitter, Button button)
        {
            var color =  ButtonColor;

            if (button.Hovered)
            {
                color = ButtonHoverColor;
            }

            if (button.Active)
            {
                color = ButtonActiveColor;

            }

            if (button.On)
            {
                color = ButtonHoverColor;
            }

            DrawPanel(blitter, button.DrawX, button.DrawY, button.Width, button.Height, color, PanelBorderColor, PanelBorderSize);
        }

        public void DrawCheckbox(Canvas blitter, CheckBox check_box)
        {
            DrawInsetPanel(blitter, check_box.DrawX , check_box.DrawY, check_box.Width, check_box.Height, CheckboxColor, CheckboxBorderColor, 2);

            int indicator_w = check_box.Width-4;
            int indicator_h = check_box.Height-4;

            if (check_box.On)
            {
                blitter.SetColor(CheckboxIndicatorColor);
                blitter.RectFill(check_box.DrawX + check_box.Width/2 - indicator_w/2, check_box.DrawY + check_box.Height/2 - indicator_h/2, indicator_w, indicator_h);
            }
        }

        public void DrawLabel(Canvas blitter, Label label)
        {
            DrawShadowedText(blitter, label.Text, label.DrawX, label.DrawY, label.Width, label.Height, label.TextMeasure.W, label.TextMeasure.H, TextColor, TextShadowColor, label.Scale);
        }

        public void DrawListView(Canvas blitter, ListView list_view)
        {
            DrawInsetPanel(blitter, list_view.DrawX, list_view.DrawY, list_view.Width, list_view.Height, ListViewBgColor, PanelBorderColor, PanelBorderSize );

            blitter.Clip(list_view.DrawX, list_view.DrawY, list_view.Width, list_view.Height);

            for (int i = 0; i < list_view.Items.Count; ++i)
            {
                var item = list_view.Items[i];

                var row_y = list_view.DrawY + i * list_view.RowHeight - list_view.TranslateY;

                if (i%2 == 0)
                {
                    blitter.SetColor(ListViewRowColor);
                    blitter.RectFill(list_view.DrawX, row_y, list_view.Width, list_view.RowHeight);
                }

                if (i == list_view.HoveredIndex)
                {
                    blitter.SetColor(ListViewRowHoverColor);
                    blitter.RectFill(list_view.DrawX, row_y, list_view.Width, list_view.RowHeight);
                }

                if (i == list_view.SelectedIndex)
                {
                    blitter.SetColor(ListViewRowSelectedColor);
                    blitter.RectFill(list_view.DrawX, row_y, list_view.Width, list_view.RowHeight);
                }
                

                var (_, Height) = Canvas.TextMeasure(item.Label);
                blitter.SetColor(TextColor);
                blitter.Text(list_view.DrawX + 15, row_y + list_view.RowHeight/2 - Height/2, item.Label, 1);

                if (!list_view.ScrollThumbRect.IsEmpty)
                {
                    var thumb_rect = list_view.ScrollThumbRect;
                    blitter.SetColor(10);
                    blitter.RectFill(thumb_rect.X + list_view.DrawX, thumb_rect.Y + list_view.DrawY + list_view.TranslateY, thumb_rect.W, thumb_rect.H);
                }
            }

            blitter.Clip();
        }

        public void DrawPanel(Canvas blitter, Panel panel)
        {
            DrawPanel(blitter, panel.DrawX, panel.DrawY, panel.Width, panel.Height, PanelColor, PanelBorderColor, PanelBorderSize);
        }



        public void DrawSelectorSlider(Canvas blitter, SelectorSlider selector_slider)
        {
            var options = selector_slider.Options;
            var draw_x = selector_slider.DrawX;
            var draw_y = selector_slider.DrawY;
            var width = selector_slider.Width;
            var height = selector_slider.Height;
            var steps = options.Length;
            var thumb_size = selector_slider.ThumbSize;

            // Draw Background Steps

            for (int i = 0; i < steps; ++i)
            {
                var option_rect = options[i].Rect;
                blitter.SetColor(28);
                blitter.RectFill(
                    draw_x + option_rect.X, 
                    draw_y + option_rect.Y, 
                    option_rect.W, 
                    option_rect.H
                );
            }

            // Draw Background Bar

            switch (selector_slider.Orientation)
            {
                case Orientation.Horizontal:
                    blitter.SetColor(35);
                    blitter.RectFill(draw_x + thumb_size/2 + 2, draw_y + thumb_size/2 -1, width - thumb_size/2 - 4, 2);
                    blitter.SetColor(28);
                    blitter.Rect(draw_x + thumb_size/2 + 2, draw_y + thumb_size/2 - 1, width - thumb_size/2 - 4, 2, 2);
                    break;
                case Orientation.Vertical:
                    blitter.SetColor(35);
                    blitter.RectFill(draw_x + thumb_size/2 + 2  , draw_y + thumb_size/2 -1, 2, height - thumb_size/2 - 4);
                    blitter.SetColor(28);
                    blitter.Rect(draw_x + thumb_size/2 + 2  , draw_y + thumb_size/2 -1, 2, height - thumb_size/2 - 4, 2);
                    break;
            }

            var current_option_rect = options[selector_slider.SelectedIndex].Rect.Deflate(3);

            // Draw Slider

            blitter.SetColor(35);
            blitter.RectFill(
                draw_x + current_option_rect.X,
                draw_y + current_option_rect.Y,
                current_option_rect.W,
                current_option_rect.H
            );

            blitter.SetColor(28);
            blitter.Rect(
                draw_x + current_option_rect.X,
                draw_y + current_option_rect.Y,
                current_option_rect.W,
                current_option_rect.H
            );
        }

        public void DrawTabs(Canvas blitter, Tabs tabs)
        {
            throw new NotImplementedException();
        }

        public void DrawWindow(Canvas blitter, Window window)
        {
            DrawPanel(blitter, window.DrawX, window.DrawY, window.Width, window.Height, WindowColor, PanelBorderColor, PanelBorderSize);

            blitter.SetColor(WindowHeaderColor);
            blitter.RectFill(window.DrawX, window.DrawY, window.Width, WindowHeaderHeight);
        }

        
        private static void DrawPanel(Canvas blitter, int x, int y, int w, int h, int color, int border_color, int border_size)
        {
            blitter.SetColor(color);
            blitter.RectFill(x, y, w, h);
            blitter.SetColor(border_color);
            blitter.Rect(x, y, w, h, border_size);
        }

        private static void DrawShadowedText(
            Canvas blitter, 
            string text, 
            int x, 
            int y, 
            int w, 
            int h, 
            int text_w, 
            int text_h, 
            int color, 
            int shadow_color,
            int scale = 1)
        {
            int draw_x = x + w / 2 - text_w / 2;
            int draw_y = y + h / 2 - text_h / 2;

            blitter.SetColor(shadow_color);
            blitter.Text(draw_x, draw_y+scale, text, scale);
            blitter.SetColor(color);
            blitter.Text(draw_x, draw_y, text, scale);
        }

        private static void DrawInsetPanel(Canvas blitter, int x, int y, int w, int h, int color, int border_color, int border_size)
        {
            blitter.SetColor(color);
            blitter.RectFill(x, y, w, h);
            blitter.SetColor(border_color);
            blitter.Rect(x, y, w, h, border_size);
        }

    }
}
