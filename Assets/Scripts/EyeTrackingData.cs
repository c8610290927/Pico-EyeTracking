using DataSync;

namespace Data
{
    public class EyeTrackingData : LabDataBase
    {
        
    }

    public class EyePositionData : LabDataBase
    {
        public float timeStep;
        public float positionX;
        public float positionY;
        public float positionZ;
        public int leftEyeOpenness;
        public int rightEyeOpenness;
    }

    public class EyeFeatureData : LabDataBase
    {
        public float distance;
        public float pgameTime;
        public float speed;
        public int winkTimesL;
        public int winkTimesR;
    }

    /*public enum GameModeEnum
    {
        Checkboard = 0,
        Pure = 1
    }*/
}