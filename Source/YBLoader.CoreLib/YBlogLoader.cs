using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using YBLoader.CoreLib.InternalUtils;
using YBLoader.CoreLib.Models;

namespace YBLoader.CoreLib
{
    public class YBlogLoader
    {
        // 非公開フィールド
        private HttpClient _httpClient;


        // コンストラクタ

        /// <summary>
        /// <see cref="YBLoader"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        public YBlogLoader()
        {
            this._httpClient = new HttpClient();
        }


        // 公開メソッド

        #region GET: YBlogInfo

        /// <summary>
        /// ブログの基本的な情報を表す <see cref="YBlogInfo"/> を取得します。
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<YBlogInfo> GetBlogInfoAsync(string id)
        {
            var result = new YBlogInfo();
            result.Id = id;
            result.BaseUrl = UrlUtils.BlogIdToUrl(id);

            using (var httpResponse = await this._httpClient.GetAsync(result.BaseUrl))
            using (var httpStream = await httpResponse.Content.ReadAsStreamAsync())
            using (var sr = new StreamReader(httpStream, Encoding.GetEncoding("euc-jp")))
            {
                // タイトル
                // <title>０と１の世界の見習い探検家Ⅱ - Yahoo!ブログ</title>
                var titleKeyword = " - Yahoo!ブログ</title>";
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();
                    if (!line.Contains(titleKeyword))
                        continue;

                    line = line.Replace("<title>", "\n").Replace(titleKeyword, "\n");
                    result.Title = line.Split('\n')[1];
                    break;
                }

                // 著者名
                // <a class="usercard__link" href="/PROFILE/T83csZQwKPTtQBP4wzgI">あおと</a>
                var authorKeyword = "<a class=\"usercard__link\" href=\"";
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();
                    if (!line.Contains(authorKeyword))
                        continue;

                    line = line.Replace(authorKeyword, "\n").Split('\n')[1]
                        .Replace("\">", "\n").Split('\n')[1]
                        .Replace("</a>", "\n").Split('\n')[0];
                    result.Author = line;
                    break;
                }

                // 開設日
                // document.write(zwsp("開設日: 2011/7/21(木) "));
                var estDateKeyword = "document.write(zwsp(\"開設日: ";
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();
                    if (!line.Contains(estDateKeyword))
                        continue;

                    line = line.Replace(estDateKeyword, "\n").Split('\n')[1]
                        .Replace("(", "\n").Split('\n')[0];

                    var vals = line.Split('/').Select(Int32.Parse).ToArray();
                    result.EstablishmentAt = new DateTime(vals[0], vals[1], vals[2]);
                    break;
                }
            }

            return result;
        }

        #endregion

        #region GET: YBlogArticleMetaInfo

        /// <summary>
        /// 記事の基本的な情報を表す <see cref="YBlogArticleMetaInfo"/> を取得します。
        /// </summary>
        /// <param name="blogInfo"></param>
        /// <param name="articleId"></param>
        /// <returns></returns>
        public async Task<YBlogArticleMetaInfo> GetArticleMetaInfoAsync(YBlogInfo blogInfo, string articleId)
        {
            return await this.GetArticleMetaInfoAsync(UrlUtils.ArticleIdToUrl(blogInfo, articleId));
        }

        /// <summary>
        /// 記事の基本的な情報を表す <see cref="YBlogArticleMetaInfo"/> を取得します。
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<YBlogArticleMetaInfo> GetArticleMetaInfoAsync(string url)
        {
            var result = new YBlogArticleMetaInfo();
            result.Url = new Uri(url);
            result.Id = url.Split('/').Last().Split('.')[0];

            using (var httpResponse = await this._httpClient.GetAsync(result.Url))
            using (var httpStream = await httpResponse.Content.ReadAsStreamAsync())
            using (var sr = new StreamReader(httpStream, Encoding.GetEncoding("euc-jp")))
            {
                // 件名
                // <span itemprop="headline">2019年07月17日 本日の乗車記録</span>
                var subjectKeyword = "<span itemprop=\"headline\">";
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();
                    if (!line.Contains(subjectKeyword))
                        continue;

                    line = line.Replace(subjectKeyword, "\n").Split('\n')[1]
                        .Replace("</span>", "\n").Split('\n')[0];
                    result.Subject = line;

                    break;
                }

                // 投稿日時
                // <span itemprop="datePublished" content="2019-08-21T23:25:00+09:00">2019/8/21(水) 午後 11:25</span>
                var pubKeyword = "<span itemprop=\"datePublished\" content=\"";
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();
                    if (!line.Contains(pubKeyword))
                        continue;

                    line = line.Replace(pubKeyword, "\n").Split('\n')[1]
                        .Replace("\">", "\n").Split('\n')[0];
                    result.PublishedAt = DateTime.Parse(line);

                    break;
                }

                // 書庫名
                // <li class="library"><a href="https://blogs.yahoo.co.jp/a32kita/folder/437705.html">乗車記録</a></li>
                var groupKeyword = "<li class=\"library\"><a href=\"";
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();
                    if (!line.Contains(groupKeyword))
                        continue;

                    line = line.Replace(groupKeyword, "\n").Split('\n')[1]
                        .Replace("\">", "\n").Split('\n')[1]
                        .Replace("</a>", "\n").Split('\n')[0];
                    result.ArchiveGroup = line;

                    break;
                }
            }

            return result;
        }

        #endregion


        /// <summary>
        /// 全ての記事一覧のページから記事の ID を取得します。
        /// </summary>
        /// <param name="blogInfo"></param>
        /// <param name="pageNum"></param>
        /// <returns></returns>
        public async Task<YBlogArticleIdCollection> GetArticleIdsAsync(YBlogInfo blogInfo, int pageNum)
        {
            var result = new YBlogArticleIdCollection();
            var url = UrlUtils.BlogIdToAllArticlesPageUrl(blogInfo, pageNum);

            using (var httpResponse = await this._httpClient.GetAsync(url))
            using (var httpStream = await httpResponse.Content.ReadAsStreamAsync())
            using (var sr = new StreamReader(httpStream, Encoding.GetEncoding("euc-jp")))
            {
                // 記事 ID
                // <div class="clearFix entryTitle myblog" data-title="2019年07月11日 本日の乗車記録" data-articleId="16558706" >
                var idKeyword = "<div class=\"clearFix entryTitle";
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();
                    if (!line.Contains(idKeyword))
                        continue;

                    var id = line.Replace("data-articleId=\"", "\n").Split('\n')[1]
                        .Replace("\"", "\n").Split('\n')[0];

                    result.Add(new YBlogArticleIdInfo()
                    {
                        Id = line.Replace("data-articleId=\"", "\n").Split('\n')[1].Replace("\"", "\n").Split('\n')[0],
                        Subject = line.Replace("data-title=\"", "\n").Split('\n')[1].Replace("\" data", "\n").Split('\n')[0],
                    });
                }
            }

            return result;
        }


    }
}
