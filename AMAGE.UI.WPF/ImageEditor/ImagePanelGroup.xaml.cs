using AMAGE.Presentation.View.ImageEditor;
using System.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace AMAGE.UI.WPF.ImageEditor
{
    public partial class ImagePanelGroup : UserControl, IPanelGroup<IImagePanel>
    {
        private readonly Dictionary<string, IImagePanel> panels
           = new Dictionary<string, IImagePanel>();

        public event EventHandler SelectedPanelChanged;

        public string this[IImagePanel panel] => panels.First(i => i.Value == panel).Key;
        public IImagePanel this[string key] => panels[key];

        public IImagePanel SelectedPanel => panels.Values.ElementAtOrDefault(uiTabs.SelectedIndex);
        public string SelectedPanelKey => panels.Keys.ElementAtOrDefault(uiTabs.SelectedIndex);

        public ImagePanelGroup()
        {
            InitializeComponent();
        }

        public IImagePanel AddPanel(string key, string title)
        {
            ImagePanel imagePanel = new ImagePanel();
            panels.Add(key, imagePanel);

            TabItem tab = new TabItem()
            {
                Header = title,
                Content = imagePanel
            };

            uiTabs.Items.Add(tab);
            uiTabs.SelectedItem = tab;
            return imagePanel;
        }

        public void RemovePanel(string key)
        {
            for (int i = 0; i < uiTabs.Items.Count; ++i)
            {
                TabItem tabItem = (TabItem)uiTabs.Items[i];

                if (tabItem.Content == panels[key])
                    uiTabs.Items.RemoveAt(i);
            }
            panels.Remove(key);
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source == uiTabs)
                SelectedPanelChanged?.Invoke(this, e);
        }

        public IEnumerator<IImagePanel> GetEnumerator()
        {
            return panels.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
