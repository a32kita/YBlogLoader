using System;
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

            Console.WriteLine("Title={0}", blogInfo.Title);
            Console.WriteLine("Author={0}", blogInfo.Author);
            Console.WriteLine("Est={0}", blogInfo.EstablishmentAt);

            Console.WriteLine();
            var articleInfo = Task.Run(() => loader.GetArticleMetaInfoAsync(blogInfo, "16558709")).Result;

            Console.WriteLine("Id={0}", articleInfo.Id);
            Console.WriteLine("Subject={0}", articleInfo.Subject);
            Console.WriteLine("PublishedAt={0}", articleInfo.PublishedAt);
            Console.WriteLine("ArchiveGroup={0}", articleInfo.ArchiveGroup);

            Console.WriteLine();
            Console.Write("Completed!!");
            Console.ReadKey();
        }
    }
}
