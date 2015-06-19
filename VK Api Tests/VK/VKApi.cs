using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using VK_Api_Tests.VK.Model;

namespace VK_Api_Tests.VK
{
    public class VkApi
    {
        public const string ApiVersion = "5.34";
        public const string ApiRoot = @"https://api.vk.com/method/";
        private static readonly string ApiVersionParam = $"v={WebUtility.UrlEncode(ApiVersion)}";

        private static readonly Random Rand = new Random();

        public string Token
        {
            get; private set;
        }
        public long UserId { get; private set; }

        public bool Auth(string scope, int appId, bool revoke = false)
        {
            var form = new LoginWindow(appId, ApiVersion, scope, revoke);

            if (form.ShowDialog().GetValueOrDefault())
            {
                Token = form.AccessToken;
                UserId = form.UserId;

                return true;
            }

            return false;
        }

        public static VkApi AuthNew(string scope, int appId, bool revoke = false)
        {
            var api = new VkApi();
            api.Auth(scope, appId, revoke);
            return api;
        }

        public JToken RunRawMethod(string method, string args)
        {
            using (var client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;

                var responseString =
                    client.DownloadString(string.IsNullOrWhiteSpace(args)
                        ? $"{ApiRoot}{method}"
                        : $"{ApiRoot}{method}?{args}");

                var responseJson = JObject.Parse(responseString);

                return responseJson;
            }
        }

        public JToken RunMethod(string method, string args)
        {
            var tokenAndVersion = string.IsNullOrWhiteSpace(Token) ? ApiVersionParam : $"access_token={Token}&{ApiVersionParam}";
            var request = string.IsNullOrWhiteSpace(args) ? tokenAndVersion : $"{args}&{tokenAndVersion}";

            var response = RunRawMethod(method, request).Value<JObject>();

            JToken error;
            while (response.TryGetValue("error", out error) && error["error_code"].Value<int>() == 14)
            {
                var window = new CaptchaWindow(error["captcha_img"].Value<string>());
                if (window.ShowDialog().GetValueOrDefault())
                {
                    var newRequest = $"{request}&captcha_sid={error["captcha_sid"]}&captcha_key={WebUtility.UrlEncode(window.Text)}";
                    response = RunRawMethod(method, newRequest).Value<JObject>();
                }
                else
                {
                    return null;
                }
            }

            return response;
        }

        public JToken RunMethod(string method, params object[] args)
        {
            var pairs = new List<string>();
            if (args.Length > 0)
            {
                if (args.Length % 2 != 0)
                    throw new ArgumentException("args count must be an even number");

                for (var i = 0; i < args.Length; i += 2)
                {
                    pairs.Add(string.Format(@"{0}={1}",
                        WebUtility.UrlEncode(args[i].ToString()),
                        WebUtility.UrlEncode(args[i + 1].ToString())));
                }
            }
            return RunMethod(method, string.Join("&", pairs));
        }


        #region VK api methods

        public long MessagesSend(long userId, string message, string attachment = null)
        {
            return (string.IsNullOrWhiteSpace(attachment)
                ? RunMethod("messages.send", "user_id", userId, "message", message, "guid", Rand.Next())
                : RunMethod("messages.send", "user_id", userId, "message", message, "guid", Rand.Next(), "attachment", attachment))
                ["response"].Value<long>();
        }

        public JObject PhotosGetMessagesUploadServer()
        {
            return RunMethod("photos.getMessagesUploadServer")?["response"]?.Value<JObject>();
        }

        public JObject PhotosSaveMessagesPhoto(JObject photo)
        {
            return RunMethod("photos.saveMessagesPhoto",
                "photo", photo["photo"],
                "server", photo["server"],
                "hash", photo["hash"])
                ["response"].Value<JArray>()[0].Value<JObject>();
        }

        public User UsersGetSingle(string user_id, string fields = null, string name_case = null)
        {
            var request = new StringBuilder($"user_ids={WebUtility.UrlEncode(user_id)}");
            if (!string.IsNullOrWhiteSpace(fields))
                request.Append($"&fields={WebUtility.UrlEncode(fields)}");
            if (!string.IsNullOrWhiteSpace(name_case))
                request.Append($"&name_case={WebUtility.UrlEncode(name_case)}");

            var response = RunMethod("users.get", request.ToString())?["response"]?.FirstOrDefault();
            return response == null ? null : new User(response);
        }

        public List<Post> WallGet(int owner_id, int offset, int count)
        {
            return RunMethod("wall.get", "owner_id", owner_id, "offset", offset, "count", count)
                ["response"]["items"].Value<JArray>()
                .Select(t => new Post(t)).ToList();
        }

        #endregion
    }
}