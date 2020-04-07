using AMAGE.Imaging.Tools.Tuners;
using System;

namespace AMAGE.Presentation.View.ImageEditor
{
    public interface IToolPanel
    {
        event EventHandler Tuning;
        event EventHandler SelectedToolChanged;

        object SelectedTool { get; }
        void SetTools(params object[] tools);
        void SetTuner(ITuner tuner);
    }
}
