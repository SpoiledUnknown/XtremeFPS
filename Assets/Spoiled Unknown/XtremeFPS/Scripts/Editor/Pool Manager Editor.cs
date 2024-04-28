using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using XtremeFPS.PoolingSystem;

namespace XtremeFPS.Editor
{
    [CustomEditor(typeof(PoolManager)), CanEditMultipleObjects]
    public class PoolManagerEditor : UnityEditor.Editor
    {
        private PoolManager poolManager;
        private SerializedObject serializedPoolManager;
        private SerializedProperty itemsToPoolProperty;
        private bool showObjectPoolItems = true;
        private bool[] foldoutStates;

        private void OnEnable()
        {
            poolManager = (PoolManager)target;
            serializedPoolManager = new SerializedObject(poolManager);
            itemsToPoolProperty = serializedPoolManager.FindProperty("itemsToPool");
            int listSize = itemsToPoolProperty.arraySize;
            foldoutStates = new bool[listSize];
            for (int i = 0; i < listSize; i++)
            {
                foldoutStates[i] = true; // Initially, all foldouts are expanded
            }
        }

        public override void OnInspectorGUI()
        {
            serializedPoolManager.Update();

            #region Intro
            EditorGUILayout.Space();
            GUI.color = Color.black;
            GUILayout.Label("Xtreme FPS Controller", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 16 });
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUI.color = Color.green;
            GUILayout.Label("Pool Manager", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 16 });
            GUI.color = Color.black;
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            EditorGUILayout.Space();
            GUI.color = Color.white;
            #endregion



            // Display the foldout for Object Pool Items
            GUIStyle foldoutStyle = new GUIStyle(EditorStyles.foldout);
            foldoutStyle.fontStyle = FontStyle.Bold;
            foldoutStyle.fontSize = 13;
            showObjectPoolItems = EditorGUILayout.Foldout(showObjectPoolItems, new GUIContent("Items To Pool", "Store the Referrence and Properties of all the object that needs to be pooled."), true, foldoutStyle);
            if (showObjectPoolItems)
            {
                EditorGUI.indentLevel++;
                int listSize = itemsToPoolProperty.arraySize;
                for (int i = 0; i < listSize; i++)
                {
                    SerializedProperty itemProperty = itemsToPoolProperty.GetArrayElementAtIndex(i);
                    SerializedProperty objectToPoolProperty = itemProperty.FindPropertyRelative("objectToPool");
                    SerializedProperty amountToPoolProperty = itemProperty.FindPropertyRelative("amountToPool");
                    SerializedProperty shouldExpandProperty = itemProperty.FindPropertyRelative("shouldExpand");
                    SerializedProperty shouldRecycleProperty = itemProperty.FindPropertyRelative("shouldRecycle");

                    // Display foldout for each ObjectPoolItem with dynamically generated name
                    foldoutStates[i] = EditorGUILayout.Foldout(foldoutStates[i], new GUIContent("Element " + i, "Contains all the property required by the element " + i + "."));
                    if (foldoutStates[i])
                    {
                        EditorGUI.indentLevel++;

                        EditorGUILayout.PropertyField(objectToPoolProperty, new GUIContent("Pooled Item", "Referrence to the object that should be pooled."), true);
                        EditorGUILayout.PropertyField(amountToPoolProperty, new GUIContent("Pool Size", "How many items should be stored in the pool."), true);
                        EditorGUILayout.PropertyField(shouldExpandProperty, new GUIContent("Can Pool Expand", "Should the pool expand (if not enough items are left in the pool)."), true);
                        EditorGUILayout.PropertyField(shouldRecycleProperty, new GUIContent("Can Pool Recycle", "Should the pool recycle the oldest element store in pool (if not enough items are left in the pool)."), true);

                        if (GUILayout.Button("Remove Element " + i))
                        {
                            RemoveObjectPoolItem(i);
                            break;
                        }

                        EditorGUILayout.Space();
                        EditorGUI.indentLevel--;
                    }
                }
                EditorGUI.indentLevel--;
            }



            // Add button to add new ObjectPoolItem
            EditorGUILayout.Space(25);
            if (GUILayout.Button("Add New Item To Pool"))
            {
                AddObjectPoolItem();
            }

            serializedPoolManager.ApplyModifiedProperties();
        }

        private void AddObjectPoolItem()
        {
            itemsToPoolProperty.arraySize++;
            serializedPoolManager.ApplyModifiedProperties();

            // Expand foldoutStates array to match the new size
            bool[] newFoldoutStates = new bool[foldoutStates.Length + 1];
            for (int i = 0; i < foldoutStates.Length; i++)
            {
                newFoldoutStates[i] = foldoutStates[i];
            }
            newFoldoutStates[newFoldoutStates.Length - 1] = true; // Newly added foldout is expanded by default
            foldoutStates = newFoldoutStates;
        }

        private void RemoveObjectPoolItem(int index)
        {
            itemsToPoolProperty.DeleteArrayElementAtIndex(index);
        }
    }

}


