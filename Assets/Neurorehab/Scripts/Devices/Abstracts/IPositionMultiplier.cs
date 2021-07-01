namespace Neurorehab.Scripts.Devices.Abstracts
{
    /// <summary>
    /// Abstraction for the position multiplier algorithms
    /// </summary>
    public interface IPositionMultiplier
    {
        /// <summary>
        /// The value that the position received via UDP must be multiplayed by.
        /// </summary>
        float PositionMultiplier { get; set; }
    }
}