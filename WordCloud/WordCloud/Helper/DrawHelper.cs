using SkiaSharp;
using WordCloud.Models;
using WordCloud.Type;

namespace WordCloud.Helper
{
    public static class DrawHelper
    {
        /// <summary>
        /// 绘制文字
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="drawArea"></param>
        internal static void DrawText(SKCanvas canvas, DrawArea drawArea)
        {
            if (drawArea.DrawType == DrawType.Vertical)
            {
                DrawVerticalText(canvas, drawArea);
                return;
            }
            if (drawArea.DrawType == DrawType.Rotational)
            {
                DrawRotationalText(canvas, drawArea);
                return;
            }
            if (drawArea.DrawType == DrawType.Horizontal)
            {
                DrawHorizontalText(canvas, drawArea);
                return;
            }
            throw new Exception($"未定义文字绘制方式:{drawArea.DrawType}");
        }

        /// <summary>
        /// 绘制垂直文字
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="drawArea"></param>
        internal static void DrawVerticalText(SKCanvas canvas, DrawArea drawArea)
        {
            int startX = drawArea.StartX;
            int startY = drawArea.StartY;
            foreach (var textArea in drawArea.TextAreas)
            {
                canvas.DrawText(textArea.Words, startX, startY + textArea.Height - textArea.TextRect.Bottom, drawArea.Paint);
                startY += textArea.Height + drawArea.Margin;
            }
        }

        /// <summary>
        /// 绘制翻转文字
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="drawArea"></param>
        internal static void DrawRotationalText(SKCanvas canvas, DrawArea drawArea)
        {
            SKPath path = new SKPath();
            TextArea textArea = drawArea.TextAreas.First();
            path.MoveTo(drawArea.StartX + textArea.TextRect.Bottom, drawArea.StartY);
            path.LineTo(drawArea.StartX + textArea.TextRect.Bottom, drawArea.EndY);
            canvas.DrawTextOnPath(drawArea.Words, path, new(), drawArea.Paint);
        }

        /// <summary>
        /// 绘制水平文字
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="drawArea"></param>
        internal static void DrawHorizontalText(SKCanvas canvas, DrawArea drawArea)
        {
            TextArea textArea = drawArea.TextAreas.First();
            canvas.DrawText(drawArea.Words, drawArea.StartX, drawArea.StartY + drawArea.Height - textArea.TextRect.Bottom, drawArea.Paint);
        }

