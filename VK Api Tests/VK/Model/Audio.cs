using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace VK_Api_Tests.VK.Model
{
    public class Audio : VkObject
    {
        public int Id { get; set; }
        public int OwnerId { get; set; }
        public string Artist { get; set; }
        public string Title { get; set; }
        public int Duration { get; set; }
        public string Url { get; set; }

        public Audio(JToken json) : base(json) { }
    }
}
