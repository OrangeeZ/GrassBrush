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
                            brush.Draw(_target);
                        }
                        else
                        {
                            brush.Erase(_target);
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

        private GUIStyle _activeBrushStyle;
        private GUIStyle _removeButtonStyle;

        public DetailObjectPresetList(SerializedObject targetObject, SerializedProperty targetProperty, DetailObjectLayer detailObjectLayer)
        {
            _targetObject = targetObject;
            _targetProperty = targetProperty;
            _detailObjectLayer = detailObjectLayer;
        }

        public void OnInspectorGUI()
        {
            if (_activeBrushStyle == null)
            {
                _activeBrushStyle = new GUIStyle(GUI.skin.button)
                {
                    fontStyle = FontStyle.Bold,
                    normal = { textColor = Color.green }
                };

                _removeButtonStyle = new GUIStyle(_activeBrushStyle)
                {
                    normal = { textColor = Color.red }
                };
            }

            _targetObject.Update();

            for (var i = 0; i < _targetProperty.arraySize; i++)
            {
                var element = _targetProperty.GetArrayElementAtIndex(i);

                using (new EditorGUILayout.HorizontalScope())
                {
                    var isCurrentBrushActive = _detailObjectLayer.ActiveBrushIndex == i;

                    var style = isCurrentBrushActive ? _activeBrushStyle : GUI.skin.button;

                    if (GUILayout.Button(new GUIContent("A", "Activate"), style, GUILayout.Width(20)))
                    {
                        _detailObjectLayer.SetPresetActive(i);
                    }

                    if (GUILayout.Button(new GUIContent("D", "Duplicate"), GUILayout.Width(20)))
                    {
                        _detailObjectLayer.DuplicatePreset(i);
                    }

                    GUILayout.Space(10);

                    EditorGUILayout.PropertyField(element, includeChildren: true);

                    GUILayout.Space(10);

                    if (GUILayout.Button(new GUIContent("X", "Remove"), _removeButtonStyle, GUILayout.Width(20)))
                    {
                        _detailObjectLayer.RemovePreset(i);
                        break;
                    }
                }
            }

            _targetObject.ApplyModifiedProperties();
        }
    }
}
