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
using System.Text;
using Telerik.Windows.Controls;
using System.Xml.Linq;

namespace PrototypeBeta
{
    public partial class TicketPricePage : PhoneApplicationPage
    {

        public List<StationList> stationList;
        public List<PriceList> priceList;

        public TicketPricePage()
        {
            InitializeComponent();

            createTicketPrice();
        }
        bool isInterntConAvailable()
        {
            if (DeviceNetworkInformation.IsNetworkAvailable)
                return true;
            return false;
        }
        void createTicketPrice()
        {
            if (isInterntConAvailable())
            {
                WebClient webClient = new WebClient();
                webClient.DownloadStringCompleted += webClient_DownloadTrainRoutesCompleted;
                webClient.DownloadStringAsync(new Uri(WebService.RailwayStations));
                this.busyIndicator.IsRunning = true;
            }
            else
            {
                populateOfflineStationList();
                //RadMessageBox.ShowAsync("Error!", MessageBoxButtons.OK, "Network issue. Please check your internet connection.");
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
                RadMessageBox.ShowAsync("AW SNAP :-(", MessageBoxButtons.OK, "We are having trouble downloading data due to temporary server unavailability at the moment.");
            }
            this.busyIndicator.IsRunning = false;
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
                if(validateInputs()){

                StationList startStation = (StationList)acbFromStation.SelectedItem;
                StationList endStation = (StationList)acbToStation.SelectedItem;

                getTicketPrices(startStation.stationID.ToString(), endStation.stationID.ToString());
              
                }else{

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
                RadMessageBox.ShowAsync("AW SNAP :-(", MessageBoxButtons.OK, "We are having trouble downloading data due to network connectivity or temporary server unavailability at the moment. Please make sure you are connected to internet and try again.");
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
                RadMessageBox.ShowAsync("AW SNAP :-(", MessageBoxButtons.OK, "We are having trouble downloading data due to temporary server unavailability at the moment.");
            }
        }

        void setTicketPrices()
        {

            txtFirstClass.Text = string.Format(APPCONTEXT.globalTicketPrice, priceList[0].priceLKR);
            txtSecondClass.Text = string.Format(APPCONTEXT.globalTicketPrice, priceList[1].priceLKR);
            txtThirdClass.Text = string.Format(APPCONTEXT.globalTicketPrice, priceList[2].priceLKR);

            ApplicationBarIconButton appBarButton = ((ApplicationBarIconButton)ApplicationBar.Buttons[1]);
            appBarButton.IsEnabled = true;

        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {

            StationList startStation = (StationList)acbFromStation.SelectedItem;
            StationList endStation = (StationList)acbToStation.SelectedItem;
            if (startStation == null || endStation == null || txtFirstClass.Text == "")
            {
                RadMessageBox.ShowAsync("AW SNAP!", MessageBoxButtons.OK, "Ticket prices are unavailabe for sharing");
            }
            else
            {
                string[] args = { startStation.stationName, endStation.stationName, txtFirstClass.Text, txtSecondClass.Text, txtThirdClass.Text };
                string shareMessage = string.Format(APPCONTEXT.ticketPriceShareMessage, args);
                AppUtility.shareStatusTask(shareMessage);
            }

        }

        private void acbFromStation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            setPageControlValues();
        }

        private void acbToStation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            setPageControlValues();
        }

        void setPageControlValues() {

            ApplicationBarIconButton appBarButton = ((ApplicationBarIconButton)ApplicationBar.Buttons[1]);
            if (acbFromStation.SelectedItem==null || acbToStation.SelectedItem == null)
            {
                appBarButton.IsEnabled = false;
            }
            txtFirstClass.Text = "";
            txtSecondClass.Text = "";
            txtThirdClass.Text = "";
        }


        private void GoHome_Click(object sender, EventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}