using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Data.Json;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace CompanySearcher
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class NoticeDetailPage : Page
    {
        private string noticeId, noticeName, companyName, companyId, companyNo;


        public NoticeDetailPage()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.NavigationMode != NavigationMode.New)
                return;

            noticeId = Functions.tryGetValueFromNavigation(e.Parameter.ToString(), "id");
            currentCpNameTxt.Text = noticeName = Functions.tryGetValueFromNavigation(e.Parameter.ToString(), "noticeName");
            companyName = Functions.tryGetValueFromNavigation(e.Parameter.ToString(), "companyName");
            companyId = Functions.tryGetValueFromNavigation(e.Parameter.ToString(), "companyId");
            companyNo = Functions.tryGetValueFromNavigation(e.Parameter.ToString(), "companyNo");

            httpClient_loadNoticeInfo(WebUrl.getNoticeDetailJsonHead + companyId + WebUrl.getNoticeDetailJsonCenter + companyNo + WebUrl.getNoticeDetailJsonEnd + noticeId);
        }

        private async void httpClient_loadNoticeInfo(string url)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    progressRing.IsActive = true;

                    loadReportBasicInfo(await client.GetStringAsync(url));

                    //isBasicInfoLoaded = true;
                    progressRing.IsActive = false;
                }
        }
            catch
            {
                progressRing.IsActive = false;
            }
}

        private void loadReportBasicInfo(string contents)
        {
            JsonObject jContents = JsonObject.Parse(contents);
            JsonObject jNoticeResult = jContents.GetNamedObject("result").GetNamedObject("EquInfo").GetNamedObject("EquDjInfo");

            froDocNoTxt.Text = jNoticeResult.GetNamedString("FRODOCNO");
            exeDocNoTxt.Text = jNoticeResult.GetNamedString("EXECUTENO");
            publicDateTxt.Text = jNoticeResult.GetNamedString("PUBLICDATE");
            froAuthTxt.Text = jNoticeResult.GetNamedString("FROAUTH");
            invNameTxt.Text = jNoticeResult.GetNamedString("INV");
            froAmTxt.Text = jNoticeResult.GetNamedString("FROAM");
            blicTypeTxt.Text = jNoticeResult.GetNamedString("BLICTYPE");
            blicNoTxt.Text = jNoticeResult.GetNamedString("BLICNO");
            exeItemTxt.Text = jNoticeResult.GetNamedString("EXECUTEITEM");
            froDeadLineTxt.Text = jNoticeResult.GetNamedString("FROZDEADLINE");
            froFromDateTxt.Text = jNoticeResult.GetNamedString("FROFROM");
            froToDateTxt.Text = jNoticeResult.GetNamedString("FROTO");

            relatedCompanyIcon.Visibility = Visibility.Visible;
            companyNameTxt.Text = companyName;
            companyNoTxt.Text = companyNo;
        }

        private void relatedCompanyList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (relatedCompanyList.SelectedItem == null)
                return;
            //Frame.Navigate(typeof(CompanyDetailPage), "id=" + actualCompanyId + "&regNo=" + companyNo + "&name=" + companyName);
            relatedCompanyList.SelectedItem = null;
        }
    }
}
