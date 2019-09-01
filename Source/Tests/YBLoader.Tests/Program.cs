﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using YBLoader.CoreLib;
using YBLoader.CoreLib.Models;

namespace YBLoader.Tests
{
    public static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            var loader = new YBlogLoader();

            Console.WriteLine("YBlog Loader Test");

            Console.WriteLine();
            var blogInfo = Task.Run(() => loader.GetBlogInfoAsync("a32kita")).Result;

            Console.WriteLine("[ブログ情報]");
            Console.WriteLine("Title={0}", blogInfo.Title);
            Console.WriteLine("Author={0}", blogInfo.Author);
            Console.WriteLine("Est={0}", blogInfo.EstablishmentAt);

            Console.WriteLine();
            var recentlyArticles = Task.Run(() => loader.GetArticleIdsAsync(blogInfo, 1)).Result;

            Console.WriteLine("[最近の記事]");
            foreach (var id in recentlyArticles)
            {
                Console.WriteLine("* {0}: {1}", id.Id, id.Subject);
            }

            Console.WriteLine();
            var articleDocument = Task.Run(() => loader.GetArticleDocumentAsync(blogInfo, recentlyArticles.First().Id)).Result;
            var articleMetaInfo = articleDocument.MetaInfo;

            var articleLoader = new YBlogArticleLoader(articleDocument);
            var articleComments = articleLoader.GetComments();

            Console.WriteLine("[一番最新の記事の詳細]");
            Console.WriteLine("Id={0}", articleMetaInfo.Id);
            Console.WriteLine("Subject={0}", articleMetaInfo.Subject);
            Console.WriteLine("PublishedAt={0}", articleMetaInfo.PublishedAt);
            Console.WriteLine("ArchiveGroup={0}", articleMetaInfo.ArchiveGroup);
            Console.WriteLine("Document Size={0} bytes", articleDocument.SourceRawData.Length);
            Console.WriteLine("Comments={0}", articleComments.Length);

            Console.WriteLine();

            Console.WriteLine("[一番最新の記事のコメント]");
            foreach (var comment in articleComments)
                Console.WriteLine("* {0} by {1} at {2}", comment.Text, comment.Author, comment.PostedAt);
            
            Console.WriteLine();
            Console.Write("Completed!!");
            Console.ReadKey();
        }
    }
}
