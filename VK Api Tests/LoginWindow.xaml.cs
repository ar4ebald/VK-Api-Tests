using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VK_Api_Tests
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private const string redirectUri = "https://oauth.vk.com/blank.html";

        private int _appID;
        private string _apiVersion;
        private string _scope;
        private int _revoke;

        public string AccessToken { get; private set; }
        public long UserId { get; private set; }

        public LoginWindow(int appID, string apiVersion, string scope, bool revoke)
        {
            _appID = appID;
            _apiVersion = WebUtility.UrlEncode(apiVersion);
            _scope = WebUtility.UrlEncode(scope);
            _revoke = revoke ? 1 : 0;
            InitializeComponent();
        }

        protected override void OnInitialized(EventArgs e)
        {
            var url = "https://" + $"oauth.vk.com/authorize?client_id={_appID}&scope={_scope}&redirect_uri={WebUtility.UrlEncode(redirectUri)}&display=page&v={_apiVersion}&response_type=token&revoke={_revoke}";
            Browser.Navigate(url);
            Browser.Navigated += BrowserOnNavigated;

            base.OnInitialized(e);
        }

        private void BrowserOnNavigated(object sender, NavigationEventArgs e)
        {
            if (e.Uri.LocalPath == @"/blank.html")
            {
                var frag = e.Uri.Fragment;

                var access_token = Regex.Match(frag, @"(?<=access_token=)[\da-f]+");
                var user_id = Regex.Match(frag, @"(?<=user_id=)\d+");

                if (access_token.Success && user_id.Success)
                {
                    AccessToken = access_token.Value;
                    UserId = int.Parse(user_id.Value);

                    DialogResult = true;
                    Close();
                }
            }
        }
    }
}
