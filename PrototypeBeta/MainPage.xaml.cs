using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Telerik.Windows.Controls.Input;
using Telerik.Windows.Controls;
using PrototypeBeta.DataModels;
using PrototypeBeta.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Phone.Net.NetworkInformation;
using PrototypeBeta.ViewModels;
using PrototypeBeta.SQLite;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using SQLite;
using System.Xml.Linq;
using System.Windows.Controls.Primitives;

namespace PrototypeBeta
{
    public partial class MainPage : PhoneApplicationPage
    {
        public List<StationList> stationList;
        List<RecentSearchViewModel> recentSearchList;
        List<FavoriteScheduleViewModel> favoriteScheduleList;

        public MainPage()
        {
            InitializeComponent();

            createStationList();
            //Shows the rate reminder message, according to the settings of the RateReminder.
            //(App.Current as App).rateReminder.Notify();
        }


        private void rbSchedule_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton rbSchedule = (RadioButton)sender;
            try
            {
                if (rbSchedule.Name == "rbNextSchedule")
                {
                    radDatePicker.Value = DateTime.Now;
                    radDatePicker.IsReadOnly = true;

                    rtFromTime.Value = DateTime.Now;
                    rtFromTime.IsReadOnly = true;

                }
                else
                {
                    radDatePicker.Value = null;
                    radDatePicker.IsReadOnly = false;

                    rtFromTime.IsReadOnly = false;
                    rtFromTime.Value = DateTime.MinValue;
                }
            }
            catch (Exception ex)
            {

            }
        }


        private void cbTimeFilter_Checked(object sender, RoutedEventArgs e)
        {
            if (rbNextSchedule.IsChecked == true)
            {
                rtToTime.IsReadOnly = false;
                rtFromTime.Value = System.DateTime.Now;
            }
            else
            {
                rtFromTime.IsReadOnly = false;
            }
        }


        bool isInterntConAvailable()
        {
            if (DeviceNetworkInformation.IsNetworkAvailable)
                return true;
            return false;
        }
        void createStationList()
        {
            if (isInterntConAvailable())
            {
                WebClient webClient = new WebClient();
                webClient.DownloadStringCompleted += webClient_DownloadTrainRoutesCompleted;
                webClient.DownloadStringAsync(new Uri(WebService.RailwayStations));
            }
            else
            {
                populateOfflineStationList();
                // RadMessageBox.ShowAsync("Error!", MessageBoxButtons.OK, "Network issue. Please check your internet connection.");
            }
        }

        void populateOfflineStationList()
        {
            XDocument loadedData = XDocument.Load("StaticData/StationsStaticData.xml");

            var data = from query in loadedData.Descendants("Station")
                       select new StationList()
                       {
                           stationCode = (string)query.Element("stationCode"),
                           stationID = (int)query.Element("stationID"),
                           stationName = (string)query.Element("stationName")

                       };
            stationList = data.ToList();
            formatStationName();
            populateStations();
        }
        void createRecentSearchList()
        {
            recentSearchList = new List<RecentSearchViewModel>();
            List<Recent_Search_Table> recentSearches = DataAccess.getRecentSearches();

            foreach (Recent_Search_Table item in recentSearches)
            {
                RecentSearchViewModel recentSearch = new RecentSearchViewModel();
                recentSearch.recordId = item.RecordId;
                recentSearch.startStationId = item.startStationId;
                recentSearch.endStationId = item.endStationId;
                recentSearch.startStationName = AppUtility.ToCamelCase(item.startStationName);
                recentSearch.endStationName = AppUtility.ToCamelCase(item.endStationName);
                recentSearch.startTime = item.startTime;
                recentSearch.endTime = item.endTime;
                recentSearch.dayOfWeek = item.startTime.DayOfWeek.ToString();
                recentSearch.startTimeStr = item.startTime.ToShortTimeString();
                recentSearch.endTimeStr = item.endTime.ToShortTimeString();
                recentSearchList.Add(recentSearch);
            }

        }
        void createFavoriteScheduleList()
        {
            favoriteScheduleList = new List<FavoriteScheduleViewModel>();
            List<Favorite_Schedule_Table> favoriteSchedules = DataAccess.getFavoriteSchedule();

            foreach (Favorite_Schedule_Table item in favoriteSchedules)
            {
                FavoriteScheduleViewModel favoriteSchedule = new FavoriteScheduleViewModel();
                favoriteSchedule.RecordId = item.RecordId;
                favoriteSchedule.favoriteName = item.favoriteName;
                favoriteSchedule.startStationId = item.startStationId;
                favoriteSchedule.endStationId = item.endStationId;
                favoriteSchedule.startStationName = AppUtility.ToCamelCase(item.startStationName);
                favoriteSchedule.endStationName = AppUtility.ToCamelCase(item.endStationName);
                favoriteSchedule.startTimeStr = item.startTime.ToShortTimeString();
                favoriteSchedule.dayOfWeek = item.startTime.DayOfWeek.ToString();
                favoriteSchedule.startTime = item.startTime;
                favoriteSchedule.endTime = item.endTime;
                favoriteSchedule.endTimeStr = item.endTime.ToShortTimeString();
                favoriteScheduleList.Add(favoriteSchedule);
            }

        }

