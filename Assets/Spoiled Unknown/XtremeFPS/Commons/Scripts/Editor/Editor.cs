/*Copyright � Spoiled Unknown*/
/*2024*/
/*Note: This is an important editor script*/

using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

public class NDSEditor : EditorWindow
{
    #region Setup
    [MenuItem("Window/Spoiled Unknown/Xtreme FPS")]
    public static void ShowWindow()
    {
        // Create a new Editor Window instance and show it
        NDSEditor window = GetWindow<NDSEditor>("XtremeFPS");
        window.Show();
    }



    #endregion
    #region Varibales

    //bools
    private bool aboutButton = true;
    private bool systemButton = false;
    private bool layerButton = false;
    private bool completeButton = false;

    private const string physicsLayer = "Physics";

    private const string concreteTag = "Concrete";
    private const string grassTag = "Grass";
    private const string gravelTag = "Gravel";
    private const string waterTag = "Water";
    private const string metalTag = "Metals";
    private const string woodTag = "Wood";
    #endregion
    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        
        // Left section (buttons)
        EditorGUILayout.BeginVertical(GUILayout.Width(200));
        if (GUILayout.Button("About", GUILayout.Width(200), GUILayout.Height(100)))
        {
            aboutButton = true;
            systemButton = false;
            layerButton = false;
            completeButton = false;
        }
        if(GUILayout.Button("System", GUILayout.Width(200), GUILayout.Height(100)))
        {
            systemButton = true;
            aboutButton = false;
            layerButton = false;
            completeButton = false;
        }
        if(GUILayout.Button("Layer & Tags", GUILayout.Width(200), GUILayout.Height(100)))
        {
            systemButton = false;
            aboutButton = false;
            layerButton = true;
            completeButton = false;
        }
        if(GUILayout.Button("Create Complete", GUILayout.Width(200), GUILayout.Height(100)))
        {
            systemButton = false;
            aboutButton = false;
            layerButton = false;
            completeButton = true;
        }
        
        

        // Add more buttons as needed
        EditorGUILayout.EndVertical();

        GUI.color = Color.black;

