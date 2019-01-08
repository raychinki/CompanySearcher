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
    public sealed partial class CheckListPage : Page
    {
        private ObservableCollection<CheckCompanyListItem> checkCompanyListItems = new ObservableCollection<CheckCompanyListItem>();

        public CheckListPage()
        {
            this.InitializeComponent();

            NavigationCacheMode = NavigationCacheMode.Enabled;

            httpClient_loadCheckCompanyList(WebUrl.getCheckCompanyListJsonHead + WebUrl.getCheckCompanyListJsonCenter1 + "1" + WebUrl.getCheckCompanyListJsonCenter2 + "1" + WebUrl.getCheckCompanyListJsonEnd + "20");

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
                id = jo.GetNamedValue("ID").ToString();
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
            Frame.Navigate(typeof(CompanyDetailPage), "id=" + template.Id + "&regNo=" + template.RegNo + "&name=" + template.Name + "&index=3");
            checkCompanyList.SelectedItem = null;
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
                checkCompanyListItems.Clear();
                httpClient_loadCheckCompanyList(WebUrl.getCheckCompanyListJsonHead + Functions.convertStringToBase64(searchTxtBox.Text) + WebUrl.getCheckCompanyListJsonCenter1 + "1" + WebUrl.getCheckCompanyListJsonCenter2 + "1" + WebUrl.getCheckCompanyListJsonEnd + "10");
            }
        }

        private void searchButton_Click(object sender, RoutedEventArgs e)
        {
            checkCompanyListItems.Clear();
            if (searchTxtBox.Text == "请输入公司名称、注册号或统一社会信用代码")
                httpClient_loadCheckCompanyList(WebUrl.getCheckCompanyListJsonHead + WebUrl.getCheckCompanyListJsonCenter1 + "1" + WebUrl.getCheckCompanyListJsonCenter2 + "1" + WebUrl.getCheckCompanyListJsonEnd + "20");
            else
                httpClient_loadCheckCompanyList(WebUrl.getCheckCompanyListJsonHead + Functions.convertStringToBase64(searchTxtBox.Text) + WebUrl.getCheckCompanyListJsonCenter1 + "1" + WebUrl.getCheckCompanyListJsonCenter2 + "1" + WebUrl.getCheckCompanyListJsonEnd + "10");
        }
    }
}
