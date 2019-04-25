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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace CompanySearcher
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AbnormalListPage : Page
    {
        private ObservableCollection<AbnormalCompanyListItem> abnormalCompanyListItems = new ObservableCollection<AbnormalCompanyListItem>();
        private SearchedCompanyClipboardItem scciForClipboard = new SearchedCompanyClipboardItem("", "", "", "", "");
        public ScrollViewer scrollViewer = new ScrollViewer();
        public ScrollBar verticalScrollBar = new ScrollBar();
        private int searchedCompanyCountInPage = 20, searchedCompanyPageIndex = 1;
        private string searchString = "";

        public AbnormalListPage()
        {
            this.InitializeComponent();

            NavigationCacheMode = NavigationCacheMode.Enabled;

            abnormalCompanyList.ItemsSource = abnormalCompanyListItems;

            searchCompany();
        }

        private void searchCompany()
        {
            abnormalCompanyListItems.Clear();
            searchedCompanyPageIndex = 1;
            //if (searchTxtBox.Text == "请输入公司名称、注册号或统一社会信用代码" || searchTxtBox.Text.Trim() == "")
            //    searchString = "";
            //else
            searchString = searchTxtBox.Text.Trim();
            httpClient_loadAbnormalCompanyList(WebUrl.getAbnormalCompanyListJsonHead + Functions.convertStringToBase64(searchString) + WebUrl.getAbnormalCompanyListJsonCenter + searchedCompanyPageIndex.ToString() + WebUrl.getAbnormalCompanyListJsonEnd + searchedCompanyCountInPage.ToString());
        }

        private async void httpClient_loadAbnormalCompanyList(string url)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    progressRing.IsActive = true;
                    noResultTxt.Visibility = Visibility.Collapsed;

                    loadAbnormalCompanyList(await client.GetStringAsync(url));

                    progressRing.IsActive = false;
                    if (abnormalCompanyListItems.Count == 0)
                        noResultTxt.Visibility = Visibility.Visible;
                }
            }
            catch
            {
                progressRing.IsActive = false;
            }
        }

        private void loadAbnormalCompanyList(string contents)
        {
            JsonObject jContents = JsonObject.Parse(contents);
            JsonArray jaCompanies = jContents.GetNamedArray("result");

            ObservableCollection<AbnormalCompanyListItem> tempListItems = new ObservableCollection<AbnormalCompanyListItem>();
            string id, regNo, name, abnDate;
            for (int i = 0; i < jaCompanies.Count; i++)
            {
                JsonObject jo = jaCompanies[i].GetObject();
                id = jo.GetNamedString("ID");
                regNo = jo.GetNamedString("REGNO");
                name = jo.GetNamedString("ENTNAME");
                abnDate = jo.GetNamedString("ABNDATE");

                AbnormalCompanyListItem acli = new AbnormalCompanyListItem(id, regNo, name, abnDate);
                tempListItems.Add(acli);
            }

            //sortAbnormalCompanyList();
            foreach (var item in new ObservableCollection<AbnormalCompanyListItem>(tempListItems.OrderByDescending(item => item.Date)))
                abnormalCompanyListItems.Add(item);
        }

        private void sortAbnormalCompanyList()
        {
            ObservableCollection<AbnormalCompanyListItem> tempItems = new ObservableCollection<AbnormalCompanyListItem>(abnormalCompanyListItems.OrderByDescending(item => item.Date));
            abnormalCompanyListItems.Clear();
            foreach (var item in tempItems)
                abnormalCompanyListItems.Add(item);
        }

        private void abnormalCompanyList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (abnormalCompanyList.SelectedItem == null)
                return;
            var template = abnormalCompanyList.SelectedItem as AbnormalCompanyListItem;
            Frame.Navigate(typeof(CompanyDetailPage), "id=" + template.Id + "&regNo=" + template.RegNo + "&name=" + template.Name + "&index=3");
            abnormalCompanyList.SelectedItem = null;
        }

        private void searchTxtBox_GotFocus(object sender, RoutedEventArgs e)
        {
            //if (searchTxtBox.Text == "请输入公司名称、注册号或统一社会信用代码")
            //    searchTxtBox.Text = "";
            //else
            searchTxtBox.SelectAll();
            //searchTxtBox.Foreground = new SolidColorBrush(Colors.Black);
        }

        //private void searchTxtBox_LostFocus(object sender, RoutedEventArgs e)
        //{
        //    if (searchTxtBox.Text.Trim() == "")
        //        searchTxtBox.Text = "请输入公司名称、注册号或统一社会信用代码";
        //    searchTxtBox.Foreground = new SolidColorBrush(Colors.Gray);
        //}

        private void searchTxtBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                e.Handled = true;
                if (!progressRing.IsActive)
                    searchCompany();
            }
        }

        private void searchButton_Click(object sender, RoutedEventArgs e)
        {
            if (!progressRing.IsActive)
                searchCompany();
        }

        private void menuFlyout_Closed(object sender, object e)
        {
            scciForClipboard = new SearchedCompanyClipboardItem("", "", "", "", "");
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

        private void copyDateButton_Click(object sender, RoutedEventArgs e)
        {
            DataPackage dp = new DataPackage();
            dp.SetText(scciForClipboard.EstDate);
            Clipboard.SetContent(dp);
        }

        private void copyCompanyAbnormalInfoButton_Click(object sender, RoutedEventArgs e)
        {
            DataPackage dp = new DataPackage();
            dp.SetText("公司名称: " + scciForClipboard.Name + "\r\n注册号: " + scciForClipboard.RegNo + "\r\n被列入日期: " + scciForClipboard.EstDate);
            Clipboard.SetContent(dp);
        }

        private void abnormalCompanyItemGrid_Holding(object sender, HoldingRoutedEventArgs e)
        {
            AbnormalCompanyListItem acli = (sender as Grid).DataContext as AbnormalCompanyListItem;
            scciForClipboard = new SearchedCompanyClipboardItem(acli.RegNo, acli.Name, acli.Date, "", "");

            menuFlyout.ShowAt(abnormalCompanyList, e.GetPosition(abnormalCompanyList));
        }

        private void abnormalCompanyItemGrid_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            AbnormalCompanyListItem acli = (sender as Grid).DataContext as AbnormalCompanyListItem;
            scciForClipboard = new SearchedCompanyClipboardItem(acli.RegNo, acli.Name, acli.Date, "", "");

            menuFlyout.ShowAt(abnormalCompanyList, e.GetPosition(abnormalCompanyList));
        }

        private void abnormalCompanyList_Loaded(object sender, RoutedEventArgs e)
        {
            scrollViewer = Functions.FindVisualChildByName<ScrollViewer>(abnormalCompanyList, "ScrollViewer");
            verticalScrollBar = Functions.FindVisualChildByName<ScrollBar>(scrollViewer, "VerticalScrollBar");
            verticalScrollBar.ValueChanged += verticalScrollBar_ValueChanged;
        }

        private void verticalScrollBar_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (e.NewValue > e.OldValue && e.NewValue >= (verticalScrollBar.Maximum - 200))
            {
                if (searchedCompanyPageIndex < 5 && !progressRing.IsActive && abnormalCompanyListItems.Count == (searchedCompanyPageIndex * searchedCompanyCountInPage))
                {
                    Task.Factory.StartNew(async () =>
                    {
                        await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                        {
                            searchedCompanyPageIndex++;
                            httpClient_loadAbnormalCompanyList(WebUrl.getAbnormalCompanyListJsonHead + Functions.convertStringToBase64(searchString) + WebUrl.getAbnormalCompanyListJsonCenter + searchedCompanyPageIndex.ToString() + WebUrl.getAbnormalCompanyListJsonEnd + searchedCompanyCountInPage.ToString());
                        });
                    });
                }
            }
        }
    }
}
