using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Xml.Linq;
using PrototypeBeta.Util;

namespace PrototypeBeta
{
    public partial class LinesPage : PhoneApplicationPage
    {
        public LinesPage()
        {
            InitializeComponent();
            populateLines();
        }

        void populateLines(){

            XDocument loadedData = XDocument.Load("StaticData/LinesStaticData.xml");

            var data = from query in loadedData.Descendants("Line")
                       select new Line()
                       {
                           LineID=(int)query.Element("LineID"),
                           LineName = (string)query.Element("LineName"),
                           LineRoute = (string)query.Element("LineRoute"),
                           LineServices = (string)query.Element("LineServices")
                       };
            data=data.OrderBy(x => x.LineName);
            lsMainLines.ItemsSource = data.ToList();
        }

        private void GoHome_Click(object sender, EventArgs e)
        {
            NavigationService.GoBack();
        }

        private void lsMainLines_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Line selectedLine = (Line)lsMainLines.SelectedItem;
            if (selectedLine != null)
            {
                lsMainLines.SelectedItem = null;
                NavigationService.Navigate(new Uri("/LineStationsPage.xaml", UriKind.Relative), selectedLine);
            }
        }
    }
    public class Line
    {
        int lineID;
        string lineName;
        string lineRoute;
        string lineServices;

        public string LineName
        {
            get { return lineName; }
            set { lineName = value; }
        }

        public string LineRoute
        {
            get { return lineRoute; }
            set { lineRoute = value; }
        }

        public string LineServices
        {
            get { return lineServices; }
            set { lineServices = value; }
        }

        public int LineID
        {
            get { return lineID; }
            set { lineID = value; }
        }

    }
}