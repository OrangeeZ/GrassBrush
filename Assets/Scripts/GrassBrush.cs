using System.Collections.Generic;
using UnityEngine;
using System.Collections;

namespace Grass
{
    [ExecuteInEditMode]
    public class GrassBrush : MonoBehaviour
    {
        public float Density { get { return _density; } }

        [SerializeField]
        private DetailPreset _prefab;

        [SerializeField]
        private float _spacing;

        [SerializeField]
        private GrassCircleGrid _grassGrid;

        [SerializeField]
        [Range(0.1f, 1f)]
        private float _density = 1f;

        [SerializeField]
        private DistributedCircleGenerator _distributedCircleGenerator;

        [SerializeField]
        private float _minHeight = 1f;

        [SerializeField]
        private float _maxHeight = 2f;

        [ContextMenu("Draw")]
        public void Draw()
        {
            _distributedCircleGenerator.transform.position = transform.position;

            _distributedCircleGenerator.SetRadius(transform.localScale.x);
            _distributedCircleGenerator.Generate(_density, _prefab.Radius);

            for (int i = 0; i < _distributedCircleGenerator.GetCircles().Count; i++)
            {
                var each = _distributedCircleGenerator.GetCircles()[i];

                each.Radius = _prefab.Radius;

                if (!_grassGrid.TryAddCircle(each, _spacing))
                {
                    continue;
                }

                var prefabInstance = UnityEditor.PrefabUtility.InstantiatePrefab(_prefab) as DetailPreset;

                var height = Terrain.activeTerrain.SampleHeight(each.Position);
                var scale = Mathf.Lerp(_minHeight, _maxHeight, Random.Range(0, 1f));

                each.Position.y = height + _prefab.GetComponent<MeshFilter>().sharedMesh.bounds.extents.y * scale;
                var normal = Terrain.activeTerrain.terrainData.GetInterpolatedNormal(each.Position.x / Terrain.activeTerrain.terrainData.size.x, each.Position.z / Terrain.activeTerrain.terrainData.size.z);

                prefabInstance.transform.position = each.Position;
                prefabInstance.transform.up = normal;// = Quaternion.FromToRotation(Vector3.up, normal);
                prefabInstance.transform.localScale = Vector3.one * scale;
                prefabInstance.gameObject.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;

                each.Instance = prefabInstance;
            }

            //_grassGrid.DrawBrush(transform.position, transform.localScale.x, this, _grassParameters);
        }

        public void Erase()
        {
            _distributedCircleGenerator.transform.position = transform.position;

            _distributedCircleGenerator.SetRadius(transform.localScale.x);
            _distributedCircleGenerator.Generate(_density, _prefab.Radius);

            for (int i = 0; i < _distributedCircleGenerator.GetCircles().Count; i++)
            {
                var each = _distributedCircleGenerator.GetCircles()[i];
                _grassGrid.Erase(each.Position, each.Radius);
            }
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, transform.localScale.x);
        }
    }
}
