using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordCloud.Helper;
using WordCloud.Models;

namespace WordCloud
{
    public class WordCloud
    {
        private SKTypeface Typeface { get; init; }

        private bool UseVertical { get; set; }

        public WordCloud(string fontName, bool useVertical)
        {
            this.Typeface = SKTypeface.FromFamilyName(fontName);
            this.UseVertical = useVertical;
        }

        public WordCloud(FileInfo fontFile, bool useVertical)
        {
            this.Typeface = SKTypeface.FromFile(fontFile.FullName);
            this.UseVertical = useVertical;
        }

        public async Task Draw(List<WordItem> wordItems, int width, int height, string fullImageSavePath)
        {
            int maxFontSize = Math.Min(width, height);
            int minFontSize = (int)Math.Ceiling(Convert.ToDecimal(maxFontSize) / 100);
            maxFontSize = maxFontSize / wordItems.First().Word.Length;
            int fontSize = maxFontSize;
            SKColor backColor = SKColors.White;
            bool[,] pixels = new bool[height, width];
            var imgInfo = new SKImageInfo(width, height);
            using SKSurface surface = SKSurface.Create(imgInfo);
            SKCanvas canvas = surface.Canvas;
            canvas.Clear(backColor);
            wordItems = wordItems.OrderByDescending(o => o.Weight).ToList();
            var sumWeight = wordItems.Sum(o => o.Weight);
            var wordScales = wordItems.Select(o => new WordScale(o.Word, o.Weight / sumWeight)).ToList();
            var wordScale = wordScales.First();
            var sumScale = wordScales.First().Scale;
            while (fontSize >= minFontSize && wordScales.Count > 0)
            {
                var scaling = maxFontSize * (1 - sumScale);
                if (scaling < minFontSize) scaling = minFontSize;
                fontSize = (int)Math.Min(fontSize, scaling);
                var drawArea = Draw(canvas, pixels, wordScale.Word, fontSize, minFontSize);
                if (drawArea is null)
                {
                    fontSize--;
                }
                else
                {
                    wordScales.RemoveAt(0);
                    wordScale = wordScales.FirstOrDefault();
                    if (wordScale is not null) sumScale += wordScale.Scale;
                    DrawHelper.UpdatePixels(surface, backColor, drawArea, pixels);
                }
            }
            using SKImage image = surface.Snapshot();
            using SKData data = image.Encode(SKEncodedImageFormat.Jpeg, 100);
            using FileStream outputStream = File.OpenWrite(fullImageSavePath);
            data.SaveTo(outputStream);
            await Task.CompletedTask;
        }

        public async Task Draw(List<WordItem> wordItems, FileInfo maskImage, int width, string fullImageSavePath)
        {
            bool[,] pixels = DrawHelper.LoadPixels(maskImage, width);
            int height = pixels.GetLength(0);
            int maxFontSize = Math.Min(width, height);
            int minFontSize = (int)Math.Ceiling(Convert.ToDecimal(maxFontSize) / 100);
            maxFontSize = maxFontSize / wordItems.First().Word.Length;
            int fontSize = maxFontSize;
            SKColor backColor = SKColors.White;
            var imgInfo = new SKImageInfo(width, height);
            using SKSurface surface = SKSurface.Create(imgInfo);
            SKCanvas canvas = surface.Canvas;
            canvas.Clear(backColor);
            wordItems = wordItems.OrderByDescending(o => o.Weight).ToList();
            var sumWeight = wordItems.Sum(o => o.Weight);
            var wordScales = wordItems.Select(o => new WordScale(o.Word, o.Weight / sumWeight)).ToList();
            var wordScale = wordScales.First();
            var sumScale = wordScales.First().Scale;
            while (fontSize >= minFontSize && wordScales.Count > 0)
            {
                var scaling = maxFontSize * (1 - sumScale);
                if (scaling < minFontSize) scaling = minFontSize;
                fontSize = (int)Math.Min(fontSize, scaling);
                var drawArea = Draw(canvas, pixels, wordScale.Word, fontSize, minFontSize);
                if (drawArea is null)
                {
                    fontSize--;
                }
                else
                {
                    wordScales.RemoveAt(0);
                    wordScale = wordScales.FirstOrDefault();
                    if (wordScale is not null) sumScale += wordScale.Scale;
                    DrawHelper.UpdatePixels(surface, backColor, drawArea, pixels);
                }
            }
            using SKImage image = surface.Snapshot();
            using SKData data = image.Encode(SKEncodedImageFormat.Jpeg, 100);
            using FileStream outputStream = File.OpenWrite(fullImageSavePath);
            data.SaveTo(outputStream);
            await Task.CompletedTask;
        }

        private DrawArea Draw(SKCanvas canvas, bool[,] pixels, string words, int fontSize, int minFontSize)
        {
            var paint = DrawHelper.CreatePaint(Typeface, fontSize);
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
