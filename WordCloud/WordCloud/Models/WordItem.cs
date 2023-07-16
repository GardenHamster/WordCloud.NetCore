namespace WordCloud.Models
{
    public record WordItem
    {
        public string Word { get; set; }

        public double Weight { get; set; }

        public WordItem(string word, double weight)
        {
            this.Word = word;
            this.Weight = weight;
        }

    }
}
