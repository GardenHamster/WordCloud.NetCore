using SkiaSharp;

namespace WordCloud.Models
{
    internal class TextArea
    {
        public int Width { get; init; }
        public int Height { get; init; }
        public string Words { get; init; }
        public SKRect TextRect { get; set; }

        public TextArea(SKRect textRect, float width, float height, string words)
        {
            this.TextRect = textRect;
            this.Width = (int)Math.Ceiling(width);
            this.Height = (int)Math.Ceiling(height);
            this.Words = words;
        }

    }
}
