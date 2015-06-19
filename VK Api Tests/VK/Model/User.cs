using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace VK_Api_Tests.VK.Model
{
    public class User : VkObject
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Deactivated { get; set; }
        public bool Hidden { get; set; }

        public User(JToken json) : base(json) { }
    }
}
