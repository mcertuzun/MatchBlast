
namespace Core
{
    public static class LevelDataFactory
    {
        public static LevelData CreateLevelData(LevelName levelName)
        {
            LevelData levelData = null;
            switch (levelName)
            {
                case LevelName.Level0:
                    levelData = new LevelData_0();
                    break;
            }

            levelData.Initialize();
            return levelData;
        }
    }
}