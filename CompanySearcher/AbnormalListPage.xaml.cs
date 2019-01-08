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

        public AbnormalListPage()
        {
            this.InitializeComponent();

            NavigationCacheMode = NavigationCacheMode.Enabled;

            httpClient_loadAbnormalCompanyList(WebUrl.getAbnormalCompanyListJsonHead + WebUrl.getAbnormalCompanyListJsonCenter + "1" + WebUrl.getAbnormalCompanyListJsonEnd + "20");

            abnormalCompanyList.ItemsSource = abnormalCompanyListItems;
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
                id = jo.GetNamedValue("ID").ToString();
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
            Frame.Navigate(typeof(CompanyDetailPage), "id=" + template.Id + "&regNo=" + template.RegNo + "&name=" + template.Name);
            abnormalCompanyList.SelectedItem = null;
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
                abnormalCompanyListItems.Clear();
                httpClient_loadAbnormalCompanyList(WebUrl.getAbnormalCompanyListJsonHead + Functions.convertStringToBase64(searchTxtBox.Text) + WebUrl.getAbnormalCompanyListJsonCenter + "1" + WebUrl.getAbnormalCompanyListJsonEnd + "10");
            }
        }

        private void searchButton_Click(object sender, RoutedEventArgs e)
        {
            abnormalCompanyListItems.Clear();
            if(searchTxtBox.Text == "请输入公司名称、注册号或统一社会信用代码")
                httpClient_loadAbnormalCompanyList(WebUrl.getAbnormalCompanyListJsonHead + WebUrl.getAbnormalCompanyListJsonCenter + "1" + WebUrl.getAbnormalCompanyListJsonEnd + "20");
            else
                httpClient_loadAbnormalCompanyList(WebUrl.getAbnormalCompanyListJsonHead + Functions.convertStringToBase64(searchTxtBox.Text) + WebUrl.getAbnormalCompanyListJsonCenter + "1" + WebUrl.getAbnormalCompanyListJsonEnd + "10");
        }
    }
}
