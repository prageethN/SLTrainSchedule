﻿#pragma checksum "E:\App Dev\Windows Phone\Beta Release\SLTrainSchedule\PrototypeBeta\ScheduleDetailPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "3545D80ADAA4F71D2772EFBBF9B65C44"
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
    
    
    public partial class ScheduleDetailPage : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.ScrollViewer spSearch;
        
        internal System.Windows.Controls.Grid ContentPanel;
        
        internal System.Windows.Controls.Grid TimePanel;
        
        internal System.Windows.Controls.Grid InfoPanel;
        
        internal System.Windows.Controls.TextBlock txtDistance;
        
        internal System.Windows.Controls.Grid TicketPanel;
        
        internal System.Windows.Controls.TextBlock txtFirstClass;
        
        internal System.Windows.Controls.TextBlock txtSecondClass;
        
        internal System.Windows.Controls.TextBlock txtThirdClass;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/PrototypeBeta;component/ScheduleDetailPage.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.spSearch = ((System.Windows.Controls.ScrollViewer)(this.FindName("spSearch")));
            this.ContentPanel = ((System.Windows.Controls.Grid)(this.FindName("ContentPanel")));
            this.TimePanel = ((System.Windows.Controls.Grid)(this.FindName("TimePanel")));
            this.InfoPanel = ((System.Windows.Controls.Grid)(this.FindName("InfoPanel")));
            this.txtDistance = ((System.Windows.Controls.TextBlock)(this.FindName("txtDistance")));
            this.TicketPanel = ((System.Windows.Controls.Grid)(this.FindName("TicketPanel")));
            this.txtFirstClass = ((System.Windows.Controls.TextBlock)(this.FindName("txtFirstClass")));
            this.txtSecondClass = ((System.Windows.Controls.TextBlock)(this.FindName("txtSecondClass")));
            this.txtThirdClass = ((System.Windows.Controls.TextBlock)(this.FindName("txtThirdClass")));
        }
    }
}

