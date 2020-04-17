
using System;

namespace BLIT64
{
    public class Blitter
    {
        internal bool NeedsUpdate { get; set; }

        private Pixmap _current_blit_surface;
        private Rect _clip_rect;
        private readonly Pixmap _blit_surface;
        private readonly RenderSurface _render_surface_ref;
        private readonly Font _default_font;

        internal Blitter(RenderSurface render_surface)
        {
            _blit_surface = Assets.CreatePixmap(render_surface.Width, render_surface.Height);
            _render_surface_ref = render_surface;
            _current_blit_surface = _blit_surface;
            _clip_rect = new Rect(0, 0, render_surface.Width, render_surface.Height);
            _default_font = Assets.GetEmbedded<Font>("default_font");
        }

        public void Clip(int x=0, int y=0, int w=0, int h=0)
        {
            _clip_rect = new Rect(x, y, w, h);
            if (_clip_rect.IsEmpty)
            {
                _clip_rect = new Rect(0, 0, _current_blit_surface.Width, _current_blit_surface.Height);
            }
            else if (!(new Rect(0, 0, _current_blit_surface.Width, _current_blit_surface.Height)).Contains(_clip_rect))
            {
                _clip_rect = new Rect(0, 0, _current_blit_surface.Width, _current_blit_surface.Height);
            }
        }

        public void Clear(int color_index = 0)
        {
            var clip_rect = _clip_rect;
           
            Rect(clip_rect.X, clip_rect.Y, clip_rect.W, clip_rect.H, color_index);

            NeedsUpdate = true;
        }

        public void SetSurface(Pixmap pixmap)
        {
            _current_blit_surface = pixmap;
            _clip_rect = new Rect(0, 0, _current_blit_surface.Width, _current_blit_surface.Height);
        }

        public void ResetSurface()
        {
            SetSurface(_blit_surface);
        }

        public void UpdateRenderSurface(Palette palette)
        {
            NeedsUpdate = false;

            var rgba_surface = _render_surface_ref.DataPtr;

            unsafe
            {
                var p = (byte*)rgba_surface;
                var rgb_surface_idx = 0;
                var color_idxs = _current_blit_surface.Colors;

                for (int i = 0; i < color_idxs.Length; ++i)
                {
                    var color_idx = color_idxs[i];

                    if (color_idx > -1)
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

        public void Pixel(int x, int y, int col_index = 0)
        {
            if (!_clip_rect.Contains(x, y))
            {
                return;
            }

            var tx = Calc.Clamp(x, _clip_rect.Left, _clip_rect.Right);
            var ty = Calc.Clamp(y, _clip_rect.Top, _clip_rect.Bottom);

            var idx = tx + ty * _current_blit_surface.Width;
            _current_blit_surface.Colors[idx] = col_index;
        }

        public void Rect(int x, int y, int w, int h, int col_index = 0)
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
                    colors[idx] = col_index;
                }
            }
        }

