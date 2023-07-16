namespace WordCloud.Models
{
    internal class WordScale
    {
        public string Word { get; set; }

        public double Scale { get; set; }

        public WordScale(string word, double scale)
        {
            Word = word;
            Scale = scale;
        }

    }
}
