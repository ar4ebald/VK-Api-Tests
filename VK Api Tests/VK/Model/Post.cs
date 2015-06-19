using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace VK_Api_Tests.VK.Model
{
    public class Post : VkObject
    {
        public int Id { get; set; }
        public int OwnerId { get; set; }
        public int FromId { get; set; }
        public int Date { get; set; }
        public string Text { get; set; }

        public LikesClass Likes { get; set; }
        public RepostsClass Reposts { get; set; }

        public Post(JToken json) : base(json) { }


        public class LikesClass : VkObject
        {
            public int Count { get; set; }
            public bool UserLikes { get; set; }
            public bool CanLike { get; set; }
            public bool CanPublish { get; set; }

            public LikesClass(JToken json) : base(json) { }
        }

        public class RepostsClass : VkObject
        {
            public int Count { get; set; }
            public bool UserReposted { get; set; }

            public RepostsClass(JToken json) : base(json) { }
        }

    }
}
