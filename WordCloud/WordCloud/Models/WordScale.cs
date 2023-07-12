using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
