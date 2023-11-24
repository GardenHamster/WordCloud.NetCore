using WordCloud.Type;

namespace WordCloud.Models
{
    internal class DrawArea
    {
        public ushort StartX { get; init; }
        public ushort StartY { get; init; }
        public TextArea TextArea { get; init; }
        public ushort Width => TextArea.Width;
        public ushort Height => TextArea.Height;
        public ushort EndX => (ushort)(StartX + Width);
        public ushort EndY => (ushort)(StartY + Height);
        public DrawType DrawType => TextArea.DrawType;
        

        public DrawArea(TextArea textArea, ushort startX, ushort startY)
        {
            this.TextArea = textArea;
            this.StartX = startX;
            this.StartY = startY;
        }

    }
}
