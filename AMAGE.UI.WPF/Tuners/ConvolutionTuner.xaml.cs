using AMAGE.Imaging.Tools.Tuners;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace AMAGE.UI.WPF.Tuners
{
    // TODO: Улучшить реализацию
    public partial class ConvolutionTuner : UserControl, IConvolutionTuner
    {
        Tuple<int[,], int, int> sharpness = Tuple.Create(
            new int[,]
            {
                { 0, -2, 0 },
                { -2, 11, -2 },
                { 0, -2, 0 }
            }, 3, 0);

        Tuple<int[,], int, int> blur = Tuple.Create(
          new int[,]
          {
                { 1, 1, 1 },
                { 1, 1, 1 },
                { 1, 1, 1 }
          }, 9, 0);

        Tuple<int[,], int, int> emboss = Tuple.Create(
           new int[,]
           {
                { -1, 0, -1 },
                { 0, 4, 0 },
                { -1, 0, -1 }
           }, 1, 127);

        public event EventHandler Tuning;

        private ConvolutionTunerViewModel ViewModel
        {
            get { return (ConvolutionTunerViewModel)DataContext; }
        }

        public object TunableTool { get; set; }

        public int[,] Kernel
        {
            get
            {
                return new int[,]
                {
                    { ViewModel.Kernel00, ViewModel.Kernel01, ViewModel.Kernel02 },
                    { ViewModel.Kernel10, ViewModel.Kernel11, ViewModel.Kernel12 },
                    { ViewModel.Kernel20, ViewModel.Kernel21, ViewModel.Kernel22 }
                };
            }
        }

        public int KernelFactorSum
        {
            get
            {
                return ViewModel.KernelFactorSum;
            }
        }

        public int KernelOffsetSum
        {
            get
            {
                return ViewModel.KernelOffsetSum;
            }
        }

        public ConvolutionTuner()
        {
            InitializeComponent();

            uiPresets.Items.Add(Properties.Resources.Sharpness);
            uiPresets.Items.Add(Properties.Resources.Blur);
            uiPresets.Items.Add(Properties.Resources.Emboss);

            uiPresets.SelectedIndex = 0;
        }

        public void RefreshTunableTool()
        {

        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            Tuning?.Invoke(this, e);
        }

        private void Presets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int[,] kernel = null;

            switch (uiPresets.SelectedIndex)
            {
                case 0:
                    kernel = sharpness.Item1;

                    ViewModel.KernelFactorSum = sharpness.Item2;
                    ViewModel.KernelOffsetSum = sharpness.Item3;
                    break;

                case 1:
                    kernel = blur.Item1;

                    ViewModel.KernelFactorSum = blur.Item2;
                    ViewModel.KernelOffsetSum = blur.Item3;
                    break;

                case 2:
                    kernel = emboss.Item1;

                    ViewModel.KernelFactorSum = emboss.Item2;
                    ViewModel.KernelOffsetSum = emboss.Item3;
                    break;
            }


            for (int y = 0; y < kernel.GetLength(1); ++y)
                for (int x = 0; x < kernel.GetLength(0); ++x)
                    SetItem(x, y, kernel);
        }

        private void SetItem(int x, int y, int[,] kernel)
        {
            ViewModel.GetType().GetProperty("Kernel" + x + y).SetValue(ViewModel, kernel[x, y]);
            ViewModel.RefreshAll();
        }
    }
}
