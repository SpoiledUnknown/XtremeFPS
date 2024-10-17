using UnityEditor;
using UnityEngine;
using XtremeFPS.InputHandling.TouchControls;

namespace XtremeFPS.CustomEditors
{
    [CustomEditor(typeof(TouchControlActivator)), CanEditMultipleObjects]
    public class TouchControlActivatorEditor : Editor
    {
        TouchControlActivator touchControlsUI;
        SerializedObject serTouchControl_UI;

        private void OnEnable()
        {
            touchControlsUI = (TouchControlActivator)target;
            serTouchControl_UI = new SerializedObject(touchControlsUI);
        }

        public override void OnInspectorGUI()
        {
            serTouchControl_UI.Update();
            #region Intro
            EditorGUILayout.Space();
            GUI.color = Color.black;
            GUILayout.Label("XtremeFPS Controller", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 16 });
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUI.color = Color.green;
            GUILayout.Label("Touch Control Activator", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 16 });
            EditorGUILayout.Space();
            GUI.color = Color.black;
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUI.color = Color.white;
            #endregion
            #region Update Changes
            //Sets any changes from the prefab
            if (GUI.changed)
            {
                EditorUtility.SetDirty(touchControlsUI);
                Undo.RecordObject(touchControlsUI, "Touch Control Activator Change");
                serTouchControl_UI.ApplyModifiedProperties();
            }
            #endregion
        }

    }
}