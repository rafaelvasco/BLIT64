using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace BLIT64
{
    public unsafe class Canvas
    {
        private const int RENDER_STACK_SIZE = 10;

        public int Width { get; private set;}
        public int Height { get; private set; }

        public Palette BasePalette => _base_palette;
        public Palette[] AvailablePalettes => _available_palettes;

        private RenderSurface _render_surface;
        private readonly Palette[] _available_palettes;
        private readonly Palette _base_palette;
        private readonly int[] _palette_stack;
        private int _palette_stack_index;

        //private readonly RenderSurface[] _render_surface_stack;
        //private int _render_surface_stack_index;
        //private int _render_surface_stack_top_index;

        private readonly Pixmap _blit_surface;
        private Font _default_font;
        private Font _current_font;
        private SpriteSheet _current_sprite_sheet;
        private byte _current_color = Palette.WhiteColor;
        private Pixmap _current_blit_surface;
        private Rect _clip_rect;
        private ushort _dither_pattern = 0b1111_1111_1111_1111;
        private byte _dither_color = Palette.NoColor;
        private int _camera_x;
        private int _camera_y;
        private bool _blit_surface_dirty = true;
        


        internal Canvas(int width, int height)
        {
            Width = width;

            Height = height;

            _available_palettes = new Palette[]
            {
                Palettes.Journey,
                Palettes.Famicube
            };

            _base_palette = Palettes.Journey;
            _blit_surface = Assets.CreatePixmap(width, height);
            _current_blit_surface = _blit_surface;
            _clip_rect = new Rect(0, 0, width, height);
            //_render_surface_stack = new RenderSurface[RENDER_STACK_SIZE];
            _render_surface = Assets.CreateRenderSurface(0, width, height);
            _palette_stack = new int[RENDER_STACK_SIZE];
        }

        //private int PushRenderSurface()
        //{
        //    int index = _render_surface_stack_index;
        //    _render_surface_stack[_render_surface_stack_index] = Assets.CreateRenderSurface(_render_surface_stack_index, Width, Height);

        //    ++_render_surface_stack_index;

        //    return index;
        //}

        internal void LoadDefaultAssets()
        {
            _default_font = Assets.GetEmbedded<Font>("default_font");
            _current_font = _default_font;
        }

        public void BeginDraw(int palette_index = 0)
        {
            //if (_render_surface_stack_index >= RENDER_STACK_SIZE)
            //{
            //    throw new Exception("Canvas Render Stack Size Exceeded");
            //}

            palette_index = Calc.Clamp(palette_index, 0, _available_palettes.Length - 1);

            _palette_stack[_palette_stack_index++] = palette_index;

            //if (_render_surface_stack[_render_surface_stack_index] == null)
            //{
            //    PushRenderSurface();
            //}
            //else
            //{
            //    _render_surface_stack_index ++;
            //}

            //if (_render_surface_stack_index > _render_surface_stack_top_index)
            //{
            //    _render_surface_stack_top_index = _render_surface_stack_index;
            //}
        }

        public void EndDraw()
        {
            //if (_render_surface_stack_index == 0)
            //{
            //    return;
            //}

            //var render_surface = _render_surface_stack[--_render_surface_stack_index];

            --_palette_stack_index;

            if (_blit_surface_dirty)
            {
                UpdateRenderSurface(_render_surface);
            }
        }

        internal void Present()
        {
            //for (int i = 0; i < _render_surface_stack_top_index; ++i)
            //{
            //    var render_surface = _render_surface_stack[i];

            //    Platform.PresentDrawSurface(render_surface);
            //}

            //_render_surface_stack_index = 0;
            //_render_surface_stack_top_index = 0;

            Platform.PresentDrawSurface(_render_surface);

            _palette_stack_index = 0;
        }

        public void SetColor(byte color)
        {
            _current_color = color;
        }

        public byte GetColor()
        {
            return _current_color;
        }

        public void SetSpriteSheet(SpriteSheet sprite_sheet)
        {
            _current_sprite_sheet = sprite_sheet;
        }

        #region CAMERA

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

        #endregion


        #region PIXEL DRAWING

        public void Clear()
        {
            _blit_surface_dirty = true;

            int length = _current_blit_surface.Colors.Length;
            int left = _clip_rect.Left;
            int right = _clip_rect.Right;
            int top = _clip_rect.Top;
            int bottom = _clip_rect.Bottom;
            int sw = _current_blit_surface.Width;

            fixed (byte* ptr = &MemoryMarshal.GetArrayDataReference(_current_blit_surface.Colors))
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

        public void SetSurface(Pixmap pixmap)
        {
            _current_blit_surface = pixmap ?? _blit_surface;
            _clip_rect = new Rect(0, 0, _current_blit_surface.Width, _current_blit_surface.Height);
        }

        public void DitherPattern(ushort pattern = 0b1111_1111_1111_1111)
        {
            _dither_pattern = pattern;
        }

        public void DitherColor(byte color = Palette.NoColor)
        {
            _dither_color = color;
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PixelSet(int x, int y)
        {
            x -= _camera_x;
            y -= _camera_y;

            if (x < _clip_rect.Left || x > _clip_rect.Right || y < _clip_rect.Top || y > _clip_rect.Bottom)
            {
                return;
            }

            _blit_surface_dirty = true;

            _current_blit_surface.Colors[x + y * _current_blit_surface.Width] = _current_color;
        }

        public byte PixelGet(int x, int y)
        {
            x -= _camera_x;
            y -= _camera_y;

            if (x > _blit_surface.Width - 1 ||  x < 0 || y > _blit_surface.Height - 1 ||  y < 0)
            {
                return 0;
            }

            return _current_blit_surface.Colors[x + y * _current_blit_surface.Width];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void HLine(int sx, int ex, int y)
        {
            var minX = _clip_rect.Left + _camera_x;
            var maxX = _clip_rect.Right + _camera_x;

            if (y < _clip_rect.Top || y > _clip_rect.Bottom)
            {
                return;
            }

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

            fixed (byte* ptr = &MemoryMarshal.GetArrayDataReference(_current_blit_surface.Colors))
            {
                for (int x = sx; x < ex; ++x)
                {
                    if (DitherPass(x, y))
                    {
                        Unsafe.WriteUnaligned(ptr + x + y * sw, _current_color);
                    }
                    else if(_dither_color != Palette.NoColor)
                    {
                        Unsafe.WriteUnaligned(ptr + x + y * sw, _dither_color);
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void VLine(int sy, int ey, int x)
        {
            if (x < _clip_rect.Left || x > _clip_rect.Right)
            {
                return;
            }

            var minY = _clip_rect.Top + _camera_y;
            var maxY = _clip_rect.Bottom + _camera_y;
            if (sy < minY && ey < minY) return;
            if (sy > maxY && ey > maxY) return;

            if (ey < sy)
            {
                (sy, ey) = (ey, sy);
            }

            int sw = _current_blit_surface.Width;

            fixed (byte* ptr = &MemoryMarshal.GetArrayDataReference(_current_blit_surface.Colors))
            {
                for (int y = sy; y < ey; ++y)
                {
                    if (y < _clip_rect.Top || y > _clip_rect.Bottom)
                    {
                        continue;
                    }

                    if (DitherPass(x, y))
                    {
                        Unsafe.WriteUnaligned(ptr + x + y * sw, _current_color);
                    }
                    else if (_dither_color != Palette.NoColor)
                    {
                        Unsafe.WriteUnaligned(ptr + x + y * sw, _dither_color);
                    }
                }
            }
        }

        public void RectFill(int x, int y, int w, int h)
        {
            _blit_surface_dirty = true;

            int length = _current_blit_surface.Colors.Length;
            int left = Math.Max(x, _clip_rect.Left);
            int right = Math.Min(x + w, _clip_rect.Right);
            int top = Math.Max(y, _clip_rect.Top);
            int bottom = Math.Min(y + h, _clip_rect.Bottom);
            int sw = _current_blit_surface.Width;

            fixed (byte* ptr = &MemoryMarshal.GetArrayDataReference(_current_blit_surface.Colors))
            {
                for (int i = left; i < right; ++i)
                {
                    for (int j = top; j < bottom; ++j)
                    {
                        Unsafe.WriteUnaligned(ptr + i + j * sw, _current_color);
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
            dash_size = Math.Max(1, dash_size);

            _blit_surface_dirty = true;

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
                    paint_color = (i - dash_offset) % dash_size == 0 ? Palette.TransparentColor : _current_color;

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
                    paint_color = (i + dash_offset) % dash_size == 0 ? Palette.TransparentColor : _current_color;

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
                        paint_color = (j + dash_offset) % dash_size == 0 ? Palette.TransparentColor : _current_color;

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
                        paint_color = (j - dash_offset) % dash_size == 0 ? Palette.TransparentColor : _current_color;

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
                int err = dx + dy, e2;

                while (true)
                {  
                    PixelSet(x0, y0);
                    if (x0 == x1 && y0 == y1) break;
                    e2 = 2 * err;
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
         byte color_key = Palette.TransparentColor,
         byte tint = Palette.NoColor,
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
                var start_pix_x = src_rect.Right - 1;
                for (var i = min_x; i < max_x; ++i)
                {
                    for (var j = min_y; j < max_y; ++j)
                    {
                        var surf_idx = (i + j * sw);
                        var pix_idx = (start_pix_x - (int)((i - x) / factor_w) + (src_rect.Y + (int)((j - y) / factor_h)) * pw);
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
            int id,
            int x,
            int y,
            byte color_key = Palette.TransparentColor,
            byte tint = Palette.NoColor,
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
                color_key,
                tint,
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

                        if (pix_color == 0)
                        {
                            continue;
                        }

                        pix_color = _current_color;

                        surface_colors[surf_idx] = pix_color;
                    }
                }
            }

        }

        #endregion

        #region PIXEL TRANSFORMATION

        public void FloodFill(int x, int y, byte color)
        {
            _blit_surface_dirty = true;

            var sw = _current_blit_surface.Width;
            var colors = _current_blit_surface.Colors;
            var clip_rect = _clip_rect;

            var target_color = colors[x + y * sw];

            var color_queue = new Queue<(int, int)>();

            color_queue.Enqueue((x, y));

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
                        color_queue.Enqueue((west_x, west_y - 1));
                    }

                    if ((west_y < max_y - 1) && ColorMatch(west_x, west_y + 1))
                    {
                        color_queue.Enqueue((west_x, west_y + 1));
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


        public unsafe void Rotate90(int src_x, int src_y, int src_w, int src_h, int target_x = 0, int target_y = 0, int target_w = 0, int target_h = 0)
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

            for (int r = 0; r < rows; ++r)
            {
                for (int c = 0; c < cols; ++c)
                {
                    aux_colors[(c * rows) + (rows - r - 1)] = colors[(r + src_y) * sw + (c + src_x)];
                }
            }

            for (int j = target_y, c = 0; j < target_y + target_h; ++j)
            {
                for (int i = target_x; i < target_x + target_w; ++i)
                {
                    colors[i + j * sw] = aux_colors[c++];
                }
            }
        }

        #endregion

        #region UTILS
        public static (int Width, int Height) TextMeasure(string text, int scale = 1)
        {
            return (text.Length * 8 * scale, 8 * scale); //TODO
        }
        #endregion

        #region PRIVATE
        private void UpdateRenderSurface(RenderSurface surface)
        {

            var rgba_surface = surface.DataPtr;

            unsafe
            {
                var p = (byte*)rgba_surface;
                var rgb_surface_idx = 0;
                var color_idxs = _current_blit_surface.Colors;
                var length = color_idxs.Length;

                for (int i = 0; i < length; ++i)
                {
                    var color_idx = color_idxs[i];

                    if (color_idx > 0)
                    {
                        var palette = _available_palettes[_palette_stack[_palette_stack_index]];
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
        #endregion

    }
}
