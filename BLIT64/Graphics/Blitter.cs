using System;
using System.Collections.Generic;

namespace BLIT64
{
    public class Blitter
    {
        internal bool NeedsUpdate { get; set; }

        private readonly Pixmap _blit_surface;
        private readonly DrawSurface _draw_surface_ref;
        private readonly Font _default_font;
        private Font _current_font;
        private Pixmap _current_blit_surface;
        private Rect _clip_rect;
        

        internal Blitter(DrawSurface draw_surface)
        {
            _blit_surface = Assets.CreatePixmap(draw_surface.Width, draw_surface.Height);
            _draw_surface_ref = draw_surface;
            _current_blit_surface = _blit_surface;
            _clip_rect = new Rect(0, 0, draw_surface.Width, draw_surface.Height);
            _default_font = Assets.GetEmbedded<Font>("default_font");
            _current_font = _default_font;
        }

        public void Clip(Rect rect)
        {
            _clip_rect = rect;
            if (_clip_rect.IsEmpty)
            {
                _clip_rect = new Rect(0, 0, _current_blit_surface.Width, _current_blit_surface.Height);
            }
            else if (!(new Rect(0, 0, _current_blit_surface.Width, _current_blit_surface.Height)).Contains(_clip_rect))
            {
                _clip_rect = new Rect(0, 0, _current_blit_surface.Width, _current_blit_surface.Height);
            }
        }
        
        public void Clip(int x=0, int y=0, int w=0, int h=0)
        {
            Clip(new Rect(x, y, w, h));
        }

        public void Clear(byte color = Palette.TransparentColor)
        {
            var clip_rect = _clip_rect;
           
            Rect(clip_rect.X, clip_rect.Y, clip_rect.W, clip_rect.H, color);

            NeedsUpdate = true;
        }

        public void SetSurface(Pixmap pixmap)
        {
            _current_blit_surface = pixmap ?? _blit_surface;
            _clip_rect = new Rect(0, 0, _current_blit_surface.Width, _current_blit_surface.Height);
        }

        public void SetFont(Font font)
        {
            _current_font = font ?? _default_font;
        }

        public void Pixel(int x, int y, byte color)
        {
            NeedsUpdate = true;

            if (!_clip_rect.Contains(x, y))
            {
                return;
            }

            var tx = Calc.Clamp(x, _clip_rect.Left, _clip_rect.Right);
            var ty = Calc.Clamp(y, _clip_rect.Top, _clip_rect.Bottom);

            var idx = tx + ty * _current_blit_surface.Width;
            _current_blit_surface.Colors[idx] = color;
        }

        public void Rect(int x, int y, int w, int h, byte color)
        {
            NeedsUpdate = true;

            var sw = _current_blit_surface.Width;
            var colors = _current_blit_surface.Colors;
            var clip_rect = _clip_rect;

            var min_x = Math.Max(x, clip_rect.Left);
            var min_y = Math.Max(y, clip_rect.Top);
            var max_x = Math.Min((x + w), clip_rect.Right);
            var max_y = Math.Min((y + h), clip_rect.Bottom);

            for (var i = min_x; i < max_x; ++i)
            {
                for (var j = min_y; j < max_y; j++)
                {
                    var idx = i + j * sw;
                    colors[idx] = color;
                }
            }
        }

