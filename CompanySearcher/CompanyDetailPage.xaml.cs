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
    public sealed partial class CompanyDetailPage : Page
    {
        //private ObservableCollection<CompanyBasicInfo> searchedCompanyListItems = new ObservableCollection<CompanyBasicInfo>();
        private ObservableCollection<CompanyShareholderInfoListItem> shareholderInfoListItems = new ObservableCollection<CompanyShareholderInfoListItem>();
        private ObservableCollection<CompanyChangeInfoListItem> changeInfoListItems = new ObservableCollection<CompanyChangeInfoListItem>();
        private ObservableCollection<CompanyAbnormalInfoListItem> abnormalInfoListItems = new ObservableCollection<CompanyAbnormalInfoListItem>();
        private ObservableCollection<CompanyCheckInfoListItem> checkInfoListItems = new ObservableCollection<CompanyCheckInfoListItem>();
        private ObservableCollection<CompanyReportInfoListItem> reportInfoListItems = new ObservableCollection<CompanyReportInfoListItem>();
        private string currentId, currentRegNo, currentName;
        private bool isBasicInfoLoaded = false, isAbnormalInfoLoaded = false, isCheckInfoLoaded = false, isReportInfoLoaded = false;

        public CompanyDetailPage()
        {
            this.InitializeComponent();

            NavigationCacheMode = NavigationCacheMode.Enabled;

            shareholderInfoList.ItemsSource = shareholderInfoListItems;
            changeInfoList.ItemsSource = changeInfoListItems;
            abnormalInfoList.ItemsSource = abnormalInfoListItems;
            checkInfoList.ItemsSource = checkInfoListItems;
            reportInfoList.ItemsSource = reportInfoListItems;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            
            if (e.NavigationMode != NavigationMode.New)
                return;

            pagePivot.SelectedIndex = 0;
            string pivotIndex = Functions.tryGetValueFromNavigation(e.Parameter.ToString(), "index");
            if (pivotIndex != "")
                pagePivot.SelectedIndex = Convert.ToInt32(pivotIndex);

            currentId = Functions.tryGetValueFromNavigation(e.Parameter.ToString(), "id");
            currentRegNo = Functions.tryGetValueFromNavigation(e.Parameter.ToString(), "regNo");
            currentCpNameTxt.Text = currentName = Functions.tryGetValueFromNavigation(e.Parameter.ToString(), "name");

            clearItems();

            httpClient_loadCompanyBasicInfo(WebUrl.getCompanyBasicInfoJsonHead + currentId + WebUrl.getCompanyBasicInfoJsonCenter + currentRegNo + WebUrl.getCompanyBasicInfoJsonEnd + "RegInfo");
            httpClient_loadCompanyCheckInfo(WebUrl.getCompanyBasicInfoJsonHead + currentId + WebUrl.getCompanyBasicInfoJsonCenter + currentRegNo + WebUrl.getCompanyBasicInfoJsonEnd + "CheckInfo");
            httpClient_loadCompanyAbnormalInfo(WebUrl.getCompanyBasicInfoJsonHead + currentId + WebUrl.getCompanyBasicInfoJsonCenter + currentRegNo + WebUrl.getCompanyBasicInfoJsonEnd + "ExceInfo");
            httpClient_loadCompanyReportInfo(WebUrl.getCompanyReportListJsonHead + currentId + WebUrl.getCompanyReportListJsonCenter + currentRegNo + WebUrl.getCompanyReportListJsonEnd + "ReportInfo");
        }

        private void clearItems()
        {
            shareholderInfoListItems.Clear();
            changeInfoListItems.Clear();
            abnormalInfoListItems.Clear();
            checkInfoListItems.Clear();
            reportInfoListItems.Clear();
        }

        private async void httpClient_loadCompanyBasicInfo(string url)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    progressRing.IsActive = true;

                    loadCompanyBasicInfo(await client.GetStringAsync(url));

                    isBasicInfoLoaded = true;
                    if (isBasicInfoLoaded && isAbnormalInfoLoaded && isCheckInfoLoaded && isReportInfoLoaded)
                        progressRing.IsActive = false;
                }
            }
            catch
            {
                progressRing.IsActive = false;
            }
        }

        private void loadCompanyBasicInfo(string contents)
        {
            JsonObject jContents = JsonObject.Parse(contents);
            JsonObject jCompanyResult = jContents.GetNamedObject("result");
            JsonObject jCompanyRegInfo = jCompanyResult.GetNamedObject("RegInfo");
            JsonObject jCompanyBaseInfo = jCompanyRegInfo.GetNamedObject("BaseInfo");
            JsonArray jaCompanyLegInfo = jCompanyRegInfo.GetNamedArray("LegInfo");
            JsonArray jaCompanyChgInfo = jCompanyRegInfo.GetNamedArray("ChgInfo");

            loadCompanyBaseInfo(jCompanyBaseInfo);
            loadCompanyLegInfo(jaCompanyLegInfo);
            loadCompanyChgInfo(jaCompanyChgInfo);
        }

        private void loadCompanyBaseInfo(JsonObject jContents)
        {
            currentCpRegNoTxt.Text = jContents.GetNamedString("REGNO");
            currentCpStateTxt.Text = jContents.GetNamedString("REGSTATE");
            currentCpCapitalTxt.Text = jContents.GetNamedString("REGCAP");
            currentCpLegPersonTxt.Text = jContents.GetNamedString("LEREP");
            currentCpEstDateTxt.Text = jContents.GetNamedString("ESTDATE");
            currentCpPreDateTxt.Text = jContents.GetNamedString("APPRDATE");
            currentCpBeginDateTxt.Text = jContents.GetNamedString("OPFROM");
            currentCpEndDateTxt.Text = jContents.GetNamedString("OPTO");
            currentCpTypeTxt.Text = jContents.GetNamedString("ENTTYPE");
            currentCpAddressTxt.Text = jContents.GetNamedString("DOM");
            currentCpLocationTxt.Text = jContents.GetNamedString("OPLOC");
            currentCpRegOrgTxt.Text = jContents.GetNamedString("REGORG");
            currentCpScopeTxt.Text = jContents.GetNamedString("OPSCOPE");

            if (currentCpLocationTxt.Text == "")
            {
                currentCpLocationTopGrid.Visibility = Visibility.Collapsed;
                currentCpLocationGrid.Visibility = Visibility.Collapsed;
            }
            else
            {
                currentCpLocationTopGrid.Visibility = Visibility.Visible;
                currentCpLocationGrid.Visibility = Visibility.Visible;
            }
        }

        private void loadCompanyLegInfo(JsonArray jaContents)
        {
            string id, name, regionType, cerType, regNo, avatarIcon = "";
            for (int i = 0; i < jaContents.Count; i++)
            {
                JsonObject jo = jaContents[i].GetObject();
                //id = jo.GetNamedValue("ID").ToString();
                name = jo.GetNamedString("INV");
                regionType = jo.GetNamedString("INVTYPE");
                cerType = jo.GetNamedString("BLICTYPE");
                if (regionType.Contains("公民"))
                {
                    avatarIcon = "Contact";
                }
                else
                {
                    avatarIcon = "People";
                    cerType += " " + jo.GetNamedString("BLICNO");
                }

                CompanyShareholderInfoListItem csili = new CompanyShareholderInfoListItem(name, regionType, cerType, avatarIcon);
                shareholderInfoListItems.Add(csili);
            }

            if (shareholderInfoListItems.Count == 0)
                noShareholderInfoTxt.Visibility = Visibility.Visible;
            else
                noShareholderInfoTxt.Visibility = Visibility.Collapsed;
        }

        private void loadCompanyChgInfo(JsonArray jaContents)
        {
            string title, date, contentBefore, contentAfter;
            for (int i = 0; i < jaContents.Count; i++)
            {
                JsonObject jo = jaContents[i].GetObject();
                title = jo.GetNamedString("ALTITEM");
                date = jo.GetNamedString("ALTDATE");
                contentBefore = jo.GetNamedString("ALTBE");
                contentAfter = jo.GetNamedString("ALTAF");

                CompanyChangeInfoListItem ccili = new CompanyChangeInfoListItem(title, date, contentBefore, contentAfter);
                changeInfoListItems.Add(ccili);
            }

            if (changeInfoListItems.Count == 0)
                noChangeInfoTxt.Visibility = Visibility.Visible;
            else
                noChangeInfoTxt.Visibility = Visibility.Collapsed;
        }

        private async void httpClient_loadCompanyAbnormalInfo(string url)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    progressRing.IsActive = true;

                    loadCompanyAbnormalInfo(await client.GetStringAsync(url));

                    isAbnormalInfoLoaded = true;
                    if (isBasicInfoLoaded && isAbnormalInfoLoaded && isCheckInfoLoaded && isReportInfoLoaded)
                        progressRing.IsActive = false;
                }
            }
            catch
            {
                progressRing.IsActive = false;
            }
        }

        private void loadCompanyAbnormalInfo(string contents)
        {
            JsonObject jContents = JsonObject.Parse(contents);
            JsonObject jCompanyResult = jContents.GetNamedObject("result");
            JsonArray jaContents = jCompanyResult.GetNamedArray("ExceInfo");

            string abnOrg, inDate, inReason, outDate, outReason;
            for (int i = 0; i < jaContents.Count; i++)
            {
                JsonObject jo = jaContents[i].GetObject();
                abnOrg = jo.GetNamedString("DECORG");
                inDate = jo.GetNamedString("ABNTIME");
                inReason = jo.GetNamedString("SPECAUSE");
                outDate = jo.GetNamedString("REMDATE");
                outReason = jo.GetNamedString("REMEXCPRES");

                CompanyAbnormalInfoListItem caili = new CompanyAbnormalInfoListItem(abnOrg, inDate, inReason, outDate, outReason);
                abnormalInfoListItems.Add(caili);
            }

            if (abnormalInfoListItems.Count == 0)
                noAbnormalInfoTxt.Visibility = Visibility.Visible;
            else
                noAbnormalInfoTxt.Visibility = Visibility.Collapsed;
        }

        private async void httpClient_loadCompanyCheckInfo(string url)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    progressRing.IsActive = true;

                    loadCompanyCheckInfo(await client.GetStringAsync(url));
                    
                    isCheckInfoLoaded = true;
                    if (isBasicInfoLoaded && isAbnormalInfoLoaded && isCheckInfoLoaded && isReportInfoLoaded)
                        progressRing.IsActive = false;
                }
            }
            catch
            {
                progressRing.IsActive = false;
            }
        }

        private void loadCompanyCheckInfo(string contents)
        {
            JsonObject jContents = JsonObject.Parse(contents);
            JsonObject jCompanyResult = jContents.GetNamedObject("result");
            JsonArray jaContents = jCompanyResult.GetNamedArray("CheckInfo");

            string date, type, checkOrg, result;
            for (int i = 0; i < jaContents.Count; i++)
            {
                JsonObject jo = jaContents[i].GetObject();
                date = jo.GetNamedString("INSDATE");
                type = jo.GetNamedString("INSTYPE");
                checkOrg = jo.GetNamedString("INSAUTH");
                result = jo.GetNamedString("INSRES");

                CompanyCheckInfoListItem ccili = new CompanyCheckInfoListItem(date, type, checkOrg, result);
                checkInfoListItems.Add(ccili);
            }

            if (checkInfoListItems.Count == 0)
                noCheckInfoTxt.Visibility = Visibility.Visible;
            else
                noCheckInfoTxt.Visibility = Visibility.Collapsed;
        }

        private async void httpClient_loadCompanyReportInfo(string url)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    progressRing.IsActive = true;

                    loadCompanyReportInfo(await client.GetStringAsync(url));

                    isReportInfoLoaded = true;
                    if (isBasicInfoLoaded && isAbnormalInfoLoaded && isCheckInfoLoaded && isReportInfoLoaded)
                        progressRing.IsActive = false;
                }
            }
            catch
            {
                progressRing.IsActive = false;
            }
        }

        private void loadCompanyReportInfo(string contents)
        {
            JsonObject jContents = JsonObject.Parse(contents);
            JsonObject jCompanyResult = jContents.GetNamedObject("result");
            JsonArray jaContents = jCompanyResult.GetNamedArray("ReportInfo");

            string id, name, date;
            for (int i = 0; i < jaContents.Count; i++)
            {
                JsonObject jo = jaContents[i].GetObject();
                id = jo.GetNamedValue("ID").ToString();
                name = jo.GetNamedString("ANCHEYEAR");
                date = jo.GetNamedString("ANCHEDATE");

                CompanyReportInfoListItem crili = new CompanyReportInfoListItem(id, name, date);
                reportInfoListItems.Add(crili);
            }

            if (reportInfoListItems.Count == 0)
                noReportInfoTxt.Visibility = Visibility.Visible;
            else
                noReportInfoTxt.Visibility = Visibility.Collapsed;
        }

        private void pagePivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (pagePivot.SelectedIndex)
            {
                case 0:
                    {
                        basicInfoPivotItemHeader.Foreground = new SolidColorBrush(SearcherColors.ShieldRed);
                        shareholderInfoPivotItemHeader.Foreground = new SolidColorBrush(SearcherColors.LightRed);
                        changeInfoPivotItemHeader.Foreground = new SolidColorBrush(SearcherColors.LightRed);
                        abnormalInfoPivotItemHeader.Foreground = new SolidColorBrush(SearcherColors.LightRed);
                        checkInfoPivotItemHeader.Foreground = new SolidColorBrush(SearcherColors.LightRed);
                        reportInfoPivotItemHeader.Foreground = new SolidColorBrush(SearcherColors.LightRed);
                    }
                    break;
                case 1:
                    {
                        basicInfoPivotItemHeader.Foreground = new SolidColorBrush(SearcherColors.LightRed);
                        shareholderInfoPivotItemHeader.Foreground = new SolidColorBrush(SearcherColors.ShieldRed);
                        changeInfoPivotItemHeader.Foreground = new SolidColorBrush(SearcherColors.LightRed);
                        abnormalInfoPivotItemHeader.Foreground = new SolidColorBrush(SearcherColors.LightRed);
                        checkInfoPivotItemHeader.Foreground = new SolidColorBrush(SearcherColors.LightRed);
                        reportInfoPivotItemHeader.Foreground = new SolidColorBrush(SearcherColors.LightRed);
                    }
                    break;
                case 2:
                    {
                        basicInfoPivotItemHeader.Foreground = new SolidColorBrush(SearcherColors.LightRed);
                        shareholderInfoPivotItemHeader.Foreground = new SolidColorBrush(SearcherColors.LightRed);
                        changeInfoPivotItemHeader.Foreground = new SolidColorBrush(SearcherColors.ShieldRed);
                        abnormalInfoPivotItemHeader.Foreground = new SolidColorBrush(SearcherColors.LightRed);
                        checkInfoPivotItemHeader.Foreground = new SolidColorBrush(SearcherColors.LightRed);
                        reportInfoPivotItemHeader.Foreground = new SolidColorBrush(SearcherColors.LightRed);
                    }
                    break;
                case 3:
                    {
                        basicInfoPivotItemHeader.Foreground = new SolidColorBrush(SearcherColors.LightRed);
                        shareholderInfoPivotItemHeader.Foreground = new SolidColorBrush(SearcherColors.LightRed);
                        changeInfoPivotItemHeader.Foreground = new SolidColorBrush(SearcherColors.LightRed);
                        abnormalInfoPivotItemHeader.Foreground = new SolidColorBrush(SearcherColors.ShieldRed);
                        checkInfoPivotItemHeader.Foreground = new SolidColorBrush(SearcherColors.LightRed);
                        reportInfoPivotItemHeader.Foreground = new SolidColorBrush(SearcherColors.LightRed);
                    }
                    break;
                case 4:
                    {
                        basicInfoPivotItemHeader.Foreground = new SolidColorBrush(SearcherColors.LightRed);
                        shareholderInfoPivotItemHeader.Foreground = new SolidColorBrush(SearcherColors.LightRed);
                        changeInfoPivotItemHeader.Foreground = new SolidColorBrush(SearcherColors.LightRed);
                        abnormalInfoPivotItemHeader.Foreground = new SolidColorBrush(SearcherColors.LightRed);
                        checkInfoPivotItemHeader.Foreground = new SolidColorBrush(SearcherColors.ShieldRed);
                        reportInfoPivotItemHeader.Foreground = new SolidColorBrush(SearcherColors.LightRed);
                    }
                    break;
                case 5:
                    {
                        basicInfoPivotItemHeader.Foreground = new SolidColorBrush(SearcherColors.LightRed);
                        shareholderInfoPivotItemHeader.Foreground = new SolidColorBrush(SearcherColors.LightRed);
                        changeInfoPivotItemHeader.Foreground = new SolidColorBrush(SearcherColors.LightRed);
                        abnormalInfoPivotItemHeader.Foreground = new SolidColorBrush(SearcherColors.LightRed);
                        checkInfoPivotItemHeader.Foreground = new SolidColorBrush(SearcherColors.LightRed);
                        reportInfoPivotItemHeader.Foreground = new SolidColorBrush(SearcherColors.ShieldRed);
                    }
                    break;
                default:break;
            }
        }

        private void shareholderInfoList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (shareholderInfoList.SelectedItem == null)
                return;
            shareholderInfoList.SelectedItem = null;
        }

        private void changeInfoList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (changeInfoList.SelectedItem == null)
                return;
            changeInfoList.SelectedItem = null;
        }

        private void abnormalInfoList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (abnormalInfoList.SelectedItem == null)
                return;
            abnormalInfoList.SelectedItem = null;
        }

        private void chekInfoList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (checkInfoList.SelectedItem == null)
                return;
            checkInfoList.SelectedItem = null;
        }

        private void reportInfoList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (reportInfoList.SelectedItem == null)
                return;
            var template = reportInfoList.SelectedItem as CompanyReportInfoListItem;
            Frame.Navigate(typeof(ReportDetailPage), "id=" + currentId + "&regNo=" + currentRegNo + "&name=" + currentName + "&reportId=" + template.Id);
            reportInfoList.SelectedItem = null;
        }

        private void basicInfoPivotItemHeader_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (pagePivot.SelectedIndex != 0)
                pagePivot.SelectedIndex = 0;
        }

        private void shareholderInfoPivotItemHeader_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (pagePivot.SelectedIndex != 1)
                pagePivot.SelectedIndex = 1;
        }

        private void changeInfoPivotItemHeader_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (pagePivot.SelectedIndex != 2)
                pagePivot.SelectedIndex = 2;
        }

        private void abnormalInfoPivotItemHeader_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (pagePivot.SelectedIndex != 3)
                pagePivot.SelectedIndex = 3;
        }

        private void chekInfoPivotItemHeader_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (pagePivot.SelectedIndex != 4)
                pagePivot.SelectedIndex = 4;
        }

        private void reportInfoPivotItemHeader_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (pagePivot.SelectedIndex != 5)
                pagePivot.SelectedIndex = 5;
        }
    }
}
