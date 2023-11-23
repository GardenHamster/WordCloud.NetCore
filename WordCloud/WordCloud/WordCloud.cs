using SkiaSharp;
using WordCloud.Helper;
using WordCloud.Models;
using WordCloud.Type;

namespace WordCloud
{
    public class WordCloud : IDisposable
    {
        private SKTypeface Typeface { get; init; }

        private SKColor[] Colors { get; init; }

        private SKColor BackColor { get; init; }

        private bool UseVertical { get; init; }

        private int Step { get; init; }

        /// <summary>
        /// 初始化词云
        /// </summary>
        /// <param name="fontName">字体名称</param>
        /// <param name="useVertical">允许垂直绘制字体</param>
        /// <param name="step">数值越低，密度越高，耗时越长，单位px</param>
        /// <param name="backColor">背景颜色,默认白色</param>
        /// <param name="colors">字体颜色,默认随机</param>
        public WordCloud(string fontName, bool useVertical, int step = 3, SKColor? backColor = null, SKColor[] colors = null)
        {
            this.Typeface = SKTypeface.FromFamilyName(fontName);
            this.UseVertical = useVertical;
            this.BackColor = backColor ?? SKColors.White;
            this.Colors = colors ?? new SKColor[0];
            this.Step = step <= 0 ? 1 : step;
        }

        /// <summary>
        /// 初始化词云
        /// </summary>
        /// <param name="fontFile">字体文件</param>
        /// <param name="useVertical">允许垂直绘制字体</param>
        /// <param name="step">数值越低，密度越高，耗时越长，单位px</param>
        /// <param name="backColor">背景颜色,默认白色</param>
        /// <param name="colors">字体颜色,默认随机</param>
        public WordCloud(FileInfo fontFile, bool useVertical, int step = 3, SKColor? backColor = null, SKColor[] colors = null)
        {
            this.Typeface = SKTypeface.FromFile(fontFile.FullName);
            this.UseVertical = useVertical;
            this.BackColor = backColor ?? SKColors.White;
            this.Colors = colors ?? new SKColor[0];
            this.Step = step <= 0 ? 1 : step;
        }

        /// <summary>
        /// 绘制矩形词云
        /// </summary>
        /// <param name="words">关键词</param>
        /// <param name="width">长度(像素)</param>
        /// <param name="height">高度(像素)</param>
        /// <param name="fullImageSavePath">图片保存路径</param>
        /// <param name="format">图片格式</param>
        /// <returns></returns>
        public async Task<FileInfo> Draw(List<string> words, int width, int height, string fullImageSavePath, SKEncodedImageFormat format = SKEncodedImageFormat.Jpeg)
        {
            bool[,] pixels = new bool[height, width];
            int minSide = Math.Min(height, width);
            int minFontSize = (int)Math.Ceiling(Convert.ToDecimal(minSide) / 100);
            int maxFontSize = (int)Math.Ceiling(Convert.ToDecimal(minSide) / words.First().Length * 0.8m);
            return await Draw(words, pixels, maxFontSize, minFontSize, fullImageSavePath, format);
        }

        /// <summary>
        /// 绘制矩形词云
        /// </summary>
        /// <param name="words">关键词</param>
        /// <param name="width">长度(像素)</param>
        /// <param name="height">高度(像素)</param>
        /// <param name="maxFontSize">最大字体大小，值不能大于height</param>
        /// <param name="minFontSize">最小字体大小，值不能大于height</param>
        /// <param name="fullImageSavePath">图片保存路径</param>
        /// <param name="format">图片格式</param>
        /// <returns></returns>
        public async Task<FileInfo> Draw(List<string> words, int width, int height, int maxFontSize, int minFontSize, string fullImageSavePath, SKEncodedImageFormat format = SKEncodedImageFormat.Jpeg)
        {
            bool[,] pixels = new bool[height, width];
            return await Draw(words, pixels, maxFontSize, minFontSize, fullImageSavePath, format);
        }

