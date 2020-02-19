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
using PrototypeBeta.ViewModels;
using PrototypeBeta.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Phone.Net.NetworkInformation;
using System.Globalization;
using System.Text;
using PrototypeBeta.SQLite;
using Telerik.Windows.Controls;
using System.Threading.Tasks;
using System.Windows.Input;


namespace PrototypeBeta
{
    public partial class ScheduleListPage : PhoneApplicationPage
    {
        public List<TrainListViewModel> combinedTrainList = new List<TrainListViewModel>();
        TRAINQUERY trianQuery;
        public ScheduleListPage()
        {
            InitializeComponent();

            createCombinedTrainList();
        }


        bool isInterntConAvailable()
        {
            if (DeviceNetworkInformation.IsNetworkAvailable)
                return true;
            return false;
        }

        void createCombinedTrainList()
        {
            trianQuery = (TRAINQUERY)NavigationService.GetNavigationData();
            populateHeader(trianQuery);
            if (isInterntConAvailable())
            {
                    WebClient webClient = new WebClient();
                    webClient.DownloadStringCompleted += webClient_DownloadCombinedTrainsCompleted;
                    webClient.DownloadStringAsync(new Uri(getServiceUri(trianQuery)));
                    this.busyIndicator.IsRunning = true;
            }
           else {
               RadMessageBox.ShowAsync("AW SNAP :-(", MessageBoxButtons.OK, "We are having trouble downloading data due to temporary server unavailability at the moment.");
                }
        }


        string getServiceUri(TRAINQUERY trianQuery)
        {
            string serviceUri = string.Format(WebService.ScheduleList, trianQuery.startStaion, trianQuery.endStaion, trianQuery.searchDate, trianQuery.searchTimeStart, trianQuery.searchTimeEnd);
            return serviceUri;
        }

        private void webClient_DownloadCombinedTrainsCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            
            try
            {
                string json = e.Result;


                if (!string.IsNullOrEmpty(json))
                {
                    RootTrainObject rootStationObj = JsonConvert.DeserializeObject<RootTrainObject>(json);
                    createResultSet(rootStationObj);

                    if (combinedTrainList != null)
                    {
                        populateTrains();
                    }
                }
            }
            catch (Exception ex)
            {
                RadMessageBox.ShowAsync("AW SNAP :-(", MessageBoxButtons.OK, "We are having trouble downloading data due to temporary server unavailability at the moment.");
            }
            this.busyIndicator.IsRunning = false;
        }