        public void RectBorder(int x, int y, int w, int h, int line_size = 1, int col_index = 0)
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
                        colors[idx] = col_index;
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
                        colors[idx] = col_index;
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
                        colors[idx] = col_index;
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
                        colors[idx] = col_index;
                    }
                }
            }
        }

        public void Line2(int x1, int y1, int x2, int y2, int size, int col_index = 0)
        {
            NeedsUpdate = true;

            var dx = x2 - x1;
            var dy = y1 - y1;
            var adx = Math.Abs(dx);
            var ady = Math.Abs(dy);
            var eps = 0;
            var sx = dx > 0 ? size : -size;
            var sy = dy > 0 ? size : -size;
            var sw = _current_blit_surface.Width;
            var colors = _current_blit_surface.Colors;
            var clip_rect = _clip_rect;

            if (adx > ady)
            {
                for (int x = x1, y = y1; sx < 0 ? x >= x2 - sx : x <= x2 - sx; x += sx)
                {
                    var min_x = Math.Max(x, clip_rect.Left);
                    var min_y = Math.Max(y, clip_rect.Top);
                    var max_x = Math.Min((x + size), clip_rect.Right);
                    var max_y = Math.Min((y + size), clip_rect.Bottom);

                    for (int j = min_y; j < max_y; ++j)
                    {
                        for (int i = min_x; i < max_x; ++i)
                        {
                            var idx = i + j * sw;
                            colors[idx] = col_index;
                        }
                    }

                    eps += ady;
                    if (eps << 1 >= adx)
                    {
                        y += sy;
                        eps -= adx;
                    }
                }
            }
            else
            {
                for (int x = x1, y = y1; sy < 0 ? y >= y2 - sy : y <= y2 - sy; y += sy)
                {
                    var min_x = Math.Max(x, clip_rect.Left);
                    var min_y = Math.Max(y, clip_rect.Top);
                    var max_x = Math.Min((x + size), clip_rect.Right);
                    var max_y = Math.Min((y + size), clip_rect.Bottom);

                    for (int j = min_y; j < max_y; ++j)
                    {
                        for (int i = min_x; i < max_x; ++i)
                        {
                            var idx = i + j * sw;
                            colors[idx] = col_index;
                        }
                    }

                    eps += adx;
                    if (eps << 1 >= ady)
                    {
                        x += sx;
                        eps -= ady;
                    }
                }
            }
        }

        public void Line(int x1, int y1, int x2, int y2, int size, int col_index=0)
        {
            NeedsUpdate = true;

            var dx = Math.Abs(x2 - x1);
            var dy = Math.Abs(y2 - y1);
            var sx = x1 < x2 ? size : -size;
            var sy = y1 < y2 ? size : -size;
            var err = (dx > dy ? dx : -dy) / 2;
            var sw = _current_blit_surface.Width;
            var colors = _current_blit_surface.Colors;
            var clip_rect = _clip_rect;

            int iterations = 0;

            while (true)
            {
                iterations += 1;

                if (iterations > 1000)
                {
                    break;
                }

                var min_x = Math.Max(x1, clip_rect.Left);
                var min_y = Math.Max(y1, clip_rect.Top);
                var max_x = Math.Min((x1 + size), clip_rect.Right);
                var max_y = Math.Min((y1 + size), clip_rect.Bottom);

                for (var i = min_x; i < max_x; ++i)
                {
                    for (var j = min_y; j < max_y; j++)
                    {
                        var idx = i + j * sw;
                        colors[idx] = col_index;
                    }
                }

                if ((x1 == x2 && y1 == y2))
                {
                    break;
                }

                var e2 = err;

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

        public void Pixmap(Pixmap pixmap, 
            int x, 
            int y, 
            Rect src_rect, 
            int width=-1, 
            int height=-1, 
            int color_key=-1, 
            bool flip=false)
        {
            var sw = _current_blit_surface.Width;
            var pw = pixmap.Width;
            var surface_colors = _current_blit_surface.Colors;
            var pixmap_colors = pixmap.Colors;
            var clip_rect = _clip_rect;

            if (src_rect.IsEmpty)
            {
                src_rect = new Rect(0, 0, pixmap.Width, pixmap.Height);
            }

            if (width == -1)
            {
                width = src_rect.W;
            }

            if (height == -1)
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
                        var pix_idx = ((src_rect.X + ((i - x)/factor_w)) + (src_rect.Y +((j - y)/factor_h)) * pw);
                        var pix_color = pixmap_colors[pix_idx];

                        if (color_key > -1 && pix_color == color_key)
                        {
                            continue;
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
                        var pix_idx = (start_pix_x - (i - x)/factor_w + (src_rect.Y +((j - y)/factor_h)) * pw);
                        var pix_color = pixmap_colors[pix_idx];

                        if (color_key > -1 && pix_color == color_key)
                        {
                            continue;
                        }

                        surface_colors[surf_idx] = pix_color;
                    }
                }
            }
        }

        public void Text(int x, int y, string text, int scale=1)
        {
            //TODO: Parameterize Glyph Size

            var default_font = _default_font;
            
            var sw = _current_blit_surface.Width;
            var pw = default_font.Width;
            var surface_colors = _current_blit_surface.Colors;
            var font_colors = default_font.Colors;
            var clip_rect = _clip_rect;

            for (int ci = 0; ci < text.Length; ci++)
            {
                char ch = text[ci];
                int glyph_index = ch - 32;

                var min_x = Math.Max(x + ci * 8, clip_rect.Left);
                var min_y = Math.Max(y, clip_rect.Top);
                var max_x = Math.Min(x + ci * 8 + 8, clip_rect.Right);
                var max_y = Math.Min(y + 8, clip_rect.Bottom);

                var src_rect = new Rect(
                    (glyph_index % 16)*8,
                    (glyph_index / 16)*8,
                    8,
                    8
                );

                for (var i = min_x; i < max_x; ++i)
                {
                    for (var j = min_y; j < max_y; ++j)
                    {
                        var surf_idx = (i + j * sw);
                        var pix_idx = ((src_rect.X + ((i - (x + ci * 8))/scale)) + (src_rect.Y +((j - y)/scale)) * pw);
                        var pix_color = font_colors[pix_idx];

                        if (pix_color == -1)
                        {
                            continue;
                        }

                        surface_colors[surf_idx] = pix_color;
                    }
                }
            }

        }

    }
}
