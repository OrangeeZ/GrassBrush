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
        private GrassParameters _grassParameters;

        [SerializeField]
        private GrassCircleGrid _grassGrid;

        [SerializeField]
        [Range(0, 1)]
        private float _density = 1f;

        [SerializeField]
        private DistributedCircleGenerator _distributedCircleGenerator;

        [ContextMenu("Draw")]
        public void Draw()
        {
            //_distributedCircleGenerator.Generate();

            _distributedCircleGenerator.transform.position = transform.position;

            for (int i = 0; i < _distributedCircleGenerator.GetCircles().Count; i++)
            {
                var each = _distributedCircleGenerator.GetCircles()[i];

                if (!_grassGrid.TryAddCircle(each, _prefab.Radius + _spacing))
                {
                    continue;
                }

                var prefabInstance = UnityEditor.PrefabUtility.InstantiatePrefab(_prefab) as DetailPreset;
                prefabInstance.transform.position = each.Position;
                prefabInstance.gameObject.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;
            }

            //_grassGrid.DrawBrush(transform.position, transform.localScale.x, this, _grassParameters);
        }

        public void Erase()
        {
            _grassGrid.Erase(transform.position, transform.localScale.x);
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, transform.localScale.x);
        }
    }
}
