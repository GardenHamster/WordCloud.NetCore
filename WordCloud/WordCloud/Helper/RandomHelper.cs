using SkiaSharp;

namespace WordCloud.Helper
{
    internal static class RandomHelper
    {
        /// <summary>
        /// 随机获取数组中的一个项目
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static T Random<T>(this T[] arr)
        {
            if (arr is null) return default(T);
            if (arr.Length == 0) return default(T);
            var random = new Random();
            int randomIndex = random.Next(arr.Length);
            return arr[randomIndex];
        }

        /// <summary>
        /// 随机获取集合中的一个项目
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static T Random<T>(this List<T> list)
        {
            if (list is null) return default(T);
            if (list.Count == 0) return default(T);
            var random = new Random();
            int randomIndex = random.Next(list.Count);
            return list[randomIndex];
        }

        /// <summary>
        /// 随机颜色
        /// </summary>
        /// <returns></returns>
        public static SKColor RandomColor()
        {
            var random = new Random();
            byte red = (byte)random.Next(0, 255);
            byte green = (byte)random.Next(0, 255);
            byte blue = (byte)random.Next(0, 255);
            return new SKColor(red, green, blue);
        }

    }
}
