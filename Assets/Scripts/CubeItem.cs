namespace Core
{
    public class CubeItem : Item
    {
        private MatchType _matchType;
        private ItemType _itemType;

        public void PrepareCubeItem(ItemType itemType)
        {
            _itemType = itemType;

            switch (itemType)
            {
                case ItemType.GreenCube:
                    _matchType = MatchType.Green;
                    break;
                case ItemType.YellowCube:
                    _matchType = MatchType.Yellow;
                    break;
                case ItemType.BlueCube:
                    _matchType = MatchType.Blue;
                    break;
                case ItemType.RedCube:
                    _matchType = MatchType.Red;
                    break;
            }
        }

        public override MatchType GetMatchType()
        {
            return _matchType;
        }

        public override ItemType GetItemType()
        {
            return _itemType;
        }

        public override bool CanBeMatchedByTouch()
        {
            return true;
        }

        public override bool CanBeExplodedByTouch()
        {
            return true;
        }

        public override bool CanBeExplodedByNeighbourMatch()
        {
            return true;
        }

        public override bool TryExecute(MatchType matchType)
        {
            return base.TryExecute(matchType);
        }
    }
}