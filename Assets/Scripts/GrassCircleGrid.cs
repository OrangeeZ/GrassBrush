using System;
using System.Collections.Generic;
using BO.Utilities;
using UnityEngine;
using System.Collections;

namespace Grass
{
    [ExecuteInEditMode]
    public class GrassCircleGrid : MonoBehaviour
    {
        public WorldSpaceCircleGrid Grid { get; private set; }

        [SerializeField] private Vector3 _size;

        [SerializeField] private int _grassPerUnit = 4;

        private int _gridSize;

        private List<DistributedCircleGenerator.Circle> _circles; 

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

        [ContextMenu("Update settings")]
        private void UpdateSettings()
        {
            var terrain = GetComponent<Terrain>();
            _size = terrain.terrainData.size;
            _gridSize = Mathf.RoundToInt(_size.x*_grassPerUnit);

            _circles = new List<DistributedCircleGenerator.Circle>();

            Grid = new WorldSpaceCircleGrid(_gridSize, _gridSize, _size);
            Grid.Circles = _circles;
        }

        public bool TryAddCircle(DistributedCircleGenerator.Circle circle)
        {
            if (Grid.Intersects(circle))
            {
                return false;
            }

            Grid.AddCircle(circle, _circles.Count);
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
            //Grid.ForEachInRadius(worldPosition, radius, (items, x, z, index) =>
            //{
            //    items[index] = null;

            //    var tileId = TileSet.GetTileIdAtGridPosition(x, z);
            //    TileSet.SetTileDirty(tileId, true);
            //});

            //UpdateMeshes();
        }
    }
}
