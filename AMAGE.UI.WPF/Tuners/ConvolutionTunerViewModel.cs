using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AMAGE.UI.WPF.Tuners
{
    public class ConvolutionTunerViewModel : INotifyPropertyChanged
    {
        public int Kernel00 { get; set; }
        public int Kernel01 { get; set; }
        public int Kernel02 { get; set; }

        public int Kernel10 { get; set; }
        public int Kernel11 { get; set; }
        public int Kernel12 { get; set; }

        public int Kernel20 { get; set; }
        public int Kernel21 { get; set; }
        public int Kernel22 { get; set; }

        public int KernelFactorSum { get; set; }
        public int KernelOffsetSum { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void RefreshAll()
        {
            foreach (PropertyInfo p in GetType().GetProperties())
            {
                OnPropertyChanged(p.Name);
            }
        }

        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
