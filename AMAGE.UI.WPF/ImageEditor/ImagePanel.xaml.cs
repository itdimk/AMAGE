using AMAGE.Presentation.View.ImageEditor;
using DAP.Adorners;
using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AMAGE.UI.WPF.ImageEditor
{
    public partial class ImagePanel : UserControl, IImagePanel
    {
        public event EventHandler SelectedIconsChanged;
        public event EventHandler SelectedAreaChanged;

        private CroppingAdorner cropping;
        private FrameworkElement croppedElement;
        private bool imageAreaSelection = false;


        public int[] SelectedIcons
        {
            get
            {
                IList items = uiIcons.Items;
                IList selectedItems = uiIcons.SelectedItems;
                int[] selectedIndices = new int[selectedItems.Count];

                for (int i = 0; i < selectedIndices.Length; ++i)
                    selectedIndices[i] = items.IndexOf(selectedItems[i]);

                Array.Sort(selectedIndices);
                return selectedIndices;
            }
        }

        public bool MultiSelection
        {
            set
            {
                if (uiIcons.SelectionMode == SelectionMode.Single && value)
                {
                    uiIcons.SelectionMode = SelectionMode.Extended;
                }
                else
                    uiIcons.SelectionMode = value ? SelectionMode.Extended
                                                  : SelectionMode.Single;
            }
        }

        public Int32Rect SelectedArea
        {
            get
            {
                if (croppedElement != null)
                {
                    double scaleX = uiImage.ActualWidth / (uiImage.Source as BitmapSource).PixelWidth;
                    double scaleY = uiImage.ActualHeight / (uiImage.Source as BitmapSource).PixelHeight;
                    double scale = Math.Max(scaleX, scaleY);

                    Rect selectedControlArea = cropping.ClippingRectangle;

                    return new Int32Rect(
                        (int)(selectedControlArea.X / scale),
                        (int)(selectedControlArea.Y / scale),
                        (int)((selectedControlArea.Width - 1) / scale),
                        (int)((selectedControlArea.Height - 1) / scale)
                        );
                }
                else if (uiImage.Source != null)
                {
                    return new Int32Rect(0, 0, (uiImage.Source as BitmapSource).PixelWidth,
                       (uiImage.Source as BitmapSource).PixelHeight);
                }
                else
                    return default(Int32Rect);
            }
        }

        public bool AreaSelection
        {
            get
            {
                return imageAreaSelection;
            }
            set
            {
                imageAreaSelection = value;

                if (!value)
                    RemoveCrop();
            }
        }

        public ImagePanel()
        {
            InitializeComponent();
        }


        public void SetIcons(params ImageSource[] icons)
        {
            while (uiIcons.Items.Count > icons.Length)
                uiIcons.Items.RemoveAt(0);

            while (uiIcons.Items.Count < icons.Length)
                uiIcons.Items.Add(new Image() { Margin = new Thickness(4) });

            for (int i = 0; i < icons.Length; ++i)
                ((Image)uiIcons.Items[i]).Source = icons[i];
        }

        private void AddCrop()
        {
            if (croppedElement != null)
                RemoveCrop();

            if (AreaSelection)
            {
                Rect rcInterior = new Rect(
                uiImage.ActualWidth * 0.2,
                uiImage.ActualHeight * 0.2,
                uiImage.ActualWidth * 0.6,
                uiImage.ActualHeight * 0.6);
                AdornerLayer layer = AdornerLayer.GetAdornerLayer(uiImage);

                cropping = new CroppingAdorner(uiImage, rcInterior);
                cropping.Fill = new SolidColorBrush(Color.FromArgb(128, 0, 0, 0));

                layer.Add(cropping);
                croppedElement = uiImage;

                cropping.MouseMove += Cropping_MouseMove;
                SelectedAreaChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private void Cropping_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                SelectedAreaChanged?.Invoke(this, e);
        }

        private void RemoveCrop()
        {
            if (croppedElement != null)
            {
                AdornerLayer layer = AdornerLayer.GetAdornerLayer(croppedElement);
                layer.Remove(cropping);
                croppedElement = null;

                cropping.MouseMove -= Cropping_MouseMove;
                SelectedAreaChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public void SetImage(ImageSource image)
        {
            uiImage.Source = image;
        }

        public void SetImageInfo(string text)
        {
            throw new NotImplementedException();
        }

        private void Icons_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source == uiIcons)
                SelectedIconsChanged?.Invoke(this, e);
        }

        private void Image_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            ImageSource source = uiImage.Source;

            if (croppedElement != null)
                RemoveCrop();
            else
                AddCrop();

            uiImage.Source = source;
        }
    }
}
