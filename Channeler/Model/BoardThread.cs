using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Channeler.Model
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Post
    {
        public int no { get; set; }
        public int sticky { get; set; }
        public int closed { get; set; }
        public string now { get; set; }
        public string name { get; set; }
        public string sub { get; set; }
        public string? com { get; set; }
        public string filename { get; set; }
        public string ext { get; set; }
        public string fullFile => filename + ext;
        public string fileInfo => $"({fsize} KB, {w}x{h})";
        public int w { get; set; }
        public int h { get; set; }
        public int tn_w { get; set; }
        public int tn_h { get; set; }
        public object tim { get; set; }
        public string? imageThumbUrl { get; set; }
        public string? imageUrl { get; set; }
        public int time { get; set; }
        public string md5 { get; set; }
        public int fsize { get; set; }
        public int resto { get; set; }
        public string capcode { get; set; }
        public string semantic_url { get; set; }
        public int replies { get; set; }
        public List<Post> RepliesPosts { get; set; } = new List<Post>();
        public List<Post> QuotesPosts { get; set; } = new List<Post>();
        public int images { get; set; }
        public int unique_ips { get; set; }
    }
    public class ThreadPosts
    {
        public List<Post> posts { get; set; }
    }
}
