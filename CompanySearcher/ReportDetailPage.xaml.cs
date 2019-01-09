using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace CompanySearcher
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ReportDetailPage : Page
    {
        private ObservableCollection<ReportPayInfoListItem> payInfoListItems = new ObservableCollection<ReportPayInfoListItem>();
        private ObservableCollection<ReportWebInfoListItem> webInfoListItems = new ObservableCollection<ReportWebInfoListItem>();
        private string currentId, currentReportId, currentRegNo, currentName;
        //private bool isBasicInfoLoaded = false;

        public ReportDetailPage()
        {
            this.InitializeComponent();

            NavigationCacheMode = NavigationCacheMode.Enabled;

            payInfoList.ItemsSource = payInfoListItems;
            webInfoList.ItemsSource = webInfoListItems;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.NavigationMode != NavigationMode.New)
                return;

            pagePivot.SelectedIndex = 0;

            currentId = Functions.tryGetValueFromNavigation(e.Parameter.ToString(), "id");
            currentRegNo = Functions.tryGetValueFromNavigation(e.Parameter.ToString(), "regNo");
            currentCpNameTxt.Text = currentName = Functions.tryGetValueFromNavigation(e.Parameter.ToString(), "name");
            currentReportId = Functions.tryGetValueFromNavigation(e.Parameter.ToString(), "reportId");

            clearItems();

            httpClient_loadReportBasicInfo(WebUrl.getReportBasicInfoJsonHead + currentId + WebUrl.getReportBasicInfoJsonCenter + currentRegNo + WebUrl.getReportBasicInfoJsonEnd + currentReportId);
        }

        private void clearItems()
        {
            payInfoListItems.Clear();
            webInfoListItems.Clear();
        }

        private async void httpClient_loadReportBasicInfo(string url)
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
            JsonObject jReportResult = jContents.GetNamedObject("result");
            JsonObject jReportBaseInfo = jReportResult.GetNamedObject("BaseInfo");
            JsonArray jaReportPayInfo = jReportResult.GetNamedArray("PayInfo");
            JsonObject jReportAssetInfo = jReportResult.GetNamedObject("AssetInfo");
            JsonArray jaReportWebInfo = jReportResult.GetNamedArray("WebInfo");

            loadReportBaseInfo(jReportBaseInfo);
            loadReportPayInfo(jaReportPayInfo);
            loadReportAssetInfo(jReportAssetInfo);
            loadReportWebInfo(jaReportWebInfo);
        }

        private void loadReportBaseInfo(JsonObject jContents)
        {
            reportYearTxt.Text = jContents.GetNamedValue("REPORT_YEAR").ToString();
            reportDateTxt.Text = jContents.GetNamedString("REPORT_DATE");
            reportCpRegNoTxt.Text = jContents.GetNamedString("REGNO");
            reportStatusTxt.Text = jContents.GetNamedString("BUSST");
            reportTelTxt.Text = jContents.GetNamedString("TEL");
            reportEmailTxt.Text = jContents.GetNamedString("EMAIL");
            reportPersonsCountTxt.Text = jContents.GetNamedValue("PRAC_PERSON_NUM").ToString();
            reportPostCodeTxt.Text = jContents.GetNamedString("POSTALCODE");
            reportAddressTxt.Text = jContents.GetNamedString("ADDR");
            reportIsWebTxt.Text = jContents.GetNamedString("IF_WEBSITE");
            reportIsInvestTxt.Text = jContents.GetNamedString("IF_INVEST");
            reportIsEquityTxt.Text = jContents.GetNamedString("IF_EQUITY");
        }

        private void loadReportPayInfo(JsonArray jaContents)
        {
            string name, conBalance, conDate, conType, paidBalance, paidDate, paidType;
            for (int i = 0; i < jaContents.Count; i++)
            {
                JsonObject jo = jaContents[i].GetObject();
                name = jo.GetNamedString("Inv");
                conBalance = jo.GetNamedString("LiSubConAm");
                conDate = jo.GetNamedString("ConDate");
                conType = jo.GetNamedString("ConForm");
                paidBalance = jo.GetNamedString("LiAcConAm");
                paidDate = jo.GetNamedString("LiAcConDate");
                paidType = jo.GetNamedString("LiAcConForm");

                ReportPayInfoListItem rpili = new ReportPayInfoListItem(name, conBalance, conDate, conType, paidBalance, paidDate, paidType);
                payInfoListItems.Add(rpili);
            }

            if (payInfoListItems.Count == 0)
                noPayInfoTxt.Visibility = Visibility.Visible;
            else
                noPayInfoTxt.Visibility = Visibility.Collapsed;
        }

        private void loadReportAssetInfo(JsonObject jContents)
        {
            reportAssetAmountTxt.Text = jContents.GetNamedString("ASSGRO").Replace("不公示", "企业选择不公示");
            reportOwnerAmountTxt.Text = jContents.GetNamedString("TOTEQU").Replace("不公示", "企业选择不公示");
            reportBussinessAmountTxt.Text = jContents.GetNamedString("VENDINC").Replace("不公示", "企业选择不公示");
            reportBussinessMainAmountTxt.Text = jContents.GetNamedString("MAIBUSINC").Replace("不公示", "企业选择不公示");
            reportProfitAmountTxt.Text = jContents.GetNamedString("PROGRO").Replace("不公示", "企业选择不公示");
            reportNetProfitTxt.Text = jContents.GetNamedString("NETINC").Replace("不公示", "企业选择不公示");
            reportTaxAmountTxt.Text = jContents.GetNamedString("RATGRO").Replace("不公示", "企业选择不公示");
            reportDebitAmountTxt.Text = jContents.GetNamedString("LIAGRO").Replace("不公示", "企业选择不公示");
        }

        private void loadReportWebInfo(JsonArray jaContents)
        {
            string name, webType, url, avatarIcon;
            for (int i = 0; i < jaContents.Count; i++)
            {
                JsonObject jo = jaContents[i].GetObject();
                name = jo.GetNamedString("WEBSITNAME");
                webType = jo.GetNamedString("WEBTYPE");
                url = jo.GetNamedString("DOMAIN");
                if (webType == "网站")
                    avatarIcon = "Globe";
                else
                    avatarIcon = "Shop";

                ReportWebInfoListItem rwili = new ReportWebInfoListItem(name, webType, url, avatarIcon);
                webInfoListItems.Add(rwili);
            }

            if (webInfoListItems.Count == 0)
                noWebInfoTxt.Visibility = Visibility.Visible;
            else
                noWebInfoTxt.Visibility = Visibility.Collapsed;
        }

        private void pagePivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (pagePivot.SelectedIndex)
            {
                case 0:
                    {
                        basicInfoPivotItemHeader.Foreground = new SolidColorBrush(SearcherColors.ShieldRed);
                        payInfoPivotItemHeader.Foreground = new SolidColorBrush(SearcherColors.LightRed);
                        webInfoPivotItemHeader.Foreground = new SolidColorBrush(SearcherColors.LightRed);
                        assetInfoPivotItemHeader.Foreground = new SolidColorBrush(SearcherColors.LightRed);
                    }
                    break;
                case 1:
                    {
                        basicInfoPivotItemHeader.Foreground = new SolidColorBrush(SearcherColors.LightRed);
                        payInfoPivotItemHeader.Foreground = new SolidColorBrush(SearcherColors.ShieldRed);
                        webInfoPivotItemHeader.Foreground = new SolidColorBrush(SearcherColors.LightRed);
                        assetInfoPivotItemHeader.Foreground = new SolidColorBrush(SearcherColors.LightRed);
                    }
                    break;
                case 2:
                    {
                        basicInfoPivotItemHeader.Foreground = new SolidColorBrush(SearcherColors.LightRed);
                        payInfoPivotItemHeader.Foreground = new SolidColorBrush(SearcherColors.LightRed);
                        webInfoPivotItemHeader.Foreground = new SolidColorBrush(SearcherColors.LightRed);
                        assetInfoPivotItemHeader.Foreground = new SolidColorBrush(SearcherColors.ShieldRed);
                    }
                    break;
                case 3:
                    {
                        basicInfoPivotItemHeader.Foreground = new SolidColorBrush(SearcherColors.LightRed);
                        payInfoPivotItemHeader.Foreground = new SolidColorBrush(SearcherColors.LightRed);
                        webInfoPivotItemHeader.Foreground = new SolidColorBrush(SearcherColors.ShieldRed);
                        assetInfoPivotItemHeader.Foreground = new SolidColorBrush(SearcherColors.LightRed);
                    }
                    break;
                default: break;
            }
        }

        private void payInfoList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (payInfoList.SelectedItem == null)
                return;
            payInfoList.SelectedItem = null;
        }

        private void webInfoList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (webInfoList.SelectedItem == null)
                return;
            webInfoList.SelectedItem = null;
        }

        private void basicInfoPivotItemHeader_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (pagePivot.SelectedIndex != 0)
                pagePivot.SelectedIndex = 0;
        }

        private void payInfoPivotItemHeader_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (pagePivot.SelectedIndex != 1)
                pagePivot.SelectedIndex = 1;
        }

        private void assetInfoPivotItemHeader_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (pagePivot.SelectedIndex != 2)
                pagePivot.SelectedIndex = 2;
        }

        private void webInfoPivotItemHeader_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (pagePivot.SelectedIndex != 3)
                pagePivot.SelectedIndex = 3;
        }
    }
}
