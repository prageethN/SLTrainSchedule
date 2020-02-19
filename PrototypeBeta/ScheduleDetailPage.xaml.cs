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
using Microsoft.Phone.Net.NetworkInformation;
using PrototypeBeta.DataModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using Telerik.Windows.Controls;
using System.IO;
using Microsoft.Phone.Tasks;

namespace PrototypeBeta
{
    public partial class ScheduleDetailPage : PhoneApplicationPage
    {
        public List<PriceList> priceList;
        ScheduleDetailViewModel scheduleDetailObj;

        public ScheduleDetailPage()
        {
            InitializeComponent();
            
            populateShedule();
        }

        void populateShedule()
        {

            scheduleDetailObj = (ScheduleDetailViewModel)NavigationService.GetNavigationData();
            this.DataContext = scheduleDetailObj;
            populateTicketPrice(scheduleDetailObj.startStationId.ToString(),scheduleDetailObj.endStationId.ToString());
        }
        void populateTicketPrice(string startStationId, string endStationId)
        {

            getTicketPrices(startStationId, endStationId);
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
            string[] args = { scheduleDetailObj.startStationName, scheduleDetailObj.endStationName, scheduleDetailObj.departureTime, scheduleDetailObj.arrivalTimeEndStation,scheduleDetailObj.travelTime ,scheduleDetailObj.trainFrequncy };
            string shareMessage = string.Format(APPCONTEXT.scheduleDetailShareMessage, args);
            AppUtility.shareStatusTask(shareMessage);
        }

        private void GoHome_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }

        private void TicketReservation_Click(object sender, EventArgs e)
        {
            ControlTemplate imageTemplate = (ControlTemplate)this.Resources["ImageTemplate"];
            RadMessageBox.Show(imageTemplate, APPCONTEXT.scheduleDetailReservationTitle, new string[] { "Call", "Cancel" }, ReadFileContents(), closedHandler: (args) =>
            {
                DialogResult result = args.Result;
                int buttonIndex = args.ButtonIndex;
                if (buttonIndex == 0)
                {
                    PhoneCallTask phoneCallTask = new PhoneCallTask();

                    phoneCallTask.PhoneNumber = APPCONTEXT.scheduleDetailReservationNo;
                    phoneCallTask.DisplayName = string.Format(APPCONTEXT.stationListStationName, APPCONTEXT.scheduleDetailReservationContactName);

                    phoneCallTask.Show();
                }
            });
        }
        public string ReadFileContents()
        {
            Uri x = new Uri("TicketReservationInfo.txt", UriKind.Relative);
            var ResrouceStream = Application.GetResourceStream(x);
            if (ResrouceStream != null)
            {
                using (Stream myFileStream = ResrouceStream.Stream)
                {
                    if (myFileStream.CanRead)
                    {
                        StreamReader myStreamReader = new StreamReader(myFileStream);
                        return myStreamReader.ReadToEnd();
                    }

                }
            }

            return string.Empty;

        }
    }
}