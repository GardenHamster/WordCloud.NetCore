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
            int maxFontSize = height;
            SKColor backColor = SKColors.White;
            bool[,] pixels = new bool[height, width];
            var imgInfo = new SKImageInfo(width, height);
            using SKSurface surface = SKSurface.Create(imgInfo);
            SKCanvas canvas = surface.Canvas;
            canvas.Clear(backColor);
            wordItems = wordItems.OrderByDescending(o => o.Weight).ToList();
            var sumWeight = wordItems.Sum(o => o.Weight);
            var wordScales = wordItems.Select(o => new WordScale(o.Word, o.Weight / sumWeight)).ToList();
            while (maxFontSize > 10 && wordScales.Count > 0)
            {
                var wordScale = wordScales.First();
                int fontSize = (int)Math.Ceiling(maxFontSize * (wordScale.Scale + 0.2));
                var drawArea = Draw(canvas, pixels, wordScale.Word, fontSize);
                if (drawArea is null)
                {
                    maxFontSize--;
                }
                else
                {
                    wordScales.RemoveAt(0);
                    DrawHelper.UpdatePixels(surface, backColor, drawArea, pixels);
                }
            }
            using SKImage image = surface.Snapshot();
            using SKData data = image.Encode(SKEncodedImageFormat.Jpeg, 100);
            using FileStream outputStream = File.OpenWrite(fullImageSavePath);
            data.SaveTo(outputStream);
            await Task.CompletedTask;
        }

        private DrawArea? Draw(SKCanvas canvas, bool[,] pixels, string words, int fontSize)
        {
            var paint = DrawHelper.CreatePaint(Typeface, fontSize);
            List<DrawArea> drawAreas = new List<DrawArea>();
            drawAreas.AddRange(DrawHelper.GetDrawAreas(pixels, paint, words, false));
            if (UseVertical) drawAreas.AddRange(DrawHelper.GetDrawAreas(pixels, paint, words, true));
            if (drawAreas.Count == 0) return null;
            DrawArea randomArea = drawAreas[new Random().Next(drawAreas.Count)];
            DrawHelper.DrawText(canvas, randomArea);
            return randomArea;
        }

    }
}
