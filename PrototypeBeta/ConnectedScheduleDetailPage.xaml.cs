using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using PrototypeBeta.ViewModels;
using PrototypeBeta.Util;
using PrototypeBeta.DataModels;
using Newtonsoft.Json;
using System.Text;
using Microsoft.Phone.Net.NetworkInformation;
using System.Globalization;

namespace PrototypeBeta
{
    public partial class ConnectedScheduleDetailPage : PhoneApplicationPage
    {
        public List<PriceList> priceList;
        ScheduleDetailViewModel scheduleDetailObj;

        public ConnectedScheduleDetailPage()
        {
            InitializeComponent();
            populateShedule();
        }
        void populateShedule()
        {
            scheduleDetailObj = (ScheduleDetailViewModel)NavigationService.GetNavigationData();
            this.DataContext = scheduleDetailObj;
            populateConnectedTrain(scheduleDetailObj.connectedTrain);
            populateTicketPrice(scheduleDetailObj.startStationId.ToString(), scheduleDetailObj.endStationId.ToString());
        }
         void populateTicketPrice(string startStationId, string endStationId)
        {

            getTicketPrices(startStationId, endStationId);
        }
        void populateConnectedTrain(ConnetedTrainViewModel connectedTrain ){
        
            List<ConnectedTrain> connectedTrains=connectedTrain.connectedTrains;

            foreach (ConnectedTrain item in connectedTrains) {

                formatConnectedTrainItem(item);
            }
            
            lsConnectedTrainResult.ItemsSource = connectedTrains;
        }

        void formatConnectedTrainItem(ConnectedTrain connectedTrain){

            DateTime dtDepartureTime = DateTime.ParseExact(connectedTrain.startTime, "HH:mm:ss", CultureInfo.InvariantCulture);
            DateTime dtArrivalTime = DateTime.ParseExact(connectedTrain.endTime, "HH:mm:ss", CultureInfo.InvariantCulture);            

            string arrivalTime = dtArrivalTime.ToString("h:mm", CultureInfo.CurrentCulture);
            string arrivalTimeZone = dtArrivalTime.ToString("tt", CultureInfo.CurrentCulture);
            string departureTime = dtDepartureTime.ToString("h:mm", CultureInfo.CurrentCulture);
            string departureTimeZone = dtDepartureTime.ToString("tt", CultureInfo.CurrentCulture);
            
            connectedTrain.startStation = AppUtility.ToCamelCase(connectedTrain.startStation);
            connectedTrain.endStation = AppUtility.ToCamelCase(connectedTrain.endStation);
            connectedTrain.startTime = string.Format(APPCONTEXT.sceduleDetailTime, departureTime, departureTimeZone);
            connectedTrain.endTime = string.Format(APPCONTEXT.sceduleDetailTime, arrivalTime, arrivalTimeZone);

        
        }

        bool isInterntConAvailable()
        {
            if (DeviceNetworkInformation.IsNetworkAvailable)
                return true;
            return false;
        }
        void getTicketPrices(string startStationId, string endStationId)
        {
            string serviceUri = getServiceUri(WebService.TicketPrices, startStationId, endStationId);
            if (isInterntConAvailable())
            {
                WebClient webClient = new WebClient();
                webClient.DownloadStringCompleted += webClient_DownloadTicketPricesCompleted;
                webClient.DownloadStringAsync(new Uri(serviceUri));
            }
            else
            {
                MessageBox.Show("No Internet Connection!");
            }

        }

        string getServiceUri(string defaultUri, string startStationId, string endStationId)
        {

            StringBuilder temp = new StringBuilder(defaultUri);
            temp.Replace("SSID", startStationId);
            temp.Replace("ESID", endStationId);

            return temp.ToString();
        }
        private void webClient_DownloadTicketPricesCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                string json = e.Result;


                if (!string.IsNullOrEmpty(json))
                {
                    RootPriceObject rootPriceObj = JsonConvert.DeserializeObject<RootPriceObject>(json);
                    priceList = rootPriceObj.RESULTS.priceList;

                    if (priceList != null)
                    {
                        setTicketPrices();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Connection has timed out!");
            }
        }
        void setTicketPrices()
        {

            txtFirstClass.Text = string.Format(APPCONTEXT.globalTicketPrice,priceList[0].priceLKR);
            txtSecondClass.Text = string.Format(APPCONTEXT.globalTicketPrice,priceList[1].priceLKR);
            txtThirdClass.Text = string.Format(APPCONTEXT.globalTicketPrice, priceList[2].priceLKR);
            txtDistance.Text = string.Format(APPCONTEXT.sceduleDetailDistance,priceList[0].distanceKM.ToString());

        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            string[] args = { scheduleDetailObj.startStationName, scheduleDetailObj.endStationName, scheduleDetailObj.departureTime, scheduleDetailObj.arrivalTimeEndStation, scheduleDetailObj.travelTime, scheduleDetailObj.trainFrequncy };
            string shareMessage = string.Format(APPCONTEXT.scheduleDetailShareMessage, args);
            AppUtility.shareStatusTask(shareMessage);
        }

        private void GoHome_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }
    
    }
}