using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YBLoader.CoreLib.Models
{
    public struct YBlogArticleMetaInfo
    {
        /// <summary>
        /// ID を取得または設定します。
        /// </summary>
        public string Id
        {
            get;
            set;
        }

        /// <summary>
        /// 記事の件名を取得または設定します。
        /// </summary>
        public string Subject
        {
            get;
            set;
        }

        /// <summary>
        /// 記事の URL を取得または設定します。
        /// </summary>
        public Uri Url
        {
            get;
            set;
        }

        /// <summary>
        /// 記事の投稿日時を取得または設定します。
        /// </summary>
        public DateTime PublishedAt
        {
            get;
            set;
        }

        /// <summary>
        /// 登録先書庫名を取得または設定します。
        /// </summary>
        public string ArchiveGroup
        {
            get;
            set;
        }
    }
}
