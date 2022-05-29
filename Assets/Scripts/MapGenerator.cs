using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Scripts {
    public class MapGenerator : MonoBehaviour {
        public Tilemap tilemap;
        public MapTile grassTile, waterTile, mountainTile;

        public Vector2Int size;
        public Vector2 offset;
        [Range(.01f, 1)]
        public float waterLevel;
        [Range(0, 10000)]
        public int generations;
        public float waitTime;

        int[,] tiles;
        int width, height;

        public void Refresh() {
            tiles = new int[size.x,size.y];
            width = size.x / 2;
            height = size.y / 2;

            Initialize();
        }

        public void Generate() {
            StartCoroutine(MakeContinents());
        }

        void Initialize() {
            for (int i = 0; i < size.x; i++) {
                for (int j = 0; j < size.y; j++) {
                    var altitude = Random.value;
                    // tiles[i,j] = altitude <= waterLevel ? 0 : 1;
                    tiles[i,j] = Random.Range(0,3);
                }
            }
            AssignTiles();
        }

        IEnumerator MakeContinents() {
            for (int generation = 0; generation < generations; generation++) {
                for (int i = 0; i < size.x; i++) {
                    for (int j = 0; j < size.y; j++) {
                        var neighbors = SumNeighbors(i,j);
                        // if (tiles[i,j] == 0 && neighbors == 3) {
                        //     tiles[i,j] = 1;
                        // } else if (tiles[i,j] == 1) {
                        //     if (neighbors < 2 || neighbors > 3) {
                        //         tiles[i,j] = 0;
                        //     }
                        // }
                        
                    }
                }
                AssignTiles();

                yield return new WaitForSeconds(waitTime);
            }
        }

        void AssignTiles() {
            for (int i = 0; i < size.x; i++) {
                for (int j = 0; j < size.y; j++) {
                    MapTile tile;
                    if (tiles[i,j] == 0) {
                        tile = waterTile;
                    } else if (tiles[i,j] == 1) {
                        tile = grassTile;
                    } else {
                        tile = mountainTile;
                    }
                    tilemap.SetTile(new Vector3Int(i-width, j-height), tile);
                }
            }
        }

        int SumNeighbors(int i, int j) {
            var im = Mathf.Max(i-1, 0);
            var jm = Mathf.Max(j-1, 0);
            var ip = Mathf.Min(i+1, size.x-1);
            var jp = Mathf.Min(j+1, size.y-1);
            
        }

        // int SumNeighbors(int i, int j) {
        //     var im = Mathf.Max(i-1, 0);
        //     var jm = Mathf.Max(j-1, 0);
        //     var ip = Mathf.Min(i+1, size.x-1);
        //     var jp = Mathf.Min(j+1, size.y-1);
        //     return tiles[im, j] +
        //         tiles[i, jp] +
        //         tiles[i, jm] +
        //         tiles[ip, jp] +
        //         tiles[ip, j] +
        //         tiles[ip, jm];
        // }
    }
}
