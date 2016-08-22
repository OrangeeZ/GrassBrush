using UnityEngine;
using UnityEditor;

namespace Grass
{
    [CustomEditor(typeof(DetailObjectLayer))]
    public class DetailObjectLayerEditor : Editor
    {
        private DetailObjectLayer _target;

        private DetailObjectPresetList _presetList;

        void OnEnable()
        {
            _target = target as DetailObjectLayer;

            _presetList = new DetailObjectPresetList(serializedObject, serializedObject.FindProperty("Brushes"), _target);
        }

        void OnSceneGUI()
        {
            Selection.activeObject = _target;

            var brush = _target.ActiveBrush;

            var currentEvent = Event.current;

            var ray = HandleUtility.GUIPointToWorldRay(currentEvent.mousePosition);
            var raycatHitInfo = default(RaycastHit);
            var didHit = Terrain.activeTerrain.GetComponent<TerrainCollider>().Raycast(ray, out raycatHitInfo, float.MaxValue);

            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

            var isMouseMoving = currentEvent.type == EventType.MouseDrag || currentEvent.type == EventType.MouseUp;

            if (isMouseMoving && !currentEvent.alt)
            {
                if (currentEvent.button == 0)
                {
                    if (didHit)
                    {
                        brush.Position = raycatHitInfo.point;

                        if (!currentEvent.shift)
                        {
                            brush.Draw();
                        }
                        else
                        {
                            brush.Erase();
                        }
                    }

                    currentEvent.Use();
                }
            }

            Handles.CircleCap(-1, raycatHitInfo.point, Quaternion.AngleAxis(90f, Vector3.right), brush.Radius);
        }

        public override void OnInspectorGUI()
        {
            _presetList.OnInspectorGUI();
        }
    }

    public class DetailObjectPresetList
    {
        public int ActiveBrushIndex;

        public DetailObjectBrush ActiveBrush { get; private set; }
        private readonly SerializedObject _targetObject;
        private readonly SerializedProperty _targetProperty;
        private readonly DetailObjectLayer _detailObjectLayer;

        private SerializedProperty _activeBrush;

        public DetailObjectPresetList(SerializedObject targetObject, SerializedProperty targetProperty, DetailObjectLayer detailObjectLayer)
        {
            _targetObject = targetObject;
            _targetProperty = targetProperty;
            _detailObjectLayer = detailObjectLayer;

            _activeBrush = _targetObject.FindProperty("ActiveBrush");
        }

        public void OnInspectorGUI()
        {
            _targetObject.Update();

            EditorGUILayout.PropertyField(_activeBrush, includeChildren: true);

            if (GUILayout.Button("Add active preset"))
            {
                _detailObjectLayer.AddActivePreset();

                return;
            }

            for (var i = 0; i < _targetProperty.arraySize; i++)
            {
                var element = _targetProperty.GetArrayElementAtIndex(i);

                using (var horizontalScope = new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("A", GUILayout.Width(20)))
                    {
                        _detailObjectLayer.SetPresetActive(i);
                    }

                    GUILayout.Space(10);

                    EditorGUILayout.PropertyField(element, includeChildren: true);

                    GUILayout.Space(10);

                    if (GUILayout.Button("X", GUILayout.Width(20)))
                    {
                        _detailObjectLayer.RemovePreset(i);
                        return;
                    }
                }
            }

            _targetObject.ApplyModifiedProperties();
        }
    }
}
