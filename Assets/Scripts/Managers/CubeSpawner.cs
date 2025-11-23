using UnityEngine;

namespace Core
{

    public class CubeSpawner : MonoBehaviour, IProvidable
    {
        [Header("Defaults")]
        public Transform DefaultParent;

        private void Awake()
        {
            ServiceProvider.Register(this);
        }

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
    }
}