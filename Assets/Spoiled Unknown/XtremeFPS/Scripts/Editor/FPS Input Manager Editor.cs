/*Copyright © Spoiled Unknown*/
/*2024*/
/*Note: This is an important editor script*/

using UnityEditor;
using XtremeFPS.InputHandling;
using UnityEngine;

namespace XtremeFPS.CustomEditors
{
    [CustomEditor(typeof(XtremeFPSInputHandler)), CanEditMultipleObjects]
    public class FPSInputManagerEditor : Editor
    {
        XtremeFPSInputHandler inputM_UI;
        SerializedObject serInputM_UI;

        private void OnEnable()
        {
            inputM_UI = (XtremeFPSInputHandler)target;
            serInputM_UI = new SerializedObject(inputM_UI);
        }

        public override void OnInspectorGUI()
        {
            serInputM_UI.Update();
            #region Intro
            EditorGUILayout.Space();
            GUI.color = Color.black;
            GUILayout.Label("XtremeFPS Controller", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 16 });
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUI.color = Color.green;
            GUILayout.Label("Input Manager Script", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 16 });
            EditorGUILayout.Space();
            GUI.color = Color.black;
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUI.color = Color.white;
            #endregion
            #region Update Changes
            //Sets any changes from the prefab
            if (GUI.changed)
            {
                EditorUtility.SetDirty(inputM_UI);
                Undo.RecordObject(inputM_UI, "Input Manager Change");
                serInputM_UI.ApplyModifiedProperties();
            }
            #endregion
        }

    }
}