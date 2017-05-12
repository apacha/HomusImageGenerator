using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HomusImageGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var client = new WebClient())
            {
                var databaseFilePath = Path.GetFullPath(Path.Combine("..","..", "..", "HOMUS.zip"));

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
                Console.WriteLine($"Processing {symbolFiles.Count} symbols...");

                foreach (var symbolFile in symbolFiles)
                {
                    var content = File.ReadAllText(symbolFile);

                    Console.WriteLine("Current symbol content: " + content);

                    string[] lines = content.Split(new[] { "\r", "\r\n" }, StringSplitOptions.None);
                    Console.WriteLine("Symbol name: " + lines[0]);
                    Console.WriteLine("Strokes: " + string.Join(Environment.NewLine, lines.Skip(1)));

                    break;
                }
            }
        }

        private static List<string> GetPathToAllSymbolFiles(string databaseDirectory)
        {
            var symbolFiles = new List<string>();
            foreach (var writerDirectory in Directory.EnumerateDirectories(Path.Combine(databaseDirectory, "HOMUS")))
            {
                foreach (var symbolFile in Directory.EnumerateFiles(writerDirectory))
                {
                    symbolFiles.Add(symbolFile);
                }
            }

            return symbolFiles;
        }
    }
}
