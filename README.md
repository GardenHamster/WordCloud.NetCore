## 简介
* 一个使用`SkiaSharp`制作的，基于`.net core 6.0` 的跨平台词云库，思路和流程参考自[AmmRage/WordCloudSharp](https://github.com/AmmRage/WordCloudSharp)

## 安装
- 从Nuget中添加 [WordCloud.NetCore](https://www.nuget.org/packages/WordCloud.NetCore) 包引用

## 生成矩形词云
``` csharp
var wordCloud = new WordCloud.WordCloud(fontFile, true);
await wordCloud.Draw(words, 1000, 1000, "E:\\test\\wordcloud.jpg");
```
![wordcloud3](https://github.com/GardenHamster/WordCloud.NetCore/assets/89188316/dd88899f-f6b2-493a-8ed9-4e7ad9024107)
![wordcloud5](https://github.com/GardenHamster/WordCloud.NetCore/assets/89188316/c0a9a213-6b87-4768-8233-1aa6cb0e03c2)

## 通过蒙版生成
* **你需要准备一张黑色(#000000)的蒙版图片，比如这样：**
* ![](https://github.com/GardenHamster/WordCloud.NetCore/blob/main/WordCloud/WordCloudTest/Mask/mask.png)
* **图片在经过等比例放大/缩小后，再将词填充到黑色(#000000)区域内**
``` csharp
var fontFile = new FileInfo("Fonts\\hywenhei85w.ttf");
var maskFile = new FileInfo("Mask\\mask.png");
var wordCloud = new WordCloud.WordCloud(fontFile, true);
await wordCloud.Draw(words, maskFile, 1500, "E:\\test\\wordcloud.jpg");
```
![wordcloud6](https://github.com/GardenHamster/WordCloud.NetCore/assets/89188316/125c2a11-ed64-4ce8-b2e2-3493baa755f5)
![wordcloud7](https://github.com/GardenHamster/WordCloud.NetCore/assets/89188316/1dfa3859-9a14-4f84-8c1e-5333c616555c)

## 自定义颜色
``` csharp
var colors = new SKColor[] { SKColors.LightBlue, SKColors.LightGreen, SKColors.LightPink, SKColors.LightSeaGreen };
var wordCloud = new WordCloud.WordCloud(fontFile, true, 3, SKColors.White, colors);
await wordCloud.Draw(words, 500, 500, "E:\\test\\wordcloud.jpg");
```
![wordcloud](https://github.com/GardenHamster/WordCloud.NetCore/assets/89188316/4ebc2d95-dada-480e-87a1-4cef67adefec)
![wordcloud](https://github.com/GardenHamster/WordCloud.NetCore/assets/89188316/e3528ead-5c79-469b-b8f2-a53b2abaf9b6)
