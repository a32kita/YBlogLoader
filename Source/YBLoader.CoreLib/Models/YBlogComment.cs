using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YBLoader.CoreLib.Models
{
    public struct YBlogComment
    {
        public string Author
        {
            get;
            set;
        }

        public string Text
        {
            get;
            set;
        }

        public DateTime PostedAt
        {
            get;
            set;
        }
    }
}
