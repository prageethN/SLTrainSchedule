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
using Microsoft.Phone.Tasks;
using System.Xml.Linq;
using Windows.Devices.Geolocation;
using System.IO.IsolatedStorage;
using System.Device.Location;
using System.Collections.ObjectModel;
using Nokia.Phone.HereLaunchers;

using Microsoft.Phone.Net.NetworkInformation;
using Newtonsoft.Json;
using System.Windows.Media;


namespace PrototypeBeta
{
    public partial class NearByStationsPage : PhoneApplicationPage
    {
        public ObservableCollection<DetailedStationListExt> detailedStationList = null;
        public int decendingFlag = 0;
        public double searchRadiousTemp = 10;
        public double searchRadious = 10;
        Slider sdRadiuos;

        Geoposition currentGeoposition = null;
        GeoCoordinate currentGeoCoordinate;
        Double currentLatitude;
        Double currentLongitude;

        int tempIndex = 0;

        public NearByStationsPage()
        {
            InitializeComponent();
            setPageData();
        }

        void setPageData()
        {
            tempIndex = 0;
            getGeoLocation();
            //populateStationList(20);
        }

        async void getGeoLocation()
        {
            this.busyIndicator.IsRunning = true;

            //Declare and Inizialize a Geolocator object
            Geolocator geolocator = new Geolocator();
            //Set his accuracy level
            geolocator.DesiredAccuracy = PositionAccuracy.High;

            //Let's get the position of the user. Since there is the possibility of getting an Exception, this method is called inside a try block
            try
            {
                //The await guarantee the calls  to be returned on the thread from which they were called
                //Since it is call directly from the UI thread, the code is able to access and modify the UI directly when the call returns.
                currentGeoposition = await geolocator.GetGeopositionAsync(
                maximumAge: TimeSpan.FromMinutes(5),
                timeout: TimeSpan.FromSeconds(10)
                );
                currentLatitude = currentGeoposition.Coordinate.Latitude;
                currentLongitude = currentGeoposition.Coordinate.Longitude;
                currentGeoCoordinate = new GeoCoordinate(currentLatitude, currentLongitude);

                //this.busyIndicator.IsRunning = false;
                populateStationList(20);

            }
            //If an error is catch 2 are the main causes: the first is that you forgot to includ ID_CAP_LOCATION in your app manifest. 
            //The second is that the user doesn't turned on the Location Services
            catch (Exception ex)
            {

                if ((uint)ex.HResult == 0x80004004)
                {
                    showErrorMessage();
                    this.busyIndicator.IsRunning = false;
                }
                //else
                {
                    // something else happened during the acquisition of the location
                }
            }
           

        }

        void showErrorMessage()
        {
            messageBox = new CustomMessageBox()
            {
                //set the custom template property
                ContentTemplate = (DataTemplate)this.Resources["NoLocationServiceTemplate"],
                LeftButtonContent = "settings",
                RightButtonContent = "cancel",
                IsFullScreen = false // Pivots should always be full-screen.
            };

            //Define the dismissed event handler
            messageBox.Dismissed += (s1, e1) =>
           {
               switch (e1.Result)
               {
                   case CustomMessageBoxResult.LeftButton:
                       // Add the following task here which you wish to perform on speak button
                       launchLocSettings();
                       break;
                   case CustomMessageBoxResult.RightButton:
                       // Do something. 

                       break;
                   case CustomMessageBoxResult.None:
                       // Do something. 

                       break;
                   default:
                       break;
               }
           };
            //launch the task
            messageBox.Show();
        }

        async void launchLocSettings()
        {
            var op = await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings-location:"));
            if(op){
                setPageData();
            }
        }
        void populateStationList(int lineNo)
        {
            XDocument loadedData = XDocument.Load("StaticData/StationsStaticDataV2.xml");
            ObservableCollection<DetailedStationListExt> tempStationList;

            var data = from query in loadedData.Descendants("Station")
                       select new DetailedStationListExt()
                       {
                           stationName = (string)query.Element("StationName"),
                           expanderTag = "Show route options",
                           stationGeoCoordinate = new GeoCoordinateInfo(getFormatedLocationName((string)query.Element("StationName")), 6.85087218, 79.86202922),
                           fromDistance = getDistanceFromCurrentLocation(new GeoCoordinate(6.85087218, 79.86202922)),
                           fromDistanceDisplay = "Not available",
                           lineOne = (int)query.Element("LineOne"),
                       };
            tempStationList = new ObservableCollection<DetailedStationListExt>(data);


            var filteredData = from station in tempStationList
                               where ((station.lineOne == 20) && (station.fromDistance <= searchRadious))
                               orderby station.fromDistance, station.stationName
                               select station;

            detailedStationList = new ObservableCollection<DetailedStationListExt>(filteredData);

            formatStationName();
            if (isInterntConAvailable())
            {
                for (int i = 0; i < detailedStationList.Count; i++)
                {
                    setRouteInformation(detailedStationList[i].stationGeoCoordinate);                   
                }
            }
            else {
                Telerik.Windows.Controls.RadMessageBox.ShowAsync("AW SNAP :-(", Telerik.Windows.Controls.MessageBoxButtons.OK, 
                    "We are having trouble downloading data due to network connectivity or temporary server unavailability at the moment. Please make sure you are connected to internet. But this won't affect to your routing optinos as offline maps features would still available.");
                this.busyIndicator.IsRunning = false;
                populateStations();
            }


        }

