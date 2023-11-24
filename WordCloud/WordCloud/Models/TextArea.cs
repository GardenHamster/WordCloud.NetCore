using SkiaSharp;
using WordCloud.NetCore.Models;
using WordCloud.Type;

namespace WordCloud.Models
{
    internal class TextArea
    {
        public string Text { get; init; }
        public SKPaint Paint { get; init; }
        public SKRect TextRect { get; init; }
        public DrawType DrawType { get; set; }
        public WordRect[] WordRects { get; init; }
        public ushort Width => GetWidth();
        public ushort Height => GetHeight();
        public ushort VerticalMargin => (ushort)(Paint.TextSize * MarginScale);
        public static float MarginScale => 0.15f;

        public TextArea(WordRect[] wordRects, SKRect textRect, SKPaint paint, DrawType drawType, string text)
        {
            Text = text;
            Paint = paint;
            TextRect = textRect;
            DrawType = drawType;
            WordRects = wordRects;
        }

        public ushort GetWidth()
        {
            if (DrawType == DrawType.Horizontal)
            {
                return (ushort)TextRect.Width;
            }
            if (DrawType == DrawType.Rotational)
            {
                return (ushort)TextRect.Height;
            }
            if (DrawType == DrawType.Vertical)
            {
                return WordRects.Max(o => o.Width);
            }
            throw new Exception($"未定义宽度计算方式{DrawType}");
        }

        public ushort GetHeight()
        {
            if (DrawType == DrawType.Horizontal)
            {
                return (ushort)TextRect.Height;
            }
            if (DrawType == DrawType.Rotational)
            {
                return (ushort)TextRect.Width;
            }
            if (DrawType == DrawType.Vertical)
            {
                return (ushort)(WordRects.Sum(o => o.Height) + (WordRects.Length - 1) * VerticalMargin);
            }
            throw new Exception($"未定义高度计算方式{DrawType}");
        }

    }
}
