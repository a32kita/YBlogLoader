using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using YBLoader.CoreLib.Models;

namespace YBLoader.CoreLib
{
    public class YBlogArticleLoader
    {
        // 公開プロパティ

        public YBlogArticleDocument Document
        {
            get;
            private set;
        }


        // コンストラクタ

        /// <summary>
        /// <see cref="YBlogArticleDocument"/> を指定して、 <see cref="YBlogArticleLoader"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="document"></param>
        public YBlogArticleLoader(YBlogArticleDocument document)
        {
            this.Document = document;
        }


        // 公開メソッド

        public YBlogAttachedImageInfo[] GetAttachedImages()
        {
            throw new NotImplementedException();
        }

        public YBlogComment[] GetComments()
        {
            var result = new List<YBlogComment>();
            using (var sr = new StreamReader(new MemoryStream(this.Document.SourceRawData), Encoding.GetEncoding("euc-jp")))
            {
                while (!sr.EndOfStream)
                {
                    var comment = new YBlogComment();

                    // 本文テキスト
                    if (!sr.ReadLine().Contains("<p class=\"commentBody\">"))
                        continue;
                    comment.Text = sr.ReadLine().Trim();

                    // 日時
                    var dateTimeKeyword = "<p class=\"commentInfo\"><span class=\"date\">";
                    while (!sr.EndOfStream)
                    {
                        var line = sr.ReadLine();
                        if (!line.Contains(dateTimeKeyword))
                            continue;

                        var dateTimeText = line
                            .Replace(dateTimeKeyword, "\n").Split('\n')[1]
                            .Replace("</span>", "\n").Split('\n')[0];

                        var dt = DateTime.MinValue;
                        if (!DateTime.TryParse(dateTimeText, out dt))
                            dt = DateTime.MinValue;

                        comment.PostedAt = dt;
                        break;
                    }

                    // 名前
                    var name = sr.ReadLine()
                        .Replace("<span class=\"name\">", "\n").Split('\n')[1]
                        .Replace("</span>", "\n").Split('\n')[0];
                    if (name.Contains("target=\"_blank\">"))
                    {
                        name = name
                            .Replace("target=\"_blank\">", "\n").Split('\n')[1]
                            .Replace("</a>", "\n").Split('\n')[0];
                    }

                    comment.Author = name;
                    result.Add(comment);
                }
            }

            return result.ToArray();
        }
    }
}
