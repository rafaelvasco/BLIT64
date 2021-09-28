using System;
using System.Collections.Generic;

namespace BLIT64
{
    public unsafe partial class Canvas
    {
        public void FloodFill(int x, int y, int color)
        {
            _blit_surface_dirty = true;

            var sw = _current_blit_surface.Width;
            var colors = _current_blit_surface.Colors;
            var clip_rect = _clip_rect;

            var target_color = colors[x + y * sw];

            var color_queue = new Queue<(int, int)>();

            color_queue.Enqueue((x, y));

            bool ColorMatch(int _x, int _y)
            {
                return colors[_x + _y * sw] == target_color;
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
            _blit_surface_dirty = true;

            var colors = _current_blit_surface.Colors;
            var sw = _current_blit_surface.Width;
            var row = new int[src_w];

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
            _blit_surface_dirty = true;

            var colors = _current_blit_surface.Colors;
            var sh = _current_blit_surface.Height;
            var col = new int[src_w * src_h];

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

        public void Rotate90(int src_x, int src_y, int src_w, int src_h, int target_x = 0, int target_y = 0,
            int target_w = 0, int target_h = 0)
        {
            _blit_surface_dirty = true;

            if (target_w == 0 || target_h == 0)
            {
                target_x = src_x;
                target_y = src_y;
                target_w = src_w;
                target_h = src_h;
            }

            var colors = _current_blit_surface.Colors;
            var sw = _current_blit_surface.Width;
            var aux_colors_buffer = stackalloc int[src_w * src_h];
            var aux_colors = new Span<int>(aux_colors_buffer, src_w * src_h);

            for (int r = 0; r < src_h; ++r)
            {
                for (int c = 0; c < src_w; ++c)
                {
                    aux_colors[(c * src_h) + (src_h - r - 1)] = colors[(r + src_y) * sw + (c + src_x)];
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
    }
}