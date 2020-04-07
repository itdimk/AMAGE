using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace AMAGE.UI.WPF
{
    public partial class About : Window
    {
        public About()
        {
            InitializeComponent();

            Assembly assembly = Assembly.GetExecutingAssembly();
            
            uiAppName.Text = $"{assembly.GetName().Name} {assembly.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version}";
            uiCompany.Text = $"{Properties.Resources.Developer} : {assembly.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company}";
            uiCopyright.Text = assembly.GetCustomAttribute<AssemblyCopyrightAttribute>()?.Copyright;
        }

        private void Company_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Match url = Regex.Match(uiCompany.Text, @"(http:\/\/|https:\/\/)(\w|\.)+\.(\w|\.)+");

            if (url != null)
                Process.Start(url.Value);
        }
    }
}
