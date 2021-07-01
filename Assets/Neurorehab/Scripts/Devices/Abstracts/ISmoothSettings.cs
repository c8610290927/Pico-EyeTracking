namespace Neurorehab.Scripts.Devices.Abstracts
{
    /// <summary>
    /// Abstraction for the smoothing settings.
    /// </summary>
    public interface ISmoothSettings
    {
        /// <summary>
        /// Number of samples in the smoothing algorithm.
        /// </summary>
        int NumberOfSamples { get; set; }
    }
}