        private void webClient_DownloadTrainRoutesCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                string json = e.Result;
                if (!string.IsNullOrEmpty(json))
                {
                    RootStationObject rootStationObj = JsonConvert.DeserializeObject<RootStationObject>(json);
                    stationList = rootStationObj.RESULTS.stationList;
                    formatStationName();

                    if (stationList != null)
                    {
                        populateStations();
                    }
                }
            }
            catch (Exception ex)
            {
                populateOfflineStationList();
            }
        }

        void formatStationName()
        {
            for (int i = 0; i < stationList.Count; i++)
            {
                stationList[i].stationName = AppUtility.ToCamelCase(stationList[i].stationName);
            }

        }
        void populateStations()
        {
            acbFromStation.ItemsSource = stationList;
            acbToStation.ItemsSource = stationList;
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (isInterntConAvailable())
            {
                if (validateInputs())
                {
                    TRAINQUERY queryObj = getQueryObject();
                    logRecentSearch();
                    //logFavoriteSchedule();
                    NavigationService.Navigate(new Uri("/ScheduleListPage.xaml", UriKind.Relative), queryObj);
                }
                else
                {
                    RadMessageBox.ShowAsync("AW SNAP!", MessageBoxButtons.OK, "Looks like you have not selected start station and end station");
                }
            }
            else
            {
                RadMessageBox.ShowAsync("AW SNAP :-(", MessageBoxButtons.OK, "We are having trouble downloading data due to network connectivity or temporary server unavailability at the moment. Please make sure you are connected to internet and try again.");
            }
        }

        private bool validateInputs()
        {

            if (acbFromStation.SelectedItem != null && acbToStation.SelectedItem != null)
            {
                return true;
            }
            return false;
        }
        TRAINQUERY getQueryObject()
        {

            TRAINQUERY queryObj = new TRAINQUERY();

            StationList startStaion = (StationList)acbFromStation.SelectedItem;
            StationList endStaion = (StationList)acbToStation.SelectedItem;
            DateTime searchDate = (radDatePicker.Value != null) ? Convert.ToDateTime(radDatePicker.Value) : DateTime.Now;
            DateTime searchTimeStart = (cbTimeFilter.IsChecked == true || rbNextSchedule.IsChecked == true) ? Convert.ToDateTime(rtFromTime.Value) : APPCONTEXT.defaultStartTime;
            DateTime searchTimeEnd = (cbTimeFilter.IsChecked == true) ? Convert.ToDateTime(rtToTime.Value) : APPCONTEXT.defaultEndTime;

            queryObj.startStaion = startStaion.stationID;
            queryObj.startStaionName = startStaion.stationName;
            queryObj.endStaion = endStaion.stationID;
            queryObj.endStaionName = endStaion.stationName;
            queryObj.searchDate = searchDate.ToString("yyyy-MM-dd");
            queryObj.searchTimeStart = searchTimeStart.ToString("HH:mm:ss");
            queryObj.searchTimeEnd = searchTimeEnd.ToString("HH:mm:ss");

            return queryObj;
        }

        TRAINQUERY getQueryObject(RecentSearchViewModel recentSearch)
        {

            TRAINQUERY queryObj = new TRAINQUERY();

            queryObj.startStaion = recentSearch.startStationId;
            queryObj.startStaionName = recentSearch.startStationName;
            queryObj.endStaion = recentSearch.endStationId;
            queryObj.endStaionName = recentSearch.endStationName;
            queryObj.searchDate = recentSearch.startTime.ToString("yyyy-MM-dd");
            queryObj.searchTimeStart = recentSearch.startTime.ToString("HH:mm:ss");
            queryObj.searchTimeEnd = recentSearch.endTime.ToString("HH:mm:ss");

            return queryObj;
        }
        TRAINQUERY getQueryObject(FavoriteScheduleViewModel favoriteSchedule)
        {

            TRAINQUERY queryObj = new TRAINQUERY();

            queryObj.startStaion = favoriteSchedule.startStationId;
            queryObj.startStaionName = favoriteSchedule.startStationName;
            queryObj.endStaion = favoriteSchedule.endStationId;
            queryObj.endStaionName = favoriteSchedule.endStationName;
            queryObj.searchDate = favoriteSchedule.startTime.ToString("yyyy-MM-dd");
            queryObj.searchTimeStart = favoriteSchedule.startTime.ToString("HH:mm:ss");
            queryObj.searchTimeEnd = favoriteSchedule.endTime.ToString("HH:mm:ss");

            return queryObj;
        }
        private void Panorama_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplicationBarIconButton appBarButton = ((ApplicationBarIconButton)ApplicationBar.Buttons[0]);
            switch (((Panorama)sender).SelectedIndex)
            {
                case 0:
                    ApplicationBar.IsVisible = true;
                    appBarButton.IsEnabled = true;
                    appBarButton.IconUri = new Uri("/Images/AppBarIcon/appbar.arrow.down.up.png", UriKind.Relative);
                    appBarButton.Text = "stations";
                    break;
                case 1:
                    ApplicationBar.IsVisible = false;
                    break;
                case 2:
                    ApplicationBar.IsVisible = true;
                    appBarButton.IsEnabled = false;
                    appBarButton.IconUri = new Uri("/Images/AppBarIcon/delete.png", UriKind.Relative);
                    appBarButton.Text = "clear";
                    if (lsFavoriteSchedule.ItemsSource.Count > 0)
                    {
                        appBarButton.IsEnabled = true;
                    }
                    break;
                case 3:
                    ApplicationBar.IsVisible = true;
                    appBarButton.IsEnabled = false;
                    appBarButton.IconUri = new Uri("/Images/AppBarIcon/delete.png", UriKind.Relative);
                    appBarButton.Text = "clear";
                    if (lsRecentSearch.ItemsSource.Count > 0)
                    {
                        appBarButton.IsEnabled = true;
                    }
                    break;
            }
        }
        void populateRecentSearches()
        {

            lsRecentSearch.ItemsSource = recentSearchList.OrderByDescending(x => x.recordId).ToList();
            pnlEmptyRcntList.Visibility = Visibility.Visible;
            if (lsRecentSearch.ItemsSource != null)
            {
                if (lsRecentSearch.ItemsSource.Count > 0)
                {
                    pnlEmptyRcntList.Visibility = Visibility.Collapsed;
                }
            }
        }
        void populateFavoriteSchedule()
        {

            lsFavoriteSchedule.ItemsSource = favoriteScheduleList.OrderBy(x => x.favoriteName).ToList();
            pnlEmptyFavList.Visibility = Visibility.Visible;
            if (lsFavoriteSchedule.ItemsSource != null)
            {
                if (lsFavoriteSchedule.ItemsSource.Count > 0)
                {
                    pnlEmptyFavList.Visibility = Visibility.Collapsed;
                }
            }
        }

        void logRecentSearch()
        {

            Recent_Search_Table recentSearchRecord = new Recent_Search_Table();
            StationList startStaion = (StationList)acbFromStation.SelectedItem;
            StationList endStaion = (StationList)acbToStation.SelectedItem;
            DateTime searchDate = (radDatePicker.Value != null) ? Convert.ToDateTime(radDatePicker.Value) : DateTime.Now;
            DateTime searchTimeStart = (cbTimeFilter.IsChecked == true || rbNextSchedule.IsChecked == true) ? Convert.ToDateTime(rtFromTime.Value) : APPCONTEXT.defaultStartTime;
            DateTime searchTimeEnd = (cbTimeFilter.IsChecked == true) ? Convert.ToDateTime(rtToTime.Value) : APPCONTEXT.defaultEndTime;

            recentSearchRecord.startStationId = startStaion.stationID;
            recentSearchRecord.endStationId = endStaion.stationID;
            recentSearchRecord.startStationName = startStaion.stationName;
            recentSearchRecord.endStationName = endStaion.stationName;
            recentSearchRecord.startTime = searchTimeStart;
            recentSearchRecord.endTime = searchTimeEnd;

            DataAccess.addRecentSearch(recentSearchRecord);
        }
        void logFavoriteSchedule()
        {

            Favorite_Schedule_Table favoriteScheduleRecord = new Favorite_Schedule_Table();
            StationList startStaion = (StationList)acbFromStation.SelectedItem;
            StationList endStaion = (StationList)acbToStation.SelectedItem;
            DateTime searchDate = (radDatePicker.Value != null) ? Convert.ToDateTime(radDatePicker.Value) : DateTime.Now;
            DateTime searchTimeStart = (cbTimeFilter.IsChecked == true || rbNextSchedule.IsChecked == true) ? Convert.ToDateTime(rtFromTime.Value) : APPCONTEXT.defaultStartTime;
            DateTime searchTimeEnd = (cbTimeFilter.IsChecked == true) ? Convert.ToDateTime(rtToTime.Value) : APPCONTEXT.defaultEndTime;

            favoriteScheduleRecord.favoriteName = "My Favosrite";
            favoriteScheduleRecord.startStationId = startStaion.stationID;
            favoriteScheduleRecord.endStationId = endStaion.stationID;
            favoriteScheduleRecord.startStationName = startStaion.stationName;
            favoriteScheduleRecord.endStationName = endStaion.stationName;
            favoriteScheduleRecord.startTime = searchTimeStart;
            favoriteScheduleRecord.endTime = searchTimeEnd;

            DataAccess.adFavoriteSchedule(favoriteScheduleRecord);
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            DataAccess.createTables();
            createRecentSearchList();
            populateRecentSearches();
            createFavoriteScheduleList();
            populateFavoriteSchedule();
        }

        private void lsFavoriteSchedule_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            FavoriteScheduleViewModel favoriteSchedule = (FavoriteScheduleViewModel)lsFavoriteSchedule.SelectedItem;
            if (favoriteSchedule != null)
            {
                if (isInterntConAvailable())
                {
                    TRAINQUERY queryObj = getQueryObject(favoriteSchedule);
                    lsFavoriteSchedule.SelectedItem = null;
                    NavigationService.Navigate(new Uri("/ScheduleListPage.xaml", UriKind.Relative), queryObj);
                }
                else
                {
                    lsFavoriteSchedule.SelectedItem = null;
                    RadMessageBox.ShowAsync("AW SNAP :-(", MessageBoxButtons.OK, "We are having trouble downloading data due to network connectivity or temporary server unavailability at the moment. Please make sure you are connected to internet and try again.");
                }
            }
        }

        private void lsRecentSearch_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            RecentSearchViewModel recentSearch = (RecentSearchViewModel)lsRecentSearch.SelectedItem;
            if (recentSearch != null)
            {
                if (isInterntConAvailable())
                {
                    lsRecentSearch.SelectedItem = null;
                    TRAINQUERY queryObj = getQueryObject(recentSearch);
                    NavigationService.Navigate(new Uri("/ScheduleListPage.xaml", UriKind.Relative), queryObj);
                }
                else
                {
                    lsRecentSearch.SelectedItem = null;
                    RadMessageBox.ShowAsync("AW SNAP :-(", MessageBoxButtons.OK, "We are having trouble downloading data due to network connectivity or temporary server unavailability at the moment. Please make sure you are connected to internet and try again.");
                }
            }
        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            ApplicationBarIconButton appBarButton = ((ApplicationBarIconButton)ApplicationBar.Buttons[0]);
            switch (mainPanorama.SelectedIndex)
            {
                case 0:
                    if (acbFromStation.SelectedItem != null && acbToStation.SelectedItem != null)
                    {
                        StationList startStaionTmp = (StationList)acbFromStation.SelectedItem;
                        StationList endStaionTmp = (StationList)acbToStation.SelectedItem;
                        acbFromStation.SelectedItem = endStaionTmp;
                        acbToStation.SelectedItem = startStaionTmp;
                    }
                    break;
                case 2:
                    clearFavoriteScheduleList();
                    appBarButton.IsEnabled = false;
                    break;
                case 3:
                    clearRecentSearchList();
                    appBarButton.IsEnabled = false;
                    break;
            }
        }

        void clearRecentSearchList()
        {
            ControlTemplate imageTemplate = (ControlTemplate)this.Resources["ImageTemplate"];
            RadMessageBox.Show(imageTemplate, APPCONTEXT.recentSearchListPromptTitle, new string[] { "Yes", "No" }, APPCONTEXT.recentSearchListPromptMessage, closedHandler: (args) =>
            {
                DialogResult result = args.Result;
                int buttonIndex = args.ButtonIndex;
                if (buttonIndex == 0)
                {
                    DataAccess.removeRecentSearches();
                    createRecentSearchList();
                    populateRecentSearches();
                }
            });
        }
        void clearFavoriteScheduleList()
        {
            ControlTemplate imageTemplate = (ControlTemplate)this.Resources["ImageTemplate"];
            RadMessageBox.Show(imageTemplate, APPCONTEXT.favoriteScheduleListPromptTitle, new string[] { "Yes", "No" }, APPCONTEXT.favoriteScheduleListPromptMessage, closedHandler: (args) =>
            {
                DialogResult result = args.Result;
                int buttonIndex = args.ButtonIndex;
                if (buttonIndex == 0)
                {
                    DataAccess.removeFavoriteSchedules();
                    createFavoriteScheduleList();
                    populateFavoriteSchedule();
                }
            });
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem selectedCtx = (MenuItem)sender;
            FavoriteScheduleViewModel favSchedView = (sender as MenuItem).DataContext as FavoriteScheduleViewModel;

            if (selectedCtx.Name.Equals("menuRename"))
            {
                ControlTemplate imageTemplate = (ControlTemplate)this.Resources["InputPrompt"];
                //RadInputPrompt.Show(imageTemplate, "Title", new string[] { "yes", "no" });

                RadInputPrompt.Show(imageTemplate, APPCONTEXT.scheduleListPromptTitle, new string[] { "save", "cancel" }, APPCONTEXT.scheduleListPromptMessage, closedHandler: (args) =>
                {

                    string value = args.Text;
                    int buttonIndex = args.ButtonIndex;
                    if (buttonIndex == 0)
                    {
                        updateFavoriteShceduleName(favSchedView, value);
                        createFavoriteScheduleList();
                        populateFavoriteSchedule();
                        RadMessageBox.ShowAsync("There you go :)", MessageBoxButtons.OK, "Your favorite shedule is updated successfully.");
                    }

                });
            }
            else
            {
                removeFavoriteSchedule(favSchedView);
                createFavoriteScheduleList();
                populateFavoriteSchedule();
                RadMessageBox.ShowAsync("There you go :)", MessageBoxButtons.OK, "Shedule is removed from your favorites.");
            }
        }

        void updateFavoriteShceduleName(FavoriteScheduleViewModel favSchedView, string value)
        {
            Favorite_Schedule_Table temp = getFavSchedTabObj(favSchedView, value);
            DataAccess.modifyFavoriteSchedule(temp);
        }
        void removeFavoriteSchedule(FavoriteScheduleViewModel favSchedView)
        {
            Favorite_Schedule_Table tempDelete = getFavSchedTabObj(favSchedView, "");
            DataAccess.removeFavoriteSchedule(tempDelete);

        }

        Favorite_Schedule_Table getFavSchedTabObj(FavoriteScheduleViewModel favSchedView, string value)
        {
            Favorite_Schedule_Table temp = new Favorite_Schedule_Table();
            temp.RecordId = favSchedView.RecordId;
            temp.startStationId = favSchedView.startStationId;
            temp.endStationId = favSchedView.endStationId;
            temp.startStationName = favSchedView.startStationName;
            temp.endStationName = favSchedView.endStationName;
            temp.startTime = favSchedView.startTime;
            temp.endTime = favSchedView.endTime;
            temp.favoriteName = value;

            return temp;

        }

        void setTimeFilter(Boolean isChecked) {

            try
            {
                cbTimeFilter.IsChecked = isChecked;
            }
            catch (Exception)
            {
        
            }
        }

        private void rtToTime_ValueChanged(object sender, ValueChangedEventArgs<object> args)
        {
            setTimeFilter(true);
        }

        private void rtFromTime_ValueChanged(object sender, ValueChangedEventArgs<object> args)
        {
            setTimeFilter(true);
        }

        
    }
}