namespace Core
{
    public static class LevelDataFactory
    {
        public enum LevelName
        {
            Level0,
            Level1,
            Level2,
        }
        public static LevelData CreateLevelData(LevelName levelName)
        {
            LevelData levelData = new LevelData_0();
            levelData.Initialize();
            return levelData;
        }
    }
}