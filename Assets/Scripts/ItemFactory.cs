using UnityEngine;

namespace Core
{
    public static class ItemFactory
    {
        private static GameObject _itemBasePrefab;

        public static Item CreateItem(ItemType itemType, Transform parent)
        {
            if (_itemBasePrefab == null)
            {
                _itemBasePrefab = Resources.Load("ItemBase") as GameObject;
            }

            var go = Object.Instantiate(_itemBasePrefab, parent);

            switch (itemType)
            {
                case ItemType.GreenCube:
                case ItemType.YellowCube:
                case ItemType.BlueCube:
                case ItemType.RedCube:
                {
                    var cube = go.AddComponent<CubeItem>();
                    cube.PrepareCubeItem(itemType);
                    return cube;
                }

                case ItemType.Bomb:
                {
                    var bomb = go.AddComponent<BombItem>();
                    return bomb;
                }

                case ItemType.RocketH:
                case ItemType.RocketV:
                {
                    var rocket = go.AddComponent<RocketItem>();
                    bool isHorizontal = itemType == ItemType.RocketH;
                    rocket.PrepareRocket(isHorizontal);
                    return rocket;
                }

                default:
                    Object.Destroy(go);
                    return null;
            }
        }
    }

    public enum ItemType
    {
        None = 0,
        GreenCube = 1,
        YellowCube = 2,
        BlueCube = 3,
        RedCube = 4,
        Bomb = 5,
        RocketH = 6,
        RocketV = 7
    }

    public enum MatchType
    {
        None = 0,
        Green = 1,
        Yellow = 2,
        Blue = 3,
        Red = 4,
        Special = 5
    }
}