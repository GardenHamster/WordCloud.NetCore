using SkiaSharp;
using WordCloud.Helper;
using WordCloud.Models;

namespace WordCloud
{
    public class WordCloud
    {
        public SKTypeface Typeface { get; init; }

        public SKColor[] Colors { get; init; }

        public SKColor BackColor { get; init; }

        public bool UseVertical { get; init; }

        public int Step { get; init; }

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
            int maxFontSize = Math.Min(height, width);
            int minFontSize = (int)Math.Ceiling(Convert.ToDecimal(maxFontSize) / 100);
            maxFontSize = maxFontSize / words.First().Length;
            bool[,] pixels = new bool[height, width];
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
            int maxFontSize = Math.Min(height, width);
            int minFontSize = (int)Math.Ceiling(Convert.ToDecimal(maxFontSize) / 100);
            maxFontSize = maxFontSize / words.First().Length;
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
            var imgInfo = new SKImageInfo(width, height);
            int fontSize = maxFontSize;
            using SKSurface surface = SKSurface.Create(imgInfo);
            SKCanvas canvas = surface.Canvas;
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
                    DrawHelper.UpdatePixels(surface, BackColor, drawArea, pixels, minFontSize);
                }
            }
            var saveDirPath = Path.GetDirectoryName(fullImageSavePath);
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
            var paint = DrawHelper.CreatePaint(Typeface, fontSize, Colors);
            bool isVertical = new Random().Next(2) < 1;
            List<DrawArea> drawAreas = new List<DrawArea>();
            if (UseVertical && isVertical)
            {
                drawAreas.AddRange(DrawHelper.GetDrawAreas(pixels, paint, words, minFontSize, true, Step));
            }
            else
            {
                drawAreas.AddRange(DrawHelper.GetDrawAreas(pixels, paint, words, minFontSize, false, Step));
            }

            if (drawAreas.Count == 0 && UseVertical && isVertical == false)
            {
                drawAreas.AddRange(DrawHelper.GetDrawAreas(pixels, paint, words, minFontSize, true, Step));
            }

            if (drawAreas.Count == 0) return null;
            DrawArea randomArea = drawAreas[new Random().Next(drawAreas.Count)];
            DrawHelper.DrawText(canvas, randomArea);
            return randomArea;
        }

    }
}
