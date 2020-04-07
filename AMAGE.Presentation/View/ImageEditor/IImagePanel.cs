using System;
using System.Windows;
using System.Windows.Media;

namespace AMAGE.Presentation.View.ImageEditor
{
    public interface IImagePanel
    {
        event EventHandler SelectedIconsChanged;
        event EventHandler SelectedAreaChanged;

        int[] SelectedIcons { get; }
        Int32Rect SelectedArea { get; }
        bool MultiSelection { set; }
        bool AreaSelection { set; }

        void SetImage(ImageSource image);
        void SetImageInfo(string text);
        void SetIcons(ImageSource[] icons);
    }
}
