﻿using System.Text.RegularExpressions;

namespace WordCloud.Helper
{
    internal static class StringHelper
    {
        /// <summary>
        /// 判断一个字符串是否包含数字或英文字母
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool ContainNumberOrLetter(this string str)
        {
            return Regex.IsMatch(str, @"^[A-Za-z0-9]+$");
        }


    }
}
