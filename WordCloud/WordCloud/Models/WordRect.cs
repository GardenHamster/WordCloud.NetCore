using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordCloud.NetCore.Models
{
    internal class WordRect
    {
        public char Word { get; init; }

        public SKRect Rect { get; set; }

        public ushort Width => (ushort)Rect.Width;

        public ushort Height => (ushort)Rect.Height;

        public WordRect(SKRect rect, char word)
        {
            this.Rect = rect;
            this.Word = word;
        }

    }
}
