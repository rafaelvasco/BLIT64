using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using BLIT64_Common;

namespace BLIT64
{
    public static class Assets
    {
        private static readonly Dictionary<string, GameAsset> _game_assets = new Dictionary<string, GameAsset>();
        private static readonly Dictionary<string, GameAsset> _embedded_assets = new Dictionary<string, GameAsset>();
        private static readonly List<GameAsset> _runtime_assets = new List<GameAsset>();
        
        public static string RootPath = "Assets";
        private const string EmbeddedAssetsNamespace = "BLIT64.Embedded.";
        private const string CommonAssetsFileName = "CommonAssets.pak";


        internal static void LoadEmbeddedAssetsPak()
        {
            using var file_stream = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream(EmbeddedAssetsNamespace + CommonAssetsFileName);

            if (file_stream != null)
            {
                var pak = AssetsPakLoader.LoadPak(file_stream);

                LoadAssetsFromPak(_embedded_assets, pak);
            }
        }

        internal static async void LoadMainAssetsPak()
        {
            await using var file_stream = File.OpenRead(Path.Combine(RootPath, "Main.pak"));

            if (file_stream != null)
            {
                var pak = AssetsPakLoader.LoadPak(file_stream);

                LoadAssetsFromPak(_game_assets, pak);
            }
        }

        private static void LoadAssetsFromPak(Dictionary<string, GameAsset> target, AssetsPak pak)
        {
            foreach (var font_data in pak.Fonts)
            {
                var font = LoadFont(font_data);

                target.Add(font.Id, font);
            }

            foreach (var sprite_sheet_data in pak.SpriteSheets)
            {
                var sheet = LoadSpriteSheet(sprite_sheet_data);

                target.Add(sheet.Id, sheet);
            }

        }

        public static T GetEmbedded<T>(string id) where T : GameAsset
        {
            if (_embedded_assets.TryGetValue(id, out var asset))
            {
                return (T) asset;
            }

            return null;
        }

        public static T Get<T>(string id) where T : GameAsset
        {
            if (_game_assets.TryGetValue(id, out var asset))
            {
                return (T) asset;
            }

            throw new Exception($"Asset with Id {id} is not Loaded" );
        }
        

        public static Pixmap CreatePixmap(int width, int height)
        {
            var pixmap = new Pixmap(width, height);
            _runtime_assets.Add(pixmap);
            return pixmap;
        }

        public static SpriteSheet CreateSpriteSheet(int width, int height)
        {
            var sprite_sheet = new SpriteSheet(width, height, Game.Instance.TileSize);
            _runtime_assets.Add(sprite_sheet);
            return sprite_sheet;
        }

        internal static DrawSurface CreateRenderSurface(int width, int height)
        {
            var render_surface = new DrawSurface(width, height);
            _runtime_assets.Add(render_surface);
            return render_surface;
        }

        private static GameAsset LoadFont(FontData font_data)
        {
            var font = new Font(font_data.ImageData, font_data.ImageWidth, font_data.ImageHeight, font_data.GlyphSize)
            {
                Id = font_data.Id
            };

            return font;
        }

        private static GameAsset LoadSpriteSheet(SpriteSheetData sheet_data)
        {
            var sheet = new SpriteSheet(sheet_data.ImageData, sheet_data.ImageWidth, sheet_data.ImageHeight, sheet_data.CellSize)
            {
                Id = sheet_data.Id
            };

            return sheet;
        }
      
        internal static void Release()
        {
            foreach (var resource in _game_assets)
            {
                resource.Value.Dispose();
            }

            foreach (var runtime_resource in _runtime_assets)
            {
                runtime_resource.Dispose();
            }

            foreach (var embedded_resource in _embedded_assets)
            {
                embedded_resource.Value.Dispose();
            }

            _game_assets.Clear();
        }
    }
}
