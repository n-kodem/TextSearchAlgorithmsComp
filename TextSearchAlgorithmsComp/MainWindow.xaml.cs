﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TextSearchAlgorithmsComp
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void onFileSelectionBtnClick(object sender, RoutedEventArgs e)
        {

        }

        private void onCompareCheckboxChange(object sender, RoutedEventArgs e)
        {
            if (compareAllCheckbox.IsChecked==true)
            {
                showHighest.IsEnabled = true;
                showLowest.IsEnabled = true;
            }
            else
            {
                showHighest.IsChecked = false;
                showLowest.IsChecked = false;
                showHighest.IsEnabled = false;
                showLowest.IsEnabled = false;
            }
        }
    }
}
