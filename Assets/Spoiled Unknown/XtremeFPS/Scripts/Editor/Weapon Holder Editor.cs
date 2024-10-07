/*Copyright © Spoiled Unknown*/
/*2024*/
/*Note: This is an important editor script*/

using UnityEditor;
using UnityEngine;
using XtremeFPS.WeaponSystem.Holder;

namespace XtremeFPS.CustomEditors
{
    [CustomEditor(typeof(WeaponHolder)), CanEditMultipleObjects]
    public class WeaponHolderEditor : Editor
    {
        WeaponHolder weaponHolder;
        SerializedObject serWeaponHolder_UI;

        private void OnEnable()
        {
            weaponHolder = (WeaponHolder)target;
            serWeaponHolder_UI = new SerializedObject(weaponHolder);
        }

        public override void OnInspectorGUI()
        {
            serWeaponHolder_UI.Update();
            #region Intro
            EditorGUILayout.Space();
            GUI.color = Color.black;
            GUILayout.Label("XtremeFPS Controller", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 16 });
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUI.color = Color.green;
            GUILayout.Label("Weapon Holder Script", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 16 });
            EditorGUILayout.Space();
            GUI.color = Color.black;
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            #endregion
            #region Update Changes
            //Sets any changes from the prefab
            if (GUI.changed)
            {
                EditorUtility.SetDirty(weaponHolder);
                Undo.RecordObject(weaponHolder, "Weapon Holder Change");
                serWeaponHolder_UI.ApplyModifiedProperties();
            }
            #endregion
        }
    }

}