        public void RectBorderDashed(
            int x, 
            int y, 
            int w, 
            int h, 
            byte color, 
            int line_size, 
            int dash_offset, 
            int dash_size)
        {
            dash_size = Math.Max(1, dash_size);

            NeedsUpdate = true;
            
            var sw = _current_blit_surface.Width;
            var colors = _current_blit_surface.Colors;
            var clip_rect = _clip_rect;

            var clip_right = clip_rect.X + _clip_rect.W;
            var clip_bottom = _clip_rect.Y + _clip_rect.H;

            var min_x1 = Math.Max(x, _clip_rect.Left);
            var max_x1 = Math.Min(x + w, clip_right);
            var min_y1 = Math.Max(y - line_size, _clip_rect.Top);
            var max_y1 = Math.Min(y + h + line_size, clip_bottom);

            // Top Side

            byte paint_color;

            if (y > _clip_rect.Y)
            {
                var min_x = min_x1;
                var max_x = max_x1;
                var min_y = min_y1;
                var max_y = Math.Min(y, clip_bottom);

                for (var i = min_x; i < max_x; ++i)
                {
                    paint_color = (i - dash_offset) % dash_size == 0 ? Palette.TransparentColor : color;

                    for (var j = min_y; j < max_y; ++j)
                    {
                        var idx = i + j * sw;
                        colors[idx] = paint_color;
                    }
                }
            }

            // Bottom Side

            if (y + h < clip_bottom)
            {
                var min_x = min_x1;
                var max_x = max_x1;
                var min_y = Math.Max(y + h, _clip_rect.Top);
                var max_y = max_y1;

                for (var i = min_x; i < max_x; ++i)
                {
                    paint_color = (i + dash_offset) % dash_size == 0 ? Palette.TransparentColor : color;

                    for (var j = min_y; j < max_y; ++j)
                    {
                        var idx = i + j * sw;
                        colors[idx] = paint_color;
                    }
                }
            }

            // Left Side

            if (x  > _clip_rect.X)
            {
                var min_x = Math.Max(x - line_size, _clip_rect.Left);
                var max_x = Math.Min(x, clip_right);
                var min_y = min_y1;
                var max_y = max_y1;

                for (var i = min_x; i < max_x; ++i)
                {
                    for (var j = min_y; j < max_y; ++j)
                    {
                        paint_color = (j + dash_offset) % dash_size == 0 ? Palette.TransparentColor : color;

                        var idx = i + j * sw;
                        colors[idx] = paint_color;
                    }
                }
            }

            // Right Side

            if (x + w < clip_right)
            {
                var min_x = Math.Max(x + w, _clip_rect.Left);
                var max_x = Math.Min(x + w + line_size, clip_right);
                var min_y = min_y1;
                var max_y = max_y1;

                for (var i = min_x; i < max_x; ++i)
                {
                    for (var j = min_y; j < max_y; ++j)
                    {
                        paint_color = (j - dash_offset) % dash_size == 0 ? Palette.TransparentColor : color;

                        var idx = i + j * sw;
                        colors[idx] = paint_color;
                    }
                }
            }
        }

        public void RectBorder(int x, int y, int w, int h, byte color, int line_size = 1)
        {
            NeedsUpdate = true;
            
            var sw = _current_blit_surface.Width;
            var colors = _current_blit_surface.Colors;
            var clip_rect = _clip_rect;

            var clip_right = clip_rect.X + _clip_rect.W;
            var clip_bottom = _clip_rect.Y + _clip_rect.H;

            // Top Side

            var min_x1 = Math.Max(x, _clip_rect.Left);
            var max_x1 = Math.Min(x + w, clip_right);
            var min_y1 = Math.Max(y - line_size, _clip_rect.Top);
            var max_y1 = Math.Min(y + h + line_size, clip_bottom);

            if (y > _clip_rect.Y)
            {
                var min_x = min_x1;
                var max_x = max_x1;
                var min_y = min_y1;
                var max_y = Math.Min(y, clip_bottom);

                for (var i = min_x; i < max_x; ++i)
                {
                    for (var j = min_y; j < max_y; j++)
                    {
                        var idx = i + j * sw;
                        colors[idx] = color;
                    }
                }
            }

            // Bottom Side

            if (y + h < clip_bottom)
            {
                var min_x = min_x1;
                var max_x = max_x1;
                var min_y = Math.Max(y + h, _clip_rect.Top);
                var max_y = max_y1;

                for (var i = min_x; i < max_x; ++i)
                {
                    for (var j = min_y; j < max_y; j++)
                    {
                        var idx = i + j * sw;
                        colors[idx] = color;
                    }
                }
            }

            // Left Side

            if (x  > _clip_rect.X)
            {
                var min_x = Math.Max(x - line_size, _clip_rect.Left);
                var max_x = Math.Min(x, clip_right);
                var min_y = min_y1;
                var max_y = max_y1;

                for (var i = min_x; i < max_x; ++i)
                {
                    for (var j = min_y; j < max_y; j++)
                    {
                        var idx = i + j * sw;
                        colors[idx] = color;
                    }
                }
            }

            // Right Side

            if (x + w < clip_right)
            {
                var min_x = Math.Max(x + w, _clip_rect.Left);
                var max_x = Math.Min(x + w + line_size, clip_right);
                var min_y = min_y1;
                var max_y = max_y1;

                for (var i = min_x; i < max_x; ++i)
                {
                    for (var j = min_y; j < max_y; j++)
                    {
                        var idx = i + j * sw;
                        colors[idx] = color;
                    }
                }
            }
        }
        
