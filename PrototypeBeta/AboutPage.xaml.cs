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

namespace PrototypeBeta
{
    public partial class AboutPage : PhoneApplicationPage
    {
        public AboutPage()
        {
            InitializeComponent();
            setAppDescription();

        }
        private void setAppDescription()
        {

            txtAppDescription.Text = ReadFileContents();
        }
        public string ReadFileContents()
        {
            Uri x = new Uri("AppDescription.txt", UriKind.Relative);
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
