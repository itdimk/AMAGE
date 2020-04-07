using System.Collections.Generic;

namespace AMAGE.Imaging.Tools
{
    public interface ICustomFramesSupport
    {
        IReadOnlyList<int> CustomFrames { set; }
    }
}
