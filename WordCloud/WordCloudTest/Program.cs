var txt_en = File.ReadAllText(@"Text\txt_en.txt");
var txt_zh = File.ReadAllText(@"Text\txt_zh.txt");

var wordWeightPairs1 = new JiebaNet.Analyser.TfidfExtractor().ExtractTagsWithWeight(txt_zh, 500).OrderByDescending(o => o.Weight);
var wordWeightPairs2 = new JiebaNet.Analyser.TfidfExtractor().ExtractTagsWithWeight(txt_en, 500).OrderByDescending(o => o.Weight);

var wordItems1 = wordWeightPairs1.Select(o => o.Word).ToList();
var wordItems2 = wordWeightPairs2.Select(o => o.Word).ToList();

var debug = true;
var fontFile = new FileInfo("Fonts\\hywenhei85w.ttf");
var maskFile = new FileInfo("Mask\\mask.png");

if (debug == true)
{
    using var wordCloud = new WordCloud.WordCloud(fontFile, true, 3);
    await wordCloud.Draw(wordItems1, 1000, 1000, "E:\\test\\wordcloud.jpg");
}

if (debug == false)
{
    using var wordCloud = new WordCloud.WordCloud(fontFile, true, 3);
    Task task1 = wordCloud.Draw(wordItems1, 500, 1200, "E:\\test\\wordcloud1.jpg");
    Task task2 = wordCloud.Draw(wordItems1, 1200, 500, "E:\\test\\wordcloud2.jpg");
    Task task3 = wordCloud.Draw(wordItems1, 1000, 1000, "E:\\test\\wordcloud3.jpg");
    Task task4 = wordCloud.Draw(wordItems1, 300, 300, "E:\\test\\wordcloud4.jpg");
    Task task5 = wordCloud.Draw(wordItems2, 1000, 1000, "E:\\test\\wordcloud5.jpg");
    Task task6 = wordCloud.Draw(wordItems1, maskFile, 1500, "E:\\test\\wordcloud6.jpg");
    Task task7 = wordCloud.Draw(wordItems2, maskFile, 1500, "E:\\test\\wordcloud7.jpg");
    Task.WaitAll(task1, task2, task3, task4, task5, task6, task7);
}

Console.ReadLine();









