using WordCloud.Type;

namespace WordCloud.Models
{
    internal class DrawArea
    {
        public string Words { get; set; }
        public int StartX { get; init; }
        public int StartY { get; init; }
        public int EndX { get; init; }
        public int EndY { get; init; }
        public int Width { get; init; }
        public int Height { get; init; }
        public int Margin { get; init; }
        public bool IsVertical { get; init; }
        public int FontSize { get; init; }
        public DrawType DrawType { get; set; }
        public List<TextArea> TextAreas { get; init; }

        public DrawArea(List<TextArea> textAreas, DrawType drawType, string words, int fontSize, int startX, int startY, int width, int height, int margin, bool isVertical)
        {
            this.Words = words;
            this.StartX = startX;
            this.StartY = startY;
            this.Margin = margin;
            this.Width = width;
            this.Height = height;
            this.EndX = startX + Width;
            this.EndY = startY + Height;
            this.IsVertical = isVertical;
            this.TextAreas = textAreas;
            this.DrawType = drawType;
            this.FontSize = fontSize;
        }

    }
}
