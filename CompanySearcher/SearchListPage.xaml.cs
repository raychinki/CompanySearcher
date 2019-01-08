using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Data.Json;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace CompanySearcher
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SearchListPage : Page
    {
        private ObservableCollection<SearchedCompanyListItem> searchedCompanyListItems = new ObservableCollection<SearchedCompanyListItem>();
        private SearchedCompanyClipboardItem scciForClipboard = new SearchedCompanyClipboardItem("", "", "", "", "");
        public ScrollViewer scrollViewer = new ScrollViewer();
        public ScrollBar verticalScrollBar = new ScrollBar();
        private int searchedCompanyCountInPage = 20, searchedCompanyPageIndex = 1;
        private string searchString = "";

        public SearchListPage()
        {
            this.InitializeComponent();

            NavigationCacheMode = NavigationCacheMode.Enabled;

            searchedCompanyList.ItemsSource = searchedCompanyListItems;
        }

        private async void httpClient_loadSearchedCompanyList(string url)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    progressRing.IsActive = true;
                    backgroundImg.Visibility = Visibility.Collapsed;
                    noResultTxt.Visibility = Visibility.Collapsed;

                    loadSearchedCompanyList(await client.GetStringAsync(url));

                    progressRing.IsActive = false;
                    if (searchedCompanyListItems.Count == 0)
                        noResultTxt.Visibility = Visibility.Visible;
                }
            }
            catch
            {
                progressRing.IsActive = false;
            }
        }

        private void loadSearchedCompanyList(string contents)
        {
            JsonObject jContents = JsonObject.Parse(contents);
            JsonArray jaCompanies = jContents.GetNamedArray("result");

            ObservableCollection<SearchedCompanyListItem> tempListItems = new ObservableCollection<SearchedCompanyListItem>();
            string id, regNo, name, status, type, estDate, legPerson, regOrg, rN, recColor, nameColor, summaryColor;
            for (int i = 0; i < jaCompanies.Count; i++)
            {
                JsonObject jo = jaCompanies[i].GetObject();
                id = jo.GetNamedValue("ID").ToString();
                regNo = jo.GetNamedString("REGNO");
                name = jo.GetNamedString("ENTNAME");
                status = jo.GetNamedString("CORP_STATUS");
                if (status == "01")
                {
                    recColor = "Green";
                    nameColor = SearcherColors.CyanString;
                    summaryColor = "Black";
                }
                else
                {
                    recColor = "Gray";
                    nameColor = "Gray";
                    summaryColor = "Gray";
                }
                type = jo.GetNamedString("ENTTYPE");
                estDate = jo.GetNamedString("ESTDATE");
                legPerson = jo.GetNamedString("LEREP");
                regOrg = jo.GetNamedString("REGORG");
                rN = jo.GetNamedValue("RN").ToString();

                SearchedCompanyListItem scli = new SearchedCompanyListItem(id, regNo, name, status, type, estDate, legPerson, regOrg, rN, recColor, nameColor, summaryColor);
                tempListItems.Add(scli);
            }

            foreach (var item in new ObservableCollection<SearchedCompanyListItem>(tempListItems.OrderByDescending(item => item.RN)))
                searchedCompanyListItems.Add(item);
        }

        private void searchTxtBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (searchTxtBox.Text == "请输入公司名称、注册号或统一社会信用代码")
                searchTxtBox.Text = "";
            searchTxtBox.Foreground = new SolidColorBrush(Colors.Black);
        }

        private void searchTxtBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (searchTxtBox.Text.Trim() == "")
                searchTxtBox.Text = "请输入公司名称、注册号或统一社会信用代码";
            searchTxtBox.Foreground = new SolidColorBrush(Colors.Gray);
        }

        private void searchTxtBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                e.Handled = true;
                searchedCompanyListItems.Clear();
                searchedCompanyPageIndex = 1;
                if (searchTxtBox.Text == "请输入公司名称、注册号或统一社会信用代码" || searchTxtBox.Text.Trim() == "")
                {
                    backgroundImg.Visibility = Visibility.Visible;
                    noResultTxt.Visibility = Visibility.Collapsed;
                }
                else
                {
                    searchString = searchTxtBox.Text;
                    httpClient_loadSearchedCompanyList(WebUrl.getSearchedCompanyListJsonHead + Functions.convertStringToBase64(searchString) + WebUrl.getSearchedCompanyListJsonCenter + searchedCompanyPageIndex.ToString() + WebUrl.getSearchedCompanyListJsonEnd + searchedCompanyCountInPage.ToString());
                }
            }
        }

        private void searchButton_Click(object sender, RoutedEventArgs e)
        {
            searchedCompanyListItems.Clear();
            searchedCompanyPageIndex = 1;
            if (searchTxtBox.Text == "请输入公司名称、注册号或统一社会信用代码" || searchTxtBox.Text.Trim() == "")
            {
                backgroundImg.Visibility = Visibility.Visible;
                noResultTxt.Visibility = Visibility.Collapsed;
            }
            else
            {
                searchString = searchTxtBox.Text;
                httpClient_loadSearchedCompanyList(WebUrl.getSearchedCompanyListJsonHead + Functions.convertStringToBase64(searchString) + WebUrl.getSearchedCompanyListJsonCenter + searchedCompanyPageIndex.ToString() + WebUrl.getSearchedCompanyListJsonEnd + searchedCompanyCountInPage.ToString());
            }
        }

        private void searchedCompanyItemGrid_Holding(object sender, HoldingRoutedEventArgs e)
        {
            SearchedCompanyListItem scli = (sender as Grid).DataContext as SearchedCompanyListItem;
            scciForClipboard = new SearchedCompanyClipboardItem(scli.RegNo, scli.Name, scli.EstDate, scli.LegPerson, scli.RegOrg);

            menuFlyout.ShowAt(searchedCompanyList, e.GetPosition(searchedCompanyList));
        }

        private void searchedCompanyItemGrid_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            SearchedCompanyListItem scli = (sender as Grid).DataContext as SearchedCompanyListItem;
            scciForClipboard = new SearchedCompanyClipboardItem(scli.RegNo, scli.Name, scli.EstDate, scli.LegPerson, scli.RegOrg);

            menuFlyout.ShowAt(searchedCompanyList, e.GetPosition(searchedCompanyList));
        }

        private void copyNameButton_Click(object sender, RoutedEventArgs e)
        {
            DataPackage dp = new DataPackage();
            dp.SetText(scciForClipboard.Name);
            Clipboard.SetContent(dp);
        }

        private void copyRegNoButton_Click(object sender, RoutedEventArgs e)
        {
            DataPackage dp = new DataPackage();
            dp.SetText(scciForClipboard.RegNo);
            Clipboard.SetContent(dp);
        }

        private void copyLegPersonButton_Click(object sender, RoutedEventArgs e)
        {
            DataPackage dp = new DataPackage();
            dp.SetText(scciForClipboard.LegPerson);
            Clipboard.SetContent(dp);
        }

        private void copyCompanyBasicInfoButton_Click(object sender, RoutedEventArgs e)
        {
            DataPackage dp = new DataPackage();
            dp.SetText("公司名称: " + scciForClipboard.Name + "\r\n注册号: " + scciForClipboard.RegNo + "\r\n登记机关: " + scciForClipboard.RegOrg + "\r\n法定代表人: " + scciForClipboard.LegPerson + "\r\n成立日期: " + scciForClipboard.EstDate);
            Clipboard.SetContent(dp);
        }

        private void menuFlyout_Closed(object sender, object e)
        {
            scciForClipboard = new SearchedCompanyClipboardItem("", "", "", "", "");
        }

        private void searchedCompanyList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (searchedCompanyList.SelectedItem == null)
                return;
            var template = searchedCompanyList.SelectedItem as SearchedCompanyListItem;
            Frame.Navigate(typeof(CompanyDetailPage), "id=" + template.Id + "&regNo=" + template.RegNo + "&name=" + template.Name);
            searchedCompanyList.SelectedItem = null;
        }

        private void searchedCompanyList_Loaded(object sender, RoutedEventArgs e)
        {
            scrollViewer = Functions.FindVisualChildByName<ScrollViewer>(searchedCompanyList, "ScrollViewer");
            verticalScrollBar = Functions.FindVisualChildByName<ScrollBar>(scrollViewer, "VerticalScrollBar");
            verticalScrollBar.ValueChanged += verticalScrollBar_ValueChanged;
        }

        private void verticalScrollBar_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (e.NewValue > e.OldValue && e.NewValue >= (verticalScrollBar.Maximum - 200))
            {
                if (searchedCompanyPageIndex < 5 && !progressRing.IsActive && searchedCompanyListItems.Count == (searchedCompanyPageIndex * searchedCompanyCountInPage))
                {
                    Task.Factory.StartNew(async () =>
                    {
                        await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                        {
                            searchedCompanyPageIndex++;
                            httpClient_loadSearchedCompanyList(WebUrl.getSearchedCompanyListJsonHead + Functions.convertStringToBase64(searchString) + WebUrl.getSearchedCompanyListJsonCenter + searchedCompanyPageIndex.ToString() + WebUrl.getSearchedCompanyListJsonEnd + searchedCompanyCountInPage.ToString());
                        });
                    });
                }
            }
        }
    }
}
