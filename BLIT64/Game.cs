using System;
using System.Runtime;
using static SDL2.SDL;

namespace BLIT64
{
    public sealed class Game : IDisposable
    {
        public Palette CurrentPalette => _current_palette;

        private bool _running;
        private readonly RenderSurface _render_surface;
        private readonly Blitter _blitter;
        private Palette _current_palette;
        private static Game _instance;

        public double FrameRate { get; set; } = 60;

        public Scene CurrentScene { get; private set; }

        public static Game Instance => _instance;

        public (int Width, int Height) WindowSize
        {
            get => Platform.GetWindowSize();
            set
            {
                var current_size = Platform.GetWindowSize();
                if (value.Width == current_size.width && value.Height == current_size.height)
                {
                    return;
                }

                Platform.SetWindowSize(value.Width, value.Height);
            }
        }

        public int Width => _render_surface.Width;

        public int Height => _render_surface.Height;

        public (int Width, int Height) Size => (_render_surface.Width, _render_surface.Height);

        public string Title
        {
            get => Platform.GetWindowTitle();
            set => Platform.SetWindowTitle(value);
        }

        public bool Fullscreen
        {
            get => Platform.IsFullscreen();
            set
            {
                if (Platform.IsFullscreen() == value)
                {
                    return;
                }

                Platform.SetWindowFullscreen(value);
            }
        }

        public bool ShowCursor
        {
            get => Platform.CursorVisible();
            set => Platform.ShowCursor(value);
        }

        public Game(
            string title, 
            int display_width, 
            int display_height, 
            int render_surface_width = 0, 
            int render_surface_height = 0, 
            bool fullscreen = false)
        {

            if (render_surface_width == 0)
            {
                render_surface_width = display_width;
            }

            if (render_surface_height == 0)
            {
                render_surface_height = display_height;
            }
            _instance = this;
            
            Platform.Init(title, display_width, display_height, render_surface_width, render_surface_height, fullscreen);
            Platform.OnQuit += OnClose;
            Platform.WindowResized += OnWindowResize;

            _current_palette = Palettes.Journey;

            Assets.LoadEmbeddedAssets();

            _render_surface = Assets.CreateRenderSurface(render_surface_width, render_surface_height);
            
            _blitter = new Blitter(_render_surface);

            Input.Init();

            CurrentScene = new EmptyScene();

            GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;
        }

        public double Time()
        {
            return (SDL_GetPerformanceCounter() * 1000.0) / SDL_GetPerformanceFrequency();
        }

        public void Run(Scene scene = null)
        {
            Console.WriteLine("BLIT64 Started");

            if (_running)
            {
                return;
            }

            if (scene != null)
            {
                scene.Game = this;
                scene.Blitter = _blitter;
                CurrentScene = scene;
            }


            Platform.PresentPixmap(_render_surface);
            Platform.ShowWindow(true);

            Tick();
        }

        public void SetPalette(Palette palette)
        {
            _current_palette = palette;
        }

        public void Exit()
        {
            _running = false;
        }

        public void ToggleFullscreen()
        {
            Fullscreen = !Fullscreen;
        }

        private void Tick()
        {
            _running = true;

            var next_tick = (double)SDL_GetPerformanceCounter();
            var delta = SDL_GetPerformanceFrequency() / FrameRate; 

            CurrentScene.Load();

            while (_running)
            {
                Platform.ProcessEvents();

                Input.Update();

                if (Input.KeyPressed(Key.F11))
                {
                    ToggleFullscreen();
                }

#if DEBUG
                if (Input.KeyPressed(Key.Escape))
                {
                    Exit();
                }
#endif

                next_tick += delta;

                CurrentScene.Update();

                CurrentScene.Draw(_blitter);

                if (_blitter.NeedsUpdate)
                {
                    _blitter.UpdateRenderSurface(_current_palette);
                }

                Platform.PresentPixmap(_render_surface);

                var delay = next_tick - SDL_GetPerformanceCounter();

                if (delay < 0)
                {
                    next_tick -= delay;
                }
                else
                {
                    SDL_Delay((uint) (delay * 1000 / SDL_GetPerformanceFrequency()));
                }
            }

        }

        private void OnWindowResize((int width, int height) size)
        {

        }

        private void OnClose()
        {
            Exit();
        }

        private void ReleaseResources()
        {
            Assets.Release();
            Platform.Shutdown();
        }

        public void Dispose()
        {
            ReleaseResources();
        }
    }
}
