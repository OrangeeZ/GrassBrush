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
        private GrassParameters _grassParameters;

        [SerializeField]
        private GrassGrid _grassGrid;

        [SerializeField]
        [Range(0, 1)]
        private float _density = 1f;

        [ContextMenu("Draw")]
        public void Draw()
        {
            _grassGrid.DrawBrush(transform.position, transform.localScale.x, this, _grassParameters);
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