        public void Line(int x1, int y1, int x2, int y2, int size, byte color)
        {
            NeedsUpdate = true;

            var dx = Math.Abs(x2 - x1);
            var dy = Math.Abs(y2 - y1);
            var sx = x1 < x2 ? size : -size;
            var sy = y1 < y2 ? size : -size;
            var err = (dx > dy ? dx : -dy) / 2;
            int e2;
            var sw = _current_blit_surface.Width;
            var colors = _current_blit_surface.Colors;
            var clip_rect = _clip_rect;

            while (true)
            {
                if ((x1 >= x2 && y1 >= y2))
                {
                    break;
                }

                for (var i = x1; i < x1+size; ++i)
                {
                    for (var j = y1; j < y1+size; ++j)
                    {
                        if (i < clip_rect.Left || i > clip_rect.Right-1 || j < clip_rect.Top || j > clip_rect.Bottom-1)
                        {
                            continue;
                        }

                        var idx = i + j * sw;

                        colors[idx] = color;    
                    }
                }

                e2 = err;

                if (e2 > -dx)
                {
                    err -= dy;
                    x1 += sx;
                }

                if (e2 < dy)
                {
                    err += dx;
                    y1 += sy;
                }
            }
        }

        public void Circle(int center_x, int center_y, int radius, byte color)
        {
            NeedsUpdate = true;

            int radius_sqr = radius * radius;

            var colors = _current_blit_surface.Colors;
            var clip_rect = _clip_rect;
            var sw = _current_blit_surface.Width;

            for (int x = -radius; x < radius; ++x)
            {
                int height = (int)Math.Sqrt(radius_sqr - x * x);

                for (int _y = -height; _y < height; ++_y)
                {
                    if (!clip_rect.Contains(center_x+x, center_y+_y))
                    {
                        continue;
                    }

                    var idx = (center_x+x) + (center_y+_y) * sw;

                    colors[idx] = color;    
                }
            }
        }

        public void FloodFill(int x, int y, byte color)
        {
            NeedsUpdate = true;

            var sw = _current_blit_surface.Width;
            var colors = _current_blit_surface.Colors;
            var clip_rect = _clip_rect;

            var target_color = colors[x + y * sw];

            var color_queue = new Queue<(int, int)>();

            color_queue.Enqueue((x,y));

            bool ColorMatch(int x, int y)
            {
                return colors[x + y * sw] == target_color;
            }

            while (color_queue.Count > 0)
            {
                var (current_x, current_y) = color_queue.Dequeue();

                if (!ColorMatch(current_x, current_y))
                {
                    continue;
                }

                var min_x = clip_rect.Left;
                var min_y = clip_rect.Top;
                var max_x = clip_rect.Right;
                var max_y = clip_rect.Bottom;

                var west_x = current_x;
                var west_y = current_y;
                var east_x = current_x + 1;
                var east_y = current_y;

                while ((west_x >= min_x) && ColorMatch(west_x, west_y))
                {
                    colors[west_x + west_y * sw] = color;

                    if ((west_y > min_y) && ColorMatch(west_x, west_y - 1))
                    {
                        color_queue.Enqueue((west_x, west_y-1));
                    }

                    if ((west_y < max_y - 1) && ColorMatch(west_x, west_y+1))
                    {
                        color_queue.Enqueue((west_x, west_y+1));
                    }

                    west_x--;
                }

                while ((east_x <= max_x - 1) && ColorMatch(east_x, east_y))
                {
                    colors[east_x + east_y * sw] = color;

                    if ((east_y > min_y) && ColorMatch(east_x, east_y - 1))
                    {
                        color_queue.Enqueue((east_x, east_y - 1));
                    }

                    if ((east_y < max_y - 1) && ColorMatch(east_x, east_y + 1))
                    {
                        color_queue.Enqueue((east_x, east_y + 1));
                    }

                    east_x++;
                }
            }
        }

        public void FlipH(int src_x, int src_y, int src_w, int src_h)
        {
            var colors = _current_blit_surface.Colors;
            var sw = _current_blit_surface.Width;
            var row = new byte[src_w];

            for (var y = src_y; y < src_y + src_h; ++y)
            {
                var row_idx = 0;

                for (var x = src_x; x < src_x + src_w; ++x)
                {
                    var idx = x + y * sw;
                    row[row_idx++] = colors[idx];
                }

                for (var x = src_x; x < src_x + src_w; ++x)
                {
                    var idx = x + y * sw;
                    colors[idx] = row[--row_idx];
                }
            }
        }

