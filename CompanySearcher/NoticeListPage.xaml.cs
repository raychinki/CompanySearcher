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
    public sealed partial class NoticeListPage : Page
    {
        private ObservableCollection<NoticeListItem> noticeListItems = new ObservableCollection<NoticeListItem>();
        private SearchedCompanyClipboardItem scciForClipboard = new SearchedCompanyClipboardItem("", "", "", "", "");
        public ScrollViewer scrollViewer = new ScrollViewer();
        public ScrollBar verticalScrollBar = new ScrollBar();
        private int noticeCountInPage = 20, noticePageIndex = 1;
        private string searchString = "";

        public NoticeListPage()
        {
            this.InitializeComponent();

            NavigationCacheMode = NavigationCacheMode.Enabled;

            noticeList.ItemsSource = noticeListItems;

            searchNoticeList();
        }

        private void searchNoticeList()
        {
            noticeListItems.Clear();
            noticePageIndex = 1;

            httpClient_loadAbnormalCompanyList(WebUrl.getNoticeListJsonHead + noticePageIndex.ToString() + WebUrl.getNoticeListJsonEnd + noticeCountInPage.ToString());
        }

        private async void httpClient_loadAbnormalCompanyList(string url)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    progressRing.IsActive = true;
                    noResultTxt.Visibility = Visibility.Collapsed;

                    loadNoticeList(await client.GetStringAsync(url));

                    progressRing.IsActive = false;
                    if (noticeListItems.Count == 0)
                        noResultTxt.Visibility = Visibility.Visible;
                }
        }
            catch
            {
                progressRing.IsActive = false;
                if (noticeListItems.Count == 0)
                    noResultTxt.Visibility = Visibility.Visible;
            }
}

        private void loadNoticeList(string contents)
        {
            JsonObject jContents = JsonObject.Parse(contents);
            JsonArray jaNoticeList = jContents.GetNamedArray("result");

            ObservableCollection<NoticeListItem> tempListItems = new ObservableCollection<NoticeListItem>();
            string id, noticeName, noticeDate, companyName, companyId, companyNo;
            for (int i = 0; i < jaNoticeList.Count; i++)
            {
                JsonObject jo = jaNoticeList[i].GetObject();
                id = jo.GetNamedString("ID");
                noticeName = jo.GetNamedString("CNAME");
                noticeDate = jo.GetNamedString("CDATE");
                companyName = jo.GetNamedString("CORP_NAME");
                companyId = jo.GetNamedValue("CORP_ID").ToString();
                companyNo = jo.GetNamedString("REG_NO");

                NoticeListItem acli = new NoticeListItem(id, noticeName, noticeDate, companyName, companyId, companyNo);
                tempListItems.Add(acli);
            }

            //sortAbnormalCompanyList();
            foreach (var item in new ObservableCollection<NoticeListItem>(tempListItems.OrderByDescending(item => item.NoticeDate)))
                noticeListItems.Add(item);
        }

        private void sortAbnormalCompanyList()
        {
            ObservableCollection<NoticeListItem> tempItems = new ObservableCollection<NoticeListItem>(noticeListItems.OrderByDescending(item => item.NoticeDate));
            noticeListItems.Clear();
            foreach (var item in tempItems)
                noticeListItems.Add(item);
        }

        private void noticeList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (noticeList.SelectedItem == null)
                return;
            var template = noticeList.SelectedItem as NoticeListItem;
            Frame.Navigate(typeof(NoticeDetailPage), "id=" + template.Id + "&noticeName=" + template.NoticeName + "&companyName=" + template.CompanyName + "&companyId=" + template.CompanyId + "&companyNo=" + template.CompanyNo);
            noticeList.SelectedItem = null;
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
                    searchNoticeList();
            }
        }

        private void searchButton_Click(object sender, RoutedEventArgs e)
        {
            if (!progressRing.IsActive)
                searchNoticeList();
        }

        private void menuFlyout_Closed(object sender, object e)
        {
            scciForClipboard = new SearchedCompanyClipboardItem("", "", "", "", "");
        }

        private void copyNoticeNameButton_Click(object sender, RoutedEventArgs e)
        {
            DataPackage dp = new DataPackage();
            dp.SetText(scciForClipboard.Name);
            Clipboard.SetContent(dp);
        }

        private void copyCompanyNameButton_Click(object sender, RoutedEventArgs e)
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

        private void copyNoticeInfoButton_Click(object sender, RoutedEventArgs e)
        {
            DataPackage dp = new DataPackage();
            dp.SetText("公告名称: " + scciForClipboard.Name + "\r\n公司名称: " + scciForClipboard.RegNo + "\r\n公告时间: " + scciForClipboard.EstDate);
            Clipboard.SetContent(dp);
        }

        private void noticeItemGrid_Holding(object sender, HoldingRoutedEventArgs e)
        {
            NoticeListItem nli = (sender as Grid).DataContext as NoticeListItem;
            scciForClipboard = new SearchedCompanyClipboardItem(nli.NoticeName, nli.CompanyName, nli.NoticeDate, "", "");

            menuFlyout.ShowAt(noticeList, e.GetPosition(noticeList));
        }

        private void noticeItemGrid_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            NoticeListItem nli = (sender as Grid).DataContext as NoticeListItem;
            scciForClipboard = new SearchedCompanyClipboardItem(nli.CompanyName, nli.NoticeName, nli.NoticeDate, "", "");

            menuFlyout.ShowAt(noticeList, e.GetPosition(noticeList));
        }

        private void noticeList_Loaded(object sender, RoutedEventArgs e)
        {
            scrollViewer = Functions.FindVisualChildByName<ScrollViewer>(noticeList, "ScrollViewer");
            verticalScrollBar = Functions.FindVisualChildByName<ScrollBar>(scrollViewer, "VerticalScrollBar");
            verticalScrollBar.ValueChanged += verticalScrollBar_ValueChanged;
        }

        private void verticalScrollBar_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (e.NewValue > e.OldValue && e.NewValue >= (verticalScrollBar.Maximum - 200))
            {
                if (noticePageIndex < 5 && !progressRing.IsActive && noticeListItems.Count == (noticePageIndex * noticeCountInPage))
                {
                    Task.Factory.StartNew(async () =>
                    {
                        await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                        {
                            noticePageIndex++;
                            httpClient_loadAbnormalCompanyList(WebUrl.getNoticeListJsonHead + noticePageIndex.ToString() + WebUrl.getNoticeListJsonEnd + noticeCountInPage.ToString());
                        });
                    });
                }
            }
        }
    }
}
