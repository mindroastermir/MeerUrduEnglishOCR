using MeerUrduEnglishOCR.Dialogs;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace MeerUrduEnglishOCR
{
  public partial class MainWindow : Window, IComponentConnector
  {
    /*
        internal Button btcOCR;
    private bool _contentLoaded;
    */
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btcOCR_Click(object sender, RoutedEventArgs e) => new OcrDialog().Show();
    
  }
}
