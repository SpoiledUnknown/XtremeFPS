/*Copyright © Spoiled Unknown*/
/*2024*/
/*Note: This is an important editor script*/

using XtremeFPS.WeaponSystem;
using UnityEditor;
using UnityEngine;

namespace XtremeFPS.Editor
{
    [CustomEditor(typeof(ParabolicBullet)), CanEditMultipleObjects]
    public class ParabolicBulletEditor : UnityEditor.Editor
    {
        ParabolicBullet parabolicBullet;
        SerializedObject serParabolicBullet_UI;

        private void OnEnable()
        {
            parabolicBullet = (ParabolicBullet)target;
            serParabolicBullet_UI = new SerializedObject(parabolicBullet);
        }

        public override void OnInspectorGUI()
        {
            serParabolicBullet_UI.Update();
            #region Intro
            EditorGUILayout.Space();
            GUI.color = Color.black;
            GUILayout.Label("Xtreme FPS Controller", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 16 });
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUI.color = Color.green;
            GUILayout.Label("Parabolic Bullet Script", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 16 });
            EditorGUILayout.Space();
            GUI.color = Color.black;
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            #endregion
            #region Update Changes
            //Sets any changes from the prefab
            if (GUI.changed)
            {
                EditorUtility.SetDirty(parabolicBullet);
                Undo.RecordObject(parabolicBullet, "Parabolic Bullet Change");
                serParabolicBullet_UI.ApplyModifiedProperties();
            }
            #endregion
        }
    }

}


