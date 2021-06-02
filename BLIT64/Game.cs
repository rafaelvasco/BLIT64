using System;
using System.Runtime;

namespace BLIT64
{
    public sealed class Game : IDisposable
    {
        public Canvas Canvas { get; private set; }

        private bool _running;
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
            int pixel_size = 1,
            bool fullscreen = false)
        {

            if (pixel_size < 1)
            {
                pixel_size = 1;
            }

            _instance = this;

            Platform.Init(title, display_width, display_height, pixel_size, fullscreen);
            Platform.OnQuit += OnClose;
            Platform.WindowResized += OnWindowResize;

            Canvas = new Canvas(display_width / pixel_size, display_height / pixel_size);

            Assets.LoadEmbeddedAssetsPak();

            Assets.LoadMainAssetsPak();

            Canvas.LoadDefaultAssets();

            Input.Init();

            CurrentScene = new EmptyScene();

            GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;

            Scene.Game = this;
            Scene.Canvas = Canvas;
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
                CurrentScene = scene;
            }

            Canvas.BeginDraw();

            Canvas.EndDraw();

            Canvas.Present();
            
            Platform.ShowWindow(true);

            Tick();
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

                Canvas.BeginDraw();

                CurrentScene.Draw(Canvas);

                Canvas.EndDraw();

                Canvas.Present();

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
