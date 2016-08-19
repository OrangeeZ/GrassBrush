using System;
using System.Collections.Generic;
using System.Linq;
using BO.Utilities;
using UnityEngine;
using System.Collections;

namespace Grass
{
    [ExecuteInEditMode]
    public class GrassCircleGrid : MonoBehaviour
    {
        public WorldSpaceCircleGrid Grid { get; private set; }

        [SerializeField]
        private Vector3 _size;

        [SerializeField]
        private int _grassPerUnit = 4;

        private int _gridSize;

        private List<DistributedCircleGenerator.Circle> _circles;

        private DetailObjectsData _detailObjectsData;

        private void OnEnable()
        {
            UpdateSettings();
        }

        private void OnDisable()
        {
            //var gridItems = Grid.GetItemsRaw();
            //for (var i = 0; i < gridItems.Length; i++)
            //{
            //    DestroyImmediate(gridItems[i]);
            //}
        }

        void Start()
        {
            if (!Application.isPlaying)
            {
                Load();
            }
        }

        [ContextMenu("Update settings")]
        private void UpdateSettings()
        {
            var terrain = GetComponent<Terrain>();
            _size = terrain.terrainData.size;
            _gridSize = Mathf.RoundToInt(_size.x * _grassPerUnit);

            _circles = new List<DistributedCircleGenerator.Circle>();

            Grid = new WorldSpaceCircleGrid(_gridSize, _gridSize, _size);
        }

        public bool TryAddCircle(DistributedCircleGenerator.Circle circle, float spacing)
        {
            if (Grid.Intersects(circle, spacing))
            {
                return false;
            }

            Grid.AddCircle(circle);
            _circles.Add(circle);

            return true;
        }

        //public void DrawBrush(IList<DistributedCircleGenerator.Circle> circles)
        //{
        //    for (var i = 0; i < circles.Count; i++)
        //    {
        //        if (!Grid.Intersects(circles[i]))
        //        {
        //            Grid.AddCircle(circles[i], _circles.Count);

        //            _circles.Add(circles[i]);
        //        }
        //    }
        //}

        public void DrawBrush(Vector3 worldPosition, float radius, GrassBrush brush, GrassParameters parameters)
        {
            //Grid.ForEachInRadius(worldPosition, radius, (items, x, z, index) =>
            //{
            //    var grassInstance = parameters.PlaceGrassAtPosition(new Vector3(x, 0, z) / _grassPerUnit, brush.Density);

            //    if (grassInstance == null)
            //    {
            //        return;
            //    }

            //    items[index] = grassInstance;//.GetComponent<MeshFilter>().sharedMesh;// grassInstance;

            //    var tileId = TileSet.GetTileIdAtGridPosition(x, z);
            //    TileSet.SetTileDirty(tileId, true);
            //});

            //UpdateMeshes();
        }

        public void Erase(Vector3 worldPosition, float radius)
        {
            Grid.RemoveCircle(worldPosition, radius, circle =>
            {
                if (circle.Instance != null)
                {
                    DestroyImmediate(circle.Instance.gameObject);
                }
            });

            _circles.RemoveAll(_ => _.Instance == null);

            //Grid.ForEachInRadius(worldPosition, radius, (items, x, z, index) =>
            //{
            //    items[index] = null;

            //    var tileId = TileSet.GetTileIdAtGridPosition(x, z);
            //    TileSet.SetTileDirty(tileId, true);
            //});

            //UpdateMeshes();
        }

        [ContextMenu("Save")]
        private void Save()
        {
            var scene = gameObject.scene;
            var path = scene.path.Replace(".unity", "_DetailObjectsData.asset");

            _detailObjectsData = ScriptableObject.CreateInstance<DetailObjectsData>();
            _detailObjectsData.SetDetailObjects(_circles);

            UnityEditor.AssetDatabase.CreateAsset(_detailObjectsData, path);
        }

        [ContextMenu("Load")]
        private void Load()
        {
            var scene = gameObject.scene;
            var path = scene.path.Replace(".unity", "_DetailObjectsData.asset");

            _detailObjectsData = UnityEditor.AssetDatabase.LoadAssetAtPath<DetailObjectsData>(path);

            _circles = _detailObjectsData.GetDetailObjects().Select(_ => new DistributedCircleGenerator.Circle
             {
                 Position = _.Position,
                 Radius = _.Radius,
                 Instance = UnityEditor.PrefabUtility.InstantiatePrefab(_.Instance) as DetailPreset
             }).ToList();

            for (var i = 0; i < _circles.Count; i++)
            {
                Grid.AddCircle(_circles[i]);
                _circles[i].Instance.transform.position = _circles[i].Position;
            }
        }

        void OnDrawGizmos()
        {
            //Grid.OnDrawGizmos();
        }
    }
}
