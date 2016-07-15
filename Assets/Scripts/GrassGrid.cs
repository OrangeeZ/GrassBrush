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
        public WorldSpaceGridTileSet<Mesh> TileSet { get; private set; }
        public WorldSpaceGrid<Mesh> Grid { get; private set; }

        [SerializeField]
        private Vector3 _size;

        [SerializeField]
        private int _grassPerUnit = 4;

        private int _gridSize;

        void OnEnable()
        {
            UpdateSettings();
        }

        void OnDisable()
        {
            //var gridItems = Grid.GetItemsRaw();
            //for (var i = 0; i < gridItems.Length; i++)
            //{
            //    DestroyImmediate(gridItems[i]);
            //}
        }

        [ContextMenu("Update settings")]
        private void UpdateSettings()
        {
            var terrain = GetComponent<Terrain>();
            _size = terrain.terrainData.size;
            _gridSize = Mathf.RoundToInt(_size.x * _grassPerUnit);

            Grid = new WorldSpaceGrid<Mesh>(_gridSize, _gridSize, _size);
            TileSet = new WorldSpaceGridTileSet<Mesh>(Grid, 32);
        }

        [ContextMenu("Perform tiled batching")]
        private void PerformTiledBatching()
        {
            for (var i = 0; i < TileSet.TileCount; i++)
            {
                GenerateTileMesh(i);
            }
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

                                                                items[index] = grassInstance;//.GetComponent<MeshFilter>().sharedMesh;// grassInstance;

                                                                var tileId = TileSet.GetTileIdAtGridPosition(x, z);
                                                                TileSet.SetTileDirty(tileId, true);
                                                            });

            UpdateMeshes();
        }

        public void Erase(Vector3 worldPosition, float radius)
        {
            Grid.ForEachInRadius(worldPosition, radius, (items, x, z, index) =>
                                                            {
                                                                items[index] = null;

                                                                var tileId = TileSet.GetTileIdAtGridPosition(x, z);
                                                                TileSet.SetTileDirty(tileId, true);
                                                            });

            UpdateMeshes();
        }

        private void UpdateMeshes()
        {
            for (var i = 0; i < TileSet.TileCount; i++)
            {
                if (TileSet.GetTileDirty(i))
                {
                    GenerateTileMesh(i);

                    TileSet.SetTileDirty(i, false);
                }
            }
        }

        private void GenerateTileMesh(int tileId)
        {
            var combineInstances = new List<CombineInstance>(capacity: TileSet.ItemsPerTile);

            TileSet.ForEachInPatch(tileId, (items, x, z, index) =>
            {
                var item = items[index];

                if (item == null)
                {
                    return;
                }

                var mesh = items[index];
                var localToWorldMatrix = Matrix4x4.TRS(new Vector3(x, 0, z) / _grassPerUnit, Quaternion.identity, Vector3.one);
                var instance = new CombineInstance { mesh = mesh, transform = localToWorldMatrix };

                combineInstances.Add(instance);
            });

            var tileIndex = tileId + 1;

            var targetMeshFilter = default(MeshFilter);

            if (transform.FindChild("Tile" + tileIndex) == null)
            {
                var tile = new GameObject("Tile" + tileIndex);
                targetMeshFilter = tile.AddComponent<MeshFilter>();
                tile.AddComponent<MeshRenderer>();
                tile.transform.SetParent(transform);
            }
            else
            {
                targetMeshFilter = transform.FindChild("Tile" + tileIndex).gameObject.GetComponent<MeshFilter>();
            }

            DestroyImmediate(targetMeshFilter.sharedMesh);

            if (combineInstances.Count < 1)
            {
                return;
            }

            var batchedMesh = new Mesh();
            batchedMesh.CombineMeshes(combineInstances.ToArray(), mergeSubMeshes: true, useMatrices: true);
            targetMeshFilter.sharedMesh = batchedMesh;
        }
    }
}
