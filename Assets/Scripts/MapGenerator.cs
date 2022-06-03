using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Scripts {
    public class MapGenerator : MonoBehaviour {
        public Tilemap tilemap;
        public MapTile blankTile, grassTile, waterTile;

        public Vector2Int size;
        [Range(0, 100)]
        public int makeGenerations;
        [Range(0,1f)]
        public float waitTime, waterLevel;
        public NoiseLayer[] noiseLayers;

        TileInfo[,] readTiles, writeTiles;
        int width, height;

        void Start() {
            Clear();
        }

        public void Clear() {
            writeTiles = new TileInfo[size.x,size.y];
            width = size.x / 2;
            height = size.y / 2;

            StartCoroutine(NoiseAltitude());
            StartCoroutine(RefreshTiles());
        }

        public void Generate() {
            StartCoroutine(GenerateCoroutine());
        }

        IEnumerator GenerateCoroutine() {
            yield return StartCoroutine(MakeContinents());
            yield return new WaitForSeconds(1);
            yield return StartCoroutine(MakePoles());
        }

        IEnumerator NoiseAltitude() {
            for (int i = 0; i < size.x; i++) {
                for (int j = 0; j < size.y; j++) {
                    var fi = i / (float)size.x;
                    var fj = j / (float)size.y;
                    float x = 0, y = 0;
                    foreach (var layer in noiseLayers) {
                        x += (fi * layer.frequency) + layer.offset.x;
                        y += (fj * layer.frequency) + layer.offset.y;
                    }
                    writeTiles[i,j].altitude = Mathf.PerlinNoise(x, y);
                    writeTiles[i,j].type = writeTiles[i,j].altitude <= waterLevel ? 0 : 1;
                }
            }
            yield return RefreshTiles();
        }

        IEnumerator MakeContinents() {
            var tiles = readTiles.Cast<TileInfo>().Where(t => t.type == 1);
            var avg = tiles.Sum(t => t.altitude) / (float)tiles.Count();
            for (int generation = 0; generation < makeGenerations; generation++) {
                for (int i = 0; i < size.x; i++) {
                    for (int j = 0; j < size.y; j++) {
                        if (readTiles[i,j].altitude > waterLevel && readTiles[i,j].altitude < avg) {
                            var sum = SumNeighbors(i,j);
                            var landBridge = sum >= 2 && sum <= 4;
                            var island = sum <= 1;
                            if ((landBridge || island) && Random.value < .5f) {
                                writeTiles[i,j].altitude = waterLevel;
                                writeTiles[i,j].type = 0;
                            }
                        }
                    }
                }
                yield return RefreshTiles();
            }
        }

        IEnumerator MakePoles() {
            var tiles = readTiles.Cast<TileInfo>().Where(t => t.type == 1);

            yield return RefreshTiles();
        }

        void DrawTiles() {
            for (int i = 0; i < size.x; i++) {
                for (int j = 0; j < size.y; j++) {
                    // TODO: Color should be determined in a method in the TileInfo class derived from other props
                    Color color;
                    if (readTiles[i,j].type == 0) {
                        color = Color.Lerp(Color.blue, Color.cyan, readTiles[i,j].altitude);
                    } else {
                        color = Color.Lerp(new Color(0, .5f, 0), new Color(0, 1, 0), readTiles[i,j].altitude);
                    }
                    tilemap.SetColor(new Vector3Int(i-width, j-height), color);
                    tilemap.SetTile(new Vector3Int(i-width, j-height), blankTile);
                }
            }
        }

        IEnumerator RefreshTiles() {
            readTiles = (TileInfo[,])writeTiles.Clone();
            DrawTiles();
            yield return new WaitForSeconds(waitTime);
        }

        Vector2Int[] GetNeighbors(int i, int j) {
            // TODO: This likely doesn't work right on the edges. Instead just don't include if past the limits.
            var im = Mathf.Max(i-1, 0);
            var jm = Mathf.Max(j-1, 0);
            var ip = Mathf.Min(i+1, size.x-1);
            var jp = Mathf.Min(j+1, size.y-1);
            return new Vector2Int[] {
                new Vector2Int(im, j),
                new Vector2Int(i, jp),
                new Vector2Int(i, jm),
                new Vector2Int(ip, jp),
                new Vector2Int(ip, j),
                new Vector2Int(ip, jm),
            };
        }

        int SumNeighbors(int i, int j) {
            var neighbors = GetNeighbors(i, j);
            int sum = 0;
            foreach (var neighbor in neighbors) {
                sum += readTiles[neighbor.x, neighbor.y].type;
            }
            return sum;
        }
    }
}
