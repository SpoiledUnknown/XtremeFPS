/*Copyright © Spoiled Unknown*/
/*2024*/

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
                foldoutStates[i] = false;
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
            GUILayout.Label("Items To Pool", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
            EditorGUI.indentLevel++;
            int listSize = itemsToPoolProperty.arraySize;
            for (int i = 0; i < listSize; i++)
            {
                SerializedProperty itemProperty = itemsToPoolProperty.GetArrayElementAtIndex(i);
                SerializedProperty objectToPoolProperty = itemProperty.FindPropertyRelative("objectToPool");
                SerializedProperty objectAmountToPoolProperty = itemProperty.FindPropertyRelative("objectAmount");
                SerializedProperty canExpandProperty = itemProperty.FindPropertyRelative("canExpand");
                SerializedProperty canRecycleProperty = itemProperty.FindPropertyRelative("canRecycle");

                // Get the GameObject reference
                GameObject objectToPool = objectToPoolProperty.objectReferenceValue as GameObject;
                string objectName = objectToPool != null ? objectToPool.name : "Element " + i.ToString();

                // Display foldout for each ObjectPoolItem with the GameObject name
                foldoutStates[i] = EditorGUILayout.Foldout(foldoutStates[i], new GUIContent(objectName, "Contains all the property required by the element " + i + "."));
                if (foldoutStates[i])
                {
                    EditorGUI.indentLevel++;

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(objectToPoolProperty, new GUIContent("Pooled Item", "Referrence to the object that should be pooled."), true);
                    if (GUILayout.Button("Remove"))
                    {
                        RemoveObjectPoolItem(i);
                        break;
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.PropertyField(objectAmountToPoolProperty, new GUIContent("Pool Size", "How many items should be stored in the pool."), true);
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(canExpandProperty, new GUIContent("Expand Pool", "Should the pool expand (if not enough items are left in the pool)."), true);
                    EditorGUILayout.PropertyField(canRecycleProperty, new GUIContent("Recycle", "Should the pool recycle the oldest element store in pool (if not enough items are left in the pool)."), true);
                    EditorGUILayout.EndHorizontal();

                    if (canExpandProperty.boolValue) canRecycleProperty.boolValue = false;
                    if (canRecycleProperty.boolValue) canExpandProperty.boolValue = false;

                    EditorGUILayout.Space();
                    EditorGUI.indentLevel--;
                }
            }
            EditorGUI.indentLevel--;


            // Add button to add new ObjectPoolItem
            Rect itemRect = GUILayoutUtility.GetRect(100, 25);
            if (GUI.Button(itemRect, "Add Item")) AddObjectPoolItem();

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
            newFoldoutStates[newFoldoutStates.Length - 1] = false;
            foldoutStates = newFoldoutStates;
        }

        private void RemoveObjectPoolItem(int index)
        {
            itemsToPoolProperty.DeleteArrayElementAtIndex(index);
        }
    }
}