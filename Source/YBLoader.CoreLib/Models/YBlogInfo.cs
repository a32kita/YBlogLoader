using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YBLoader.CoreLib.Models
{
    public struct YBlogInfo
    {
        // 公開プロパティ

        /// <summary>
        /// ブログのタイトルを取得または設定します。
        /// </summary>
        public string Title
        {
            get;
            set;
        }

        /// <summary>
        /// ブログの著者名を取得または設定します。
        /// 例: あおと
        /// </summary>
        public string Author
        {
            get;
            set;
        }

        /// <summary>
        /// ブログの ID を取得または設定します。
        /// 例: a32kita
        /// </summary>
        public string Id
        {
            get;
            set;
        }

        /// <summary>
        /// ブログの基本 URL を取得または設定します。
        /// 例: https://blogs.yahoo.co.jp/a32kita
        /// </summary>
        public string BaseUrl
        {
            get;
            set;
        }

        /// <summary>
        /// ブログの開設日を取得します。
        /// </summary>
        public DateTime EstablishmentAt
        {
            get;
            set;
        }
    }
}
