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
using System.Windows.Shapes;

namespace monitor.Reports
{
    /// <summary>
    /// Interaction logic for ReportViewer.xaml
    /// </summary>
    public partial class ReportViewer : Window
    {
        public ReportViewer(int reportContext)
        {
            InitializeComponent();
            SetDataContext(reportContext);
        }

        private void SetDataContext(int value)
        {
            switch (value)
            {
                case 1:
                    DataContext = new ViewModels.PiezasPorModeloViewModel();
                    break;


            }
        }
    }
}
