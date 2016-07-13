using System.Collections.Generic;
using UnityEngine;
using System.Collections;

namespace Grass
{
    [ExecuteInEditMode]
    public class GrassGrid : MonoBehaviour
    {
        [SerializeField]
        private Vector3 _size;

        [SerializeField]
        private int _grassPerUnit = 4;

        private GameObject[] _grassGrid;

        private int _gridSize;

        void OnEnable()
        {
            UpdateSettings();
        }

        [ContextMenu("Update settings")]
        private void UpdateSettings()
        {
            var terrain = GetComponent<Terrain>();
            _size = terrain.terrainData.size;
            _gridSize = Mathf.RoundToInt(_size.x * _grassPerUnit);

            _grassGrid = new GameObject[_gridSize * _gridSize];
        }

        [ContextMenu("Toggle grass visibility")]
        private void ToggleGrassVisibility()
        {
            for (var i = 0; i < _grassGrid.Length; i++)
            {
                if (_grassGrid[i] == null)
                {
                    continue;
                }

                _grassGrid[i].SetActive(!_grassGrid[i].activeSelf);
            }
        }

        [ContextMenu("Batch grass in single mesh")]
        private void BatchGrassInSingleMesh()
        {
            DestroyImmediate(GetComponent<MeshFilter>().sharedMesh);

            var combineInstances = new List<CombineInstance>(capacity: _grassGrid.Length);
            for (var i = 0; i < _grassGrid.Length; i++)
            {
                if (_grassGrid[i] == null)
                {
                    continue;
                }

                var mesh = _grassGrid[i].GetComponent<MeshFilter>().sharedMesh;
                var instance = new CombineInstance {mesh = mesh, transform = _grassGrid[i].transform.localToWorldMatrix};

                combineInstances.Add(instance);
                _grassGrid[i].SetActive(false);
                //DestroyImmediate(_grassGrid[i]);
            }

            var batchedMesh = new Mesh();
            batchedMesh.CombineMeshes(combineInstances.ToArray(), mergeSubMeshes: true, useMatrices: true);

            GetComponent<MeshFilter>().sharedMesh = batchedMesh;
        }

        public void DrawBrush(Vector3 worldPosition, float radius, GrassBrush brush, GrassParameters parameters)
        {
            var quantizedRadius = WorldToGridX(radius);
            var centerX = WorldToGridX(worldPosition.x);
            var centerZ = WorldToGridZ(worldPosition.z);

            for (var x = -quantizedRadius; x <= quantizedRadius; x++)
            {
                for (var z = -quantizedRadius; z <= quantizedRadius; z++)
                {
                    if (new Vector2(x, z).sqrMagnitude > quantizedRadius * quantizedRadius)
                    {
                        continue;
                    }

                    var gridPositionX = x + centerX;
                    var gridPositionZ = z + centerZ;

                    var gridIndex = gridPositionZ * _gridSize + gridPositionX;

                    var grassInstance = parameters.PlaceGrassAtPosition(new Vector3(gridPositionX, 0, gridPositionZ) / _grassPerUnit, brush.Density);
                    
                    if (grassInstance == null)
                    {
                        continue;
                    }

                    DestroyImmediate(_grassGrid[gridIndex]);

                    _grassGrid[gridIndex] = grassInstance;
                }
            }
        }

        public void Erase(Vector3 worldPosition, float radius)
        {
            var quantizedRadius = WorldToGridX(radius);
            var centerX = WorldToGridX(worldPosition.x);
            var centerZ = WorldToGridZ(worldPosition.z);

            for (var x = -quantizedRadius; x <= quantizedRadius; x++)
            {
                for (var z = -quantizedRadius; z <= quantizedRadius; z++)
                {
                    if (new Vector2(x, z).sqrMagnitude > quantizedRadius * quantizedRadius)
                    {
                        continue;
                    }

                    var gridPositionX = x + centerX;
                    var gridPositionZ = z + centerZ;

                    var gridIndex = gridPositionZ * _gridSize + gridPositionX;

                    DestroyImmediate(_grassGrid[gridIndex]);
                }
            }
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
