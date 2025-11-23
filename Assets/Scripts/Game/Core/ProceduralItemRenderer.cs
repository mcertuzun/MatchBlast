using UnityEngine;

namespace Core
{
    public class ProceduralItemRenderer : MonoBehaviour
    {
        public Board Board;
        public Mesh Mesh;
        public Material Material;

        private readonly Matrix4x4[] _matrices = new Matrix4x4[1023];
        private readonly Vector4[] _colors = new Vector4[1023];
        private MaterialPropertyBlock _mpb;
        private static readonly int ColorId = Shader.PropertyToID("_BaseColor");

        private void Awake()
        {
            _mpb = new MaterialPropertyBlock();

            if (Mesh == null)
            {
                Mesh = new Mesh();
                Mesh.vertices = new Vector3[]
                {
                    new Vector3(-0.5f, -0.5f, 0f),
                    new Vector3( 0.5f, -0.5f, 0f),
                    new Vector3(-0.5f,  0.5f, 0f),
                    new Vector3( 0.5f,  0.5f, 0f)
                };
                Mesh.uv = new Vector2[]
                {
                    new Vector2(0f, 0f),
                    new Vector2(1f, 0f),
                    new Vector2(0f, 1f),
                    new Vector2(1f, 1f)
                };
                Mesh.triangles = new int[] { 0, 2, 1, 2, 3, 1 };
                Mesh.RecalculateNormals();
            }
        }

        private void LateUpdate()
        {
            if (Board == null || Mesh == null || Material == null)
                return;

            var config = ServiceProvider.GetGameConfig;
            int rows = Board.boardData.rowCount;
            int cols = Board.boardData.columnCount;

            int count = 0;

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    var cell = Board.Cells[x, y];
                    if (cell == null || !cell.HasItem())
                        continue;

                    var item = cell.Item;
                    if (item == null)
                        continue;

                    // position tam olarak cell pozisyonu ile hizalansin
                    Vector3 pos = cell.transform.position;
                    // Düşüş animasyonunda item transform'u y ekseninde oynatıyor, onu alalım
                    var itemPos = item.transform.position;
                    pos.y = itemPos.y;
                    pos.z = itemPos.z;

                    _matrices[count] = Matrix4x4.TRS(pos, Quaternion.identity, Vector3.one);
                    _colors[count] = GetColorForItem(item, config);

                    count++;
                    if (count == 1023)
                    {
                        FlushBatch(count);
                        count = 0;
                    }
                }
            }

            if (count > 0)
            {
                FlushBatch(count);
            }
        }

        private void FlushBatch(int count)
        {
            _mpb.Clear();
            _mpb.SetVectorArray(ColorId, _colors);
            Graphics.DrawMeshInstanced(Mesh, 0, Material, _matrices, count, _mpb);
        }

        private static Vector4 GetColorForItem(Item item, GameConfig config)
        {
            if (config == null)
                return Color.white;

            switch (item.GetItemType())
            {
                case ItemType.GreenCube: return config.GreenColor;
                case ItemType.YellowCube: return config.YellowColor;
                case ItemType.BlueCube: return config.BlueColor;
                case ItemType.RedCube: return config.RedColor;
                case ItemType.Bomb: return config.BombColor;
                case ItemType.RocketH:
                case ItemType.RocketV: return config.RocketColor;
                default: return Color.white;
            }
        }
    }
}
