﻿#pragma checksum "E:\App Dev\Windows Phone\PrototypeBeta\PrototypeBeta\ScheduleListPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "F75CA78A5AE33BD065623EC3C2E4650F"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34014
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.Phone.Controls;
using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace PrototypeBeta {
    
    
    public partial class ScheduleListPage : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.TextBlock txtStations;
        
        internal System.Windows.Controls.TextBlock txtDate;
        
        internal System.Windows.Controls.TextBlock txtTimePeriod;
        
        internal System.Windows.Controls.StackPanel spSearch;
        
        internal Microsoft.Phone.Controls.LongListSelector lsSearchResult;
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Windows.Application.LoadComponent(this, new System.Uri("/PrototypeBeta;component/ScheduleListPage.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.txtStations = ((System.Windows.Controls.TextBlock)(this.FindName("txtStations")));
            this.txtDate = ((System.Windows.Controls.TextBlock)(this.FindName("txtDate")));
            this.txtTimePeriod = ((System.Windows.Controls.TextBlock)(this.FindName("txtTimePeriod")));
            this.spSearch = ((System.Windows.Controls.StackPanel)(this.FindName("spSearch")));
            this.lsSearchResult = ((Microsoft.Phone.Controls.LongListSelector)(this.FindName("lsSearchResult")));
        }
    }
}

