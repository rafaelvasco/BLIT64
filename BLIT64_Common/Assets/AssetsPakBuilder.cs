﻿using System;
using System.Collections.Generic;
using System.IO;
using Binaron.Serializer;
using BLIT64_Common.IO;

namespace BLIT64_Common
{
    public static class AssetsPakBuilder
    {
        public static void Build(string pak_name, string manifest_file_path)
        {
            AssetsPak pak = new AssetsPak();

            BonFile manifest = BonFileReader.Parse(manifest_file_path);

            var output_path = Path.GetDirectoryName(manifest_file_path);

            Console.WriteLine($"Building PAK from manifest file: {manifest_file_path}");

            Console.WriteLine("Parsed BON File:");
            Console.WriteLine(manifest.ToString());

            foreach (var section in manifest.Sections)
            {
                if (section.Key == "Root")
                {
                    continue;
                }

                AssetTypes asset_type;

                Console.WriteLine($"Adding Asset: {section.Key}");

                Console.WriteLine($"Type: {section.Value.ValueProps["type"].GetValue()}");

                try
                {
                    asset_type  = (AssetTypes) Enum.Parse(typeof(AssetTypes), section.Value.ValueProps["type"].GetValue());
                }
                catch (Exception)
                {
                    throw new Exception("Unrecognized 'type' property or 'type' property is missing.");
                }

                if (!section.Value.ValueProps.ContainsKey("file_path"))
                {
                    throw new Exception("Missing 'file_path' property.");
                }
                
                var asset_file_path = section.Value.ValueProps["file_path"].GetValue();

                switch (asset_type)
                {
                    case AssetTypes.Font:

                        var font_image_path =
                            Path.Combine(output_path, asset_file_path);

                        var font_image = ResourceLoader.LoadImage(font_image_path);

                        if (!section.Value.ValueProps.ContainsKey("glyph_size"))
                        {
                            throw new Exception("Missing 'glyph_size' property.");
                        }

                        var glyph_size = section.Value.ValueProps["glyph_size"].GetIntValue();

                        var font_data = new FontData()
                        {
                            GlyphSize = glyph_size,
                            Id = section.Key,
                            ImageData = font_image.ImageData,
                            ImageWidth = font_image.Width,
                            ImageHeight = font_image.Height
                        };

                        pak.AddFont(font_data);

                        break;
                    case AssetTypes.SpriteSheet:

                        var sheet_image_path = Path.Combine(output_path, asset_file_path);
                        var sheet_image = ResourceLoader.LoadImage(sheet_image_path);

                        if (!section.Value.ValueProps.ContainsKey("cell_size"))
                        {
                            throw new Exception("Missing 'cell_size' property.");
                        }

                        var cell_size = section.Value.ValueProps["cell_size"].GetIntValue();

                        Dictionary<string, int> sprite_map = new Dictionary<string, int>();

                        if (section.Value.ListProps.ContainsKey("sprites"))
                        {
                            var sprite_map_pairs = section.Value.ListProps["sprites"].Items;

                            foreach (var sprite_map_pair in sprite_map_pairs)
                            {
                                if (!sprite_map_pair.Contains(':'))
                                {
                                    continue;
                                }

                                var values = sprite_map_pair.Split(':');

                                sprite_map.Add(values[0], int.Parse(values[1]));
                            }
                        }

                        var sheet_data = new SpriteSheetData()
                        {
                            Id = section.Key,
                            CellSize = cell_size,
                            ImageData = sheet_image.ImageData,
                            ImageWidth = sheet_image.Width,
                            ImageHeight = sheet_image.Height,
                            SpriteMap = sprite_map
                        };

                        pak.AddSheet(sheet_data);

                        break;
                }
            }

            if (!pak_name.Contains(".pak"))
            {
                pak_name += ".pak";
            }
            
            var pak_file_path = Path.Combine(output_path, pak_name);

            using var pak_output = File.OpenWrite(pak_file_path);

            BinaronConvert.Serialize(pak, pak_output);
        }
    }
}