        string getDistanceFromDisplay(double fromDistance ) {

            return String.Format("{0} KMs",fromDistance);
                    
        }

        double getDistanceFromCurrentLocation(GeoCoordinate stationGeoCoordinate)
        {
            Double distance = 0;

            distance = Math.Round(currentGeoCoordinate.GetDistanceTo(stationGeoCoordinate) / 1000, 2);

            return distance;
        }

        string getFormatedLocationName(string stationName)
        {

            return AppUtility.ToCamelCase(stationName) + " Railway Station";

        }
        void formatStationName()
        {

            for (int i = 0; i < detailedStationList.Count; i++)
            {
                detailedStationList[i].stationName = AppUtility.ToCamelCase(detailedStationList[i].stationName);
            }

        }
        void populateStations()
        {

            lsNearBy.ItemsSource = detailedStationList;

            if (detailedStationList.Count < 0)
            {
                pnlEmptyStationList.Visibility = Visibility.Visible;
                txtStationCountSummary.Text = "";
            }
            else
            {
                pnlEmptyStationList.Visibility = Visibility.Collapsed;
                txtStationCountSummary.Text = String.Format(APPCONTEXT.nearByStationCountSummary, detailedStationList.Count, searchRadious);
            }

        }

        void reloadPageData()
        {

            searchRadious = searchRadiousTemp;
            detailedStationList.Clear();
            lsNearBy.ItemsSource = detailedStationList;
            txtStationCountSummary.Text = "";

            setPageData();

            if(detailedStationList.Count==0){
                pnlEmptyStationList.Visibility = Visibility.Visible;
            }
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
                if (callType == 1 || callType == 3)
                {

                }
                else { 
                
                }
                MessageBox.Show("Error!");
            }
        }

        bool isInterntConAvailable()
        {
            if (DeviceNetworkInformation.IsNetworkAvailable)
                return true;
            return false;
        }

        string getServiceUri(GeoCoordinate geoCoordinate)
        {
            string serviceUri = string.Format(WebService.RouteInformation, currentLatitude,currentLongitude,geoCoordinate.Latitude,geoCoordinate.Longitude);
            return serviceUri;
        }


        void setRouteInformation(GeoCoordinate geoCoordinate)
        {
            try
            {
                WebClient webClient = new WebClient();
                webClient.DownloadStringCompleted += webClient_DownloadCombinedTrainsCompleted;
                webClient.DownloadStringAsync(new Uri(getServiceUri(geoCoordinate)));
            }
            catch (Exception ex)
            {
                //Telerik.Windows.Controls.RadMessageBox.ShowAsync("AW SNAP :-(", Telerik.Windows.Controls.MessageBoxButtons.OK, "We are having trouble downloading data due to network connectivity or temporary server unavailability at the moment. Please make sure you are connected to internet and try again.");
                ShellToast toast = new ShellToast();
                toast.Title = "AW SNAP :(";
                toast.Content = "We could not retreive some routing infomation. But this won't affect to your routing options as offline map features would still available";
                toast.Show();
            }
           
        }

        private void webClient_DownloadCombinedTrainsCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            
            try
            {
                string json = e.Result;


                if (!string.IsNullOrEmpty(json))
                {
                    RouteInformation routeInformation = JsonConvert.DeserializeObject<RouteInformation>(json);
                    detailedStationList[tempIndex].fromDistance = Math.Round(routeInformation.resourceSets[0].resources[0].travelDistance,1);
                    detailedStationList[tempIndex].fromDistanceDisplay = getDistanceFromDisplay(detailedStationList[tempIndex].fromDistance);
                }
            }
            catch (Exception ex)
            {
                //Telerik.Windows.Controls.RadMessageBox.ShowAsync("AW SNAP :-(", Telerik.Windows.Controls.MessageBoxButtons.OK, "We are having trouble downloading data due to network connectivity or temporary server unavailability at the moment. Please make sure you are connected to internet and try again.");
                detailedStationList[tempIndex].fromDistance = 1000;
                ShellToast toast = new ShellToast();
                toast.Title = "AW SNAP :(";
                toast.Content = "We could not retreive some routing infomation. But this won't affect to your routing options as offline maps features would still available";
                toast.Show();
            }
            tempIndex++;
            if (detailedStationList.Count == tempIndex)
            {
                this.busyIndicator.IsRunning = false;
                populateStations();
            }

        }

        private void GoHome_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            DetailedStationListExt stationObj = (sender as MenuItem).DataContext as DetailedStationListExt;
            PhoneCallTask phoneCallTask = new PhoneCallTask();

            phoneCallTask.DisplayName = string.Format(APPCONTEXT.stationListStationName, stationObj.stationName);

            phoneCallTask.Show();
        }

        private void MenuItem_Loaded(object sender, RoutedEventArgs e)
        {
            DetailedStationListExt stationObj = (sender as MenuItem).DataContext as DetailedStationListExt;
            {
                (sender as MenuItem).IsEnabled = false;
            }
        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            popupMessageBox();
        }
        private void GestureListener_Tap(object sender, GestureEventArgs e)
        {
            Border border = sender as Border;
            ContextMenu contextMenu = ContextMenuService.GetContextMenu(border);
            if (contextMenu.Parent == null)
            {
                contextMenu.IsOpen = true;
            }
        }

        private void Header2_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ExpanderView exView = sender as ExpanderView;

            if (exView.IsExpanded)
            {
                exView.Expander = "Hide route options                                  ";
            }
            else
            {
                exView.Expander = "Show route options                                  ";
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            GeoCoordinateInfo stationGeoCoordinate;
            Button btnItem = sender as Button;
            DetailedStationListExt stationObj = (sender as Button).DataContext as DetailedStationListExt;
            stationGeoCoordinate = stationObj.stationGeoCoordinate;

            if (!stationObj.isExpanded)
                return;
            if (btnItem.Name == "btnRoute")
            {
                callHereLauncher(stationGeoCoordinate, 1);
            }
            else if (btnItem.Name == "btnDrive")
            {
                callHereLauncher(stationGeoCoordinate, 2);
            }
            else
            {
                callHereLauncher(stationGeoCoordinate, 3);
            }


        }

        void test() {

            var ListBox = lsNearBy;

           
        
        }
     
        CustomMessageBox messageBox;

        void popupMessageBox()
        {

            messageBox = new CustomMessageBox()
           {
               //set the custom template property
               ContentTemplate = (DataTemplate)this.Resources["ChangeRadiusTemplate"],
               LeftButtonContent = "Search",
               RightButtonContent = "Cancel",
               IsFullScreen = false // Pivots should always be full-screen.
           };

            //Define the dismissed event handler
            messageBox.Dismissed += (s1, e1) =>
            {
                switch (e1.Result)
                {
                    case CustomMessageBoxResult.LeftButton:
                        // Add the following task here which you wish to perform on speak button
                        reloadPageData();
                        break;
                    case CustomMessageBoxResult.RightButton:
                        // Do something. 
                        searchRadiousTemp = searchRadious;
                        break;
                    case CustomMessageBoxResult.None:
                        // Do something. 
                        searchRadiousTemp = searchRadious;
                        break;
                    default:
                        break;
                }
            };
            //launch the task
            messageBox.Show();

        }

        private void sdRadious_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int[] array = new int[6] { 5, 10, 15, 20, 25, 30 };
            sdRadiuos = sender as Slider;

            if ((sdRadiuos.Minimum == 0 && sdRadiuos.Maximum == 1) || (sdRadiuos.Minimum == 5 && sdRadiuos.Maximum == 5))
            {
                //Do nothing
            }
            else
            {
                double roundedValue = Math.Round(sdRadiuos.Value, 0);
                roundedValue = array.OrderBy(x => Math.Abs((long)x - roundedValue)).First();
                //Check if it is a rounded value
                if (sdRadiuos.Value != roundedValue)
                {
                    searchRadiousTemp = roundedValue;
                }
                sdRadiuos.Value = searchRadiousTemp;
            }


        }

        private void ApplicationBarIconButton_Click_1(object sender, EventArgs e)
        {
            reloadPageData();
        }

        private void ExpanderView_Collapsed(object sender, RoutedEventArgs e)
        {
            DetailedStationListExt stationObj = (sender as ExpanderView).DataContext as DetailedStationListExt;
            stationObj.isExpanded = false;
          
        }

        private void ExpanderView_Expanded(object sender, RoutedEventArgs e)
        {

           DetailedStationListExt stationObj = (sender as ExpanderView).DataContext as DetailedStationListExt;
           stationObj.isExpanded = true;
        }

 
    }
}