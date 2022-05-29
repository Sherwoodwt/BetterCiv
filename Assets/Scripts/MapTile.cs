using UnityEngine;
using UnityEngine.Tilemaps;

namespace Scripts {
    [CreateAssetMenu]
    public class MapTile : TileBase {
        public Sprite sprite;

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        {
            tileData.sprite = sprite;
        }
    }
}
