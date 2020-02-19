using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO;
using System.Windows.Resources;
using System.IO.IsolatedStorage;

namespace PrototypeBeta
{
    public partial class TrainLineMapPage : PhoneApplicationPage
    {
        public TrainLineMapPage()
        {
            InitializeComponent();
           // loadBrowser();
        }

        void loadBrowser()
        {


            //wbTrainLine.Navigate(new Uri("http://www.google.lk"));
        }
        private void SaveHelpFileToIsoStore()
        {
            string strFileName = "Map.htm";
            IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication();

            //remove the file if exists to allow each run to independently write to
            // the Isolated Storage
            if (isoStore.FileExists(strFileName) == true)
            {
                isoStore.DeleteFile(strFileName);
            }
            StreamResourceInfo sr = Application.GetResourceStream(new Uri(strFileName,UriKind.Relative));
            using (BinaryReader br = new BinaryReader(sr.Stream))
            {
                byte[] data = br.ReadBytes((int)sr.Stream.Length);
                //save file to Isolated Storage
                using (BinaryWriter bw = new BinaryWriter(isoStore.CreateFile(strFileName)))
                {
                    bw.Write(data);
                    bw.Close();
                }
            }
        }

        private void wbTrainLine_Loaded(object sender, RoutedEventArgs e)
        {
            SaveHelpFileToIsoStore();
            wbTrainLine.Navigate(new Uri("Map.htm", UriKind.Relative));

        }

        private void GoHome_Click(object sender, EventArgs e)
        {
            NavigationService.GoBack();
        }

    }
}