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
        private ObservableCollection<CompanyCheckInfoListItem> checkInfoListItems = new ObservableCollection<CompanyCheckInfoListItem>();
        private string currentId, currentRegNo, currentName;
        private bool isBasicInfoLoaded = false, isCheckInfoLoaded = false;

        public CompanyDetailPage()
        {
            this.InitializeComponent();

            shareholderInfoList.ItemsSource = shareholderInfoListItems;
            changeInfoList.ItemsSource = changeInfoListItems;
            checkInfoList.ItemsSource = checkInfoListItems;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            currentId = Functions.tryGetValueFromNavigation(e.Parameter.ToString(), "id");
            currentRegNo = Functions.tryGetValueFromNavigation(e.Parameter.ToString(), "regNo");
            currentCpNameTxt.Text = currentName = Functions.tryGetValueFromNavigation(e.Parameter.ToString(), "name");

            string pivotIndex = Functions.tryGetValueFromNavigation(e.Parameter.ToString(), "index");
            if (pivotIndex != "")
                pagePivot.SelectedIndex = Convert.ToInt32(pivotIndex);

            httpClient_loadCompanyBasicInfo(WebUrl.getCompanyBasicInfoJsonHead + currentId + WebUrl.getCompanyBasicInfoJsonCenter + currentRegNo + WebUrl.getCompanyBasicInfoJsonEnd + "RegInfo");
            httpClient_loadCompanyCheckInfo(WebUrl.getCompanyBasicInfoJsonHead + currentId + WebUrl.getCompanyBasicInfoJsonCenter + currentRegNo + WebUrl.getCompanyBasicInfoJsonEnd + "CheckInfo");
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
                    if (isBasicInfoLoaded && isCheckInfoLoaded)
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
            currentCpRegOrgTxt.Text = jContents.GetNamedString("REGORG");
            currentCpScopeTxt.Text = jContents.GetNamedString("OPSCOPE");
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
                    if (isBasicInfoLoaded && isCheckInfoLoaded)
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
                        checkInfoPivotItemHeader.Foreground = new SolidColorBrush(SearcherColors.LightRed);
                    }
                    break;
                case 1:
                    {
                        basicInfoPivotItemHeader.Foreground = new SolidColorBrush(SearcherColors.LightRed);
                        shareholderInfoPivotItemHeader.Foreground = new SolidColorBrush(SearcherColors.ShieldRed);
                        changeInfoPivotItemHeader.Foreground = new SolidColorBrush(SearcherColors.LightRed);
                        checkInfoPivotItemHeader.Foreground = new SolidColorBrush(SearcherColors.LightRed);
                    }
                    break;
                case 2:
                    {
                        basicInfoPivotItemHeader.Foreground = new SolidColorBrush(SearcherColors.LightRed);
                        shareholderInfoPivotItemHeader.Foreground = new SolidColorBrush(SearcherColors.LightRed);
                        changeInfoPivotItemHeader.Foreground = new SolidColorBrush(SearcherColors.ShieldRed);
                        checkInfoPivotItemHeader.Foreground = new SolidColorBrush(SearcherColors.LightRed);
                    }
                    break;
                case 3:
                    {
                        basicInfoPivotItemHeader.Foreground = new SolidColorBrush(SearcherColors.LightRed);
                        shareholderInfoPivotItemHeader.Foreground = new SolidColorBrush(SearcherColors.LightRed);
                        changeInfoPivotItemHeader.Foreground = new SolidColorBrush(SearcherColors.LightRed);
                        checkInfoPivotItemHeader.Foreground = new SolidColorBrush(SearcherColors.ShieldRed);
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

        private void chekInfoList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (checkInfoList.SelectedItem == null)
                return;
            checkInfoList.SelectedItem = null;
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

        private void chekInfoPivotItemHeader_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (pagePivot.SelectedIndex != 3)
                pagePivot.SelectedIndex = 3;
        }
    }
}
