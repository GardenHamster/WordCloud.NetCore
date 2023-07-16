using SkiaSharp;

namespace WordCloud.Helper
{
    internal static class RandomHelper
    {
        private static readonly Random random = new Random();

        /// <summary>
        /// 随机获取数组中的一个项目
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static T Random<T>(this T[] arr)
        {
            int randomIndex = random.Next(arr.Length);
            return arr[randomIndex];
        }

        /// <summary>
        /// 随机颜色
        /// </summary>
        /// <returns></returns>
        public static SKColor RandomColor()
        {
            byte red = (byte)random.Next(0, 255);
            byte green = (byte)random.Next(0, 255);
            byte blue = (byte)random.Next(0, 255);
            return new SKColor(red, green, blue);
        }

    }
}
