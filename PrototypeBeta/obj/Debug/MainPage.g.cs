﻿#pragma checksum "E:\App Dev\Windows Phone\PrototypeBeta\PrototypeBeta\MainPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "1D6CE1F9BC9927C41B154DBA2C3D6E5C"
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
    
    
    public partial class MainPage : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.Grid SearchPanel;
        
        internal System.Windows.Controls.ScrollViewer spSearch;
        
        internal Microsoft.Phone.Controls.AutoCompleteBox acbFromStation;
        
        internal Microsoft.Phone.Controls.AutoCompleteBox acbToStation;
        
        internal Telerik.Windows.Controls.RadDatePicker radDatePicker;
        
        internal System.Windows.Controls.RadioButton rbFullSchedule;
        
        internal System.Windows.Controls.RadioButton rbNextSchedule;
        
        internal System.Windows.Controls.StackPanel pnlTimePick;
        
        internal System.Windows.Controls.CheckBox cbTimeFilter;
        
        internal Telerik.Windows.Controls.RadTimePicker rtFromTime;
        
        internal Telerik.Windows.Controls.RadTimePicker rtToTime;
        
        internal System.Windows.Controls.Button btnSearch;
        
        internal System.Windows.Controls.ScrollViewer spMenu;
        
        internal System.Windows.Controls.Grid MenuPanel;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/PrototypeBeta;component/MainPage.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.SearchPanel = ((System.Windows.Controls.Grid)(this.FindName("SearchPanel")));
            this.spSearch = ((System.Windows.Controls.ScrollViewer)(this.FindName("spSearch")));
            this.acbFromStation = ((Microsoft.Phone.Controls.AutoCompleteBox)(this.FindName("acbFromStation")));
            this.acbToStation = ((Microsoft.Phone.Controls.AutoCompleteBox)(this.FindName("acbToStation")));
            this.radDatePicker = ((Telerik.Windows.Controls.RadDatePicker)(this.FindName("radDatePicker")));
            this.rbFullSchedule = ((System.Windows.Controls.RadioButton)(this.FindName("rbFullSchedule")));
            this.rbNextSchedule = ((System.Windows.Controls.RadioButton)(this.FindName("rbNextSchedule")));
            this.pnlTimePick = ((System.Windows.Controls.StackPanel)(this.FindName("pnlTimePick")));
            this.cbTimeFilter = ((System.Windows.Controls.CheckBox)(this.FindName("cbTimeFilter")));
            this.rtFromTime = ((Telerik.Windows.Controls.RadTimePicker)(this.FindName("rtFromTime")));
            this.rtToTime = ((Telerik.Windows.Controls.RadTimePicker)(this.FindName("rtToTime")));
            this.btnSearch = ((System.Windows.Controls.Button)(this.FindName("btnSearch")));
            this.spMenu = ((System.Windows.Controls.ScrollViewer)(this.FindName("spMenu")));
            this.MenuPanel = ((System.Windows.Controls.Grid)(this.FindName("MenuPanel")));
        }
    }
}

