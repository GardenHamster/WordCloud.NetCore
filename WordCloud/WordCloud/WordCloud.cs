using SkiaSharp;
using WordCloud.Helper;
using WordCloud.Models;

namespace WordCloud
{
    public class WordCloud
    {
        private SKTypeface Typeface { get; init; }

        private bool UseVertical { get; init; }

        /// <summary>
        /// 初始化词云
        /// </summary>
        /// <param name="fontName">字体名称</param>
        /// <param name="useVertical">允许垂直绘制字体</param>
        public WordCloud(string fontName, bool useVertical)
        {
            this.Typeface = SKTypeface.FromFamilyName(fontName);
            this.UseVertical = useVertical;
        }

        /// <summary>
        /// 初始化词云
        /// </summary>
        /// <param name="fontFile">字体文件</param>
        /// <param name="useVertical">允许垂直绘制字体</param>
        public WordCloud(FileInfo fontFile, bool useVertical)
        {
            this.Typeface = SKTypeface.FromFile(fontFile.FullName);
            this.UseVertical = useVertical;
        }

        /// <summary>
        /// 绘制矩形词云
        /// </summary>
        /// <param name="wordItems">关键词和权重</param>
        /// <param name="backColor">背景颜色</param>
        /// <param name="width">长度(像素)</param>
        /// <param name="height">高度(像素)</param>
        /// <param name="fullImageSavePath">图片保存路径</param>
        /// <param name="colors">字体颜色</param>
        /// <param name="format">图片格式</param>
        /// <returns></returns>
        public async Task Draw(List<WordItem> wordItems, SKColor backColor, int width, int height, string fullImageSavePath, SKColor[] colors = null, SKEncodedImageFormat format = SKEncodedImageFormat.Jpeg)
        {
            int maxFontSize = Math.Max(height, width);
            int minFontSize = (int)Math.Ceiling(Convert.ToDecimal(maxFontSize) / 100);
            maxFontSize = maxFontSize / wordItems.First().Word.Length;
            bool[,] pixels = new bool[height, width];
            await Draw(wordItems, pixels, backColor, maxFontSize, minFontSize, fullImageSavePath, colors, format);
        }

        /// <summary>
        /// 根据蒙版绘制词云
        /// </summary>
        /// <param name="wordItems">关键词和权重</param>
        /// <param name="maskImage">蒙版图片文件，需要填充词云区域必须为纯黑色</param>
        /// <param name="backColor">背景颜色</param>
        /// <param name="width">长度(像素)</param>
        /// <param name="fullImageSavePath">图片保存路径</param>
        /// <param name="colors">字体颜色</param>
        /// <param name="format">图片格式</param>
        /// <returns></returns>
        public async Task Draw(List<WordItem> wordItems, FileInfo maskImage, SKColor backColor, int width, string fullImageSavePath, SKColor[] colors = null, SKEncodedImageFormat format = SKEncodedImageFormat.Jpeg)
        {
            bool[,] pixels = DrawHelper.LoadPixels(maskImage, width);
            int height = pixels.GetLength(0);
            int maxFontSize = Math.Max(height, width);
            int minFontSize = (int)Math.Ceiling(Convert.ToDecimal(maxFontSize) / 100);
            maxFontSize = maxFontSize / wordItems.First().Word.Length;
            await Draw(wordItems, pixels, backColor, maxFontSize, minFontSize, fullImageSavePath, colors, format);
        }

        /// <summary>
        /// 绘制矩形词云
        /// </summary>
        /// <param name="wordItems">关键词和权重</param>
        /// <param name="pixels">像素二维数组，true表示已存在像素</param>
        /// <param name="backColor">背景颜色</param>
        /// <param name="maxFontSize">最大字体大小</param>
        /// <param name="minFontSize">最小字体大小</param>
        /// <param name="fullImageSavePath">图片保存路径</param>
        /// <param name="colors">字体颜色</param>
        /// <param name="format">图片格式</param>
        /// <returns></returns>
        public async Task Draw(List<WordItem> wordItems, bool[,] pixels, SKColor backColor, int maxFontSize, int minFontSize, string fullImageSavePath, SKColor[] colors = null, SKEncodedImageFormat format = SKEncodedImageFormat.Jpeg)
        {
            if (maxFontSize <= 0) maxFontSize = 1;
            int fontSize = maxFontSize;
            int width = pixels.GetLength(1);
            int height = pixels.GetLength(0);
            var imgInfo = new SKImageInfo(width, height);
            var drawItems = wordItems.OrderByDescending(o => o.Weight).ToList();
            var sumWeight = drawItems.Sum(o => o.Weight);
            var wordAndScales = drawItems.Select(o => new WordScale(o.Word, o.Weight / sumWeight)).ToList();
            var wordAndScale = wordAndScales.First();
            var sumScale = wordAndScales.First().Scale;
            using SKSurface surface = SKSurface.Create(imgInfo);
            SKCanvas canvas = surface.Canvas;
            canvas.Clear(backColor);
            while (fontSize >= minFontSize && wordAndScales.Count > 0)
            {
                var scale = maxFontSize * (1 - sumScale);
                if (scale < minFontSize) scale = minFontSize;
                fontSize = (int)Math.Min(fontSize, scale);
                var drawArea = DrawWords(canvas, pixels, wordAndScale.Word, fontSize, minFontSize, colors);
                if (drawArea is null)
                {
                    fontSize--;
                }
                else
                {
                    wordAndScales.RemoveAt(0);
                    wordAndScale = wordAndScales.FirstOrDefault();
                    if (wordAndScale is not null) sumScale += wordAndScale.Scale;
                    DrawHelper.UpdatePixels(surface, backColor, drawArea, pixels);
                }
            }
            using SKImage image = surface.Snapshot();
            using SKData data = image.Encode(format, 100);
            using FileStream outputStream = File.OpenWrite(fullImageSavePath);
            data.SaveTo(outputStream);
            await Task.CompletedTask;
        }

        private DrawArea DrawWords(SKCanvas canvas, bool[,] pixels, string words, int fontSize, int minFontSize, SKColor[] colors = null)
        {
            var paint = DrawHelper.CreatePaint(Typeface, fontSize, colors);
            List<DrawArea> drawAreas = new List<DrawArea>();
            drawAreas.AddRange(DrawHelper.GetDrawAreas(pixels, paint, words, minFontSize, false));
            if (UseVertical) drawAreas.AddRange(DrawHelper.GetDrawAreas(pixels, paint, words, minFontSize, true));
            if (drawAreas.Count == 0) return null;
            DrawArea randomArea = drawAreas[new Random().Next(drawAreas.Count)];
            DrawHelper.DrawText(canvas, randomArea);
            return randomArea;
        }

    }
}
