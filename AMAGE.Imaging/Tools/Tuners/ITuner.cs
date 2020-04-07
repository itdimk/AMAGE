using System;

namespace AMAGE.Imaging.Tools.Tuners
{
    public interface ITuner
    {
        event EventHandler Tuning;

        object TunableTool { get; set; }
        void RefreshTunableTool();
    }
}
