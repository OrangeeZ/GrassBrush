using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Grass
{
    [ExecuteInEditMode]
    public class DetailObjectLayer : MonoBehaviour
    {
        public WorldSpaceCircleGrid Grid { get; private set; }

        public DetailObjectBrush ActiveBrush;
        public List<DetailObjectBrush> Brushes;
        public int ActiveBrushIndex = 0;

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

            if (!Application.isPlaying)
            {
                Load();
            }
        }

        private void OnDisable()
        {
            for (int i = 0; i < _circles.Count; i++)
            {
                UnityEngine.Object.DestroyImmediate(_circles[i].Instance.gameObject);
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
        }

        public void OnEraseFinish()
        {
            _circles.RemoveAll(_ => _.Instance == null);
        }

        public void SetPresetActive(int presetIndex)
        {
            ActiveBrush = Brushes[presetIndex].Copy();
        }

        public void RemovePreset(int presetIndex)
        {
            Brushes.RemoveAt(presetIndex);
        }

        public void AddActivePreset()
        {
            Brushes.Add(ActiveBrush.Copy());
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

            if (_detailObjectsData == null)
            {
                return;
            }

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

            SnapTargetToTerrain(target, instance);

            instance.transform.localScale = Vector3.one * target.Scale;
            instance.gameObject.hideFlags = HideFlags.HideAndDontSave;

            var renderer = instance.GetComponent<Renderer>();
            var propertyBlock = new MaterialPropertyBlock();
            propertyBlock.SetColor("_Color", target.Color);
            renderer.SetPropertyBlock(propertyBlock);

            target.Instance = instance;
        }

        private void SnapTargetToTerrain(DistributedCircleGenerator.Circle target, DetailPreset instance)
        {
            var height = Terrain.activeTerrain.SampleHeight(target.Position);
            target.Position.y = height + instance.GetComponent<MeshFilter>().sharedMesh.bounds.extents.y * target.Scale;

            target.AngleY = 0;

            instance.transform.position = target.Position;

            if (ActiveBrush.SnapToNormals)
            {
                var normal = Terrain.activeTerrain.terrainData.GetInterpolatedNormal(target.Position.x / Terrain.activeTerrain.terrainData.size.x, target.Position.z / Terrain.activeTerrain.terrainData.size.z);
                instance.transform.up = normal;
            }
        }

        void OnDrawGizmos()
        {
            //Grid.OnDrawGizmos();
        }
    }
}
