using Core;
using UnityEngine;

namespace Core
{
    /// <summary>
    /// Basit bir cube spawner:
    /// - RandomService'e bağlı deterministik random cube üretir.
    /// - İsteğe göre LevelData üzerinden de item tipini alabilir.
    /// </summary>
    public class CubeSpawner : MonoBehaviour, IProvidable
    {
        [Header("Defaults")]
        public Transform DefaultParent;

        private void Awake()
        {
            ServiceProvider.Register(this);
        }

        /// <summary>
        /// Verilen item tipi setinden rastgele cube üretir.
        /// </summary>
        public Item SpawnRandomCube(ItemType[] itemTypes, Transform parentOverride = null)
        {
            if (itemTypes == null || itemTypes.Length == 0)
            {
                itemTypes = new[]
                {
                    ItemType.GreenCube,
                    ItemType.YellowCube,
                    ItemType.BlueCube,
                    ItemType.RedCube
                };
            }

            int index = Core.RandomService.Range(0, itemTypes.Length);
            var type = itemTypes[index];
            var parent = parentOverride != null ? parentOverride : (DefaultParent != null ? DefaultParent : transform);

            return ItemFactory.CreateItem(type, parent);
        }

        /// <summary>
        /// LevelData'nın GetNextFillItemType metodunu kullanarak cube üretir.
        /// </summary>
        public Item SpawnFromLevelData(LevelData levelData, Transform parentOverride)
        {
            if (levelData == null)
            {
                return null;
            }

            var itemType = levelData.GetNextFillItemType();
            var parent = parentOverride != null ? parentOverride : (DefaultParent != null ? DefaultParent : transform);

            return ItemFactory.CreateItem(itemType, parent);
        }

        /// <summary>
        /// Verilen cell için LevelData'ya göre item üretir ve cell'e yerleştirir.
        /// Görsel pozisyon cell pozisyonuna set edilir.
        /// </summary>
        public Item SpawnAtCell(Cell cell, LevelData levelData, Transform parentOverride)
        {
            if (cell == null)
            {
                return null;
            }

            var item = SpawnFromLevelData(levelData, parentOverride);
            if (item == null)
            {
                return null;
            }

            cell.Item = item;
            item.transform.position = cell.transform.position;

            return item;
        }
    }
}