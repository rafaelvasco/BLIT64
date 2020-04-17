using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using STB;

namespace BLIT64
{
    public static class Assets
    {
        private static readonly ImageStreamLoader _image_loader = new ImageStreamLoader();
        private static readonly Dictionary<string, Resource> _resources = new Dictionary<string, Resource>();
        private static readonly Dictionary<string, Resource> _embedded_resources = new Dictionary<string, Resource>();
        private static readonly List<Resource> _runtime_resources = new List<Resource>();
        
        public static string RootPath = "Assets";
        private const string EMBEDDED_ASSETS_NAMESPACE = "BLIT64.Embedded.";


        internal static void LoadEmbeddedAssets()
        {
            // Default Font
            using var default_font_stream = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream(EMBEDDED_ASSETS_NAMESPACE + "default_font.png");

            if (default_font_stream != null)
            {
                var font = LoadFont(default_font_stream);
                _embedded_resources.Add("default_font", font);
            }
        }

        public static T GetEmbedded<T>(string name) where T : Resource
        {
            if (_embedded_resources.TryGetValue(name, out var asset))
            {
                return (T) asset;
            }

            return null;
        }

        public static T Get<T>(string asset_file_name) where T : Resource
        {
            if (_resources.TryGetValue(asset_file_name, out var asset))
            {
                return (T) asset;
            }

            var asset_full_path = Path.Combine(RootPath, asset_file_name);

            var asset_type = typeof(T);

            if (asset_type == typeof(Pixmap))
            {
                asset_full_path += ".png";

                try
                {
                    using var stream = File.OpenRead(asset_full_path);

                    if (stream != null)
                    {
                        var pixmap = LoadPixmap(stream);

                        _resources.Add(asset_file_name, pixmap);

                        return (T)pixmap;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                
            }
            else if (asset_type == typeof(Font))
            {
                asset_full_path += ".png";

                try
                {
                    using var stream = File.OpenRead(asset_full_path);

                    if (stream != null)
                    {
                        var font = LoadFont(stream);

                        _resources.Add(asset_file_name, font);

                        return (T)font;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }

            throw new Exception($"Unrecognized Asset Type: {asset_type}");
        }

        public static Pixmap CreatePixmap(int width, int height)
        {
            var pixmap = new Pixmap(width, height);
            _runtime_resources.Add(pixmap);
            return pixmap;
        }

        internal static RenderSurface CreateRenderSurface(int width, int height)
        {
            var render_surface = new RenderSurface(width, height);
            _runtime_resources.Add(render_surface);
            return render_surface;
        }
       
      
        private static Resource LoadPixmap(Stream stream)
        {
            var image = _image_loader.Load(stream, ColorComponents.RedGreenBlueAlpha);
            var pixmap = new Pixmap(image.Data, image.Width, image.Height);
            return pixmap;
        }

        private static Resource LoadFont(Stream stream)
        {
            var image = _image_loader.Load(stream, ColorComponents.RedGreenBlueAlpha);
            var font = new Font(image.Data, image.Width, image.Height, 8); //TODO:
            return font;
        }

        internal static void Release()
        {
            foreach (var resource in _resources)
            {
                resource.Value.Dispose();
            }

            foreach (var runtime_resource in _runtime_resources)
            {
                runtime_resource.Dispose();
            }

            foreach (var embedded_resource in _embedded_resources)
            {
                embedded_resource.Value.Dispose();
            }

            _resources.Clear();
        }
    }
}