        /// <summary>
        /// 根据蒙版绘制词云
        /// </summary>
        /// <param name="words">关键词和权重</param>
        /// <param name="maskImage">蒙版图片文件，需要填充词云区域必须为纯黑色</param>
        /// <param name="width">长度(像素)</param>
        /// <param name="fullImageSavePath">图片保存路径</param>
        /// <param name="format">图片格式</param>
        /// <returns></returns>
        public async Task<FileInfo> Draw(List<string> words, FileInfo maskImage, int width, string fullImageSavePath, SKEncodedImageFormat format = SKEncodedImageFormat.Jpeg)
        {
            bool[,] pixels = DrawHelper.LoadPixels(maskImage, width);
            int height = pixels.GetLength(0);
            int minSide = Math.Min(height, width);
            int minFontSize = (int)Math.Ceiling(Convert.ToDecimal(minSide) / 100);
            int maxFontSize = (int)Math.Ceiling(Convert.ToDecimal(minSide) / words.First().Length * 0.8m);
            return await Draw(words, pixels, maxFontSize, minFontSize, fullImageSavePath, format);
        }

        /// <summary>
        /// 根据蒙版绘制词云
        /// </summary>
        /// <param name="words">关键词</param>
        /// <param name="maskImage">蒙版图片文件，需要填充词云区域必须为纯黑色</param>
        /// <param name="width">长度(像素)</param>
        /// <param name="maxFontSize">最大字体大小，值不能大于height</param>
        /// <param name="minFontSize">最小字体大小，值不能大于height</param>
        /// <param name="fullImageSavePath">图片保存路径</param>
        /// <param name="format">图片格式</param>
        /// <returns></returns>
        public async Task<FileInfo> Draw(List<string> words, FileInfo maskImage, int width, int maxFontSize, int minFontSize, string fullImageSavePath, SKEncodedImageFormat format = SKEncodedImageFormat.Jpeg)
        {
            bool[,] pixels = DrawHelper.LoadPixels(maskImage, width);
            return await Draw(words, pixels, maxFontSize, minFontSize, fullImageSavePath, format);
        }

        /// <summary>
        /// 绘制矩形词云
        /// </summary>
        /// <param name="words">关键词</param>
        /// <param name="pixels">像素二维数组，true表示已存在像素</param>
        /// <param name="maxFontSize">最大字体大小</param>
        /// <param name="minFontSize">最小字体大小</param>
        /// <param name="fullImageSavePath">图片保存路径</param>
        /// <param name="format">图片格式</param>
        /// <returns></returns>
        public async Task<FileInfo> Draw(List<string> words, bool[,] pixels, int maxFontSize, int minFontSize, string fullImageSavePath, SKEncodedImageFormat format = SKEncodedImageFormat.Jpeg)
        {
            if (words is null || words.Count == 0)
            {
                throw new Exception("words必须包含一个关键词");
            }
            int width = pixels.GetLength(1);
            int height = pixels.GetLength(0);
            if (maxFontSize <= 0 || maxFontSize > height)
            {
                throw new Exception("maxFontSize的值必须大于0小于height");
            }
            if (minFontSize <= 0 || minFontSize > height)
            {
                throw new Exception("minFontSize的值必须大于0小于height");
            }
            if (minFontSize > maxFontSize)
            {
                throw new Exception("minFontSize必须小于等于maxFontSize");
            }
            int fontSize = maxFontSize;
            var imgInfo = new SKImageInfo(width, height);
            using SKSurface surface = SKSurface.Create(imgInfo);
            using SKCanvas canvas = surface.Canvas;
            canvas.Clear(BackColor);
            List<string> drawWords = words.ToList();
            while (fontSize >= minFontSize && drawWords.Count > 0)
            {
                var drawArea = DrawWords(canvas, pixels, drawWords.First(), fontSize, minFontSize);
                if (drawArea is null && fontSize == minFontSize)
                {
                    drawWords.RemoveAt(0);
                    continue;
                }
                else if (drawArea is null)
                {
                    fontSize--;
                }
                else
                {
                    drawWords.RemoveAt(0);
                    pixels.UpdatePixels(surface, BackColor, drawArea, fontSize, minFontSize);
                }
            }
            string saveDirPath = Path.GetDirectoryName(fullImageSavePath);
            if (Directory.Exists(saveDirPath) == false)
            {
                Directory.CreateDirectory(saveDirPath);
            }
            using SKImage image = surface.Snapshot();
            using SKData data = image.Encode(format, 100);
            using FileStream outputStream = File.OpenWrite(fullImageSavePath);
            data.SaveTo(outputStream);
            FileInfo fileInfo = new FileInfo(fullImageSavePath);
            return await Task.FromResult(fileInfo);
        }

