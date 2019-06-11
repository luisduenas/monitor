﻿using monitor.Views;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Deployment.Application;

namespace monitor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Index page = new Index();
            mainPage.NavigationService.Navigate(page);
            lblPageTitle.Content = page.Title;
            LoadMenus();
            LoadCurrentVersion();
        }

        private void LoadCurrentVersion()
        {
            string version = string.Empty;
            try
            {
                //// get deployment version
                version = ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
            }
            catch (InvalidDeploymentException)
            {
                //// you cannot read publish version when app isn't installed 
                //// (e.g. during debug)
                version = "debug";
            }
            Title = $"{Title} - {version}";
        }
        public void LoadMenus()
        {
            List<Item> items;
            using (StreamReader r = new StreamReader("Data\\data.json"))
            {
                string json = r.ReadToEnd();
                items = JsonConvert.DeserializeObject<List<Item>>(json);
            }
            foreach (var item in items)
            {

            }
        }

    }
    public class Item
    {
        public string title;
        public string icon;
    }
}
