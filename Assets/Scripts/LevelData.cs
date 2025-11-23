
namespace Core
{
    /// <summary>
    /// LevelData now uses RandomService for deterministic, seed-based random,
    /// and tanımlar üzerinden hangi kolonlarda üstten cube spawn edileceğini belirler.
    /// </summary>
    public abstract class LevelData
    {		
        public abstract ItemType GetNextFillItemType();
        public abstract void Initialize();

        // Board üzerindeki başlangıç item tipleri
        public ItemType[,] GridData { get; protected set; }

        // Hangi kolonların en üst hücresi filling cell (spawner) olacak
        public bool[] FillingColumns { get; protected set; }

        private static readonly ItemType[] DefaultCubeArray = new[]
        {
            ItemType.GreenCube,
            ItemType.YellowCube,
            ItemType.BlueCube,
            ItemType.RedCube
        };

        protected static ItemType GetRandomCubeItemType()
        {
            return GetRandomItemTypeFromArray(DefaultCubeArray);
        }

        protected static ItemType GetRandomItemTypeFromArray(ItemType[] itemTypeArray)
        {
            return itemTypeArray[RandomService.Range(0, itemTypeArray.Length)];
        }
    }
}