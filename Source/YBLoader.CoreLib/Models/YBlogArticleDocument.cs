using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YBLoader.CoreLib.Models
{
    public struct YBlogArticleDocument
    {
        public YBlogArticleMetaInfo MetaInfo
        {
            get;
            set;
        }

        public byte[] SourceRawData
        {
            get;
            set;
        }
    }
}
