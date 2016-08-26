using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BO.Client.Graphics.DetailObjects
{
    [ExecuteInEditMode]
    public class DetailObjectsLayer : MonoBehaviour
    {
        public WorldSpaceCircleGrid Grid { get; private set; }

        public DetailObjectsBrush ActiveBrush { get { return _presets[ActiveBrushIndex]; } }
        public int ActiveBrushIndex = 0;

        public DetailObjectsLayerPresetsInfo PresetsInfo;

        public bool ManualEditMode
        {
            get { return _manualEditMode; }
            set
            {
                _manualEditMode = value;

                for (var i = 0; i < _circles.Count; i++)
                {
                    _circles[i].Instance.gameObject.hideFlags = value ? HideFlags.None : HideFlags.HideAndDontSave;
                }
            }
        }

        private List<DetailObjectsBrush> _presets { get { return PresetsInfo.Presets; } }

        [SerializeField]
        private Vector3 _size;

        [SerializeField]
        private int _grassPerUnit = 4;

        private int _gridSize;

        private List<DistributedCircleGenerator.Circle> _circles;

        private DetailObjectsData _detailObjectsData;
        private bool _manualEditMode;

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
            _size = Terrain.activeTerrain.terrainData.size;
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

            PlaceInstance(circle);

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
            ActiveBrushIndex = presetIndex;// Brushes[presetIndex];//.Copy();
        }

        public void RemovePreset(int presetIndex)
        {
            _presets.RemoveAt(presetIndex);
        }

        public void DuplicatePreset(int presetIndex)
        {
            _presets.Add(_presets[presetIndex].Copy());
            SetPresetActive(_presets.Count - 1);
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

        private void LoadInstance(DistributedCircleGenerator.Circle target)
        {
            var instance = UnityEditor.PrefabUtility.InstantiatePrefab(target.Prefab) as DetailObject;

            SnapTargetToTerrain(target, instance);

            instance.transform.localScale = Vector3.one * target.Scale;
            instance.gameObject.hideFlags = HideFlags.HideAndDontSave;

            var renderer = instance.GetComponentInChildren<Renderer>();
            var propertyBlock = new MaterialPropertyBlock();
            propertyBlock.SetColor("_Color", target.Color);
            renderer.SetPropertyBlock(propertyBlock);

            target.Instance = instance;
        }

        private void PlaceInstance(DistributedCircleGenerator.Circle target)
        {
            LoadInstance(target);

            if (ActiveBrush.SnapToNormals)
            {
                var normal = Terrain.activeTerrain.terrainData.GetInterpolatedNormal(target.Position.x / Terrain.activeTerrain.terrainData.size.x, target.Position.z / Terrain.activeTerrain.terrainData.size.z);
                target.Instance.transform.up = normal;
                target.Instance.transform.rotation *= Quaternion.AngleAxis(target.AngleY, Vector3.up);
            }
        }

        private void SnapTargetToTerrain(DistributedCircleGenerator.Circle target, DetailObject instance)
        {
            var height = Terrain.activeTerrain.SampleHeight(target.Position);
            target.Position.y = height;// +instance.GetComponentInChildren<MeshFilter>().sharedMesh.bounds.extents.y * target.Scale;

            instance.transform.position = target.Position;
            instance.transform.rotation = Quaternion.AngleAxis(target.AngleY, Vector3.up);
        }

        [ContextMenu("Load")]
        private void Load()
        {
            //#if UNTIY_EDITOR
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

                LoadInstance(_circles[i]);
            }
            //#endif
        }

        void OnDrawGizmos()
        {
            //Grid.OnDrawGizmos();
        }
    }
}
