using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace VK_Api_Tests
{
    /// <summary>
    /// Interaction logic for CaptchaWindow.xaml
    /// </summary>
    public partial class CaptchaWindow : Window
    {
        private string _url;
        public string Text;

        public CaptchaWindow(string url)
        {
            this._url = url;
            InitializeComponent();
        }

        protected override void OnInitialized(EventArgs e)
        {
            image.DataContext = _url;
            base.OnInitialized(e);
        }

        private void Confirm(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Text = inputBox.Text;
            Close();
        }

        private void TextBox_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Confirm(sender, null);
            }
        }
    }
}
