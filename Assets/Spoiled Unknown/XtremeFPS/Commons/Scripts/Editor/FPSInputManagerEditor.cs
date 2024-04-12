/*Copyright � Non-Dynamic Studios*/
/*2023*/
/*Note: This is an important editor script*/

using UnityEditor;
using XtremeFPS.Common.InputSystem;
using UnityEngine;

[CustomEditor(typeof(FPSInputManager)), CanEditMultipleObjects]
public class FPSInputManagerEditor : Editor
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
        
        #endregion
        #region Update Changes
        //Sets any changes from the prefab
        if (GUI.changed)
        {
            EditorUtility.SetDirty(inputM_UI);
            Undo.RecordObject(inputM_UI, "FPC Change");
            serInputM_UI.ApplyModifiedProperties();
        }
        #endregion
    }

}
