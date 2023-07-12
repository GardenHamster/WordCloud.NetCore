using SkiaSharp;
using WordCloud.Models;

namespace WordCloud.Helper
{
    public static class DrawHelper
    {
        private static readonly Random Random = new Random();

        /// <summary>
        /// 绘制文字
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="drawArea"></param>
        internal static void DrawText(SKCanvas canvas, DrawArea drawArea)
        {
            new SKPaint();
            int startX = drawArea.StartX;
            int startY = drawArea.StartY;
            foreach (var textArea in drawArea.TextAreas)
            {
                canvas.DrawText(textArea.Words, startX, startY + textArea.Height - textArea.TextRect.Bottom, drawArea.Paint);
                startY += textArea.Height + drawArea.Margin;
            }
        }

        /// <summary>
        /// 更新快照
        /// </summary>
        /// <param name="surface"></param>
        /// <param name="backColor"></param>
        /// <param name="drawArea"></param>
        /// <param name="pixels"></param>
        internal static void UpdatePixels(SKSurface surface, SKColor backColor, DrawArea drawArea, bool[,] pixels)
        {
            int xLen = pixels.GetLength(1);
            using SKImage image = surface.Snapshot();
            using SKBitmap bitmap = SKBitmap.FromImage(image);
            SKColor[] colors = bitmap.Pixels;
            for (int y = drawArea.StartY; y <= drawArea.EndY; y++)
            {
                for (int x = drawArea.StartX; x <= drawArea.EndX; x++)
                {
                    int index = y * xLen + x;
                    if (colors[index] == SKColor.Empty) continue;
                    if (colors[index] == backColor) continue;
                    pixels[y, x] = true;
                }
            }
        }

        /// <summary>
        /// 计算绘制文本所需要的长和宽
        /// </summary>
        /// <param name="paint"></param>
        /// <param name="words"></param>
        /// <param name="isVertical"></param>
        /// <returns></returns>
        internal static List<TextArea> GetTextAreas(SKPaint paint, string words, bool isVertical)
        {
            if (!isVertical)
            {
                var textArea = GetTextAreas(paint, words);
                return new() { textArea };
            }
            var textAreas = new List<TextArea>();
            foreach (var word in words)
            {
                var textArea = GetTextAreas(paint, word.ToString());
                textAreas.Add(textArea);
            }
            return textAreas;
        }

        /// <summary>
        /// 计算横向绘制文本所需要的长和宽
        /// </summary>
        /// <param name="paint"></param>
        /// <param name="words"></param>
        /// <returns></returns>
        private static TextArea GetTextAreas(SKPaint paint, string words)
        {
            SKRect textRect = new SKRect();
            float width = paint.MeasureText(words, ref textRect);
            float height = textRect.Height;
            return new TextArea(textRect, width, height, words, paint.TextSize);
        }

        /// <summary>
        /// 获取可用的文字绘制区域
        /// </summary>
        /// <param name="pixels"></param>
        /// <param name="paint"></param>
        /// <param name="words"></param>
        /// <param name="isVertical"></param>
        /// <returns></returns>
        internal static List<DrawArea> GetDrawAreas(bool[,] pixels, SKPaint paint, string words, bool isVertical)
        {
            int yLen = pixels.GetLength(0);//画布宽
            int xLen = pixels.GetLength(1);//画布长
            var margin = (int)Math.Ceiling(paint.TextSize * 0.1);//垂直绘制时每个文字之间的上下间距
            var textAreas = GetTextAreas(paint, words, isVertical);
            var width = textAreas.Max(x => x.Width);
            var height = textAreas.Sum(x => x.Height) + (textAreas.Count - 1) * margin;
            List<DrawArea> drawAreas = new List<DrawArea>();
            bool continueY = true;
            for (int y = 0; y < yLen && continueY; y += 3)
            {
                for (int x = 0; x < xLen; x += 3)
                {
                    if (x + width >= xLen) //超出画布长
                    {
                        break;
                    }
                    if (y + height >= yLen) //超出画布宽
                    {
                        continueY = false;
                        break;
                    }
                    DrawArea drawArea = new DrawArea(textAreas, paint, x, y, width, height, margin, isVertical);
                    if (CheckAreaAvailable(pixels, drawArea) == false) continue;//区域不可用
                    drawAreas.Add(drawArea);
                }
            }
            return drawAreas;
        }

        /// <summary>
        /// 检查一个区域是否可以绘制文字
        /// </summary>
        /// <param name="pixels"></param>
        /// <param name="drawArea"></param>
        /// <returns></returns>
        internal static bool CheckAreaAvailable(bool[,] pixels, DrawArea drawArea)
        {
            for (int y = drawArea.StartY; y <= drawArea.EndY; y += 3)
            {
                for (int x = drawArea.StartX; x <= drawArea.EndX; x++)
                {
                    if (pixels[y, x] == true) return false;//某一个坐标中存在像素
                }
            }
            for (int y = drawArea.StartY; y <= drawArea.EndY; y++)
            {
                for (int x = drawArea.StartX; x <= drawArea.EndX; x += 3)
                {
                    if (pixels[y, x] == true) return false;//某一个坐标中存在像素
                }
            }
            return true;
        }

        /// <summary>
        /// 创建画笔
        /// </summary>
        /// <param name="typeface"></param>
        /// <param name="fontSize"></param>
        /// <returns></returns>
        internal static SKPaint CreatePaint(SKTypeface typeface, float fontSize)
        {
            return new()
            {
                FakeBoldText = false,
                Color = RandomColor(),
                IsAntialias = true,
                Style = SKPaintStyle.StrokeAndFill,
                TextAlign = SKTextAlign.Left,
                TextSize = fontSize,
                Typeface = typeface
            };
        }

        /// <summary>
        /// 随机颜色
        /// </summary>
        /// <returns></returns>
        private static SKColor RandomColor()
        {
            byte red = (byte)Random.Next(0, 255);
            byte green = (byte)Random.Next(0, 255);
            byte blue = (byte)Random.Next(0, 255);
            return new SKColor(red, green, blue);
        }



    }
}