using AMAGE.Common.Imaging;

namespace AMAGE.Imaging.Tools.Tuners
{
    public interface IExternalOverlayTuner : ITuner
    {
        IImage Before { get; set; }
        IImage After { get; }
    }
}
