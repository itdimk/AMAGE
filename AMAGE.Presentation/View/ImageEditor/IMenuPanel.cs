using System;

namespace AMAGE.Presentation.View.ImageEditor
{
    public interface IMenuPanel : IEnabledBySubscrSwitcher
    {
        event EventHandler OpenFile;
        event EventHandler SaveFile;
        event EventHandler CloseFile;

        event EventHandler ApplyTool;
        event EventHandler PlayAnimation;
        event EventHandler StopAnimation;
    }
}
