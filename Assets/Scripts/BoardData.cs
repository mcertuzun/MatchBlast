using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "BoardData", menuName = "BoardData", order = 0)]
    public class BoardData : ScriptableObject
    {
        public int rowCount;
        public int columnCount;
        public Cell cellPrefab;
    }
}