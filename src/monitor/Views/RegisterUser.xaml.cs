﻿using DPFP;
using DPFP.Capture;
using DPFP.Processing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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

namespace monitor.Fingerprint.Views
{
    /// <summary>
    /// Interaction logic for RgisterUser.xaml
    /// </summary>
    public partial class RegisterUser : Page, DPFP.Capture.EventHandler
    {
        public int stateEnrroller = 0;     // controla el estado del proceso de incripción.
        private Capture Capturer;
        public Enrollment Enroller;
        public string typeProcces = "checkin"; // tipo de proceso que ejecutara el lector (register/validation/checkin) 
        //public delegate void OnTemplateEventHandler(DPFP.Template template);
        //public event OnTemplateEventHandler OnTemplate;
        public DPFP.Template Template;


        public RegisterUser()
        {
            InitializeComponent();
        }

        protected virtual void Init()
        {
            try
            {
                Enroller = new Enrollment();            // Create an enrollment.
                Capturer = new Capture(Priority.Low);              // Create a capture operation.

                UpdateStatus();
                if (null != Capturer)
                    Capturer.EventHandler = this;                   // Subscribe for capturing events.
            }
            catch
            {

            }
        }

        private void UpdateStatus()
        {
            this.stateEnrroller = (int)Enroller.FeaturesNeeded;
            SetStatus();
        }

        protected void Start()
        {
            if (null != Capturer)
            {
                try
                {
                    Capturer.StartCapture();
                }
                catch
                {

                }
            }
        }

        protected void SetPrompt(string prompt)
        {
            //this.Invoke(new Function(delegate ()
            //{
            //    //Prompt.Text = prompt;
            //}));
        }

        public void OnComplete(object Capture, string ReaderSerialNumber, Sample Sample)
        {
            //MakeReport("Huella capturada correctamente.");
            //SetPrompt("Capture la misma huella nuevamente.");
            Process(Sample);
        }

        protected virtual void Process(Sample Sample)
        {
            // Draw fingerprint sample image.
            DrawPicture(ConvertSampleToBitmap(Sample));

            // Process the sample and create a feature set for the enrollment purpose.
            DPFP.FeatureSet features = ExtractFeatures(Sample, DPFP.Processing.DataPurpose.Enrollment);

            // Check quality of the sample and add to enroller if it's good
            if (features != null) try
                {
                    // MakeReport("The fingerprint feature set was created.");
                    Enroller.AddFeatures(features);     // Add feature set to template.
                }
                finally
                {
                    UpdateStatus();

                    // Check if template has been created.
                    switch (Enroller.TemplateStatus)
                    {
                        case DPFP.Processing.Enrollment.Status.Ready:   // report success and stop capturing
                            OnTemplate(Enroller.Template);
                            SetPrompt("De click en guardar para terminar.");
                            Stop();
                            break;

                        case DPFP.Processing.Enrollment.Status.Failed:  // report failure and restart capturing
                            Enroller.Clear();
                            Stop();
                            UpdateStatus();
                            OnTemplate(null);
                            Start();
                            break;
                    }
                }

        }

        private void OnTemplate(DPFP.Template template)
        {
            Dispatcher.Invoke(() =>
            {
                Template = template;
                //VerifyButton.Enabled = SaveButton.Enabled = (Template != null);
                if (Template != null)
                    MessageBox.Show("Finalizo la captura correctamente.", "Aviso");
                else
                    MessageBox.Show("La captura no se realizo correctamente, repita el proceso.", "Aviso");
            });
        }

        private void DrawPicture(Bitmap bitmap)
        {
            Dispatcher.Invoke(() =>
            {
                Picture.Source = BitmapToImageSource(bitmap);   // fit the image into the picture box
            });
        }

        protected Bitmap ConvertSampleToBitmap(Sample Sample)
        {
            SampleConversion Convertor = new SampleConversion();  // Create a sample convertor.
            Bitmap bitmap = null;
            Convertor.ConvertToPicture(Sample, ref bitmap);
            return bitmap;
        }

        BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }

        public void OnFingerGone(object Capture, string ReaderSerialNumber)
        {
            //throw new NotImplementedException();
        }

        public void OnFingerTouch(object Capture, string ReaderSerialNumber)
        {
            //throw new NotImplementedException();
        }

        public void OnReaderConnect(object Capture, string ReaderSerialNumber)
        {
            ConnectionStatus("Conectado", 1);
        }

        public void OnReaderDisconnect(object Capture, string ReaderSerialNumber)
        {
            ConnectionStatus("Desconectado", 0);
        }

        public void OnSampleQuality(object Capture, string ReaderSerialNumber, CaptureFeedback CaptureFeedback)
        {
            // throw new NotImplementedException();
        }

        protected void ConnectionStatus(string message, int conexion)
        {
            Dispatcher.Invoke(() =>
            {
                switch (conexion)
                {
                    case 0:
                        lblStatus.Foreground = new SolidColorBrush(Colors.Red);
                        break;
                    case 1:
                        lblStatus.Foreground = new SolidColorBrush(Colors.Green);
                        break;
                }
                lblStatus.Text = message;
            });
        }

        protected void SetStatus()
        {
            Dispatcher.Invoke(() =>
            {
                lblSamples.Text = String.Format("Muestras restantes: {0}", Enroller.FeaturesNeeded);
            });
        }

        protected DPFP.FeatureSet ExtractFeatures(DPFP.Sample Sample, DPFP.Processing.DataPurpose Purpose)
        {
            DPFP.Processing.FeatureExtraction Extractor = new DPFP.Processing.FeatureExtraction();  // Create a feature extractor
            DPFP.Capture.CaptureFeedback feedback = DPFP.Capture.CaptureFeedback.None;
            DPFP.FeatureSet features = new DPFP.FeatureSet();
            Extractor.CreateFeatureSet(Sample, Purpose, ref feedback, ref features);            // TODO: return features as a result?
            if (feedback == DPFP.Capture.CaptureFeedback.Good)
                return features;
            else
                return null;
        }

        protected void Stop()
        {
            if (null != Capturer)
            {
                try
                {
                    Capturer.StopCapture();
                }
                catch
                {
                    SetPrompt("No se pudo terminar la captura!");
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Init();
            Start();
            gridFingerprint.Visibility = Visibility.Visible;
        }
    }
}