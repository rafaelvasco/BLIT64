
using System;
using BLIT64_Common;

namespace BLIT64_CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Invalid arguments;");
                return;
            }

            var command = args[0];

            switch (command)
            {
                case "build_assets":
                {
                    if (args.Length < 3)
                    {
                        Console.WriteLine("Invalid arguments;");
                        Console.WriteLine("Accepted format: 'build_assets' [path/to/manifest] [output_file_name]");
                        return;
                    }

                    var manifest_file_path = args[1];
                    var output_file_name = args[2];

                    try
                    {
                        AssetsPakBuilder.Build(output_file_name, manifest_file_path);
                        Console.WriteLine("Assets Pak Built Successfully.");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Error while building assets pak: {e.Message}");
                        throw;
                    }

                    break;
                }
                case "parse_bon":
                {
                    if (args.Length < 2)
                    {
                        Console.WriteLine("Invalid arguments;");
                        Console.WriteLine("Accepted format: 'parse_bon' [path/to/bonfile]");
                        return;
                    }

                    var bon_file = BonFileReader.Parse(args[1]);

                    Console.WriteLine(bon_file.ToString());

                    Console.WriteLine("Press Any Key to Close.");

                    Console.ReadLine();

                    break;
                }
            }

        }
    }
}
