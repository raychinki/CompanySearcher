using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
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
    public sealed partial class FramePage : Page
    {
        private ApplicationDataContainer settings = ApplicationData.Current.LocalSettings;

        public FramePage()
        {
            this.InitializeComponent();

            initTitleBar();
            initSplit();

            SystemNavigationManager.GetForCurrentView().BackRequested += frame_BackRequested;

            contentFrame.Navigate(typeof(SearchListPage));
        }

        private void initTitleBar()
        {
            var view = ApplicationView.GetForCurrentView();

            view.SetPreferredMinSize(new Size(500, 500));

            view.TitleBar.BackgroundColor = SearcherColors.ShieldRed;
            view.TitleBar.ForegroundColor = Colors.White;

            view.TitleBar.ButtonBackgroundColor = SearcherColors.ShieldRed;
            view.TitleBar.ButtonForegroundColor = Colors.White;
            view.TitleBar.ButtonHoverBackgroundColor = SearcherColors.DarkRed;
            view.TitleBar.ButtonHoverForegroundColor = Colors.White;
        }

        private void initSplit()
        {
            ObservableCollection<SplitListItem> splitTopItems = new ObservableCollection<SplitListItem>();
            ObservableCollection<SplitListItem> splitBottomItems = new ObservableCollection<SplitListItem>();

            splitTopItems.Add(new SplitListItem("企业查询", "Find"));
            splitTopItems.Add(new SplitListItem("经营异常", "ReportHacked"));
            splitTopItems.Add(new SplitListItem("严重违法", "Admin"));
            splitTopItems.Add(new SplitListItem("抽查检查", "View"));
            //splitTopItems.Add(new SplitListItem("信息公告", "Page2"));

            splitBottomItems.Add(new SplitListItem("关于", "Flag"));
            //splitBottomItems.Add(new SplitListItem("系统设置", "Setting"));

            splitTopList.ItemsSource = splitTopItems;
            splitBottomList.ItemsSource = splitBottomItems;
        }

        private void initSettings()
        {
            if (!settings.Values.ContainsKey("areaCode"))
            {
                settings.Values.Add("areaCode", "320000");
            }
            if (!settings.Values.ContainsKey("areaName"))
            {
                settings.Values.Add("areaName", "江苏");
            }
        }

        private void frame_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (contentFrame != null && contentFrame.CanGoBack)
            {
                e.Handled = true;
                contentFrame.GoBack();
            }
        }

        private void contentFrame_Navigated(object sender, NavigationEventArgs e)
        {
            if (contentFrame.CanGoBack)
            {
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            }
            else
            {
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            }

            switch (e.SourcePageType.Name)
            {
                case "SearchListPage":
                    {
                        splitTopList.SelectedIndex = 0;
                        splitBottomList.SelectedIndex = -1;
                    }
                    break;
                case "AbnormalListPage":
                    {
                        splitTopList.SelectedIndex = 1;
                        splitBottomList.SelectedIndex = -1;
                    }
                    break;
                case "IllegalListPage":
                    {
                        splitTopList.SelectedIndex = 2;
                        splitBottomList.SelectedIndex = -1;
                    }
                    break;
                case "CheckListPage":
                    {
                        splitTopList.SelectedIndex = 3;
                        splitBottomList.SelectedIndex = -1;
                    }
                    break;
                case "AboutPage":
                    {
                        splitTopList.SelectedIndex = -1;
                        splitBottomList.SelectedIndex = 0;
                    }
                    break;
                default:
                    {
                        splitTopList.SelectedIndex = -1;
                        splitBottomList.SelectedIndex = -1;
                    }
                    break;
            }
        }

        private void splitTopList_ItemClick(object sender, ItemClickEventArgs e)
        {
            var template = e.ClickedItem as SplitListItem;
            switch (template.Title)
            {
                case "企业查询":
                    {
                        if (splitTopList.SelectedIndex != 0)
                            contentFrame.Navigate(typeof(SearchListPage));
                    }
                    break;
                case "经营异常":
                    {
                        if (splitTopList.SelectedIndex != 1)
                            contentFrame.Navigate(typeof(AbnormalListPage));
                    }
                    break;
                case "严重违法":
                    {
                        //contentFrame.Navigate(typeof(CompanyDetailPage), "id=128549&regNo=320000000035248&name=苏宁云商集团股份有限公司");
                        if (splitTopList.SelectedIndex != 2)
                            contentFrame.Navigate(typeof(IllegalListPage));
                    }
                    break;
                case "抽查检查":
                    {
                        if (splitTopList.SelectedIndex != 3)
                            contentFrame.Navigate(typeof(CheckListPage));
                    }
                    break;
                default: break;
            }
        }

        private void splitBottomList_ItemClick(object sender, ItemClickEventArgs e)
        {
            var template = e.ClickedItem as SplitListItem;
            switch (template.Title)
            {
                case "关于":
                    {
                        if (splitBottomList.SelectedIndex != 0)
                            contentFrame.Navigate(typeof(AboutPage));
                    }
                    break;
                default: break;
            }
        }

        private void splitViewToggle_Click(object sender, RoutedEventArgs e)
        {
            splitView.IsPaneOpen = !splitView.IsPaneOpen;
        }
    }
}
