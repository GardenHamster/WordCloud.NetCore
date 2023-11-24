using SkiaSharp;
using WordCloud.Helper;
using WordCloud.Models;
using WordCloud.NetCore.Models;
using WordCloud.Type;

namespace WordCloud
{
    /// <summary>
    /// 词云生成工具类
    /// </summary>
    public class WordCloud : IDisposable
    {
        private SKTypeface Typeface { get; init; }

        private SKColor[] Colors { get; init; }

        private SKColor BackColor { get; init; }

        private bool UseVertical { get; init; }

        private ushort Step { get; init; }

        /// <summary>
        /// 初始化词云
        /// </summary>
        /// <param name="fontName">字体名称</param>
        /// <param name="useVertical">允许垂直绘制字体</param>
        /// <param name="step">数值越低，密度越高，耗时越长，单位px</param>
        /// <param name="backColor">背景颜色,默认白色</param>
        /// <param name="colors">字体颜色,默认随机</param>
        public WordCloud(string fontName, bool useVertical, ushort step = 3, SKColor? backColor = null, SKColor[] colors = null)
        {
            this.Typeface = SKTypeface.FromFamilyName(fontName);
            this.UseVertical = useVertical;
            this.BackColor = backColor ?? SKColors.White;
            this.Colors = colors ?? new SKColor[0];
            this.Step = step > 0 ? step : (ushort)3;
        }

