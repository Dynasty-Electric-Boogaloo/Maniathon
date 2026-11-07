#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace Mainframe.Scripts.Game.Editor
{
    [CustomEditor(typeof(OverridePhysics2D))]
    public class OverridePhysics2DEditor : UnityEditor.Editor
    {
        private bool _foldout;
        
        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("gravity"));

            EditorGUILayout.BeginHorizontal();
            _foldout = EditorGUILayout.Foldout(_foldout, "Collision layer matrix");
            var maxLayerProperty = serializedObject.FindProperty("maxLayers");
            EditorGUILayout.PropertyField(maxLayerProperty);
            maxLayerProperty.intValue = Mathf.Clamp(maxLayerProperty.intValue, 1, 31);
            EditorGUILayout.EndHorizontal();
            
            if (_foldout)
            {
                LayerCollisionMatrixGUI.Draw(new GUIContent(""), GetValue, SetValue, maxLayerProperty.intValue);
            }
            
            serializedObject.ApplyModifiedProperties();
        }

        private void SetValue(int layerA, int layerB, bool val)
        {
            if (layerA > layerB)
                (layerA, layerB) = (layerB, layerA);
            
            var collisionLayerMatrix = serializedObject.FindProperty("collisionLayerMatrix");
            var element = collisionLayerMatrix.GetArrayElementAtIndex(layerA);
            var value = element.uintValue;
            value &= ~(1U << layerB);
            value |= val ? (1U << layerB) : 0;
            element.uintValue = value;
        }

        private bool GetValue(int layerA, int layerB)
        {
            var collisionLayerMatrix = serializedObject.FindProperty("collisionLayerMatrix");
            return (collisionLayerMatrix.GetArrayElementAtIndex(layerA).uintValue & (1 << layerB)) != 0;
        }

        private void Reset()
        {
            var collisionLayerMatrix = serializedObject.FindProperty("collisionLayerMatrix");
            
            for (var i = 0; i < 32; i++)
            {
                collisionLayerMatrix.GetArrayElementAtIndex(i).uintValue = 0xFFFFFFFF;
            }
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif