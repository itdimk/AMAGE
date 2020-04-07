using System;

namespace AMAGE.Imaging.Tools
{
    public interface IAsyncApplyingSupport
    {
        event EventHandler AsyncApplyingCompleted;

        bool ApplyAsync { get; set; }
        bool AllowMultipleTasks { get; set; }
    }
}
