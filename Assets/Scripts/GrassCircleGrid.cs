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

        void OnDestroy()
        {
            for (int i = 0; i < _circles.Count; i++)
            {
                UnityEngine.Object.DestroyImmediate(_circles[i].Instance.gameObject);
            }
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

            SetupInstance(circle);

            return true;
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

            _circles = _detailObjectsData.GetDetailObjects().ToList();

            for (var i = 0; i < _circles.Count; i++)
            {
                Grid.AddCircle(_circles[i]);

                SetupInstance(_circles[i]);
            }
        }

        private void SetupInstance(DistributedCircleGenerator.Circle target)
        {
            var instance = UnityEditor.PrefabUtility.InstantiatePrefab(target.Prefab) as DetailPreset;

            var height = Terrain.activeTerrain.SampleHeight(target.Position);
            target.Position.y = height + instance.GetComponent<MeshFilter>().sharedMesh.bounds.extents.y * target.Scale;

            target.AngleY = 0;

            instance.transform.position = target.Position;

            var normal = Terrain.activeTerrain.terrainData.GetInterpolatedNormal(target.Position.x / Terrain.activeTerrain.terrainData.size.x, target.Position.z / Terrain.activeTerrain.terrainData.size.z);
            instance.transform.up = normal;

            instance.transform.localScale = Vector3.one * target.Scale;
            instance.gameObject.hideFlags = HideFlags.HideAndDontSave;

            var renderer = instance.GetComponent<Renderer>();
            var propertyBlock = new MaterialPropertyBlock();
            propertyBlock.SetColor("_Color", target.Color);
            renderer.SetPropertyBlock(propertyBlock);

            target.Instance = instance;
        }

        void OnDrawGizmos()
        {
            //Grid.OnDrawGizmos();
        }
    }
}