        // Right section (content)
        EditorGUILayout.BeginVertical();
        if (aboutButton)
        {
            #region Intro
            EditorGUILayout.LabelField("About Xtreme FPS");
            EditorGUILayout.Space();
            GUI.color = Color.black;
            GUILayout.Label("Xtreme FPS Controller", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 16 });
            GUI.color = Color.green;
            GUILayout.Label("Made By SpoiledUnknown", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Normal, fontSize = 12 });
            GUI.color = Color.red;
            GUILayout.Label("version 1.6.0", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Italic, fontSize = 12 });
            GUI.color = Color.black;
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUI.color = Color.green;
            GUILayout.Label("XtremeFPS Controller Setup", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 16 });
            GUI.color = Color.black;
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Space(20); // Add some vertical spacing
            #region Discord
            GUI.color = Color.yellow;
            GUILayout.Label("Socials:-", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            GUI.color = Color.white;
            Rect buttonRect = GUILayoutUtility.GetRect(200, 50); // Define button dimensions
            if (GUI.Button(buttonRect, "Discord"))
            {
                // Code to execute when the button is clicked
                Application.OpenURL("https://discord.gg/Zd93pzBAHS");
            }
            EditorGUILayout.Space();
            #endregion
            #region Github
            GUI.color = Color.white;
            Rect inputButtonRect = GUILayoutUtility.GetRect(200, 50); // Define button dimensions
            if (GUI.Button(inputButtonRect, "Github"))
            {
                // Code to execute when the button is clicked
                Application.OpenURL("https://github.com/SpoiledUnknown");
            }
            EditorGUILayout.Space();
            #endregion
            #region Youtube
            GUI.color = Color.white;
            Rect inputButton = GUILayoutUtility.GetRect(200, 50); // Define button dimensions
            if (GUI.Button(inputButton, "Youtube"))
            {
                // Code to execute when the button is clicked
                Application.OpenURL("https://www.youtube.com/c/SpoiledUnknown");
            }
            EditorGUILayout.Space();
            #endregion
            #region Github Repository
            GUI.color = Color.white;
            Rect inputButtonRepo = GUILayoutUtility.GetRect(200, 41.5f); // Define button dimensions
            if (GUI.Button(inputButtonRepo, "Github Repository"))
            {
                // Code to execute when the button is clicked
                Application.OpenURL("https://github.com/SpoiledUnknown/Xtreme-FPS-Controller");
            }
            EditorGUILayout.Space();
            #endregion
            #endregion
        }
        if (systemButton)
        {
            EditorGUILayout.LabelField("System Intalls");
            #region Cinemachine
            GUI.color = Color.yellow;
            GUILayout.Label("Cinemachine:-", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
            GUI.color = Color.white;
            Rect buttonRect = GUILayoutUtility.GetRect(200, 50); // Define button dimensions
            if (GUI.Button(buttonRect, "Install Cinemachine"))
            {
                // Code to execute when the button is clicked
                InstallCinemachine();
            }
            EditorGUILayout.Space();
            #endregion
            #region Input System
            GUI.color = Color.yellow;
            GUILayout.Label("Input System:-", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
            GUI.color = Color.white;
            Rect inputButtonRect = GUILayoutUtility.GetRect(200, 50); // Define button dimensions
            if (GUI.Button(inputButtonRect, "Install InputSystem"))
            {
                // Code to execute when the button is clicked
                InstallInputSystem();
            }
            EditorGUILayout.Space();
            #endregion
        }
        if (layerButton)
        {
            EditorGUILayout.LabelField("Layer And Tag Setup");
            #region Create Layers
            GUI.color = Color.yellow;
            GUILayout.Label("Layers:-", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
            GUI.color = Color.white;
            Rect buttonRect = GUILayoutUtility.GetRect(200, 50); // Define button dimensions
            if (GUI.Button(buttonRect, "Create Layers"))
            {
                // Create the new layer.
                CreateLayer(physicsLayer);
            }
            EditorGUILayout.Space();
            #endregion
            #region Create Tags
            GUI.color = Color.yellow;
            GUILayout.Label("Tags:-", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
            GUI.color = Color.white;
            Rect inputButtonRect = GUILayoutUtility.GetRect(200, 50); // Define button dimensions
            if (GUI.Button(inputButtonRect, "Create Tags"))
            {
                // Code to execute when the button is clicked
                CreateTag(concreteTag);
                CreateTag(grassTag);
                CreateTag(gravelTag);
                CreateTag(waterTag);
                CreateTag(metalTag);
                CreateTag(woodTag);
            }
            EditorGUILayout.Space();
            #endregion
        }
        if (completeButton)
        {
            EditorGUILayout.LabelField("Create Player Controller");
            #region Create Character Controller
            GUI.color = Color.yellow;
            GUILayout.Label("Character Controller:-", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
            GUI.color = Color.white;
            Rect buttonRect = GUILayoutUtility.GetRect(200, 50); // Define button dimensions
            if (GUI.Button(buttonRect, "Create Player"))
            {

            }
            EditorGUILayout.Space();
            #endregion
            #region Create Rigidbody Controller
            GUI.color = Color.yellow;
            GUILayout.Label("Rigidbody Controller:-", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
            GUI.color = Color.white;
            Rect inputButtonRect = GUILayoutUtility.GetRect(200, 50); // Define button dimensions
            if (GUI.Button(inputButtonRect, "Create Player"))
            {

            }
            EditorGUILayout.Space();
            #endregion
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();
    }

    #region CineMachine
    private void InstallCinemachine()
    {
        Client.Add("com.unity.cinemachine");
    }
    #endregion
    #region Input System
    public void InstallInputSystem()
    {
        Client.Add("com.unity.inputsystem");
    }
    #endregion
    #region Create Layers
    void CreateLayer(string layerName)
    {
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty layers = tagManager.FindProperty("layers");

        for (int i = 8; i < layers.arraySize; i++)
        {
            SerializedProperty layerSP = layers.GetArrayElementAtIndex(i);
            if (string.IsNullOrEmpty(layerSP.stringValue))
            {
                layerSP.stringValue = layerName;
                tagManager.ApplyModifiedProperties();
                return;
            }
        }

        Debug.LogError("No available layer slot to create the layer: " + layerName);
    }
    #endregion
    #region Create Tags
    private bool TagExists(string tagName)
    {
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty tags = tagManager.FindProperty("tags");

        for (int i = 0; i < tags.arraySize; i++)
        {
            SerializedProperty tagSP = tags.GetArrayElementAtIndex(i);
            if (tagSP.stringValue == tagName)
            {
                return true;
            }
        }

        return false;
    }

    private void CreateTag(string tagName)
    {
        if(TagExists(tagName))
        {
            Debug.LogWarning("No available tag slot to create the tag: " + tagName);
            return;
        }
        UnityEditorInternal.InternalEditorUtility.AddTag(tagName);

    }
    #endregion
}

