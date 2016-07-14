using System;
using System.Collections.Generic;
using BO.Utilities;
using UnityEngine;
using System.Collections;

namespace Grass
{
    [ExecuteInEditMode]
    public class GrassGrid : MonoBehaviour
    {
        public WorldSpaceGridTileSet<GameObject> TileSet { get; private set; }
        public WorldSpaceGrid<GameObject> Grid { get; private set; }

        [SerializeField]
        private Vector3 _size;

        [SerializeField]
        private int _grassPerUnit = 4;

        //private GameObject[] _grassGrid;

        private int _gridSize;

        void OnEnable()
        {
            UpdateSettings();
        }

        void OnDisable()
        {
            var gridItems = Grid.GetItemsRaw();
            for (var i = 0; i < gridItems.Length; i++)
            {
                DestroyImmediate(gridItems[i]);
            }
        }

        [ContextMenu("Update settings")]
        private void UpdateSettings()
        {
            var terrain = GetComponent<Terrain>();
            _size = terrain.terrainData.size;
            _gridSize = Mathf.RoundToInt(_size.x * _grassPerUnit);

            Grid = new WorldSpaceGrid<GameObject>(_gridSize, _gridSize, _size);
            TileSet = new WorldSpaceGridTileSet<GameObject>(Grid, 32);
        }

        [ContextMenu("Toggle grass visibility")]
        private void ToggleGrassVisibility()
        {
            //for (var i = 0; i < _grassGrid.Length; i++)
            //{
            //    if (_grassGrid[i] == null)
            //    {
            //        continue;
            //    }

            //    _grassGrid[i].SetActive(!_grassGrid[i].activeSelf);
            //}
        }

        [ContextMenu("Perform tiled batching")]
        private void PerformTiledBatching()
        {
            for (var i = 0; i < TileSet.TileCount; i++)
            {
                var combineInstances = new List<CombineInstance>(capacity: TileSet.ItemsPerTile); 
                
                TileSet.ForEachInPatch(i, (items, x, z, index) =>
                                              {
                                                  var item = items[index];
                                                  
                                                  if (item == null)
                                                  {
                                                      return;
                                                  } 

                                                  var mesh = items[index].GetComponent<MeshFilter>().sharedMesh;
                                                  var instance = new CombineInstance { mesh = mesh, transform = items[index].transform.localToWorldMatrix };

                                                  combineInstances.Add(instance);
                                                  items[index].SetActive(false);
                                              });

                if (combineInstances.Count < 1)
                {
                    continue;
                }

                var batchedMesh = new Mesh();
                batchedMesh.CombineMeshes(combineInstances.ToArray(), mergeSubMeshes: true, useMatrices: true);

                var tileIndex = i + 1;

                if (transform.childCount < tileIndex)
                {
                    var tile = new GameObject("Tile" + tileIndex);
                    tile.AddComponent<MeshFilter>().sharedMesh = batchedMesh;
                    tile.AddComponent<MeshRenderer>();
                }else
                {
                    transform.GetChild(i).gameObject.GetComponent<MeshFilter>().sharedMesh = batchedMesh;
                }
            }
        }

        [ContextMenu("Batch grass in single mesh")]
        private void BatchGrassInSingleMesh()
        {
            DestroyImmediate(GetComponent<MeshFilter>().sharedMesh);

            var gridItems = Grid.GetItemsRaw();

            var combineInstances = new List<CombineInstance>(capacity: gridItems.Length);
            for (var i = 0; i < gridItems.Length; i++)
            {
                if (gridItems[i] == null)
                {
                    continue;
                }

                var mesh = gridItems[i].GetComponent<MeshFilter>().sharedMesh;
                var instance = new CombineInstance { mesh = mesh, transform = gridItems[i].transform.localToWorldMatrix };

                combineInstances.Add(instance);
                gridItems[i].SetActive(false);
            }

            var batchedMesh = new Mesh();
            batchedMesh.CombineMeshes(combineInstances.ToArray(), mergeSubMeshes: true, useMatrices: true);

            GetComponent<MeshFilter>().sharedMesh = batchedMesh;
        }

        public void DrawBrush(Vector3 worldPosition, float radius, GrassBrush brush, GrassParameters parameters)
        {
            Grid.ForEachInRadius(worldPosition, radius, (items, x, z, index) =>
                                                            {
                                                                var grassInstance = parameters.PlaceGrassAtPosition(new Vector3(x, 0, z) / _grassPerUnit, brush.Density);

                                                                if (grassInstance == null)
                                                                {
                                                                    return;
                                                                }

                                                                DestroyImmediate(items[index]);

                                                                items[index] = grassInstance;

                                                                var tileId = TileSet.GetTileIdAtGridPosition(x, z);
                                                                TileSet.SetTileDirty(tileId, true);
                                                            });
        }

        public void Erase(Vector3 worldPosition, float radius)
        {
            Grid.ForEachInRadius(worldPosition, radius, (items, x, z, index) => DestroyImmediate(items[index]));
        }

        private int WorldToGridX(float worldX)
        {
            return Mathf.FloorToInt(worldX / _size.x * _gridSize);
        }

        private int WorldToGridZ(float worldZ)
        {
            return Mathf.FloorToInt(worldZ / _size.z * _gridSize);
        }
    }
}