        public void FlipV(int src_x, int src_y, int src_w, int src_h)
        {
            var colors = _current_blit_surface.Colors;
            var sh = _current_blit_surface.Height;
            var col = new byte[src_w * src_h];

            for (var x = src_x; x < src_x + src_w; ++x)
            {
                var col_idx = 0;

                for (var y = src_y; y < src_y + src_h; ++y)
                {
                    var idx = x + y * sh;
                    col[col_idx++] = colors[idx];
                }

                for (var y = src_y; y < src_y + src_h; ++y)
                {
                    var idx = x + y * sh;
                    colors[idx] = col[--col_idx];
                }
            }
        }

        public unsafe void Rotate90(int src_x, int src_y, int src_w, int src_h, int target_x=0, int target_y=0, int target_w=0, int target_h=0)
        {
            if (target_w == 0 || target_h == 0)
            {
                target_x = src_x;
                target_y = src_y;
                target_w = src_w;
                target_h = src_h;
            }

            var colors = _current_blit_surface.Colors;
            var sw = _current_blit_surface.Width;
            var aux_colors_buffer = stackalloc byte[src_w * src_h];
            var aux_colors = new Span<byte>(aux_colors_buffer, src_w * src_h);

            int rows = src_h;
            int cols = src_w;

            for (int r = 0;  r < rows;  ++r)
            {
                for (int c = 0; c < cols; ++c)
                {
                    aux_colors[(c*rows) + (rows-r-1)] = colors[(r+src_y) * sw + (c+src_x)];
                }
            }

            for (int j = target_y, c = 0; j < target_y+target_h; ++j)
            {
                for (int i = target_x; i < target_x+target_w; ++i)
                {
                    colors[i + j * sw] = aux_colors[c++];
                }
            }
        }

        public void Pixmap(Pixmap pixmap, 
            int x, 
            int y, 
            Rect src_rect, 
            float width = -1,
            float height = -1,
            byte color_key = Palette.TransparentColor, 
            byte tint = Palette.NoColor,
            bool flip=false)
        {
            NeedsUpdate = true;

            var sw = _current_blit_surface.Width;
            var pw = pixmap.Width;
            var surface_colors = _current_blit_surface.Colors;
            var pixmap_colors = pixmap.Colors;
            var clip_rect = _clip_rect;

            if (src_rect.IsEmpty)
            {
                src_rect = new Rect(0, 0, pixmap.Width, pixmap.Height);
            }

            if (width < 1)
            {
                width = src_rect.W;
            }

            if (height < 1)
            {
                height = src_rect.H;
            }

            var factor_w = width / src_rect.W;
            var factor_h = height / src_rect.H;

            var min_x = Math.Max(x, clip_rect.Left);
            var min_y = Math.Max(y, clip_rect.Top);
            var max_x = Math.Min(x + width, clip_rect.Right);
            var max_y = Math.Min(y + height, clip_rect.Bottom);

            if (!flip)
            {
                for (var i = min_x; i < max_x; ++i)
                {
                    for (var j = min_y; j < max_y; ++j)
                    {
                        var surf_idx = (i + j * sw);
                        var pix_idx = ((src_rect.X + (int)((i - x)/factor_w)) + (src_rect.Y +(int)((j - y)/factor_h)) * pw);
                        var pix_color = pixmap_colors[pix_idx];

                        if (pix_color == color_key)
                        {
                            continue;
                        }

                        if (tint != Palette.NoColor)
                        {
                            pix_color = tint;
                        }

                        surface_colors[surf_idx] = pix_color;
                    }
                }
            }
            else
            {
                var start_pix_x = src_rect.Right-1;
                for (var i = min_x; i < max_x; ++i)
                {
                    for (var j = min_y; j < max_y; ++j)
                    {
                        var surf_idx = (i + j * sw);
                        var pix_idx = (start_pix_x - (int)((i - x)/factor_w) + (src_rect.Y + (int)((j - y)/factor_h)) * pw);
                        var pix_color = pixmap_colors[pix_idx];

                        if (pix_color == color_key)
                        {
                            continue;
                        }

                        if (tint != Palette.NoColor)
                        {
                            pix_color = tint;
                        }

                        surface_colors[surf_idx] = pix_color;
                    }
                }
            }
        }

        public (int Width, int Height) TextMeasure(string text, int scale=1)
        {
            return (text.Length * 8 * scale, 8 * scale); //TODO
        }