        void createResultSet(RootTrainObject rootStationObj)
        {
            TRAINRESULTS trainResults = rootStationObj.RESULTS;
            TRAINQUERY trainQuery = rootStationObj.QUERY;
            DirectTrains directTrains = trainResults.directTrains;
            ConnectingTrains connectingTrains = trainResults.connectingTrains;

            List<TrainsList> directTrainsList = directTrains.trainsList;
            List<TrainsList2> connectedTrainsList = connectingTrains.trainsList;
            foreach (TrainsList value in directTrainsList)
            {
                TrainListViewModel list = new TrainListViewModel();
                list.depatureTime = value.depatureTime;
                list.arrivalTime = value.arrivalTimeEndStation;
                list.trainType = AppUtility.ToCamelCase(value.trainType);
                list.trainTypeFlag = 1;
                list.scheduleDetail = getScheduleDetailObj(trainQuery, value);
                formatLitNode(list);

                combinedTrainList.Add(list);
            }
            foreach (TrainsList2 value in connectedTrainsList)
            {

                TrainListViewModel list = new TrainListViewModel();
                list.depatureTime = value.recordHeader[0].startArrivalTime;
                list.arrivalTime = value.recordHeader[0].endArrivalTime;
                list.trainType = APPCONTEXT.globalConnectedTrain;
                list.trainTypeFlag = 2;
                list.scheduleDetail = getScheduleDetailObj(trainQuery, value);
                formatLitNode(list);

                combinedTrainList.Add(list);
            }

        }
        ScheduleDetailViewModel getScheduleDetailObj(TRAINQUERY trainQuery, TrainsList2 trainList)
        {
            ScheduleDetailViewModel scheduleDetailObj = new ScheduleDetailViewModel();

            RecordHeader recordHeader = trainList.recordHeader[0];
            List<ConnectedTrain> connectedTrains = trainList.connectedTrains;

            DateTime dtArrivalTime = DateTime.ParseExact(recordHeader.startArrivalTime, "HH:mm:ss", CultureInfo.InvariantCulture);
            DateTime dtDepartureTime = DateTime.ParseExact(recordHeader.startDepartureTime, "HH:mm:ss", CultureInfo.InvariantCulture);
            DateTime dtArrivalTimeEndStation = DateTime.ParseExact(recordHeader.endArrivalTime, "HH:mm:ss", CultureInfo.InvariantCulture);
            TimeSpan ts = dtArrivalTimeEndStation.Subtract(dtDepartureTime).Duration();
            if (dtDepartureTime > dtArrivalTimeEndStation)
            {
                dtArrivalTimeEndStation = dtArrivalTimeEndStation.AddDays(1.0);
            }
            ConnetedTrainViewModel connectedTrain = new ConnetedTrainViewModel();


            string arrivalTime = dtArrivalTime.ToString("h:mm", CultureInfo.CurrentCulture);
            string arrivalTimeEndStation = dtArrivalTimeEndStation.ToString("h:mm", CultureInfo.CurrentCulture);
            string departureTime = dtArrivalTime.ToString("h:mm", CultureInfo.CurrentCulture);
            string departureTimeZone = dtDepartureTime.ToString("tt", CultureInfo.CurrentCulture);
            string arrivalTimeZone = dtDepartureTime.ToString("tt", CultureInfo.CurrentCulture);
            string arrivalTimeEndStationZone = dtArrivalTimeEndStation.ToString("tt", CultureInfo.CurrentCulture);
            connectedTrain.connectedTrains = connectedTrains;

            scheduleDetailObj.startStationId = trainQuery.startStaion;
            scheduleDetailObj.endStationId = trainQuery.endStaion;
            scheduleDetailObj.detailHeader = string.Format(APPCONTEXT.sheduleListHeader, recordHeader.startName, recordHeader.endName).ToLower();
            scheduleDetailObj.departureTime = string.Format(APPCONTEXT.sceduleDetailTimeHeader, dtDepartureTime.DayOfWeek.ToString(),departureTime, departureTimeZone);
            scheduleDetailObj.arrivalTime = string.Format(APPCONTEXT.sceduleDetailTime, arrivalTime, arrivalTimeZone);
            scheduleDetailObj.arrivalTimeEndStation = string.Format(APPCONTEXT.sceduleDetailTime, arrivalTimeEndStation, arrivalTimeZone);
            scheduleDetailObj.travelTime = String.Format(APPCONTEXT.sheduleListDuration, ts.ToString(@"hh\:mm"));
            scheduleDetailObj.startStationName = AppUtility.ToCamelCase(recordHeader.startName);
            scheduleDetailObj.endStationName = AppUtility.ToCamelCase(recordHeader.endName);
            scheduleDetailObj.trainType = APPCONTEXT.globalConnectedTrain;
            scheduleDetailObj.connectedTrain = connectedTrain;

            return scheduleDetailObj;

        }
        ScheduleDetailViewModel getScheduleDetailObj(TRAINQUERY trainQuery, TrainsList trainList)
        {

            ScheduleDetailViewModel scheduleDetailObj = new ScheduleDetailViewModel();

            DateTime searchDate = DateTime.ParseExact(trainQuery.searchDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime dtArrivalTime = DateTime.ParseExact(trainList.arrivalTime, "HH:mm:ss", CultureInfo.InvariantCulture);
            DateTime dtDepartureTime = DateTime.ParseExact(trainList.depatureTime, "HH:mm:ss", CultureInfo.InvariantCulture);
            DateTime dtArrivalTimeEndStation = DateTime.ParseExact(trainList.arrivalTimeEndStation, "HH:mm:ss", CultureInfo.InvariantCulture);
            if(dtDepartureTime>dtArrivalTimeEndStation){
                dtArrivalTimeEndStation=dtArrivalTimeEndStation.AddDays(1.0);
            }
            TimeSpan ts = dtArrivalTimeEndStation.Subtract(dtDepartureTime).Duration();

            string arrivalTime = dtArrivalTime.ToString("h:mm", CultureInfo.CurrentCulture);
            string arrivalTimeEndStation = dtArrivalTimeEndStation.ToString("h:mm", CultureInfo.CurrentCulture);
            string departureTime = dtDepartureTime.ToString("h:mm", CultureInfo.CurrentCulture);
            string departureTimeZone = dtDepartureTime.ToString("tt", CultureInfo.CurrentCulture);
            string arrivalTimeZone = dtArrivalTime.ToString("tt", CultureInfo.CurrentCulture);
            string arrivalTimeEndStationZone = dtArrivalTimeEndStation.ToString("tt", CultureInfo.CurrentCulture);
            StringBuilder availableClasses = new StringBuilder();

            foreach (ClassList value in trainList.classList)
            {
                if (availableClasses.ToString() == "")
                {
                    availableClasses.Append(value.className);
                }
                else
                {
                    availableClasses.Append(", " + value.className);
                }
            }

            scheduleDetailObj.startStationId = trainQuery.startStaion;
            scheduleDetailObj.endStationId = trainQuery.endStaion;
            scheduleDetailObj.detailHeader = string.Format(APPCONTEXT.sheduleListHeader, trainList.startStationName, trainList.endStationName).ToLower(); ;
            scheduleDetailObj.departureTime = string.Format(APPCONTEXT.sceduleDetailTimeHeader, searchDate.DayOfWeek.ToString(), departureTime, departureTimeZone);
            scheduleDetailObj.arrivalTime = string.Format(APPCONTEXT.sceduleDetailTime, arrivalTime, arrivalTimeZone);
            scheduleDetailObj.arrivalTimeEndStation = string.Format(APPCONTEXT.sceduleDetailTime, arrivalTimeEndStation, arrivalTimeZone);
            scheduleDetailObj.travelTime = String.Format(APPCONTEXT.sheduleListDuration, ts.ToString(@"hh\:mm"));
            scheduleDetailObj.trainFrequncy = AppUtility.ToCamelCase(trainList.trainFrequncy);
            scheduleDetailObj.startStationName = AppUtility.ToCamelCase(trainList.startStationName);
            scheduleDetailObj.endStationName = AppUtility.ToCamelCase(trainList.endStationName);
            scheduleDetailObj.finalStationName = AppUtility.ToCamelCase(trainList.finalStationName);
            scheduleDetailObj.trainType = AppUtility.ToCamelCase(trainList.trainType);
            scheduleDetailObj.trainName = AppUtility.ToCamelCase(trainList.trainName.Equals("") ? "Not Available" : trainList.trainName);
            scheduleDetailObj.classList = availableClasses.ToString();

            return scheduleDetailObj;
        }

        void formatLitNode(TrainListViewModel list)
        {
            DateTime departureTime = DateTime.ParseExact(list.depatureTime, "HH:mm:ss", CultureInfo.InvariantCulture);
            DateTime arrivalTime = DateTime.ParseExact(list.arrivalTime, "HH:mm:ss", CultureInfo.InvariantCulture);
            if (departureTime > arrivalTime)
            {
                arrivalTime = arrivalTime.AddDays(1.0);
            }
            TimeSpan ts = arrivalTime.Subtract(departureTime).Duration();

            list.depatureTime = departureTime.ToString("hh:mm", CultureInfo.CurrentCulture);
            list.arrivalTime = arrivalTime.ToString("hh:mm", CultureInfo.CurrentCulture);
            list.depZone = departureTime.ToString("tt", CultureInfo.CurrentCulture);
            list.arriveZone = arrivalTime.ToString("tt", CultureInfo.CurrentCulture);
            list.duration = String.Format(APPCONTEXT.sheduleListDuration, ts.ToString(@"hh\:mm"));
            list.depTime = departureTime;

        }
        void populateTrains()
        {
            var sortedList = from element in combinedTrainList
                             orderby element.depTime
                             select element;
            lsSearchResult.ItemsSource = sortedList.ToList();
            if (lsSearchResult.ItemsSource.Count > 0)
            {
                pnlEmptySchdlList.Visibility = Visibility.Collapsed;
            }
            else {
                pnlEmptySchdlList.Visibility = Visibility.Visible;
            }
        }
        void populateHeader(TRAINQUERY trainQuery)
        {

            DateTime dtFrom = DateTime.ParseExact(trainQuery.searchTimeStart, "HH:mm:ss", CultureInfo.InvariantCulture);
            DateTime dtTo = DateTime.ParseExact(trainQuery.searchTimeEnd, "HH:mm:ss", CultureInfo.InvariantCulture);
            DateTime searchDate = DateTime.ParseExact(trainQuery.searchDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            string fromStation = AppUtility.ToLowerCase(trainQuery.startStaionName);
            string toStation = AppUtility.ToLowerCase(trainQuery.endStaionName);
            string date = searchDate.DayOfWeek.ToString();
            string fromTime = dtFrom.ToString("h:mm", CultureInfo.CurrentCulture);
            string toTime = dtTo.ToString("h:mm", CultureInfo.CurrentCulture);
            string fromZone = dtFrom.ToString("tt", CultureInfo.CurrentCulture);
            string toZone = dtTo.ToString("tt", CultureInfo.CurrentCulture);

            txtStations.Text = String.Format(APPCONTEXT.sheduleListHeader, fromStation, toStation);
            txtDate.Text = date;
            txtTimePeriod.Text = String.Format(APPCONTEXT.sheduleListTimePeriod, fromTime, fromZone, toTime, toZone);
        }

        private void lsSearchResult_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

            TrainListViewModel trainList = (TrainListViewModel)(lsSearchResult.SelectedItem);
            if (trainList != null)
            {
                ScheduleDetailViewModel scheduleDetailObj = trainList.scheduleDetail;
                lsSearchResult.SelectedItem = null;
                if (trainList.trainTypeFlag == 1)
                {
                    NavigationService.Navigate(new Uri("/ScheduleDetailPage.xaml", UriKind.Relative), scheduleDetailObj);
                }
                else
                {
                    NavigationService.Navigate(new Uri("/ConnectedScheduleDetailPage.xaml", UriKind.Relative), scheduleDetailObj);
                }
            }

        }
        void logFavoriteSchedule(string scheduleName)
        {

            Favorite_Schedule_Table favoriteScheduleRecord = new Favorite_Schedule_Table();
            DateTime searchTimeStart = DateTime.ParseExact(trianQuery.searchTimeStart, "HH:mm:ss", CultureInfo.InvariantCulture);
            DateTime searchTimeEnd = DateTime.ParseExact(trianQuery.searchTimeEnd, "HH:mm:ss", CultureInfo.InvariantCulture);

            favoriteScheduleRecord.favoriteName = scheduleName;
            favoriteScheduleRecord.startStationId = trianQuery.startStaion;
            favoriteScheduleRecord.endStationId = trianQuery.endStaion;
            favoriteScheduleRecord.startStationName = trianQuery.startStaionName;
            favoriteScheduleRecord.endStationName = trianQuery.endStaionName;
            favoriteScheduleRecord.startTime = searchTimeStart;
            favoriteScheduleRecord.endTime = searchTimeEnd;

            DataAccess.adFavoriteSchedule(favoriteScheduleRecord);
        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {

            ControlTemplate imageTemplate = (ControlTemplate)this.Resources["ImageTemplate"];
            //RadInputPrompt.Show(imageTemplate, "Title", new string[] { "yes", "no" });

            RadInputPrompt.Show(imageTemplate, APPCONTEXT.scheduleListPromptTitle, new string[] { "save", "cancel" }, APPCONTEXT.scheduleListPromptMessage, closedHandler: (args) =>
            {

                string value = args.Text;
                int buttonIndex = args.ButtonIndex;
                if (buttonIndex == 0)
                {
                    logFavoriteSchedule(value);
                    RadMessageBox.ShowAsync("Yeey!", MessageBoxButtons.OK, "Schedule is added to the favorites.");
                }

            });

        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            TrainListViewModel trainViewModel = (sender as MenuItem).DataContext as TrainListViewModel;

            string[] args = { trainViewModel.scheduleDetail.startStationName, trainViewModel.scheduleDetail.endStationName, trainViewModel.scheduleDetail.departureTime, 
                                trainViewModel.scheduleDetail.arrivalTimeEndStation, trainViewModel.scheduleDetail.travelTime, 
                                trainViewModel.scheduleDetail.trainFrequncy};
            string shareMessage = string.Format(APPCONTEXT.scheduleDetailShareMessage, args);
            AppUtility.shareStatusTask(shareMessage);
        }

        private void GoHome_Click(object sender, EventArgs e)
        {
            NavigationService.GoBack();
        }



    }
}