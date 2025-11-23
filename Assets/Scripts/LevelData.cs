
namespace Core
{
    public abstract class LevelData
    {		
        public abstract ItemType GetNextFillItemType();
        public abstract void Initialize();

        public ItemType[,] GridData { get; protected set; }
        public bool[] FillingColumns { get; protected set; }

        private static readonly ItemType[] DefaultCubeArray = {
            ItemType.GreenCube,
            ItemType.YellowCube,
            ItemType.BlueCube,
            ItemType.RedCube
        };

        protected static ItemType GetRandomCubeItemType()
        {
            return GetRandomItemTypeFromArray(DefaultCubeArray);
        }

        private static ItemType GetRandomItemTypeFromArray(ItemType[] itemTypeArray)
        {
            return itemTypeArray[RandomService.Range(0, itemTypeArray.Length)];
        }
    }
}