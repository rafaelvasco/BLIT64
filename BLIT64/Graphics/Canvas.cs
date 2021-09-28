using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace BLIT64
{
    public unsafe partial class Canvas
    {
        public int Width { get; }
        public int Height { get; }

        public bool DrawDebugInfo { get; set; } = false;

        private readonly RenderSurface _render_surface;

        private readonly Pixmap _blit_surface;
        private Font _default_font;
        private Font _current_font;
        private SpriteSheet _current_sprite_sheet;
        private int _color;
        private int _dither_color;
        private int _tint_color;
        private Pixmap _current_blit_surface;
        private Rect _clip_rect;
        private ushort _dither_pattern = 0b1111_1111_1111_1111;
        private int _camera_x;
        private int _camera_y;
        private bool _blit_surface_dirty = true;


        internal Canvas(int width, int height)
        {
            Width = width;

            Height = height;


            _blit_surface = Assets.CreatePixmap(width, height);
            _current_blit_surface = _blit_surface;
            _clip_rect = new Rect(0, 0, width, height);
            _render_surface = Assets.CreateRenderSurface(0, width, height);

            _color = 0;
            _dither_color = Palette.NullColor;
            _tint_color = Palette.NullColor;
        }

        internal void LoadDefaultAssets()
        {
            _default_font = Assets.GetEmbedded<Font>("default_font");
            _current_font = _default_font;
        }

        public void SetTarget(Pixmap target = null)
        {
            _current_blit_surface = target ?? _blit_surface;
            _clip_rect = new Rect(0, 0, _current_blit_surface.Width, _current_blit_surface.Height);
        }

        internal void PresentToDisplay()
        {
            if (_blit_surface_dirty)
            {
                FlushBlitSurface();

                _blit_surface_dirty = false;

                Platform.PresentDrawSurface(_render_surface);
            }
        }

        public void SetColor(int color)
        {
            _color = color;
        }

        public void SetDitherColor(int color)
        {
            _dither_color = color;
        }

        public void SetTintColor(int color)
        {
            _tint_color = color;
        }

        public void SetSpriteSheet(SpriteSheet sprite_sheet)
        {
            _current_sprite_sheet = sprite_sheet;
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

        public void Clip(int x = 0, int y = 0, int w = 0, int h = 0)
        {
            Clip(new Rect(x, y, w, h));
        }

        public void SetCamera(int x, int y)
        {
            _camera_x = x;
            _camera_y = y;
        }


        public void Clear()
        {
            _blit_surface_dirty = true;

            int left = _clip_rect.Left;
            int right = _clip_rect.Right;
            int top = _clip_rect.Top;
            int bottom = _clip_rect.Bottom;
            int sw = _current_blit_surface.Width;

            fixed (int* ptr = &MemoryMarshal.GetArrayDataReference(_current_blit_surface.Colors))
            {
                for (int i = left; i <= right; ++i)
                {
                    for (int j = top; j <= bottom; ++j)
                    {
                        Unsafe.WriteUnaligned(ptr + i + j * sw, 0);
                    }
                }
            }
        }

        public void DitherPattern(ushort pattern = 0b1111_1111_1111_1111)
        {
            _dither_pattern = pattern;
        }


        public void DitherPatternScanlines()
        {
            _dither_pattern = 0b1111_0000_1111_0000;
        }

        public void DitherPatternScanlines2()
        {
            _dither_pattern = 0b0000_1111_0000_1111;
        }

        public void DitherPatternCheckerBoard()
        {
            _dither_pattern = 0b1010_0101_1010_0101;
        }

        public void DitherPatternCheckerBoard2()
        {
            _dither_pattern = 0b0101_1010_0101_1010;
        }

        public void DitherPatternBigCheckerBoard()
        {
            _dither_pattern = 0b1100_1100_0011_0011;
        }

        public void DitherPatternBigCheckerBoard2()
        {
            _dither_pattern = 0b0011_0011_1100_1100;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool DitherPass(int x, int y)
        {
            var x4 = (ushort)(x % 4);
            var y4 = (ushort)(y % 4);
            var bit = (ushort)(y4 * 4 + x4);

            return (_dither_pattern & (1 << bit)) != 0;
        }

        public void SetFont(Font font)
        {
            _current_font = font ?? _default_font;
        }

        public void PixelSet(int x, int y)
        {
            x -= _camera_x;
            y -= _camera_y;

            if (x < _clip_rect.Left || x > _clip_rect.Right || y < _clip_rect.Top || y > _clip_rect.Bottom)
            {
                return;
            }

            _blit_surface_dirty = true;

            _current_blit_surface.Colors[x + y * _current_blit_surface.Width] = _color;
        }

        public int PixelGet(int x, int y)
        {
            x -= _camera_x;
            y -= _camera_y;

            if (x > _blit_surface.Width - 1 ||  x < 0 || y > _blit_surface.Height - 1 ||  y < 0)
            {
                return 0;
            }

            return _current_blit_surface.Colors[x + y * _current_blit_surface.Width];
        }

        public void HLine(int sx, int ex, int y)
        {
            var minX = _clip_rect.Left + _camera_x;
            var maxX = _clip_rect.Right + _camera_x;

            if (y < _clip_rect.Top || y > _clip_rect.Bottom)
            {
                return;
            }

            _blit_surface_dirty = true;

            if (sx < minX && ex < minX)
            {
                return;
            }
    
            if (sx > maxX && ex > maxX) 
            {
                return;
            }

            if (ex < sx)
            {
                (sx, ex) = (ex, sx);
            }

            int sw = _current_blit_surface.Width;

            fixed (int* ptr = &MemoryMarshal.GetArrayDataReference(_current_blit_surface.Colors))
            {
                for (int x = sx; x < ex; ++x)
                {
                    if (DitherPass(x, y))
                    {
                        Unsafe.WriteUnaligned(ptr + x + y * sw, _color);
                    }
                    else if(_dither_color != Palette.NullColor)
                    {
                        Unsafe.WriteUnaligned(ptr + x + y * sw, _dither_color);
                    }
                }
            }
        }

        public void VLine(int sy, int ey, int x)
        {
            if (x < _clip_rect.Left || x > _clip_rect.Right)
            {
                return;
            }

            _blit_surface_dirty = true;

            var minY = _clip_rect.Top + _camera_y;
            var maxY = _clip_rect.Bottom + _camera_y;
            if (sy < minY && ey < minY) return;
            if (sy > maxY && ey > maxY) return;

            if (ey < sy)
            {
                (sy, ey) = (ey, sy);
            }

            int sw = _current_blit_surface.Width;

            fixed (int* ptr = &MemoryMarshal.GetArrayDataReference(_current_blit_surface.Colors))
            {
                for (int y = sy; y < ey; ++y)
                {
                    if (y < _clip_rect.Top || y > _clip_rect.Bottom)
                    {
                        continue;
                    }

                    if (DitherPass(x, y))
                    {
                        Unsafe.WriteUnaligned(ptr + x + y * sw, _color);
                    }
                    else if (_dither_color != Palette.NullColor)
                    {
                        Unsafe.WriteUnaligned(ptr + x + y * sw, _dither_color);
                    }
                }
            }
        }

        public void RectFill(int x, int y, int w, int h)
        {
            _blit_surface_dirty = true;

            int left = Math.Max(x, _clip_rect.Left);
            int right = Math.Min(x + w, _clip_rect.Right);
            int top = Math.Max(y, _clip_rect.Top);
            int bottom = Math.Min(y + h, _clip_rect.Bottom);
            int sw = _current_blit_surface.Width;

            fixed (int* ptr = &MemoryMarshal.GetArrayDataReference(_current_blit_surface.Colors))
            {
                for (int i = left; i < right; ++i)
                {
                    for (int j = top; j < bottom; ++j)
                    {
                        Unsafe.WriteUnaligned(ptr + i + j * sw, _color);
                    }
                }
            }
        }

        public void RectDashed(
            int x, 
            int y, 
            int w, 
            int h, 
            int line_size, 
            int dash_offset, 
            int dash_size)
        {
            _blit_surface_dirty = true;
            
            dash_size = Math.Max(1, dash_size);

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

            int paint_color;

            if (y > _clip_rect.Y)
            {
                var max_y = Math.Min(y, clip_bottom);

                for (var i = min_x1; i < max_x1; ++i)
                {
                    paint_color = (i - dash_offset) % dash_size == 0 ? Palette.NullColor : _color;

                    for (var j = min_y1; j < max_y; ++j)
                    {
                        var idx = i + j * sw;
                        colors[idx] = paint_color;
                    }
                }
            }

            // Bottom Side

            if (y + h < clip_bottom)
            {
                var min_y = Math.Max(y + h, _clip_rect.Top);

                for (var i = min_x1; i < max_x1; ++i)
                {
                    paint_color = (i + dash_offset) % dash_size == 0 ? Palette.NullColor : _color;

                    for (var j = min_y; j < max_y1; ++j)
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

                for (var i = min_x; i < max_x; ++i)
                {
                    for (var j = min_y1; j < max_y1; ++j)
                    {
                        paint_color = (j + dash_offset) % dash_size == 0 ? Palette.NullColor : _color;

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

                for (var i = min_x; i < max_x; ++i)
                {
                    for (var j = min_y1; j < max_y1; ++j)
                    {
                        paint_color = (j - dash_offset) % dash_size == 0 ? Palette.NullColor : _color;

                        var idx = i + j * sw;
                        colors[idx] = paint_color;
                    }
                }
            }
        }

        public void Rect(int x, int y, int w, int h, int line_size = 1)
        {
            _blit_surface_dirty = true;

            if (line_size < 1)
            {
                line_size = 1;
            }

            if (line_size == 1)
            {
                HLine(x - 1, x + w, y - 1); // Top
                HLine(x - 1, x + w, y + h); // Down
                VLine(y, y + h, x - 1); // Left
                VLine(y - 1, y + h + 1, x + w); // Right

            }
            else
            {
                RectFill(x - line_size , y - line_size, w + line_size, line_size); // Top 
                RectFill(x, y + h, w + line_size, line_size); // Down
                RectFill(x - line_size, y, line_size, h + line_size); // Left
                RectFill(x + w, y - line_size, line_size, h + line_size); // Right
            }
        }

        public void Line(int x0, int y0, int x1, int y1, int line_size = 1)
        {
            _blit_surface_dirty = true;

            void OnePxLine()
            {
                int dx = Math.Abs(x1 - x0), sx = x0 < x1 ? 1 : -1;
                int dy = -Math.Abs(y1 - y0), sy = y0 < y1 ? 1 : -1;
                int err = dx + dy;

                while (true)
                {  
                    PixelSet(x0, y0);
                    if (x0 == x1 && y0 == y1) break;
                    var e2 = 2 * err;
                    if (e2 >= dy) { err += dy; x0 += sx; }
                    if (e2 <= dx) { err += dx; y0 += sy; }
                }
            }

            void ThickLine()
            {
                int dx = Math.Abs(x1 - x0), sx = x0 < x1 ? 1 : -1;
                int dy = Math.Abs(y1 - y0), sy = y0 < y1 ? 1 : -1;
                int err = dx - dy, e2, x2, y2;
                float ed = (float)(dx + dy == 0 ? 1 : Math.Sqrt((float)dx * dx + (float)dy * dy));

                for (line_size = (line_size + 1) / 2; ;)
                {
                    //setPixelColor(x0, y0, max(0, 255 * (abs(err - dx + dy) / ed - wd + 1)));
                    PixelSet(x0, y0);
                    e2 = err; x2 = x0;
                    if (2 * e2 >= -dx)
                    {                                           
                        for (e2 += dy, y2 = y0; e2 < ed * line_size && (y1 != y2 || dx > dy); e2 += dx)
                            //setPixelColor(x0, y2 += sy, max(0, 255 * (abs(e2) / ed - wd + 1)));
                            PixelSet(x0, y2);
                        if (x0 == x1) break;
                        e2 = err; err -= dy; x0 += sx;
                    }
                    if (2 * e2 <= dy)
                    {                                            
                        for (e2 = dx - e2; e2 < ed * line_size && (x1 != x2 || dx < dy); e2 += dy)
                            //setPixelColor(x2 += sx, y0, max(0, 255 * (abs(e2) / ed - wd + 1)));
                            PixelSet(x2 += sx, y0);
                        if (y0 == y1) break;
                        err += dx; y0 += sy;
                    }
                }
            }

            if (line_size == 1)
            {
                OnePxLine();
                return;
            }

            ThickLine();
            
        }

        public void Circle(int center_x, int center_y, int radius)
        {
            _blit_surface_dirty = true;

            if (radius > 0)
            {
                int x = -radius, y = 0, err = 2 - 2 * radius;
                do
                {
                    PixelSet(center_x - x, center_y + y);
                    PixelSet(center_x - y, center_y - x);
                    PixelSet(center_x + x, center_y - y);
                    PixelSet(center_x + y, center_y + x);

                    radius = err;
                    if (radius <= y) err += ++y * 2 + 1; 
                    if (radius > x || err > y) err += ++x * 2 + 1;
                } while (x < 0);
            }
            else
            {
                PixelSet(center_x, center_y);
            }
        }

        public void CircleFill(int center_x, int center_y, int radius)
        {
            if (radius < 0 || center_x < -radius || center_y < -radius || center_x - _clip_rect.W > radius || center_y - _clip_rect.H > radius  )
            {
                return;
            }

            _blit_surface_dirty = true;

            if (radius > 0)
            {
                int x0 = 0;
                int y0 = radius;
                int d = 3 - 2 * radius;

                while(y0 >= x0)
                {
                    HLine(center_x - y0, center_x + y0, center_y - x0);

                    if (x0 > 0)
                    {
                        HLine(center_x - y0, center_x + y0, center_y + x0);
                    }

                    if (d < 0)
                    {
                        d += 4 * x0++ + 6;
                    }
                    else
                    {
                        if (x0 != y0)
                        {
                            HLine(center_x - x0, center_x + x0, center_y - y0);
                            HLine(center_x - x0, center_x + x0, center_y + y0);
                        }
                        d += 4 * (x0++ - y0--) + 10;
                    }
                }
            }
            else
            {
                PixelSet(center_x, center_y);
            }
        }

        public void Triangle(int x1, int y1, int x2, int y2, int x3, int y3, int line_size = 1)
        {
            Line(x1, y1, x2, y2, line_size);
            Line(x2, y2, x3, y3, line_size);
            Line(x3, y3, x1, y1, line_size);
        }

        public void Pixmap(Pixmap pixmap,
         int x,
         int y,
         Rect src_rect,
         float width = -1,
         float height = -1,
         bool flip = false)
        {
            _blit_surface_dirty = true;

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
                        var pix_idx = ((src_rect.X + (int)((i - x) / factor_w)) + (src_rect.Y + (int)((j - y) / factor_h)) * pw);
                        var pix_color = pixmap_colors[pix_idx];

                        if (pix_color == Palette.NullColor)
                        {
                            continue;
                        }

                        if (_tint_color != Palette.NullColor)
                        {
                            pix_color = _tint_color;
                        }

                        surface_colors[surf_idx] = pix_color;
                    }
                }
            }
            else
            {
                var start_pix_x = src_rect.Right - 1;
                for (var i = min_x; i < max_x; ++i)
                {
                    for (var j = min_y; j < max_y; ++j)
                    {
                        var surf_idx = (i + j * sw);
                        var pix_idx = (start_pix_x - (int)((i - x) / factor_w) + (src_rect.Y + (int)((j - y) / factor_h)) * pw);
                        var pix_color = pixmap_colors[pix_idx];

                        if (pix_color == Palette.NullColor)
                        {
                            continue;
                        }

                        if (_tint_color != Palette.NullColor)
                        {
                            pix_color = _tint_color;
                        }

                        surface_colors[surf_idx] = pix_color;
                    }
                }
            }
        }

        /// <summary>
        /// Draws a sprite from a SpriteSheet
        /// </summary>
        /// <param name="id">SpriteSheet cell id to draw</param>
        /// <param name="x">X position on screen</param>
        /// <param name="y">Y position on screen</param>
        /// <param name="scale">Scale to draw</param>
        /// <param name="flip">Flip horizontally</param>
        /// <param name="width">How many cells to draw horizontally. By default just the cell passed on 'id', that is width=1</param>
        /// <param name="height">How many cells to draw vertically. By default just the cell passed on 'id', that is height=1</param>
        public void Sprite(
            int id,
            int x,
            int y,
            float scale = 1,
            bool flip = false,
            int width = 1,
            int height = 1
            )
        {
            Rect src_rect;

            var sprite_sheet = _current_sprite_sheet;

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
                width: src_rect.W * scale,
                height: src_rect.H * scale,
                flip
            );
        }

        public void Text(int x, int y, string text, int scale = 1)
        {
            _blit_surface_dirty = true;

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
                    (glyph_index % count_glyphs) * glyph_size,
                    (glyph_index / count_glyphs) * glyph_size,
                    glyph_size,
                    glyph_size
                );

                for (var i = min_x; i < max_x; ++i)
                {
                    for (var j = min_y; j < max_y; ++j)
                    {
                        var surf_idx = (i + j * sw);
                        var pix_idx = ((src_rect.X + ((i - (x + ci * scaled_glyph_size)) / scale)) + (src_rect.Y + ((j - y) / scale)) * pw);
                        var pix_color = font_colors[pix_idx];

                        if (pix_color == Palette.NullColor)
                        {
                            continue;
                        }

                        pix_color = _color;

                        surface_colors[surf_idx] = pix_color;
                    }
                }
            }

        }

        public static (int Width, int Height) TextMeasure(string text, int scale = 1)
        {
            return (text.Length * 8 * scale, 8 * scale); //TODO
        }

        private void FlushBlitSurface()
        {
            Console.WriteLine("Flush");

            var rgba_render_surface = _render_surface.DataPtr;

            var p = (byte*)rgba_render_surface;

            var rgb_surface_idx = 0;
            var color_idxs = _current_blit_surface.Colors;
            var length = color_idxs.Length;


            fixed (int* c_pointer = &color_idxs[0])
            {
                for (int i = 0; i < length; ++i)
                {
                    int color_idx = *(c_pointer + i);

                    if (color_idx != Palette.NullColor)
                    {
                        ref var rgb_color = ref Palette.MapIndexToColor(color_idx);
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
