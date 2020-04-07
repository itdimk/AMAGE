using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace AMAGE.UI.WPF
{

    public partial class Settings : Window
    {
        public Settings()
        {
            InitializeComponent();


            CultureInfo[] supportedLanguages = new CultureInfo[]
            {
                new CultureInfo("ru"),
                new CultureInfo("en")
            };

            foreach (CultureInfo lang in supportedLanguages)
                uiLanguages.Items.Add(lang);


            uiLanguages.SelectedIndex = Array.FindIndex(supportedLanguages,
                i => i.Name == Presentation.Properties.Settings.Default.CultureInfo.Name);

            uiUseAsync.IsChecked = Presentation.Properties.Settings.Default.UseAsyncMode;
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            var settings = Presentation.Properties.Settings.Default;
            settings.UseAsyncMode = uiUseAsync.IsChecked == true;
            settings.CultureInfo = (CultureInfo)uiLanguages.SelectedItem;

            MessageBox.Show(Properties.Resources.RestartToApplySomeSettings,
               "Info", MessageBoxButton.OK, MessageBoxImage.Warning);
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
