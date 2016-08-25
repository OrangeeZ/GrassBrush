using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

namespace Grass
{
    [Serializable]
    public class DetailObjectBrush
    {
        public string Name;

        public float Density { get { return _density; } }

        public Vector3 Position { get; set; }

        public bool SnapToNormals = true;

        [Range(1f, 50)]
        public float Radius;

        [SerializeField]
        private DetailPreset[] _prefabs;

        [SerializeField]
        private float _spacing;

        [SerializeField]
        [Range(0.1f, 1f)]
        private float _density = 1f;

        [SerializeField]
        private DistributedCircleGenerator _distributedCircleGenerator;

        [SerializeField]
        private float _minHeight = 1f;

        [SerializeField]
        private float _maxHeight = 2f;

        [SerializeField]
        private Color32 _fromColor = Color.white;

        [SerializeField]
        private Color32 _toColor = Color.white;

        [ContextMenu("Draw")]
        public void Draw(DetailObjectLayer detailObjectLayer)
        {
            _distributedCircleGenerator = _distributedCircleGenerator ?? new DistributedCircleGenerator();

            _distributedCircleGenerator.SetRadius(Radius);
            _distributedCircleGenerator.Generate(Position, _density);

            for (int i = 0; i < _distributedCircleGenerator.GetCircles().Count; i++)
            {
                var each = _distributedCircleGenerator.GetCircles()[i];
                var randomPrefab = _prefabs[Random.Range(0, _prefabs.Length)];

                each.Prefab = randomPrefab;
                each.Radius = randomPrefab.Radius;

                each.AngleY = Random.Range(0, 360f);

                var scale = Mathf.Lerp(_minHeight, _maxHeight, Random.Range(0, 1f));
                each.Scale = scale;

                each.Color = Color32.Lerp(_fromColor, _toColor, Random.Range(0, 1f));

                if (!detailObjectLayer.TryAddCircle(each, _spacing))
                {
                    //continue;
                }
            }
        }

        public void Erase(DetailObjectLayer detailObjectLayer)
        {
            _distributedCircleGenerator = _distributedCircleGenerator ?? new DistributedCircleGenerator();

            _distributedCircleGenerator.SetRadius(Radius);
            _distributedCircleGenerator.Generate(Position, _density);

            for (int i = 0; i < _distributedCircleGenerator.GetCircles().Count; i++)
            {
                var each = _distributedCircleGenerator.GetCircles()[i];
                var randomPrefab = _prefabs[Random.Range(0, _prefabs.Length)];

                detailObjectLayer.Erase(each.Position, randomPrefab.Radius);
            }

            detailObjectLayer.OnEraseFinish();
        }

        public DetailObjectBrush Copy()
        {
            return MemberwiseClone() as DetailObjectBrush;
        }
    }
}
