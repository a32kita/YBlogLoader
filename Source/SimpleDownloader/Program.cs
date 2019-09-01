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
            start:

            Console.Write("Enter blog id>");
            var blogId = Console.ReadLine();

            var loader = new YBlogLoader();
            var blogInfo = Task.Run(() => loader.GetBlogInfoAsync(blogId)).Result;

            if (String.IsNullOrEmpty(blogInfo.Title) && String.IsNullOrEmpty(blogInfo.Author))
            {
                Console.WriteLine("Failured to getting blog information.");
                goto start;
            }

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
                            var articleDocument = Task.Run(() => loader.GetArticleDocumentAsync(blogInfo, articleIdInfo.Id)).Result;
                            var articleMetaInfo = articleDocument.MetaInfo;

                            var articleSaveDir = Path.Combine(saveDir, articleMetaInfo.ArchiveGroup);
                            if (!Directory.Exists(articleSaveDir))
                                Directory.CreateDirectory(articleSaveDir);

                            var createdAt = String.Format("{0:00}{1:00}{2:00}",
                                articleMetaInfo.PublishedAt.Year % 100,
                                articleMetaInfo.PublishedAt.Month,
                                articleMetaInfo.PublishedAt.Day);

                            var articleFilePath = Path.Combine(articleSaveDir, createdAt + "_" + articleMetaInfo.Id + "_" + articleMetaInfo.Subject
                                .Replace("*", "＊")
                                .Replace("?", "？")
                                .Replace("/", "／")
                                .Replace("\\", "￥")
                                .Replace(":", "：")
                                .Replace("\"", "”")
                                .Replace("@", "＠")
                                .Replace("|", "｜")
                                + ".html");

                            using (var bw = new BinaryWriter(File.OpenWrite(articleFilePath)))
                                bw.Write(articleDocument.SourceRawData);

                            Console.WriteLine("Article is saved!!: {0} {1}", articleMetaInfo.Id, articleMetaInfo.Subject);
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
    }
}
