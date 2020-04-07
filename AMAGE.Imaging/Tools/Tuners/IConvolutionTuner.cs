namespace AMAGE.Imaging.Tools.Tuners
{
    public interface IConvolutionTuner : ITuner
    {
        int[,] Kernel { get; }
        int KernelFactorSum { get; }
        int KernelOffsetSum { get; }
    }
}
