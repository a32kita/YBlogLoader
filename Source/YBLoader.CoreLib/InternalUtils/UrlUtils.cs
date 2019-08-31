using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using YBLoader.CoreLib.Models;

namespace YBLoader.CoreLib.InternalUtils
{
    internal static class UrlUtils
    {
        public static string BlogIdToUrl(string id)
        {
            return $"https://blogs.yahoo.co.jp/{id}";
        }

        public static string ArticleIdToUrl(YBlogInfo blogInfo, string articleId)
        {
            return $"{blogInfo.BaseUrl}/{articleId}.html";
        }
    }
}
