using AMAGE.Imaging.Tools.Tuners;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using Xceed.Wpf.Toolkit.PropertyGrid;

namespace AMAGE.UI.WPF.Tuners
{
    public partial class Rotation3DTuner : UserControl, IRotation3DTuner
    {
        private readonly List<Matrix3D> snapshots = new List<Matrix3D>();

        private int minSnapshots;
        private int maxSnapshots;

        public event EventHandler Tuning;

        public object TunableTool
        {
            get { return uiProperties.SelectedObject; }
            set { uiProperties.SelectedObject = value; }
        }

        public double Depth
        {
            get { return Math.Abs(uiGeometry.Positions[0].Z * 2); }
            set
            {
                if (value <= 0.01)
                    value = 0.01;

                for (int i = 0; i < uiGeometry.Positions.Count; ++i)
                {
                    Point3D point = uiGeometry.Positions[i];
                    point.Z = Math.Sign(point.Z) * value * 0.5;
                    uiGeometry.Positions[i] = point;
                }
            }
        }

        Color IRotation3DTuner.Background
        {
            get { return uiBackground.Color; }
            set { uiBackground.Color = value; }
        }

        public BitmapSource TargetImage
        {
            set
            {
                
                uiImageBrush.ImageSource = value;

                if (value != null)
                {
                    double aspectRatio = (double)value.PixelWidth / value.PixelHeight;



                    /*
                    uiImagePlane.Positions = new Point3DCollection()
                    {
                        new Point3D(-1 * aspectRatio, -1, 0),
                        new Point3D(-1 * aspectRatio, 1, 0),
                        new Point3D(1 * aspectRatio, 1, 0),
                        new Point3D(1 * aspectRatio, -1, 0)
                    };
                    */
                }
            }
        }

        public Rotation3DTuner()
        {
            InitializeComponent();

            UpdateButtonsClickable();

        }

        private void Snapshots_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selectedIndex = uiSnapshots.SelectedIndex;

            if (selectedIndex >= 0)
                uiCamera.Transform = new MatrixTransform3D(snapshots[selectedIndex]);

            UpdateButtonsClickable();
        }

        private void Properties_PropertyValueChanged(object sender, PropertyValueChangedEventArgs e)
        {
            Tuning?.Invoke(this, e);
        }

        private void AddSnapshot_Click(object sender, RoutedEventArgs e)
        {
            if (snapshots.Count < maxSnapshots)
            {
                snapshots.Add(uiCamera.Transform.Value);
                uiSnapshots.Items.Add(DateTime.Now.ToLongTimeString());

                UpdateButtonsClickable();
                uiSnapshots.SelectedIndex = uiSnapshots.Items.Count - 1;
            }
        }

        public BitmapSource Render(BitmapSource image, Matrix3D snapshot)
        {
            uiImageBrush.ImageSource = image;
            Transform3D originalTransform = uiCamera.Transform;
            uiCamera.Transform = new MatrixTransform3D(snapshot);

            double scale = Math.Max(image.PixelWidth, image.PixelHeight) / Math.Max(uiViewport.ActualWidth,
                uiViewport.ActualHeight);

            RenderTargetBitmap bitmap = new RenderTargetBitmap(
                (int)(scale * (uiViewport.ActualWidth)),
                (int)(scale * (uiViewport.ActualHeight)),
                scale * 96,
                scale * 96,
                PixelFormats.Pbgra32);

            bitmap.Render(uiViewport);
            uiCamera.Transform = originalTransform;

            return bitmap;
        }

        public IReadOnlyList<Matrix3D> GetSnapshots()
        {
            return snapshots;
        }

        public void SetSnapshotCount(int minSnapshots, int maxSnapshots)
        {
            this.minSnapshots = minSnapshots;
            this.maxSnapshots = maxSnapshots;

            UpdateButtonsClickable();
        }

        private void UpdateButtonsClickable()
        {
            uiRemove.IsEnabled = uiSnapshots.SelectedIndex >= 0 && uiSnapshots.Items.Count > 1;
            uiAdd.IsEnabled = uiSnapshots.Items.Count < maxSnapshots;
        }

        public void RefreshTunableTool()
        {
            object tunableTool = TunableTool;
            TunableTool = null;
            TunableTool = tunableTool;
        }

        private void RemoveSnapshot_Click(object sender, RoutedEventArgs e)
        {
            int index = uiSnapshots.SelectedIndex;

            if (index >= 0)
            {
                uiSnapshots.Items.RemoveAt(index);
                snapshots.RemoveAt(index);
                uiSnapshots.SelectedIndex = Math.Min(index, uiSnapshots.Items.Count - 1);
            }

            UpdateButtonsClickable();
        }

        private void Viewport_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (uiSnapshots.Items.Count == 0)
            {
                AddSnapshot_Click(this, e);
                uiSnapshots.SelectedIndex = 0;
            }

            int index = uiSnapshots.SelectedIndex;

            if (index >= 0)
            {
                snapshots[index] = uiCamera.Transform.Value;
                uiSnapshots.Items[index] = DateTime.Now.ToLongTimeString();
                uiSnapshots.SelectedIndex = index;
            }

            Tuning?.Invoke(this, e);
        }
    }
}

