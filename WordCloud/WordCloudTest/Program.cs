using SkiaSharp;
using WordCloud.Models;

var txt_en = File.ReadAllText(@"Text\txt_en.txt");
var txt_zh = File.ReadAllText(@"Text\txt_zh.txt");

var wordWeightPairs1 = new JiebaNet.Analyser.TfidfExtractor().ExtractTagsWithWeight(txt_zh, 250);
var wordWeightPairs2 = new JiebaNet.Analyser.TfidfExtractor().ExtractTagsWithWeight(txt_en, 250);

var wordItems1 = wordWeightPairs1.Select(o => new WordItem(o.Word, o.Weight)).ToList();
var wordItems2 = wordWeightPairs2.Select(o => new WordItem(o.Word, o.Weight)).ToList();

var wordCloud = new WordCloud.WordCloud(new FileInfo("Fonts\\幼圆.TTF"), true);

var debug = false;

if (debug)
{
    await wordCloud.Draw(wordItems1, SKColors.White, 1000, 1000, "E:\\test\\wordcloud.jpg");
}
else
{
    Task task1 = wordCloud.Draw(wordItems1, SKColors.White, 500, 1200, "E:\\test\\wordcloud1.jpg");
    Task task2 = wordCloud.Draw(wordItems1, SKColors.White, 1200, 500, "E:\\test\\wordcloud2.jpg");
    Task task3 = wordCloud.Draw(wordItems1, SKColors.White, 1000, 1000, "E:\\test\\wordcloud3.jpg", new[] { SKColors.LightGreen, SKColors.LightPink, SKColors.LightBlue });
    Task task4 = wordCloud.Draw(wordItems1, SKColors.White, 300, 300, "E:\\test\\wordcloud4.jpg");
    Task task5 = wordCloud.Draw(wordItems2, SKColors.White, 1000, 1000, "E:\\test\\wordcloud5.jpg");
    Task task6 = wordCloud.Draw(wordItems1, new FileInfo("Mask\\mask.png"), SKColors.White, 1000, "E:\\test\\wordcloud6.jpg");
    Task task7 = wordCloud.Draw(wordItems2, new FileInfo("Mask\\mask.png"), SKColors.White, 1000, "E:\\test\\wordcloud7.jpg");
    Task.WaitAll(task1, task2, task3, task4, task5, task6, task7);
}









