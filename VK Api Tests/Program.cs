using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Threading;
using Newtonsoft.Json.Linq;
using System.Reflection.Emit;
using System.Windows;
using VK_Api_Tests.VK;
using VK_Api_Tests.VK.Model;

namespace VK_Api_Tests
{
    internal class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            Console.InputEncoding = Console.OutputEncoding = Encoding.GetEncoding(@"Cyrillic");

            var api = VkApi.AuthNew("audio,photos,groups,messages", 4022821);
            var someUser = api.UsersGetSingle("ar4ebald");

            api.MessagesSend(someUser.Id, "Можно так");

            api.RunMethod("messages.send", 
                "user_id", someUser.Id,
                "message", "А можно и так :)", 
                "guid", new Random().Next());

            using (var client = new WebClient())
            {
                var response = client.DownloadString(string.Format("{0}messages.send?user_id={1}&message={2}&v={3}&access_token={4}",
                    VkApi.ApiRoot,
                    someUser.Id,
                    WebUtility.UrlEncode("Или даже так! Привет, я " + Environment.UserName),
                    VkApi.ApiVersion,
                    api.Token));
            }
        }
    }
}