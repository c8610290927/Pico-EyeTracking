using DataSync;

namespace GameData
{
    public class GobalData : LabDataBase
    {     

        /// <summary>
        /// 游戏的主UI场景名
        /// </summary>
        public const string MainUiScene = "MainUI";
        public const string MainScene = "MainScene";
        
        public const int GameUIManagerWeight = 0;
        public const int GameEntityManagerWeight = 30;
        public const int GameSceneManagerWeight = 50;
        public const int GameTaskManagerWeight = 80;
        public const int GameDataManagerWeight = 100;


    }

}