        /// <summary>
        /// 初始化词云
        /// </summary>
        /// <param name="fontFile">字体文件</param>
        /// <param name="useVertical">允许垂直绘制字体</param>
        /// <param name="step">数值越低，密度越高，耗时越长，单位px</param>
        /// <param name="backColor">背景颜色,默认白色</param>
        /// <param name="colors">字体颜色,默认随机</param>
        public WordCloud(FileInfo fontFile, bool useVertical, ushort step = 3, SKColor? backColor = null, SKColor[] colors = null)
        {
            this.Typeface = SKTypeface.FromFile(fontFile.FullName);
            this.UseVertical = useVertical;
            this.BackColor = backColor ?? SKColors.White;
            this.Colors = colors ?? new SKColor[0];
            this.Step = step > 0 ? step : (ushort)3;
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
        public async Task<FileInfo> Draw(List<string> words, ushort width, ushort height, string fullImageSavePath, SKEncodedImageFormat format = SKEncodedImageFormat.Jpeg)
        {
            var pixels = new bool[height, width];
            var minSide = Math.Min(height, width);
            var minFontSize = (ushort)Math.Ceiling(Convert.ToDecimal(minSide) / 100);
            var maxFontSize = (ushort)Math.Ceiling(Convert.ToDecimal(minSide) / words.First().Length * 0.8m);
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
        public async Task<FileInfo> Draw(List<string> words, ushort width, ushort height, ushort maxFontSize, ushort minFontSize, string fullImageSavePath, SKEncodedImageFormat format = SKEncodedImageFormat.Jpeg)
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
        public async Task<FileInfo> Draw(List<string> words, FileInfo maskImage, ushort width, string fullImageSavePath, SKEncodedImageFormat format = SKEncodedImageFormat.Jpeg)
        {
            var pixels = DrawHelper.LoadPixels(maskImage, width);
            var height = pixels.GetLength(0);
            var minSide = Math.Min(height, width);
            var minFontSize = (ushort)Math.Ceiling(Convert.ToDecimal(minSide) / 100);
            var maxFontSize = (ushort)Math.Ceiling(Convert.ToDecimal(minSide) / words.First().Length * 0.8m);
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
        public async Task<FileInfo> Draw(List<string> words, FileInfo maskImage, ushort width, ushort maxFontSize, ushort minFontSize, string fullImageSavePath, SKEncodedImageFormat format = SKEncodedImageFormat.Jpeg)
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
        public async Task<FileInfo> Draw(List<string> words, bool[,] pixels, ushort maxFontSize, ushort minFontSize, string fullImageSavePath, SKEncodedImageFormat format = SKEncodedImageFormat.Jpeg)
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
            var fontSize = maxFontSize;
            var imgInfo = new SKImageInfo(width, height);
            using SKSurface surface = SKSurface.Create(imgInfo);
            using SKCanvas canvas = surface.Canvas;
            canvas.Clear(BackColor);
            List<string> drawWords = words.ToList();
            while (fontSize >= minFontSize && drawWords.Count > 0)
            {
                using SKPaint paint = CreatePaint(fontSize);
                var drawArea = DrawWords(canvas, paint, pixels, drawWords.First(), minFontSize);
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

        private DrawArea DrawWords(SKCanvas canvas, SKPaint paint, bool[,] pixels, string words, ushort minFontSize)
        {
            List<DrawArea> drawAreas;
            var isVertical = new Random().Next(2) < 1;
            var wordRects = GetWordRects(paint, words);
            if (UseVertical && isVertical)
            {
                drawAreas = GetDrawAreas(pixels, wordRects, paint, words, minFontSize, true, Step);
            }
            else
            {
                drawAreas = GetDrawAreas(pixels, wordRects, paint, words, minFontSize, false, Step);
            }

            if (drawAreas.Count == 0 && UseVertical && isVertical == false)
            {
                drawAreas = GetDrawAreas(pixels, wordRects, paint, words, minFontSize, true, Step);
            }

            if (drawAreas.Count == 0)
            {
                return null;
            }

            DrawArea randomArea = drawAreas.Random();
            canvas.DrawText(randomArea, paint);
            return randomArea;
        }

        /// <summary>
        /// 获取可用的文字绘制区域
        /// </summary>
        /// <param name="pixels"></param>
        /// <param name="wordRects"></param>
        /// <param name="paint"></param>
        /// <param name="words"></param>
        /// <param name="minFontSize"></param>
        /// <param name="isVertical"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        private List<DrawArea> GetDrawAreas(bool[,] pixels, WordRect[] wordRects, SKPaint paint, string words, ushort minFontSize, bool isVertical, ushort step)
        {
            var continueY = true;
            int yLen = pixels.GetLength(0);//画布宽
            int xLen = pixels.GetLength(1);//画布长
            var textArea = GetTextArea(paint, wordRects, words, isVertical);
            var drawAreas = new List<DrawArea>(5000);
            for (ushort y = 0; y < yLen && continueY; y += step)
            {
                for (ushort x = 0; x < xLen; x += step)
                {
                    if (x + textArea.Width >= xLen) //超出画布长
                    {
                        break;
                    }
                    if (y + textArea.Height >= yLen) //超出画布宽
                    {
                        continueY = false;
                        break;
                    }
                    if (!pixels.CheckAvailable(x, y, textArea.Width, textArea.Height, minFontSize))
                    {
                        continue;
                    }
                    drawAreas.Add(new DrawArea(textArea, x, y));
                }
            }
            return drawAreas;
        }

        /// <summary>
        /// 计算绘制文本所需要的长和宽
        /// </summary>
        /// <param name="paint"></param>
        /// <param name="wordRects"></param>
        /// <param name="words"></param>
        /// <param name="isVertical"></param>
        /// <returns></returns>
        private TextArea GetTextArea(SKPaint paint, WordRect[] wordRects, string words, bool isVertical)
        {
            SKRect textRect = paint.GetTextRect(words);
            if (isVertical && words.ContainNumberOrLetter())
            {
                return new TextArea(wordRects, textRect, paint, DrawType.Rotational, words);
            }
            else if (isVertical == false)
            {
                return new TextArea(wordRects, textRect, paint, DrawType.Horizontal, words);
            }
            else
            {
                return new TextArea(wordRects, textRect, paint, DrawType.Vertical, words);
            }
        }

        /// <summary>
        /// 获取词汇中所有字的长宽
        /// </summary>
        /// <param name="paint"></param>
        /// <param name="words"></param>
        /// <returns></returns>
        private WordRect[] GetWordRects(SKPaint paint, string words)
        {
            var wordRects = new List<WordRect>();
            foreach (var word in words)
            {
                var rect = paint.GetTextRect(word.ToString());
                var wordRect = new WordRect(rect, word);
                wordRects.Add(wordRect);
            }
            return wordRects.ToArray();
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
