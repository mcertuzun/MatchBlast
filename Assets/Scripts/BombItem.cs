namespace Core
{
    public class BombItem : Item
    {
        private MatchType _matchType = MatchType.Special;

        public override MatchType GetMatchType()
        {
            return _matchType;
        }

        public override ItemType GetItemType()
        {
            return ItemType.Bomb;
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
    }
}