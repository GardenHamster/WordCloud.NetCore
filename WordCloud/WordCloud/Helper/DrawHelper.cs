using SkiaSharp;
using WordCloud.Models;
using WordCloud.Type;

namespace WordCloud.Helper
{
    internal static class DrawHelper
    {
        /// <summary>
        /// 绘制文字
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="drawArea"></param>
        /// <param name="paint"></param>
        /// <exception cref="Exception"></exception>
        internal static void DrawText(this SKCanvas canvas, DrawArea drawArea, SKPaint paint)
        {
            if (drawArea.DrawType == DrawType.Horizontal)
            {
                canvas.DrawHorizontalText(drawArea, paint);
                return;
            }
            if (drawArea.DrawType == DrawType.Rotational)
            {
                canvas.DrawRotationalText(drawArea, paint);
                return;
            }
            if (drawArea.DrawType == DrawType.Vertical)
            {
                canvas.DrawVerticalText(drawArea, paint);
                return;
            }
            throw new Exception($"未定义文字绘制方式:{drawArea.DrawType}");
        }

        /// <summary>
        /// 绘制水平文字
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="drawArea"></param>
        /// <param name="paint"></param>
        internal static void DrawHorizontalText(this SKCanvas canvas, DrawArea drawArea, SKPaint paint)
        {
            canvas.DrawText(drawArea.TextArea.Text, drawArea.StartX, drawArea.StartY + drawArea.Height - drawArea.TextArea.TextRect.Bottom, paint);
        }

        /// <summary>
        /// 绘制旋转文字
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="drawArea"></param>
        /// <param name="paint"></param>
        internal static void DrawRotationalText(this SKCanvas canvas, DrawArea drawArea, SKPaint paint)
        {
            using SKPath path = new SKPath();
            TextArea textArea = drawArea.TextArea;
            path.MoveTo(drawArea.StartX + textArea.TextRect.Bottom, drawArea.StartY);
            path.LineTo(drawArea.StartX + textArea.TextRect.Bottom, drawArea.EndY);
            canvas.DrawTextOnPath(drawArea.TextArea.Text, path, new(), paint);
        }

        /// <summary>
        /// 绘制垂直文字
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="drawArea"></param>
        /// <param name="paint"></param>
        internal static void DrawVerticalText(this SKCanvas canvas, DrawArea drawArea, SKPaint paint)
        {
            int startX = drawArea.StartX;
            int startY = drawArea.StartY;
            var textArea = drawArea.TextArea;
            foreach (var item in textArea.WordRects)
            {
                canvas.DrawText(item.Word.ToString(), startX, startY + item.Rect.Height - item.Rect.Bottom, paint);
                startY += item.Height + textArea.VerticalMargin;
            }
        }

        /// <summary>
        /// 计算横向绘制文本的长和宽
        /// </summary>
        /// <param name="paint"></param>
        /// <param name="word"></param>
        /// <returns></returns>
        internal static SKRect GetTextRect(this SKPaint paint, string word)
        {
            SKRect textRect = new SKRect();
            paint.MeasureText(word, ref textRect);
            return textRect;
        }

        /// <summary>
        /// 从蒙版图片中加载像素
        /// </summary>
        /// <param name="imgFile"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        internal static bool[,] LoadPixels(this FileInfo imgFile, int width)
        {
            using FileStream originStream = File.OpenRead(imgFile.FullName);
            using SKBitmap originBitmap = SKBitmap.Decode(originStream);
            var height = (int)Math.Ceiling((Convert.ToDecimal(width) / originBitmap.Width) * originBitmap.Height);
            var imgInfo = new SKImageInfo(width, height);
            using SKBitmap resizeBitmap = originBitmap.Resize(imgInfo, SKFilterQuality.High);
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
        /// <param name="pixels"></param>
        /// <param name="surface"></param>
        /// <param name="backColor"></param>
        /// <param name="drawArea"></param>
        /// <param name="fontSize"></param>
        /// <param name="minFontSize"></param>
        internal static void UpdatePixels(this bool[,] pixels, SKSurface surface, SKColor backColor, DrawArea drawArea, int fontSize, int minFontSize)
        {
            if (fontSize <= minFontSize)
            {
                for (int y = drawArea.StartY; y <= drawArea.EndY; y++)
                {
                    for (int x = drawArea.StartX; x <= drawArea.EndX; x++)
                    {
                        pixels[y, x] = true;
                    }
                }
            }
            else
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
        internal static bool CheckAvailable(this bool[,] pixels, int startX, int startY, int width, int height, int minFontSize)
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

            int step = minFontSize / 2 < 1 ? 1 : minFontSize / 2;

            for (int y = startY + step; y <= startY + height; y += step)
            {
                for (int x = startX; x <= startX + width; x++)
                {
                    if (pixels[y, x]) return false;//某一个坐标中存在像素
                }
            }

            for (int x = startX + step; x <= startX + width; x += step)
            {
                for (int y = startY; y <= startY + height; y++)
                {
                    if (pixels[y, x]) return false;//某一个坐标中存在像素
                }
            }
            return true;
        }

    }
}