using AMAGE.Presentation.View.ImageEditor;
using System;
using System.Windows.Controls;
using AMAGE.Imaging.Tools.Tuners;

namespace AMAGE.UI.WPF.ImageEditor
{
    public partial class ToolPanel : UserControl, IToolPanel
    {
        public event EventHandler Tuning;
        public event EventHandler SelectedToolChanged;

        public object SelectedTool => uiTools.SelectedItem;

        public ToolPanel()
        {
            InitializeComponent();
        }

        public void SetTools(params object[] tools)
        {
            uiTools.ItemsSource = tools;
        }

        public void SetTuner(ITuner tuner)
        {
            ITuner oldTuner = uiTuningSpace.Content as ITuner;

            if (oldTuner != null)
            {
                oldTuner.Tuning -= Tuner_Tuning;
                uiTuningSpace.Content = null;
            }

            uiTuningSpace.Content = tuner;

            if (tuner != null)
                tuner.Tuning += Tuner_Tuning;
        }

        private void Tools_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source == uiTools)
                SelectedToolChanged?.Invoke(this, e);
        }

        private void Tuner_Tuning(object sender, EventArgs e)
        {
            Tuning?.Invoke(this, e);
        }

        private void Tools_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            uiTools.SelectedItem = null;
        }
    }
}
