using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordCloud.Models
{
    internal class TextArea
    {
        public int Width { get; init; }
        public int Height { get; init; }
        public string Words { get; init; }
        public SKPaint Paint { get; init; }
        public SKRect TextRect { get; set; }

        public TextArea(SKRect textRect, SKPaint paint, float width, float height, string words)
        {
            this.TextRect = textRect;
            this.Paint = paint;
            this.Width = (int)Math.Ceiling(width);
            this.Height = (int)Math.Ceiling(height);
            this.Words = words;
        }

    }
}
