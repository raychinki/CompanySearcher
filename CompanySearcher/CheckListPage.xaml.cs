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
    public sealed partial class CheckListPage : Page
    {
        private ObservableCollection<CheckCompanyListItem> checkCompanyListItems = new ObservableCollection<CheckCompanyListItem>();
        private SearchedCompanyClipboardItem scciForClipboard = new SearchedCompanyClipboardItem("", "", "", "", "");
        public ScrollViewer scrollViewer = new ScrollViewer();
        public ScrollBar verticalScrollBar = new ScrollBar();
        private int searchedCompanyCountInPage = 20, searchedCompanyPageIndex = 1;
        private string searchString = "";

        public CheckListPage()
        {
            this.InitializeComponent();

            NavigationCacheMode = NavigationCacheMode.Enabled;

            httpClient_loadCheckCompanyList(WebUrl.getCheckCompanyListJsonHead + WebUrl.getCheckCompanyListJsonCenter1 + "1" + WebUrl.getCheckCompanyListJsonCenter2 + "1" + WebUrl.getCheckCompanyListJsonEnd + searchedCompanyCountInPage.ToString());

            checkCompanyList.ItemsSource = checkCompanyListItems;
        }

        private async void httpClient_loadCheckCompanyList(string url)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    progressRing.IsActive = true;
                    noResultTxt.Visibility = Visibility.Collapsed;

                    loadCheckCompanyList(await client.GetStringAsync(url));

                    progressRing.IsActive = false;
                    if (checkCompanyListItems.Count == 0)
                        noResultTxt.Visibility = Visibility.Visible;
                }
            }
            catch
            {
                progressRing.IsActive = false;
            }
        }

        private void loadCheckCompanyList(string contents)
        {
            JsonObject jContents = JsonObject.Parse(contents);
            JsonArray jaCompanies = jContents.GetNamedArray("result");
            
            ObservableCollection<CheckCompanyListItem> tempListItems = new ObservableCollection<CheckCompanyListItem>();
            string id, regNo, name, date, result, recColor;
            for (int i = 0; i < jaCompanies.Count; i++)
            {
                JsonObject jo = jaCompanies[i].GetObject();
                id = jo.GetNamedString("ID");
                regNo = jo.GetNamedString("REGNO");
                name = jo.GetNamedString("ENTNAME");
                date = jo.GetNamedString("CHECKDATE");
                result = jo.GetNamedString("CHECKRESULT");
                if (result.Contains("正常"))
                    recColor = "Green";
                else
                    recColor = "Black";

                CheckCompanyListItem ccili = new CheckCompanyListItem(id, regNo, name, date, result, recColor);
                tempListItems.Add(ccili);
            }

            //ObservableCollection<CheckCompanyListItem> tempItems = new ObservableCollection<CheckCompanyListItem>(checkCompanyListItems.OrderByDescending(item => item.Date));
            //checkCompanyListItems.Clear();
            foreach (var item in new ObservableCollection<CheckCompanyListItem>(tempListItems.OrderByDescending(item => item.Date)))
                checkCompanyListItems.Add(item);
        }

        //private void sortCheckCompanyList()
        //{
        //    ObservableCollection<CheckCompanyListItem> tempItems = new ObservableCollection<CheckCompanyListItem>(checkCompanyListItems.OrderByDescending(item => item.Date));
        //    checkCompanyListItems.Clear();
        //    foreach (var item in tempItems)
        //        checkCompanyListItems.Add(item);
        //}

        private void checkCompanyList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (checkCompanyList.SelectedItem == null)
                return;
            var template = checkCompanyList.SelectedItem as CheckCompanyListItem;
            Frame.Navigate(typeof(CompanyDetailPage), "id=" + template.Id + "&regNo=" + template.RegNo + "&name=" + template.Name + "&index=4");
            checkCompanyList.SelectedItem = null;
        }

        private void checkCompanyList_Loaded(object sender, RoutedEventArgs e)
        {
            scrollViewer = Functions.FindVisualChildByName<ScrollViewer>(checkCompanyList, "ScrollViewer");
            verticalScrollBar = Functions.FindVisualChildByName<ScrollBar>(scrollViewer, "VerticalScrollBar");
            verticalScrollBar.ValueChanged += verticalScrollBar_ValueChanged;
        }

        private void verticalScrollBar_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (e.NewValue > e.OldValue && e.NewValue >= (verticalScrollBar.Maximum - 200))
            {
                if (searchedCompanyPageIndex < 5 && !progressRing.IsActive && checkCompanyListItems.Count == (searchedCompanyPageIndex * searchedCompanyCountInPage))
                {
                    Task.Factory.StartNew(async () =>
                    {
                        await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                        {
                            searchedCompanyPageIndex++;
                            httpClient_loadCheckCompanyList(WebUrl.getCheckCompanyListJsonHead + Functions.convertStringToBase64(searchString) + WebUrl.getCheckCompanyListJsonCenter1 + searchedCompanyPageIndex.ToString() + WebUrl.getCheckCompanyListJsonCenter2 + "1" + WebUrl.getCheckCompanyListJsonEnd + searchedCompanyCountInPage.ToString());
                        });
                    });
                }
            }
        }

        private void searchTxtBox_GotFocus(object sender, RoutedEventArgs e)
        {
            searchTxtBox.SelectAll();
        }

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

        private void searchCompany()
        {
            checkCompanyListItems.Clear();
            searchedCompanyPageIndex = 1;
            searchString = searchTxtBox.Text.Trim();
            httpClient_loadCheckCompanyList(WebUrl.getCheckCompanyListJsonHead + Functions.convertStringToBase64(searchString) + WebUrl.getCheckCompanyListJsonCenter1 + searchedCompanyPageIndex.ToString() + WebUrl.getCheckCompanyListJsonCenter2 + "1" + WebUrl.getCheckCompanyListJsonEnd + searchedCompanyCountInPage.ToString());
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

        private void copyDateButton_Click(object sender, RoutedEventArgs e)
        {
            DataPackage dp = new DataPackage();
            dp.SetText(scciForClipboard.EstDate);
            Clipboard.SetContent(dp);
        }

        private void copyResultButton_Click(object sender, RoutedEventArgs e)
        {
            DataPackage dp = new DataPackage();
            dp.SetText(scciForClipboard.LegPerson);
            Clipboard.SetContent(dp);
        }

        private void copyCompanyCheckInfoButton_Click(object sender, RoutedEventArgs e)
        {
            DataPackage dp = new DataPackage();
            dp.SetText("公司名称: " + scciForClipboard.Name + "\r\n注册号: " + scciForClipboard.RegNo + "\r\n抽查日期: " + scciForClipboard.EstDate + "\r\n抽查结果: " + scciForClipboard.LegPerson);
            Clipboard.SetContent(dp);
        }

        private void checkCompanyItemGrid_Holding(object sender, HoldingRoutedEventArgs e)
        {
            CheckCompanyListItem ccli = (sender as Grid).DataContext as CheckCompanyListItem;
            scciForClipboard = new SearchedCompanyClipboardItem(ccli.RegNo, ccli.Name, ccli.Date, ccli.Result, "");

            menuFlyout.ShowAt(checkCompanyList, e.GetPosition(checkCompanyList));
        }

        private void checkCompanyItemGrid_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            CheckCompanyListItem ccli = (sender as Grid).DataContext as CheckCompanyListItem;
            scciForClipboard = new SearchedCompanyClipboardItem(ccli.RegNo, ccli.Name, ccli.Date, ccli.Result, "");

            menuFlyout.ShowAt(checkCompanyList, e.GetPosition(checkCompanyList));
        }
    }
}
