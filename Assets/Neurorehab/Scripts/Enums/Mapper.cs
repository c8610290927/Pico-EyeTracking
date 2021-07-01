namespace Neurorehab.Scripts.Enums
{
    /// <summary>
    /// Labels for mapping position and rotation
    /// </summary>
    public enum AxisLabels
    {
        @Bool,
        Value,
        X,
        Y,
        Z
    }

    /// <summary>
    /// Indicates which value is being calibrated at the moment
    /// </summary>
    public enum ValueBeingCalibrated
    {
        Max,
        Min,
        Center
    }

    /// <summary>
    /// Type of value being calibrated
    /// </summary>
    public enum CalibrationType
    {
        Rotation,
        Position,
        Value
    }

    /// <summary>
    /// Defines the Calibration Mode for a specific component.
    /// </summary>
    public enum CalibrationMode
    {
        /// <summary>
        /// Additive Mode changes the positions, adding a maximum and minimum value for the position.
        /// </summary>
        Additive,
        /// <summary>
        /// Absolute Mode do a 1 to 1 mapping of the position, imitating the movement of the input inside the game, only rescaling the position to match the game world scale.
        /// </summary>
        Absolute,
        /// <summary>
        /// No calibration is performed in this mode.
        /// </summary>
        Direct
    }


    /// <summary>
    /// Labels to be used for the remapping calibration
    /// </summary>
    public enum SingleInputMappingLabels
    {
        None,
        PosX,
        PosY,
        PosZ,
        RotX,
        RotY,
        RotZ,
        Value,
        @Bool,
        Sample
    }
}