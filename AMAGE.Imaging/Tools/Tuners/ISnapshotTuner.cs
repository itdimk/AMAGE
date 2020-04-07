using System.Collections.Generic;

namespace AMAGE.Imaging.Tools.Tuners
{
    public interface ISnapshotTuner : ITuner
    {
        IReadOnlyList<T> GetSnapshots<T>(string snapshotPropertyName);
        void SetSnapshotCount(int minSnapshots, int maxSnapshots);
        void SetSnapshotProperties(params string[] properties);
    }
}
