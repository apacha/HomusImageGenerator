using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;

namespace HomusImageGenerator
{
    internal class Program
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
                Console.WriteLine($"Processing {symbolFiles.Count} symbols...");

                int minX = int.MaxValue;
                int maxX = 0;
                int minY = int.MaxValue;
                int maxY = 0;

                foreach (var symbolFile in symbolFiles)
                {
                    var content = File.ReadAllText(symbolFile);
                    Symbol symbol = Symbol.InitializeFromString(content);

                    foreach (var stroke in symbol.Strokes)
                    {
                        foreach (var point in stroke)
                        {
                            if (point.X < 0)
                            {
                                
                            }

                            maxX = Math.Max(maxX, point.X);
                            minX = Math.Min(minX, point.X);

                            maxY = Math.Max(maxY, point.Y);
                            minY = Math.Min(minY, point.Y);
                        }
                    }
                }

                Console.WriteLine($"MinX {minX}, MaxX {maxX}, MinY {minY}, MaxY {maxY}");
                // Max dimension: 304x448 pixels
            }
        }

        private class Symbol
        {
            public string FileContent { get; set; }
            public string SymbolName { get; set; }
            public List<List<Point>> Strokes { get; set; }

            public Symbol()
            {
                Strokes = new List<List<Point>>();
            }

            public static Symbol InitializeFromString(string content)
            {
                var lines = content.Split(new[] { "\n", "\r\n" }, StringSplitOptions.None);

                var newSymbol = new Symbol()
                {
                    FileContent = content,
                    SymbolName = lines[0]
                };
                
                foreach (var strokeString in lines.Skip(1))
                {
                    var stroke = new List<Point>();
                    foreach (var point in strokeString.Split(';').Where(s => s != "")) // Each stroke ends with a ; so the last element after the split should be skipped
                    {
                        stroke.Add(new Point(int.Parse(point.Split(',')[0]), int.Parse(point.Split(',')[1])));
                    }

                    newSymbol.Strokes.Add(stroke);
                }
                
                return newSymbol;
            }

            public void DrawIntoBitmap()
            {
                //Bitmap bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
                //using (Graphics g = Graphics.FromImage(bmp))
                //{
                //    g.DrawLine(new Pen(Color.Red), 0, 0, 10, 10);
                //}
                //pictureBox1.Image = bmp;
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
