using AMAGE.Presentation.View.ImageEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Collections;

namespace AMAGE.UI.WPF.ImageEditor
{
    public partial class ToolPanelGroup : UserControl, IPanelGroup<IToolPanel>
    {
        private readonly Dictionary<string, IToolPanel> panels
            = new Dictionary<string, IToolPanel>();

        public event EventHandler SelectedPanelChanged;

        public string this[IToolPanel panel] => panels.First(i => i.Value == panel).Key;
        public IToolPanel this[string key] => panels[key];

        public IToolPanel SelectedPanel => panels.Values.ElementAtOrDefault(uiTabs.SelectedIndex);
        public string SelectedPanelKey => panels.Keys.ElementAtOrDefault(uiTabs.SelectedIndex);

        public ToolPanelGroup()
        {
            InitializeComponent();
        }

        public IToolPanel AddPanel(string key, string title)
        {
            ToolPanel toolPanel = new ToolPanel();
            panels.Add(key, toolPanel);

            TabItem tab = new TabItem()
            {
                Header = title,
                Content = toolPanel
            };

            uiTabs.Items.Add(tab);
            return toolPanel;
        }

        public void RemovePanel(string key)
        {
            uiTabs.Items.Remove(panels[key]);
            panels.Remove(key);
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source == uiTabs)
                SelectedPanelChanged?.Invoke(this, e);
        }

        public IEnumerator<IToolPanel> GetEnumerator()
        {
            return panels.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
