﻿using monitor.Data;
using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;

namespace monitor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static Usuario usuario;
        public static Modelo modelo;

        public App()
        {
            modelo = new Modelo()
            {
                NumeroModelo = "LM89769",
                RutaAyudaVisual = @"C:\Users\Flores\Dropbox\Monitor\",
                Routing = 12.3
            };
        }
    }
}