        private DrawArea DrawWords(SKCanvas canvas, bool[,] pixels, string words, int fontSize, int minFontSize)
        {
            var drawAreas = new List<DrawArea>();
            var isVertical = new Random().Next(2) < 1;

            if (UseVertical && isVertical)
            {
                drawAreas.AddRange(GetDrawAreas(pixels, words, fontSize, minFontSize, true, Step));
            }
            else
            {
                drawAreas.AddRange(GetDrawAreas(pixels, words, fontSize, minFontSize, false, Step));
            }

            if (drawAreas.Count == 0 && UseVertical && isVertical == false)
            {
                drawAreas.AddRange(GetDrawAreas(pixels, words, fontSize, minFontSize, true, Step));
            }

            if (drawAreas.Count == 0)
            {
                return null;
            }

            DrawArea randomArea = drawAreas.Random();
            using SKPaint paint = CreatePaint(fontSize);
            canvas.DrawText(randomArea, paint);
            return randomArea;
        }

        /// <summary>
        /// 获取可用的文字绘制区域
        /// </summary>
        /// <param name="pixels"></param>
        /// <param name="words"></param>
        /// <param name="fontSize"></param>
        /// <param name="minFontSize"></param>
        /// <param name="isVertical"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        private List<DrawArea> GetDrawAreas(bool[,] pixels, string words, int fontSize, int minFontSize, bool isVertical, int step)
        {
            var continueY = true;
            var drawType = DrawType.Horizontal;
            int yLen = pixels.GetLength(0);//画布宽
            int xLen = pixels.GetLength(1);//画布长
            var textAreas = GetTextAreas(words, fontSize, isVertical, ref drawType);
            var margin = (int)Math.Ceiling(fontSize * 0.15);//垂直绘制时每个文字之间的上下间距
            var width = textAreas.Max(x => x.Width);
            var height = textAreas.Sum(x => x.Height) + (textAreas.Count - 1) * margin;
            List<DrawArea> drawAreas = new List<DrawArea>();
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
                    if (pixels.CheckAreaAvailable(x, y, width, height, minFontSize))
                    {
                        var drawArea = new DrawArea(textAreas, drawType, words, fontSize, x, y, width, height, margin, isVertical);
                        drawAreas.Add(drawArea);
                    }
                }
            }
            return drawAreas;
        }

        /// <summary>
        /// 计算绘制文本所需要的长和宽
        /// </summary>
        /// <param name="words"></param>
        /// <param name="fontSize"></param>
        /// <param name="isVertical"></param>
        /// <param name="drawType"></param>
        /// <returns></returns>
        private List<TextArea> GetTextAreas(string words, int fontSize, bool isVertical, ref DrawType drawType)
        {
            var textRect = GetTextRect(words, fontSize);
            if (isVertical && words.ContainNumberOrLetter())
            {
                drawType = DrawType.Rotational;
                TextArea area = new TextArea(textRect, textRect.Height, textRect.Width, words);
                return new() { area };
            }
            if (isVertical == false)
            {
                drawType = DrawType.Horizontal;
                TextArea area = new TextArea(textRect, textRect.Width, textRect.Height, words);
                return new() { area };
            }
            drawType = DrawType.Vertical;
            var textAreas = new List<TextArea>();
            foreach (var word in words)
            {
                TextArea area = new TextArea(textRect, textRect.Width, textRect.Height, word.ToString());
                textAreas.Add(area);
            }
            return textAreas;
        }

        /// <summary>
        /// 计算横向绘制文本所需要的长和宽
        /// </summary>
        /// <param name="words"></param>
        /// <param name="fontSize"></param>
        /// <returns></returns>
        private SKRect GetTextRect(string words, int fontSize)
        {
            using SKPaint paint = CreatePaint(fontSize);
            SKRect textRect = new SKRect();
            paint.MeasureText(words, ref textRect);
            return textRect;
        }

        /// <summary>
        /// 创建画笔
        /// </summary>
        /// <param name="fontSize"></param>
        /// <returns></returns>
        private SKPaint CreatePaint(float fontSize)
        {
            return new()
            {
                FakeBoldText = false,
                Color = Colors.Length == 0 ? RandomHelper.RandomColor() : Colors.Random(),
                IsAntialias = true,
                Style = SKPaintStyle.StrokeAndFill,
                TextAlign = SKTextAlign.Left,
                TextSize = fontSize,
                Typeface = Typeface
            };
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (Typeface is not null)
            {
                Typeface.Dispose();
            }
        }

    }
}
