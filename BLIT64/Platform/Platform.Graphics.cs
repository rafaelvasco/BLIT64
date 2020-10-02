using System;
using System.Runtime.CompilerServices;
using static SDL2.SDL;

namespace BLIT64
{
    internal static partial class Platform
    {
        private static IntPtr _graphics_context;
        private static IntPtr _render_texture;
        private static int _render_surface_width;
        private static int _render_surface_height;

        public static float DisplayScaleFactorX { get; private set; }

        public static float DisplayScaleFactorY { get; private set; }

        private static void InitGraphics(int surface_width, int surface_height)
        {
            _graphics_context = SDL_CreateRenderer(Platform.WindowHandle, -1,
                SDL_RendererFlags.SDL_RENDERER_ACCELERATED | 
                SDL_RendererFlags.SDL_RENDERER_TARGETTEXTURE);

            _render_texture = SDL_CreateTexture(_graphics_context, SDL_PIXELFORMAT_ABGR8888,
                (int) SDL_TextureAccess.SDL_TEXTUREACCESS_STREAMING, surface_width, surface_height);

            var (display_width, display_height) = Platform.GetWindowSize();

            _render_surface_width = surface_width;

            _render_surface_height = surface_height;

            DisplayScaleFactorX = (float) display_width / surface_width;

            DisplayScaleFactorY = (float) display_height / surface_height;

            SDL_SetRenderDrawBlendMode(_graphics_context, SDL_BlendMode.SDL_BLENDMODE_NONE);
        }

        private static void UpdateDisplayScaleFactor()
        {
            var (display_width, display_height) = Platform.GetWindowSize();

            DisplayScaleFactorX = (float) display_width / _render_surface_width;

            DisplayScaleFactorY = (float) display_height / _render_surface_height;
        }

        public static void PresentPixmap(DrawSurface draw_surface)
        {
            SDL_LockTexture(_render_texture, IntPtr.Zero, out var pixels, out var pitch);
            unsafe
            {
                Unsafe.CopyBlock((void*) pixels, (void*) draw_surface.DataPtr, draw_surface.ByteCount);
            }
            SDL_UnlockTexture(_render_texture);
            SDL_RenderCopy(_graphics_context, _render_texture, IntPtr.Zero, IntPtr.Zero);

            SDL_RenderPresent(_graphics_context);
        }

        private static void ShutdownGraphics()
        {
            SDL_DestroyTexture(_render_texture);
            SDL_DestroyRenderer(_graphics_context);
        }
    }
}
