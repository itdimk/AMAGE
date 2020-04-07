using AMAGE.Presentation.View.ImageEditor;
using System.Windows;
using System.Windows.Controls;
using System;

namespace AMAGE.UI.WPF.ImageEditor
{
    public partial class MenuPanel : UserControl, IMenuPanel
    {
        public event EventHandler OpenFile;
        public event EventHandler SaveFile;
        public event EventHandler CloseFile;

        public event EventHandler ApplyTool;
        public event EventHandler PlayAnimation;
        public event EventHandler StopAnimation;

        public MenuPanel()
        {
            InitializeComponent();
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
            => OpenFile?.Invoke(this, e);

        private void SaveFile_Click(object sender, RoutedEventArgs e)
            => SaveFile?.Invoke(this, e);

        private void CloseFile_Click(object sender, RoutedEventArgs e)
            => CloseFile?.Invoke(this, e);

        private void ApplyTool_Click(object sender, RoutedEventArgs e)
            => ApplyTool?.Invoke(this, e);

        private void Play_Click(object sender, RoutedEventArgs e)
            => PlayAnimation?.Invoke(this, e);

        private void Stop_Click(object sender, RoutedEventArgs e)
            => StopAnimation?.Invoke(this, e);

        private void AppAbout_Click(object sender, RoutedEventArgs e)
        {
            new About().ShowDialog();
        }

        public void SwitchEnabledBySubscription()
        {
            uiOpenFile.IsEnabled = OpenFile != null;
            uiSaveFile.IsEnabled = SaveFile != null;
            uiCloseFile.IsEnabled = CloseFile != null;

            uiApplyTool.IsEnabled = ApplyTool != null;

            uiPlay.IsEnabled = PlayAnimation != null;
            uiStop.IsEnabled = StopAnimation != null;
        }

        private void AppSettings_Click(object sender, RoutedEventArgs e)
        {
            new Settings().ShowDialog();
        }
    }
}
