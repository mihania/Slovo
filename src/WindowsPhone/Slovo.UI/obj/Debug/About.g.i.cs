﻿#pragma checksum "D:\Projects\mihailsm\Slovo8\src\WP7\Slovo\Slovo.UI\About.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "BC8BB270509E2400B752E92CE5900CBC"
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


namespace Slovo.UI {
    
    
    public partial class About : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.StackPanel TitlePanel;
        
        internal System.Windows.Controls.TextBlock tbCopyright;
        
        internal System.Windows.Controls.TextBlock tbDescription;
        
        internal System.Windows.Controls.Image image1;
        
        internal System.Windows.Controls.TextBlock tbProgramName;
        
        internal System.Windows.Controls.TextBlock tbVocabulariesCapacity;
        
        internal System.Windows.Controls.TextBlock tbLicense;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/Slovo;component/About.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.TitlePanel = ((System.Windows.Controls.StackPanel)(this.FindName("TitlePanel")));
            this.tbCopyright = ((System.Windows.Controls.TextBlock)(this.FindName("tbCopyright")));
            this.tbDescription = ((System.Windows.Controls.TextBlock)(this.FindName("tbDescription")));
            this.image1 = ((System.Windows.Controls.Image)(this.FindName("image1")));
            this.tbProgramName = ((System.Windows.Controls.TextBlock)(this.FindName("tbProgramName")));
            this.tbVocabulariesCapacity = ((System.Windows.Controls.TextBlock)(this.FindName("tbVocabulariesCapacity")));
            this.tbLicense = ((System.Windows.Controls.TextBlock)(this.FindName("tbLicense")));
        }
    }
}

