namespace Core
{
    public class LevelData_0 : LevelData
    {
        public override ItemType GetNextFillItemType()
        {
            return GetRandomCubeItemType();
        }

        public override void Initialize()
        {
            GridData = new ItemType[Board.Rows, Board.Cols];

            for (int y = 0; y < Board.Rows; y++)
            for (int x = 0; x < Board.Cols; x++)
                GridData[x, y] = GetRandomCubeItemType();

            FillingColumns = new bool[Board.Cols];
            for (int x = 0; x < Board.Cols; x++) FillingColumns[x] = true;
        }
    }
}