using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Email;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace CompanySearcher
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AboutPage : Page
    {
        private int lisaImgCount = 0;
        private String versionString = Package.Current.Id.Version.Major + "." + Package.Current.Id.Version.Minor + "." + Package.Current.Id.Version.Build;

        public AboutPage()
        {
            this.InitializeComponent();

            versionTxt.Text = versionString;
        }

        private void logoImg_Tapped(object sender, TappedRoutedEventArgs e)
        {
            lisaImgCount++;
            if (lisaImgCount == 5)
                Functions.loadUIEImg(logoImg, "Assets/Lisa_Img.png");
        }

        private void rateButton_Click(object sender, RoutedEventArgs e)
        {
            Windows.System.Launcher.LaunchUriAsync(new Uri("ms-windows-store://review/?ProductId=9NBLGGH5VZ67"));
        }

        private void reviewButton_Click(object sender, RoutedEventArgs e)
        {
            EmailMessage emailComposeTask = new EmailMessage();
            emailComposeTask.Subject = "反馈:江苏企业查询(" + versionString + ")";
            emailComposeTask.To.Add(new EmailRecipient()
            {
                Address = "admin@raychinki.com"
            });
            EmailManager.ShowComposeNewEmailAsync(emailComposeTask);
        }
    }
}
