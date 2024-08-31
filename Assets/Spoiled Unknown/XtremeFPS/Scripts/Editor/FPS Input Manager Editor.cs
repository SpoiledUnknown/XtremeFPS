/*Copyright © Spoiled Unknown*/
/*2024*/
/*Note: This is an important editor script*/

using UnityEditor;
using XtremeFPS.InputHandling;
using UnityEngine;

namespace XtremeFPS.Editor
{
    [CustomEditor(typeof(FPSInputManager)), CanEditMultipleObjects]
    public class FPSInputManagerEditor : UnityEditor.Editor
    {
        FPSInputManager inputM_UI;
        SerializedObject serInputM_UI;

        private void OnEnable()
        {
            inputM_UI = (FPSInputManager)target;
            serInputM_UI = new SerializedObject(inputM_UI);
        }

        public override void OnInspectorGUI()
        {
            serInputM_UI.Update();
            #region Intro
            EditorGUILayout.Space();
            GUI.color = Color.black;
            GUILayout.Label("Xtreme FPS Controller", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 16 });
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUI.color = Color.green;
            GUILayout.Label("Input Manager Script", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 16 });
            EditorGUILayout.Space();
            GUI.color = Color.black;
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUI.color = Color.white;
            #endregion
#if UNITY_ANDROID || UNITY_IOS
        inputM_UI.maxTouchLimit = EditorGUILayout.IntField(new GUIContent("Max Touch Limit", "Maximum number of touches that should be handled by the input manager."), inputM_UI.maxTouchLimit);
        SerializedProperty touchDetectionMode = serializedObject.FindProperty("touchDetectionMode");
        EditorGUILayout.PropertyField(touchDetectionMode, new GUIContent("Touch Detection Mode", "Determines the mode at which the touch will be calculated."), true);
        serializedObject.ApplyModifiedProperties();
#endif
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