using AMAGE.Imaging.Tools.Tuners;
using System;
using System.Windows.Controls;
using Xceed.Wpf.Toolkit.PropertyGrid;

namespace AMAGE.UI.WPF.Tuners
{
    public partial class Tuner : UserControl, ITuner
    {
        public event EventHandler Tuning;

        public object TunableTool
        {
            get { return uiPropertyGrid.SelectedObject; }
            set { uiPropertyGrid.SelectedObject = value; }
        }

        public Tuner()
        {
            InitializeComponent();
        }

        private void PropertyGrid_PropertyValueChanged(object sender, PropertyValueChangedEventArgs e)
        {
            Tuning?.Invoke(this, e);
        }

        public void RefreshTunableTool()
        {
            object tunableTool = TunableTool;
            TunableTool = null;
            TunableTool = tunableTool;
        }
    }
}
