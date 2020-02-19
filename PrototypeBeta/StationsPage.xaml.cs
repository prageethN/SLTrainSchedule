using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using PrototypeBeta.DataModels;
using PrototypeBeta.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Phone.Net.NetworkInformation;
using System.Xml.Linq;
using Telerik.Windows.Controls;
using Microsoft.Phone.Tasks;
using Nokia.Phone.HereLaunchers;

namespace PrototypeBeta
{
    public partial class StationsPage : PhoneApplicationPage
    {
        public List<StationList> stationList;
        public List<DetailedStationListExt2> detailedStationList;

        public StationsPage()
        {
            InitializeComponent();

            createStationList();

        }

        bool isInterntConAvailable(){
            if (DeviceNetworkInformation.IsNetworkAvailable)
                return true;
            return false;
        }
        void createStationList()
        {
            populateOfflineStationList();
            /*if (isInterntConAvailable())
            {
                WebClient webClient = new WebClient();
                webClient.DownloadStringCompleted += webClient_DownloadTrainRoutesCompleted;
                webClient.DownloadStringAsync(new Uri(WebService.RailwayStations));
            }
            else {
                populateOfflineStationList();
                //RadMessageBox.ShowAsync("Error!", MessageBoxButtons.OK, "Network issue. Please check your internet connection.");
            }*/
        }
        void populateOfflineStationList()
        {
            XDocument loadedData = XDocument.Load("StaticData/StationsStaticDataV2.xml");

            var data = from query in loadedData.Descendants("Station")
                       select new DetailedStationListExt2()
                       {
                           stationCode = (string)query.Element("StationCode"),
                           stationID = (int)query.Element("StationID"),
                           stationName = (string)query.Element("StationName"),
                           lineOne = (int)query.Element("LineOne"),
                           lineTwo = query.Element("LineTwo").Value.Equals("") ? 0 :(int)query.Element("LineTwo"),
                           lineThree = query.Element("LineThree").Value.Equals("") ? 0 : (int)query.Element("LineThree"),
                           stationOrder = (int)query.Element("StationOrder"),
                           contactNo = query.Element("ContactNumber").Value.Equals("") ? APPCONTEXT.stationListNumberNotAvailableMsg : (string)query.Element("ContactNumber"),
                           stationGeoCoordinate = new GeoCoordinateInfo(getFormatedLocationName((string)query.Element("StationName")), 6.85087218, 79.86202922)
                        };
            detailedStationList = data.ToList();
            formatStationName();
            populateStations();
        }
        private void webClient_DownloadTrainRoutesCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try { 
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
            catch (Exception ex) {
                RadMessageBox.ShowAsync("Error!", MessageBoxButtons.OK, "Error retrieving information from server. Please check your internet connection and try again.");
            }
        }
        void formatStationName()
        {
            
            for (int i = 0; i < detailedStationList.Count; i++)
            {
                detailedStationList[i].stationName = AppUtility.ToCamelCase(detailedStationList[i].stationName);
            }

        }
        string getFormatedLocationName(string stationName)
        {

            return AppUtility.ToCamelCase(stationName) + " Railway Station";

        }
        void populateStations()
        {
            List<AlphaKeyGroup<DetailedStationList>> DataSource = AlphaKeyGroup<DetailedStationList>.CreateGroups(detailedStationList,
                    System.Threading.Thread.CurrentThread.CurrentUICulture,
                    (DetailedStationList s) => { return s.stationName; }, true);

            lsMainLines.ItemsSource = DataSource;

        }
        void callHereLauncher(GeoCoordinateInfo stationGeoCoordinate, int callType) // 1 - route direction, 2 - driving guidence, 3 - walking guidence
        {
            try
            {
                switch (callType)
                {
                    case 1:
                        DirectionsRouteDestinationTask routeTo = new DirectionsRouteDestinationTask();
                        routeTo.Destination = stationGeoCoordinate;
                        routeTo.Mode = RouteMode.Car;
                        routeTo.Show();
                        break;
                    case 2:
                        GuidanceDriveTask driveTo = new GuidanceDriveTask();
                        driveTo.Destination = stationGeoCoordinate;
                        driveTo.Title = stationGeoCoordinate.locationName;
                        driveTo.Show();

                        break;
                    case 3:
                        GuidanceWalkTask walkTo = new GuidanceWalkTask();
                        walkTo.Destination = stationGeoCoordinate;
                        walkTo.Title = stationGeoCoordinate.locationName;
                        walkTo.Show();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception erno)
            {
                MessageBox.Show("Error message: " + erno.Message);
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            DetailedStationListExt2 stationObj = (sender as MenuItem).DataContext as DetailedStationListExt2;
            if (menuItem.Name == "menuCall1")
            {
                PhoneCallTask phoneCallTask = new PhoneCallTask();

                phoneCallTask.PhoneNumber = stationObj.contactNo;
                phoneCallTask.DisplayName = string.Format(APPCONTEXT.stationListStationName, stationObj.stationName);

                phoneCallTask.Show();
            }
            else if (menuItem.Name == "menuCall2")
            {
                SaveContactTask saveContactTask = new SaveContactTask();

                saveContactTask.FirstName = stationObj.stationName + " Railway Station";
                saveContactTask.WorkPhone = stationObj.contactNo;

                saveContactTask.Show();
            }
            else if (menuItem.Name == "menuCall3")
            {
                callHereLauncher(stationObj.stationGeoCoordinate, 1);
            }
            else if (menuItem.Name == "menuCall4")
            {
                callHereLauncher(stationObj.stationGeoCoordinate, 2);
            }
            else
            {

                callHereLauncher(stationObj.stationGeoCoordinate, 3);
            }
        }


        private void GoHome_Click(object sender, EventArgs e)
        {
            NavigationService.GoBack();
        }

        private void MenuItem_Loaded(object sender, RoutedEventArgs e)
        {
            DetailedStationList stationObj = (sender as MenuItem).DataContext as DetailedStationList;
            if (stationObj.contactNo.Equals(APPCONTEXT.stationListNumberNotAvailableMsg))
            {
                (sender as MenuItem).IsEnabled = false;
            }
        }

        private void Border_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

            Border border = sender as Border;
            ContextMenu contextMenu = ContextMenuService.GetContextMenu(border);
            if (contextMenu.Parent == null)
            {
                contextMenu.IsOpen = true;
            }
        }
    }
}