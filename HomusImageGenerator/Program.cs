using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;

namespace HomusImageGenerator
{
    internal partial class Program
    {
        private static void Main(string[] args)
        {
            using (var client = new WebClient())
            {
                var databaseFilePath = Path.GetFullPath(Path.Combine("..", "..", "..", "HOMUS.zip"));

                if (!File.Exists(databaseFilePath))
                {
                    Console.WriteLine("Downloading database...");
                    client.DownloadFile("http://grfia.dlsi.ua.es/homus/HOMUS.zip", databaseFilePath);
                }

                var databaseDirectory = Path.GetFullPath(Path.Combine("..", "..", "..", "HOMUS"));
                if (!Directory.Exists(databaseDirectory))
                {
                    Console.WriteLine($"Extracting database into {databaseDirectory} ...");
                    ZipFile.ExtractToDirectory(databaseFilePath, databaseDirectory);
                }

                var symbolFiles = GetPathToAllSymbolFiles(databaseDirectory);
                var baseDirectoryForExport = Path.GetFullPath(Path.Combine("..", "..", "..", "HOMUS", "Export"));
                Console.WriteLine($"Processing {symbolFiles.Count} symbols...");
                StringBuilder builder = new StringBuilder();
                
                foreach (var symbolFile in symbolFiles)
                {
                    var content = File.ReadAllText(symbolFile);
                    Symbol symbol = Symbol.InitializeFromString(content);

                    var targetDirectory = Path.Combine(baseDirectoryForExport, symbol.SymbolName);
                    if (!Directory.Exists(targetDirectory))
                    {
                        Directory.CreateDirectory(targetDirectory);
                    }

                    var fileName = Path.GetFileNameWithoutExtension(symbolFile);

                    for (int i = 3; i <= 3; i++)
                    {
                        var dimensions = symbol.DrawIntoBitmap(Path.Combine(targetDirectory, $"{fileName}_{i}.png"), i, 0, 128, 224);
                        builder.AppendLine($"{dimensions.Width};{dimensions.Height}");
                    }                    
                }                

                File.WriteAllText("dimensions.txt", builder.ToString());
            }
        }

        private static List<string> GetPathToAllSymbolFiles(string databaseDirectory)
        {
            var symbolFiles = new List<string>();
            foreach (var writerDirectory in Directory.EnumerateDirectories(Path.Combine(databaseDirectory, "HOMUS")))
                foreach (var symbolFile in Directory.EnumerateFiles(writerDirectory))
                    symbolFiles.Add(symbolFile);

            return symbolFiles;
        }
    }
}