        public void Text(int x, int y, string text, int scale=1, byte tint = Palette.NoColor)
        {
            NeedsUpdate = true;

            var font = _current_font;

            var glyph_size = font.GlyphSize;
            var scaled_glyph_size = glyph_size * scale;
            

            var sw = _current_blit_surface.Width;
            var pw = font.Width;
            var count_glyphs = font.Width / glyph_size;
            var surface_colors = _current_blit_surface.Colors;
            var font_colors = font.Colors;
            var clip_rect = _clip_rect;

            for (int ci = 0; ci < text.Length; ci++)
            {
                char ch = text[ci];
                int glyph_index = ch - 32;

                var min_x = Math.Max(x + ci * scaled_glyph_size, clip_rect.Left);
                var min_y = Math.Max(y, clip_rect.Top);
                var max_x = Math.Min(x + ci * scaled_glyph_size + scaled_glyph_size, clip_rect.Right);
                var max_y = Math.Min(y + scaled_glyph_size, clip_rect.Bottom);

                var src_rect = new Rect(
                    (glyph_index % count_glyphs)*glyph_size,
                    (glyph_index / count_glyphs)*glyph_size,
                    glyph_size,
                    glyph_size
                );

                for (var i = min_x; i < max_x; ++i)
                {
                    for (var j = min_y; j < max_y; ++j)
                    {
                        var surf_idx = (i + j * sw);
                        var pix_idx = ((src_rect.X + ((i - (x + ci * scaled_glyph_size))/scale)) + (src_rect.Y +((j - y)/scale)) * pw);
                        var pix_color = font_colors[pix_idx];

                        switch (pix_color)
                        {
                            case 0:
                                continue;
                            case 1 when tint != Palette.NoColor:
                                pix_color = tint;
                                break;
                        }

                        surface_colors[surf_idx] = pix_color;
                    }
                }
            }

        }

        /// <summary>
        /// Draws a sprite from a SpriteSheet
        /// </summary>
        /// <param name="sprite_sheet"></param>
        /// <param name="id">SpriteSheet cell id to draw</param>
        /// <param name="x">X position on screen</param>
        /// <param name="y">Y position on screen</param>
        /// <param name="color_key">What color index on current Palette to remove when drawing</param>
        /// <param name="tint">If this value is a valid color on palette, all white colors on sprite are substituted with it.</param>
        /// <param name="scale">Scale to draw</param>
        /// <param name="flip">Flip horizontally</param>
        /// <param name="width">How many cells to draw horizontally. By default just the cell passed on 'id', that is width=1</param>
        /// <param name="height">How many cells to draw vertically. By default just the cell passed on 'id', that is height=1</param>
        public void Sprite(
            SpriteSheet sprite_sheet, 
            int id, 
            int x, 
            int y,
            byte color_key = Palette.TransparentColor,
            byte tint = Palette.NoColor,
            float scale=1,
            bool flip = false,
            int width = 1,
            int height = 1
            )
        {
            Rect src_rect;

            if (width < 2 && height < 2)
            {
                src_rect = sprite_sheet[id];
            }
            else
            {
                ref readonly var single_cell_src_rect = ref sprite_sheet[id];

                src_rect = new Rect(
                    single_cell_src_rect.X, 
                    single_cell_src_rect.Y,
                    width * sprite_sheet.TileSize,
                    height * sprite_sheet.TileSize
                );
            }
            
            Pixmap(
                sprite_sheet,
                x, y, 
                src_rect,
                width:src_rect.W * scale,
                height:src_rect.H * scale,
                color_key,
                tint,
                flip
            );
        }

        internal void UpdateDrawSurface(Palette palette)
        {
            NeedsUpdate = false;

            var rgba_surface = _draw_surface_ref.DataPtr;

            unsafe
            {
                var p = (byte*)rgba_surface;
                var rgb_surface_idx = 0;
                var color_idxs = _current_blit_surface.Colors;

                for (int i = 0; i < color_idxs.Length; ++i)
                {
                    var color_idx = color_idxs[i];

                    if (color_idx > 0)
                    {
                        var rgb_color = palette.Map(color_idx);
                        *(p + rgb_surface_idx) = rgb_color.R;
                        *(p + rgb_surface_idx + 1) = rgb_color.G;
                        *(p + rgb_surface_idx + 2) = rgb_color.B;
                        
                    }
                    else
                    {
                        *(p + rgb_surface_idx) = 0;
                        *(p + rgb_surface_idx + 1) = 0;
                        *(p + rgb_surface_idx + 2) = 0;
                        *(p + rgb_surface_idx + 3) = 0;
                    }
                    rgb_surface_idx += 4;
                    
                }
            }
        }

    }
}
