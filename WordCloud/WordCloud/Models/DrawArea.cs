using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordCloud.Models
{
    internal class DrawArea
    {
        public int StartX { get; init; }
        public int StartY { get; init; }
        public int EndX { get; init; }
        public int EndY { get; init; }
        public int Width { get; init; }
        public int height { get; init; }
        public int Margin { get; init; }
        public bool IsVertical { get; init; }
        public SKPaint Paint { get; init; }
        public List<TextArea> TextAreas { get; init; }

        public DrawArea(List<TextArea> textAreas, SKPaint paint, int startX, int startY, int width, int height, int margin, bool isVertical)
        {
            this.StartX = startX;
            this.StartY = startY;
            this.Margin = margin;
            this.Width = width;
            this.height = height;
            this.EndX = startX + Width;
            this.EndY = startY + this.height;
            this.IsVertical = isVertical;
            this.TextAreas = textAreas;
            this.Paint = paint;
        }


    }
}
