using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using YBLoader.CoreLib;
using YBLoader.CoreLib.Models;

namespace SimpleDownloader
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Enter blog id>");
            var blogId = Console.ReadLine();

            var loader = new YBlogLoader();
            var blogInfo = Task.Run(() => loader.GetBlogInfoAsync(blogId)).Result;

            Console.WriteLine("Title={0}", blogInfo.Title);
            Console.WriteLine("Author={0}", blogInfo.Author);
            Console.WriteLine("Est={0}", blogInfo.EstablishmentAt);
            Console.WriteLine();
            Console.Write("Please press any key to starting download...");
            Console.ReadKey();

            var saveDir = Path.Combine(
                Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location),
                "Articles",
                blogId);

            var httpClient = new HttpClient();

            for (var i = 1; true; i++)
            {
                try
                {
                    var articles = Task.Run(() => loader.GetArticleIdsAsync(blogInfo, i)).Result;
                    if (articles.Count == 0)
                        break;

                    foreach (var articleIdInfo in articles)
                    {
                        try
                        {
                            var articleInfo = Task.Run(() => loader.GetArticleMetaInfoAsync(blogInfo, articleIdInfo.Id)).Result;

                            var articleSaveDir = Path.Combine(saveDir, articleInfo.ArchiveGroup);
                            if (!Directory.Exists(articleSaveDir))
                                Directory.CreateDirectory(articleSaveDir);

                            var articleFilePath = Path.Combine(articleSaveDir, articleInfo.Id + "_" + articleInfo.Subject
                                .Replace("*", "＊")
                                .Replace("?", "？")
                                .Replace("/", "／")
                                .Replace("\\", "￥")
                                .Replace(":", "：")
                                .Replace("\"", "”")
                                .Replace("@", "＠")
                                .Replace("|", "｜")
                                + ".html");

                            Task.Run(() => SaveArticle(httpClient, articleInfo, articleFilePath));
                            Console.WriteLine("Article is saved!!: {0} {1}", articleInfo.Id, articleInfo.Subject);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.GetType().Name);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.GetType().Name);
                }
            }
        }

        static async Task SaveArticle(HttpClient httpClient, YBlogArticleMetaInfo metaInfo, string savePath)
        {
            using (var httpResponse = await httpClient.GetAsync(metaInfo.Url))
            using (var br = new BinaryReader(await httpResponse.Content.ReadAsStreamAsync()))
            {
                var buffer = br.ReadBytes((int)br.BaseStream.Length);
                using (var bw = new BinaryWriter(File.OpenWrite(savePath)))
                    bw.Write(buffer);
            }
        }
    }
}
