using UnityEngine;
using System.Collections;

namespace BO.Client.Graphics.DetailObjects
{
    public class DetailObject : MonoBehaviour
    {
        public float Radius;

        void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(transform.position, Radius);
        }
    }
}