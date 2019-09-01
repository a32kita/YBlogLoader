using System;
using System.Collections.Generic;
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
    }
}
