using AMAGE.Imaging.Tools.Tuners;
using System;
using System.Windows.Controls;
using AMAGE.Common.Imaging;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;
using Xceed.Wpf.Toolkit.PropertyGrid;
using System.Windows;

namespace AMAGE.UI.WPF.Tuners
{
    public partial class ExternalOverlayTuner : UserControl, IExternalOverlayTuner
    {
        public event EventHandler Tuning;

        public string AppFileName
        {
            get { return uiAppName.Text; }
        }

        public object TunableTool
        {
            get { return uiProperties.SelectedObject; }
            set { uiProperties.SelectedObject = value; }
        }

        IImage before;

        public IImage Before
        {
            get { return before; }
            set
            {
                before = value;
                After = null;
            }
        }

        public IImage After { get; private set; }


        public ExternalOverlayTuner()
        {
            InitializeComponent();
        }

        public void RefreshTunableTool()
        {

        }

        private void RunApp_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (Path.GetExtension(uiAppName.Text) == ".exe")
            {
                string tempFile = Path.GetTempFileName() + ".png";
                Before.ToFile(tempFile, "png");

                try
                {
                    Process app = Process.Start(uiAppName.Text, tempFile);
                    app.WaitForExit();

                    After = Imaging.Image.Create();
                    After.FromFile(tempFile);

                    Tuning?.Invoke(this, e);
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void SelectApp_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog()
            {
                Filter = "Application|*.exe"
            };

            if (dialog.ShowDialog() == true)
            {
                uiAppName.Text = dialog.FileName;
            }
        }

        private void Properties_PropertyValueChanged(object sender, PropertyValueChangedEventArgs e)
        {
            Tuning?.Invoke(this, e);
        }
    }
}
