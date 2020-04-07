using System;
using System.Collections.Generic;

namespace AMAGE.Presentation.View.ImageEditor
{
    public interface IPanelGroup<TPanel> : IEnumerable<TPanel>
    {
        event EventHandler SelectedPanelChanged;

        TPanel SelectedPanel { get; }
        string SelectedPanelKey { get; }

        TPanel this[string key] { get; }
        string this[TPanel panel] { get; }

        TPanel AddPanel(string key, string title);
        void RemovePanel(string key);
    }
}