        /// <summary>
        /// 从蒙版图片中加载像素
        /// </summary>
        /// <param name="imgFile"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        internal static bool[,] LoadPixels(FileInfo imgFile, int width)
        {
            using FileStream originStream = File.OpenRead(imgFile.FullName);
            using SKBitmap originBitmap = SKBitmap.Decode(originStream);
            var height = (int)Math.Ceiling((Convert.ToDecimal(width) / originBitmap.Width) * originBitmap.Height);
            var imgInfo = new SKImageInfo(width, height);
            var resizeBitmap = originBitmap.Resize(imgInfo, SKFilterQuality.High);
            SKColor[] colors = resizeBitmap.Pixels;
            var xLen = resizeBitmap.Width;
            bool[,] pixels = new bool[resizeBitmap.Height, resizeBitmap.Width];
            for (int i = 0; i < colors.Length; i++)
            {
                if (colors[i] == SKColors.Black) continue;
                pixels[i / xLen, i % xLen] = true;
            }
            return pixels;
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
        internal static List<TextArea> GetTextAreas(SKPaint paint, string words, bool isVertical, ref DrawType drawType)
        {
            if (isVertical && words.ContainNumberOrLetter())
            {
                drawType = DrawType.Rotational;
                SKRect textRect = GetTextRect(paint, words);
                TextArea area = new TextArea(textRect, paint, textRect.Height, textRect.Width, words);
                return new() { area };
            }
            if (isVertical == false)
            {
                drawType = DrawType.Horizontal;
                SKRect textRect = GetTextRect(paint, words);
                TextArea area = new TextArea(textRect, paint, textRect.Width, textRect.Height, words);
                return new() { area };
            }
            drawType = DrawType.Vertical;
            var textAreas = new List<TextArea>();
            foreach (var word in words)
            {
                SKRect textRect = GetTextRect(paint, words);
                TextArea area = new TextArea(textRect, paint, textRect.Width, textRect.Height, word.ToString());
                textAreas.Add(area);
            }
            return textAreas;
        }

        /// <summary>
        /// 计算横向绘制文本所需要的长和宽
        /// </summary>
        /// <param name="paint"></param>
        /// <param name="words"></param>
        /// <returns></returns>
        private static SKRect GetTextRect(SKPaint paint, string words)
        {
            SKRect textRect = new SKRect();
            paint.MeasureText(words, ref textRect);
            return textRect;
        }

        /// <summary>
        /// 获取可用的文字绘制区域
        /// </summary>
        /// <param name="pixels"></param>
        /// <param name="paint"></param>
        /// <param name="words"></param>
        /// <param name="minFontSize"></param>
        /// <param name="isVertical"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        internal static List<DrawArea> GetDrawAreas(bool[,] pixels, SKPaint paint, string words, int minFontSize, bool isVertical, int step)
        {
            int yLen = pixels.GetLength(0);//画布宽
            int xLen = pixels.GetLength(1);//画布长
            var dreaType = DrawType.Horizontal;
            var textAreas = GetTextAreas(paint, words, isVertical, ref dreaType);
            var margin = (int)Math.Ceiling(paint.TextSize * 0.15);//垂直绘制时每个文字之间的上下间距
            var width = textAreas.Max(x => x.Width);
            var height = textAreas.Sum(x => x.Height) + (textAreas.Count - 1) * margin;
            List<DrawArea> drawAreas = new List<DrawArea>();
            bool continueY = true;
            for (int y = 0; y < yLen && continueY; y += step)
            {
                for (int x = 0; x < xLen; x += step)
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
                    if (CheckAreaAvailable(pixels, x, y, width, height, minFontSize))
                    {
                        drawAreas.Add(new DrawArea(textAreas, paint, dreaType, words, x, y, width, height, margin, isVertical));
                    }
                }
            }
            return drawAreas;
        }

        /// <summary>
        /// 检查一个区域是否可以绘制文字
        /// </summary>
        /// <param name="pixels"></param>
        /// <param name="startX"></param>
        /// <param name="startY"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="minFontSize"></param>
        /// <returns></returns>
        internal static bool CheckAreaAvailable(bool[,] pixels, int startX, int startY, int width, int height, int minFontSize)
        {
            for (int y = startY; y <= startY + height; y++)
            {
                if (pixels[y, startX]) return false;//某一个坐标中存在像素
                if (pixels[y, startX + width]) return false;//某一个坐标中存在像素
            }

            for (int x = startX; x <= startX + width; x++)
            {
                if (pixels[startY, x]) return false;//某一个坐标中存在像素
                if (pixels[startY + height, x]) return false;//某一个坐标中存在像素
            }

            int step = (int)Math.Ceiling(Convert.ToDouble(minFontSize) / 3);
            if (step < 3) step = 3;

            for (int y = startY + step; y <= startY + height; y += step)
            {
                for (int x = startX; x <= startX + width; x++)
                {
                    if (pixels[y, x]) return false;//某一个坐标中存在像素
                }
            }
            for (int y = startY; y <= startY + height; y++)
            {
                for (int x = startX + step; x <= startX + width; x += step)
                {
                    if (pixels[y, x]) return false;//某一个坐标中存在像素
                }
            }
            return true;
        }

        /// <summary>
        /// 创建画笔
        /// </summary>
        /// <param name="typeface"></param>
        /// <param name="fontSize"></param>
        /// <param name="colors"></param>
        /// <returns></returns>
        internal static SKPaint CreatePaint(SKTypeface typeface, float fontSize, SKColor[] colors)
        {
            return new()
            {
                FakeBoldText = false,
                Color = colors == null || colors.Length == 0 ? RandomHelper.RandomColor() : colors.Random(),
                IsAntialias = true,
                Style = SKPaintStyle.StrokeAndFill,
                TextAlign = SKTextAlign.Left,
                TextSize = fontSize,
                Typeface = typeface
            };
        }

    }
}