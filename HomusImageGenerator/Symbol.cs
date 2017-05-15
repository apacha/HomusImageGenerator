using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;

namespace HomusImageGenerator
{
    internal partial class Program
    {
        private class Symbol
        {
            public string FileContent { get; set; }
            public string SymbolName { get; set; }
            public List<List<Point>> Strokes { get; set; }
            public Rectangle Dimensions { get; set; }

            public Symbol()
            {
                Strokes = new List<List<Point>>();
            }

            public static Symbol InitializeFromString(string content)
            {
                var lines = content.Split(new[] { "\n", "\r\n" }, StringSplitOptions.None);

                var newSymbol = new Symbol
                {
                    FileContent = content,
                    SymbolName = lines[0]
                };

                int minX = int.MaxValue;
                int maxX = 0;
                int minY = int.MaxValue;
                int maxY = 0;

                foreach (var strokeString in lines.Skip(1))
                {
                    var stroke = new List<Point>();
                    foreach (var pointString in strokeString.Split(';').Where(s => s != "")) // Each stroke ends with a ; so the last element after the split should be skipped
                    {
                        var point = new Point(int.Parse(pointString.Split(',')[0]), int.Parse(pointString.Split(',')[1]));
                        stroke.Add(point);

                        maxX = Math.Max(maxX, point.X);
                        minX = Math.Min(minX, point.X);
                        maxY = Math.Max(maxY, point.Y);
                        minY = Math.Min(minY, point.Y);
                    }

                    newSymbol.Strokes.Add(stroke);
                    newSymbol.Dimensions = new Rectangle(minX, minY, maxX - minX + 1, maxY - minY + 1);
                }

                return newSymbol;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="exportFileName"></param>
            /// <param name="strokeThickness"></param>
            /// <param name="margin">Additional margin, especially useful, if strokeThickness is bigger than one, to prevent drawing outside of image</param>
            public Size DrawIntoBitmap(string exportFileName, int strokeThickness, int margin = 2)
            {
                var width = Dimensions.Width + 2*margin;
                var height = Dimensions.Height + 2*margin;
                var offset = new Point(Dimensions.Location.X - margin, Dimensions.Location.Y - margin);
                
                using (var bmp = new Bitmap(width, height))
                {
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        g.FillRectangle(new SolidBrush(Color.White), 0, 0, width, height);
                        foreach (var stroke in Strokes)
                        {
                            for (int i = 0; i < stroke.Count - 1; i++)
                            {
                                var startPoint = SubtractOffset(stroke[i], offset);
                                var endPoint = SubtractOffset(stroke[i + 1], offset);
                                g.DrawLine(new Pen(Color.Black, strokeThickness), startPoint, endPoint);
                            }
                        }

                        bmp.Save(exportFileName, ImageFormat.Png);
                    }                    
                }

                return new Size(width, height);
            }

            private Point SubtractOffset(Point a, Point b)
            {
                return new Point(a.X - b.X, a.Y - b.Y);
            }
        }
    }
}
