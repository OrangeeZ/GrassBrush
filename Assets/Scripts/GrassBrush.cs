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

                each.Prefab = _prefab;
                each.Radius = _prefab.Radius;

                var scale = Mathf.Lerp(_minHeight, _maxHeight, Random.Range(0, 1f));
                each.Scale = scale;

                if (!_grassGrid.TryAddCircle(each, _spacing))
                {
                    //continue;
                }
            }
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
