using System;
using System.Runtime;

namespace BLIT64
{
    public sealed class Game : IDisposable
    {
        public Palette CurrentPalette { get; private set; }

        public Blitter Blitter { get; private set; }

        private bool _running;
        private readonly DrawSurface _draw_surface;
        private static Game _instance;

        public double FrameRate { get; set; } = 60;

        public int TileSize { get; } = 8;

        public Scene CurrentScene { get; private set; }

        public static Game Instance => _instance;

        public (int Width, int Height) WindowSize
        {
            get => Platform.GetWindowSize();
            set
            {
                var (width, height) = Platform.GetWindowSize();
                if (value.Width == width && value.Height == height)
                {
                    return;
                }

                Platform.SetWindowSize(value.Width, value.Height);
            }
        }

        public int Width => _draw_surface.Width;

        public int Height => _draw_surface.Height;

        public (int Width, int Height) Size => (_draw_surface.Width, _draw_surface.Height);

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
            CurrentPalette = Palettes.Journey;
            
            Platform.Init(title, display_width, display_height, render_surface_width, render_surface_height, fullscreen);
            Platform.OnQuit += OnClose;
            Platform.WindowResized += OnWindowResize;

            Assets.LoadEmbeddedAssetsPak();

            Assets.LoadMainAssetsPak();

            _draw_surface = Assets.CreateRenderSurface(render_surface_width, render_surface_height);
            
            Blitter = new Blitter(_draw_surface);

            Input.Init();

            CurrentScene = new EmptyScene();

            GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;
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
                scene.Blitter = Blitter;
                CurrentScene = scene;
            }


            Platform.PresentPixmap(_draw_surface);
            Platform.ShowWindow(true);

            Tick();
        }

        public void SetPalette(Palette palette)
        {
            CurrentPalette = palette;
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

            var next_tick = (double)Platform.GetPerformanceCounter();
            var delta = Platform.GetPerformanceFrequency() / FrameRate; 

            CurrentScene.Load();

            while (_running)
            {
                Platform.ProcessEvents();

                Input.Update();

                if (Input.KeyPressed(Key.F11))
                {
                    ToggleFullscreen();
                }

                next_tick += delta;

                CurrentScene.Update();

                CurrentScene.Draw(Blitter);

                if (Blitter.NeedsUpdate)
                {
                    Blitter.UpdateDrawSurface(CurrentPalette);
                }

                Platform.PresentPixmap(_draw_surface);

                var delay = next_tick - Platform.GetPerformanceCounter();

                if (delay < 0)
                {
                    next_tick -= delay;
                }
                else
                {
                    Platform.Delay((uint) (delay * 1000 / Platform.GetPerformanceFrequency()));
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
