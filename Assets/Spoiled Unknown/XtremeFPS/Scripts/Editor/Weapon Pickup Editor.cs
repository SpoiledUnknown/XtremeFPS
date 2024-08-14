/*Copyright © Spoiled Unknown*/
/*2024*/
/*Note: This is an important editor script*/

using TMPro;
using UnityEditor;
using UnityEngine;

namespace XtremeFPS.Editor
{
    using XtremeFPS.WeaponSystem.Pickup;
    using XtremeFPS.WeaponSystem;

    [CustomEditor(typeof(WeaponPickup)), CanEditMultipleObjects]
    public class WeaponPickupEditor : UnityEditor.Editor
    {
        WeaponPickup weaponPickup;
        SerializedObject serWeaponPickup;


        private void OnEnable()
        {
            weaponPickup = (WeaponPickup)target;
            serWeaponPickup = new SerializedObject(weaponPickup);
        }

        public override void OnInspectorGUI()
        {
            serWeaponPickup.Update();
            #region Intro
            EditorGUILayout.Space();
            GUI.color = Color.black;
            GUILayout.Label("Xtreme FPS Controller", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 16 });
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUI.color = Color.green;
            GUILayout.Label("Weapon Pickup Script", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 16 });
            EditorGUILayout.Space();
            #endregion
            #region References
            GUI.color = Color.black;
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("References", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
            EditorGUILayout.Space();

            //Main Movement Settings
            GUI.color = Color.white;
            weaponPickup.playerArmature = (CharacterController)EditorGUILayout.ObjectField(new GUIContent("Player Armature", "Reference to the character controller of the player."), weaponPickup.playerArmature, typeof(CharacterController), true);
            weaponPickup.weaponHolder = (Transform)EditorGUILayout.ObjectField(new GUIContent("Weapon Position", "Reference to the just parent of the weapon (can be weapon holder or weapon recoil(weapon recoild in our case))."), weaponPickup.weaponHolder, typeof(Transform), true);
            weaponPickup.cameraRoot = (Transform)EditorGUILayout.ObjectField(new GUIContent("camera Root", "Reference to the camera root."), weaponPickup.cameraRoot, typeof(Transform), true);
            weaponPickup.bulletText = (TextMeshProUGUI)EditorGUILayout.ObjectField(new GUIContent("Bullet Text", "Reference to the text that shows number of bullets on UI."), weaponPickup.bulletText, typeof(TextMeshProUGUI), true);
            EditorGUILayout.Space();
            #endregion

            #region Values
            GUI.color = Color.black;
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("Values", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
            EditorGUILayout.Space();

            GUI.color = Color.white;
            weaponPickup.equipped = EditorGUILayout.ToggleLeft(new GUIContent("Is Weapon Equiped", "Determines if the weapon is already equiped at the start of game  or not."), weaponPickup.equipped);
            weaponPickup.dropForwardForce = EditorGUILayout.Slider(new GUIContent("Forward Force", "Determines the force at which the weapon will be thrown forward."), weaponPickup.dropForwardForce, 0f, 50f);
            weaponPickup.dropUpwardForce = EditorGUILayout.Slider(new GUIContent("Upward Force", "Determines the force at which the weapon will be thrown upward."), weaponPickup.dropUpwardForce, 0f, 50f);
            weaponPickup.dropTorqueMultiplier = EditorGUILayout.Slider(new GUIContent("Torque Multiplier", "Determines the value at which the torque will be multiplied."), weaponPickup.dropTorqueMultiplier, 0f, 50f);
            EditorGUILayout.Space();
            #endregion
            #region Update Changes
            //Sets any changes from the prefab
            if (GUI.changed)
            {
                EditorUtility.SetDirty(weaponPickup);
                Undo.RecordObject(weaponPickup, "Weapon Pickup Change");
                serWeaponPickup.ApplyModifiedProperties();
            }
            #endregion
        }
    }
}

