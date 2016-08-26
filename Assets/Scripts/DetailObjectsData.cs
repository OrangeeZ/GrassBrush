using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;

namespace BO.Client.Graphics.DetailObjects
{
    public class DetailObjectsData : ScriptableObject
    {
        [SerializeField]
        private List<DistributedCircleGenerator.Circle> _detailObjects;

        public IList<DistributedCircleGenerator.Circle> GetDetailObjects()
        {
            return _detailObjects;
        }

        public void SetDetailObjects(IList<DistributedCircleGenerator.Circle> detailObjects)
        {
            _detailObjects = detailObjects.ToList();
        }
    }
}