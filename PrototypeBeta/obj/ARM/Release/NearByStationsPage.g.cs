﻿#pragma checksum "E:\App Dev\Windows Phone\Beta Release\SLTrainSchedule 2.0\PrototypeBeta\NearByStationsPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "9590E0B6D159A0CA59BACAC651FD0AC2"
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
using Telerik.Windows.Controls;


namespace PrototypeBeta {
    
    
    public partial class NearByStationsPage : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.TextBlock txtStationCountSummary;
        
        internal Telerik.Windows.Controls.RadBusyIndicator busyIndicator;
        
        internal System.Windows.Controls.StackPanel pnlEmptyStationList;
        
        internal System.Windows.Controls.ListBox lsNearBy;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/PrototypeBeta;component/NearByStationsPage.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.txtStationCountSummary = ((System.Windows.Controls.TextBlock)(this.FindName("txtStationCountSummary")));
            this.busyIndicator = ((Telerik.Windows.Controls.RadBusyIndicator)(this.FindName("busyIndicator")));
            this.pnlEmptyStationList = ((System.Windows.Controls.StackPanel)(this.FindName("pnlEmptyStationList")));
            this.lsNearBy = ((System.Windows.Controls.ListBox)(this.FindName("lsNearBy")));
        }
    }
}

