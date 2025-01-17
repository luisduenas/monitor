﻿using monitor.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace monitor.Views.EstacionesView
{
    /// <summary>
    /// Interaction logic for RegisterEstacion.xaml
    /// </summary>
    public partial class RegisterEstacion : Page
    {
        Estacion Estacion;
        bool isEdit;

        EstacionRepository _estacionRepository;
        List<string> monitores;

        public RegisterEstacion()
        {
            InitializeComponent();
            Loaded += RegisterEstacion_Loaded;
        }
        public RegisterEstacion(Estacion estacion)
        {
            InitializeComponent();
            Loaded += RegisterEstacion_Loaded;

            Estacion = estacion;
            isEdit = true;
        }

        private void RegisterEstacion_Loaded(object sender, RoutedEventArgs e)
        {
            _estacionRepository = new EstacionRepository();
            var screens = Screen.AllScreens;

            monitores = new List<string>();
            foreach (var screen in screens)
            {
                monitores.Add(screen.DeviceName);
            }

            cbMonitor.ItemsSource = monitores;

            if (isEdit)
            {
                tbEstacion.Text = Estacion.Nombre;
                tbIPPLC.Text = Estacion.IPPLC;
                if (Estacion.Soldador == 1)
                {
                    cbSoldadora.IsChecked = true;
                    tbIPSoldadora.Visibility = Visibility.Visible;
                }
                tbIPSoldadora.Text = Estacion.IPSoldador;
                cbMonitor.SelectedItem = Estacion.Monitor;
                cbSegundos.Text = Estacion.SegundosAyudaVisual.ToString();
            } 
        }

        private void TbEstacion_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {

        }

        private void TbIPPLC_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {

        }

        private void TbIPSoldadora_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {

        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ValidateFields())
                {
                    if (isEdit)
                    {
                        Estacion.FechaHora = DateTime.Now;
                        Estacion.Nombre = tbEstacion.Text;
                        Estacion.IPPLC = tbIPPLC.Text;
                        Estacion.Soldador = cbSoldadora.IsChecked.Value ? (byte)1 : (byte)0;
                        Estacion.IPSoldador = cbSoldadora.IsChecked.Value ? tbIPSoldadora.Text : "";
                        Estacion.Monitor = cbMonitor.SelectedItem.ToString();
                        Estacion.SegundosAyudaVisual = int.Parse(cbSegundos.Text);
                        _estacionRepository.UpdateEstacion(Estacion);
                        
                        NavigationService.GoBack();
                        return;
                    }


                    Estacion estacion = new Estacion()
                    {
                        FechaHora = DateTime.Now,
                        Nombre = tbEstacion.Text,
                        IPPLC = tbIPPLC.Text,
                        Soldador = cbSoldadora.IsChecked.Value ? (byte)1 : (byte)0,
                        IPSoldador = tbIPSoldadora.Text,
                        Monitor = cbMonitor.SelectedItem.ToString(),
                        Estatus = 1,
                        SegundosAyudaVisual = int.Parse(cbSegundos.Text)
                    };

                    _estacionRepository.InsertEstacion(estacion);

                    NavigationService.GoBack();
                    return;
                }

                throw new Exception("Verifique que todos los campos esten capturados correctamente.");

            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        private bool ValidateFields()
        {
            if (string.IsNullOrWhiteSpace(tbEstacion.Text))
            {
                return false;
            }
            //if (string.IsNullOrWhiteSpace(tbIPPLC.Text))
            //{
            //    return false;
            //}
            if (cbMonitor.SelectedItem == null)
            {
                return false;
            }
            if (cbSoldadora.IsChecked.Value && string.IsNullOrWhiteSpace(tbIPSoldadora.Text))
            {
                return false;
            }
            if (string.IsNullOrEmpty(cbSegundos.Text))
            {
                return false;
            }
            return true;
        }

        private void CbSoldadora_Checked(object sender, RoutedEventArgs e)
        {
            if (cbSoldadora.IsChecked.Value)
            {
                tbIPSoldadora.Visibility = Visibility.Visible;
                return;
            }

            tbIPSoldadora.Visibility = Visibility.Collapsed;
        }

        private void BtnHelp_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show("Activar la casilla para indicar que la estación utiliza soldadora.","Información");
        }

        private void BtnHelp2_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show("Indica los segundos de trasición entre una diapositiva y otra.", "Información");
        }

        private void CbSegundos_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}

