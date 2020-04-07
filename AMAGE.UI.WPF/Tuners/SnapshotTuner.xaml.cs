using AMAGE.Imaging.Tools.Tuners;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Xceed.Wpf.Toolkit.PropertyGrid;

namespace AMAGE.UI.WPF.Tuners
{
    public partial class SnapshotTuner : UserControl, ISnapshotTuner
    {
        private readonly Dictionary<string, List<object>> snapshots
            = new Dictionary<string, List<object>>();

        public event EventHandler Tuning;

        private int minSnapshots = 1;
        private int maxSnapshots = 1;

        public object TunableTool
        {
            get { return uiPropertyGrid.SelectedObject; }
            set { uiPropertyGrid.SelectedObject = value; }
        }

        public SnapshotTuner()
        {
            InitializeComponent();
        }

        public IReadOnlyList<T> GetSnapshots<T>(string snapshotPropertyName)
        {
            return snapshots[snapshotPropertyName].Cast<T>().ToArray();
        }

        public void SetSnapshotCount(int minSnapshots, int maxSnapshots)
        {
            if (minSnapshots >= 1 && maxSnapshots >= minSnapshots)
            {
                this.minSnapshots = minSnapshots;
                this.maxSnapshots = maxSnapshots;

                UpdateButtonsClickable();
            }
            else
                throw new Exception($"{nameof(minSnapshots)} and {nameof(maxSnapshots)} must be bigger then 0");
        }

        public void SetSnapshotProperties(params string[] properties)
        {
            foreach (string propertyName in properties)
                snapshots.Add(propertyName, new List<object>());
        }

        private void AddSnapshot_Click(object sender, RoutedEventArgs e)
        {
            if (uiSnapshots.Items.Count >= maxSnapshots)
                return;

            PropertyInfo[] selectedObjProperties = TunableTool.GetType().GetProperties()
                .Where(p => snapshots.Count == 0 || snapshots.ContainsKey(p.Name))
                .ToArray();

            StringBuilder title = new StringBuilder();

            foreach (PropertyInfo property in selectedObjProperties)
            {
                string name = property.Name;
                object value = property.GetValue(TunableTool);

                if (!snapshots.ContainsKey(name))
                    snapshots.Add(name, new List<object>());

                snapshots[name].Add(value);

                title.Append(property.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? name);
                title.Append(" : ");
                title.Append(value);
                title.Append("\r\n");
            }

            uiSnapshots.Items.Add(new TextBlock() { Text = title.ToString().TrimEnd('\r', '\n') });
            uiSnapshots.SelectedIndex = uiSnapshots.Items.Count - 1;
            UpdateButtonsClickable();
            Tuning?.Invoke(this, e);
        }

        private void ChangeSnapshot(int index)
        {
            PropertyInfo[] selectedObjProperties = TunableTool.GetType().GetProperties()
                .Where(p => snapshots.Count == 0 || snapshots.ContainsKey(p.Name))
                .ToArray();

            StringBuilder title = new StringBuilder();

            foreach (PropertyInfo property in selectedObjProperties)
            {
                string name = property.Name;
                object value = property.GetValue(TunableTool);

                snapshots[name][index] = value;

                title.Append(property.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? name);
                title.Append(" : ");
                title.Append(value);
                title.Append("\r\n");
            }

            ((TextBlock)uiSnapshots.Items[index]).Text = title.ToString().TrimEnd('\r', '\n');
            Tuning?.Invoke(this, EventArgs.Empty);
        }

        private void RemoveSnapshot_Click(object sender, RoutedEventArgs e)
        {
            int index = uiSnapshots.SelectedIndex;

            foreach (var item in snapshots)
                item.Value.RemoveAt(index);

            uiSnapshots.Items.RemoveAt(index);

            uiSnapshots.SelectedIndex = Math.Min(index, uiSnapshots.Items.Count - 1);
            Tuning?.Invoke(this, EventArgs.Empty);
        }

        private void PropertyGrid_PropertyValueChanged(object sender, PropertyValueChangedEventArgs e)
        {
            uiSnapshots.SelectionChanged -= Snapshots_SelectionChanged;

            if (uiSnapshots.Items.Count == 0)
                AddSnapshot_Click(this, e);

            if (uiSnapshots.SelectedIndex < 0)
                uiSnapshots.SelectedIndex = uiSnapshots.Items.Count - 1;

            int index = uiSnapshots.SelectedIndex;

            ChangeSnapshot(index);
            uiSnapshots.SelectedIndex = index;

            uiSnapshots.SelectionChanged += Snapshots_SelectionChanged;
        }

        private void Snapshots_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (uiSnapshots.SelectedIndex != -1)
            {
                Type selectedObjType = TunableTool.GetType();

                PropertyInfo[] selectedObjProperties = selectedObjType
                    .GetProperties()
                    .Where(p => snapshots.ContainsKey(p.Name))
                    .ToArray();

                foreach (PropertyInfo property in selectedObjProperties)
                {
                    string name = property.Name;
                    property.SetValue(TunableTool, snapshots[name][uiSnapshots.SelectedIndex]);
                }

                object tunableTool = TunableTool;

                uiPropertyGrid.SelectedObject = null;
                uiPropertyGrid.SelectedObject = tunableTool;
            }

            UpdateButtonsClickable();
        }

        private void UpdateButtonsClickable()
        {
            uiAdd.IsEnabled = uiSnapshots.Items.Count < maxSnapshots;
            uiRemove.IsEnabled = uiSnapshots.SelectedIndex >= 0 && uiSnapshots.Items.Count > 1;
        }

        public void RefreshTunableTool()
        {
            object tunableTool = TunableTool;
            TunableTool = null;
            TunableTool = tunableTool;
        }
    }
}
