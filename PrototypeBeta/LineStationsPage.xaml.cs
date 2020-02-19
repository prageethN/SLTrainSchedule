using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using PrototypeBeta.Util;
using PrototypeBeta.DataModels;
using Microsoft.Phone.Tasks;
using System.Xml.Linq;
using System.Device.Location;
using Windows.Devices.Geolocation;
using Nokia.Phone.HereLaunchers;

namespace PrototypeBeta
{
    public partial class LineStationsPage : PhoneApplicationPage
    {
        Line selectedLine;
        public List<DetailedStationListExt2> detailedStationList;
        public int decendingFlag=0;

        CustomMessageBox messageBox;

        public LineStationsPage()
        {
            InitializeComponent();
            setPageData();
        }

        void setPageData() {
            selectedLine = (Line)NavigationService.GetNavigationData();
            selectedLine.LineName = selectedLine.LineName.ToLower();
            this.DataContext = selectedLine;

            populateStationList(selectedLine.LineID);
        }        

        void populateStationList(int lineNo)
        {
            XDocument loadedData = XDocument.Load("StaticData/StationsStaticDataV2.xml");
            List<DetailedStationListExt2> tempStationList;

            var data = from query in loadedData.Descendants("Station")
                       select new DetailedStationListExt2()
                       {
                           stationCode = (string)query.Element("StationCode"),
                           stationID = (int)query.Element("StationID"),
                           stationName = (string)query.Element("StationName"),
                           lineOne = (int)query.Element("LineOne"),
                           lineTwo = query.Element("LineTwo").Value.Equals("") ? 0 : (int)query.Element("LineTwo"),
                           lineThree = query.Element("LineThree").Value.Equals("") ? 0 : (int)query.Element("LineThree"),
                           stationOrder = (int)query.Element("StationOrder"),
                           contactNo = query.Element("ContactNumber").Value.Equals("") ? APPCONTEXT.stationListNumberNotAvailableMsg : (string)query.Element("ContactNumber"),
                           stationGeoCoordinate = new GeoCoordinateInfo(getFormatedLocationName((string)query.Element("StationName")), 6.85087218, 79.86202922)
                       };
            tempStationList = data.ToList();

            var filteredData = from station in tempStationList
                                  where station.lineOne==lineNo || station.lineTwo==lineNo || station.lineThree==lineNo
                                  orderby station.stationOrder
                                  select station;

             detailedStationList = filteredData.ToList();

            formatStationName();
            populateStations();     
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

            lsMainLines.ItemsSource = detailedStationList;

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

        private void GoHome_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
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
            else {

                callHereLauncher(stationObj.stationGeoCoordinate, 3);
            }
        }

        private void MenuItem_Loaded(object sender, RoutedEventArgs e)
        {
            DetailedStationList stationObj = (sender as MenuItem).DataContext as DetailedStationList;
            if (stationObj.contactNo.Equals(APPCONTEXT.stationListNumberNotAvailableMsg))
            {
                (sender as MenuItem).IsEnabled = false;
            }
        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            if (decendingFlag == 0)
            {
                var tempList = detailedStationList.OrderByDescending(x => x.stationOrder);
                detailedStationList = tempList.ToList();
                decendingFlag = 1;
            }
            else {
                var tempList = detailedStationList.OrderBy(x => x.stationOrder);
                detailedStationList = tempList.ToList();
                decendingFlag = 0;            
            }
            formatStationName();
            populateStations();     
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