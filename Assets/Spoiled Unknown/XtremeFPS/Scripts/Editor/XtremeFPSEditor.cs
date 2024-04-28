/*Copyright � Spoiled Unknown*/
/*2024*/
/*Note: This is an important editor script*/

using Cinemachine;
using System;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using XtremeFPS.PoolingSystem;

namespace XtremeFPS.Editor
{
    using XtremeFPS.FirstPersonController;
    using XtremeFPS.WeaponSystem;

    public class XtremeFPSEditor : EditorWindow
    {
        #region Setup
        [MenuItem("Window/Spoiled Unknown/XtremeFPS")]
        public static void ShowWindow()
        {
            // Create a new Editor Window instance and show it
            XtremeFPSEditor XtremeFPSEditorWindow = GetWindow<XtremeFPSEditor>("XtremeFPS");
            XtremeFPSEditorWindow.Show();
        }

        private void OnEnable()
        {
            this.minSize = new Vector2(650, 410);
        }


        #endregion
        #region Varibales

        //Floats
        private float NearClipingPlanes = 0.01f;
        private float FieldOfView = 60f;

        #region bools
        private bool enableAboutPanel = true;
        private bool enableInitialSetupPanel = false;
        private bool enableNonArmatureSetup = false;
        #endregion

        #region Tags and Layers

        private const string physicsLayer = "Physics";

        private const string concreteTag = "Concrete";
        private const string grassTag = "Grass";
        private const string gravelTag = "Gravel";
        private const string waterTag = "Water";
        private const string metalTag = "Metals";
        private const string woodTag = "Wood";
        #endregion

        #region Other Components
        private GameObject playerParent;
        private GameObject playerArmature;
        private CinemachineVirtualCamera virtualCamera;
        private GameObject cameraFollow;

        //Weapon Related
        private GameObject weaponModel;
        private GameObject particleEffect;

        //others
        private GameObject objectPooler;
        private GameObject playerCamera;
        private GameObject cameraHolder;
        #endregion

        #endregion
        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();

            #region Left section (buttons)
            EditorGUILayout.BeginVertical(GUILayout.Width(200));
            if (GUILayout.Button("About", GUILayout.Width(200), GUILayout.Height(100)))
            {
                enableAboutPanel = true;
                enableInitialSetupPanel = false;
                enableNonArmatureSetup = false;
            }
            if (GUILayout.Button("Initial Setup", GUILayout.Width(200), GUILayout.Height(100)))
            {
                enableInitialSetupPanel = true;
                enableAboutPanel = false;
                enableNonArmatureSetup = false;
            }
            if (GUILayout.Button("Create Controller", GUILayout.Width(200), GUILayout.Height(100)))
            {
                enableInitialSetupPanel = false;
                enableAboutPanel = false;
                enableNonArmatureSetup = true;
            }
            if (GUILayout.Button("Complete/Reset Setup", GUILayout.Width(200), GUILayout.Height(100)))
            {
                Debug.Log("Completing the Setup.......");
                CompleteTheSettup();
            }
            #endregion

            EditorGUILayout.EndVertical();

            #region Right Side Buttons
            EditorGUILayout.BeginVertical();
            if (enableAboutPanel)
            {
                #region Intro
                EditorGUILayout.LabelField("About XtremeFPS");
                EditorGUILayout.Space();
                GUI.color = Color.black;
                GUILayout.Label("XtremeFPS Controller", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 16 });
                GUI.color = Color.green;
                GUILayout.Label("Made By SpoiledUnknown", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Normal, fontSize = 12 });
                GUI.color = Color.red;
                GUILayout.Label("version 1.0.0", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Italic, fontSize = 12 });
                GUI.color = Color.black;
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                GUI.color = Color.green;
                GUILayout.Label("XtremeFPS Controller Setup", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 16 });
                GUI.color = Color.black;
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                GUILayout.Space(20);
                GUI.color = Color.black;
                GUILayout.Label("Socials:-", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                GUI.color = Color.white;
                #endregion

                #region About me
                Rect inputButtonRect = GUILayoutUtility.GetRect(200, 60);
                if (GUI.Button(inputButtonRect, "About Me"))
                {
                    Application.OpenURL("https://spoiledunknown.github.io/");
                }
                EditorGUILayout.Space();
                #endregion
                #region Discord
                Rect buttonRect = GUILayoutUtility.GetRect(200, 60);
                if (GUI.Button(buttonRect, "Support Discord"))
                {
                    Application.OpenURL("https://discord.gg/Zd93pzBAHS");
                }
                EditorGUILayout.Space();
                #endregion
                #region Youtube Tutorial
                Rect inputButtonRepo = GUILayoutUtility.GetRect(200, 60);
                if (GUI.Button(inputButtonRepo, "Video Tutorials"))
                {
                    Application.OpenURL("https://www.youtube.com/playlist?list=PLY65mi5h61NSVUbvNNRwM7PH_mV5z8GpB");
                }
                EditorGUILayout.Space();
                #endregion
            }
            if (enableInitialSetupPanel)
            {
                EditorGUILayout.LabelField("XtremeFPS Initial Setup");
                #region Cinemachine
                GUI.color = Color.black;
                GUILayout.Label("CineMachine Installation:- ", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
                GUI.color = Color.white;
                Rect buttonRect = GUILayoutUtility.GetRect(200, 47);
                if (GUI.Button(buttonRect, "Install CineMachine Package"))
                {
                    InstallCinemachine();
                }
                #endregion
                #region Input System
                GUI.color = Color.black;
                GUILayout.Label("Input System:-", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
                GUI.color = Color.white;
                Rect inputButtonRect = GUILayoutUtility.GetRect(200, 47);
                if (GUI.Button(inputButtonRect, "Install InputSystem Package"))
                {
                    InstallInputSystem();
                }
                #endregion
                GUI.color = Color.red;
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                #region Create Layers
                GUI.color = Color.black;
                GUILayout.Label("Layers:-", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
                GUI.color = Color.white;
                Rect layerButtonRect = GUILayoutUtility.GetRect(200, 47);
                if (GUI.Button(layerButtonRect, "Create Layers"))
                {
                    CreateLayer(physicsLayer);
                }
                #endregion
                #region Create Tags
                GUI.color = Color.black;
                GUILayout.Label("Tags:-", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
                GUI.color = Color.white;
                Rect tagInputButtonRect = GUILayoutUtility.GetRect(200, 47);
                if (GUI.Button(tagInputButtonRect, "Create Tags"))
                {
                    CreateTag(concreteTag);
                    CreateTag(grassTag);
                    CreateTag(gravelTag);
                    CreateTag(waterTag);
                    CreateTag(metalTag);
                    CreateTag(woodTag);
                }
                #endregion
                GUI.color = Color.red;
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                #region Create Parent Object
                GUI.color = Color.black;
                GUILayout.Label("Supporting Components:-", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
                GUI.color = Color.white;
                Rect parentInputButtonRect = GUILayoutUtility.GetRect(200, 47);
                if (GUI.Button(parentInputButtonRect, "Create Parent Gameobject"))
                {
                    CreateParentObjectAndOtherComponents();
                }
                EditorGUILayout.Space();
                #endregion
            }
            if (enableNonArmatureSetup)
            {
                EditorGUILayout.LabelField("XtremeFPS Player Controller Setup");
                EditorGUILayout.Space(20);
                playerParent = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Player Parent", "Parent of the player gameobject that contains all the necessary components."), playerParent, typeof(GameObject), true);
                #region Create Character Controller
                GUI.color = Color.black;
                GUILayout.Label("Player Setup:-", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
                GUI.color = Color.white;
                playerArmature = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Player GameObject", "The referrence to the player gameObject (Leave empty if none exists already)."), playerArmature, typeof(GameObject), true);
                cameraFollow = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Player Camera Root", "The referrence to the player gameObject (Automatically setted by the editor)."), cameraFollow, typeof(GameObject), true);
                Rect buttonRect = GUILayoutUtility.GetRect(200, 50);
                if (GUI.Button(buttonRect, "Create Player"))
                {
                    CreateThePlayer();
                }
                NearClipingPlanes = EditorGUILayout.Slider(new GUIContent("Near Clipping Planes", "The near limit after which the camera should stop rendering."), NearClipingPlanes, 0.001f, 0.1f);
                FieldOfView = EditorGUILayout.Slider(new GUIContent("Field Of View", "The Field Of View of the camera."), FieldOfView, 30f, 90f);
                Rect setDefaultValues = GUILayoutUtility.GetRect(200, 50);
                if (GUI.Button(setDefaultValues, "Set Default Virtual Camera"))
                {
                    SetVirtualCamera();
                }
                EditorGUILayout.Space(25);
                #endregion

                #region Weapon Setup
                GUI.color = Color.black;
                GUILayout.Label("Weapon Setup:-", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
                GUI.color = Color.white;
                weaponModel = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Weapon Model", "Please drag and drop the weapon model which you want to use."), weaponModel, typeof(GameObject), true);
                particleEffect = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Muzzle Particle Effect", "Please drag and drop the particle effect which you want to use.."), particleEffect, typeof(GameObject), true);

                Rect createWeapon = GUILayoutUtility.GetRect(200, 50);
                if (GUI.Button(createWeapon, "Create Weapon"))
                {
                    SetupTheWeapon();
                }
                #endregion
            }
            EditorGUILayout.EndVertical();
            #endregion

            EditorGUILayout.EndHorizontal();
        }

        private void CompleteTheSettup()
        {
            Debug.Log("Cleaning Up....");
            playerArmature = null;
            virtualCamera = null;
            cameraFollow = null;
            playerParent = null;
            objectPooler = null;
            playerCamera = null;
            cameraHolder = null;
            Debug.Log("Setup Finish.");
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
            if (TagExists(tagName))
            {
                Debug.LogWarning("No available tag slot to create the tag: " + tagName);
                return;
            }
            UnityEditorInternal.InternalEditorUtility.AddTag(tagName);

        }
        #endregion

        #region Player Creation
        private void CreateParentObjectAndOtherComponents()
        {
            if (playerParent == null)
            {
                playerParent = new GameObject("Player Parent");
            }

            if (playerCamera == null)
            {
                playerCamera = new GameObject("Camera Brain");
                playerCamera.transform.parent = playerParent.transform;
                playerCamera.AddComponent<Camera>();
                playerCamera.AddComponent<AudioListener>();
                playerCamera.AddComponent<UniversalAdditionalCameraData>();
                playerCamera.AddComponent<CinemachineBrain>();
            }

            if (virtualCamera == null)
            {
                GameObject playerVirtualCamera = new GameObject("Virtual Camera");
                playerVirtualCamera.transform.parent = playerParent.transform;
                virtualCamera = playerVirtualCamera.AddComponent<CinemachineVirtualCamera>();
            }

            if (objectPooler == null)
            {
                objectPooler = new GameObject("Pool Manager");
                objectPooler.transform.parent = playerParent.transform;
                objectPooler.AddComponent<PoolManager>();
            }
        }
        private void CreateThePlayer()
        {
            if (playerParent == null || virtualCamera == null)
            {
                throw new ParentOrCameraNullException();
            }

            if (playerArmature == null)
            {
                playerArmature = new GameObject("Player Armature");
            }

            playerArmature.transform.parent = playerParent.transform;
            playerArmature.AddComponent<FirstPersonController>();

            if (cameraHolder == null)
            {
                cameraHolder = new GameObject("Camera Holder");
                cameraHolder.transform.parent = playerArmature.transform;
            }

            if (cameraFollow == null)
            {
                cameraFollow = new GameObject("Camera Root");
                cameraFollow.transform.parent = cameraHolder.transform;
            }
        }
        private void SetVirtualCamera()
        {
            virtualCamera.Follow = cameraFollow.transform;
            virtualCamera.m_Lens.NearClipPlane = NearClipingPlanes;
            virtualCamera.m_Lens.FieldOfView = FieldOfView;

            CinemachineComponentBase body = virtualCamera.GetCinemachineComponent(CinemachineCore.Stage.Body);

            if (body is not CinemachineHardLockToTarget)
            {
                virtualCamera.AddCinemachineComponent<CinemachineHardLockToTarget>();
            }

            CinemachineComponentBase aim = virtualCamera.GetCinemachineComponent(CinemachineCore.Stage.Aim);

            if (aim is not CinemachineSameAsFollowTarget)
            {
                virtualCamera.AddCinemachineComponent<CinemachineSameAsFollowTarget>();
            }

            FirstPersonController fpsPlayer = playerArmature.GetComponent<FirstPersonController>();
            fpsPlayer.characterController = playerArmature.GetComponent<CharacterController>();
            fpsPlayer.FOV = FieldOfView;
            fpsPlayer.playerVirtualCamera = virtualCamera;
            fpsPlayer.cameraFollow = cameraFollow.transform;
        }
        #endregion

        #region Weapon Setup
        private void SetupTheWeapon()
        {
            if (weaponModel == null ||
                playerParent == null ||
                cameraFollow == null ||
                playerArmature == null)
            {
                throw new ParentOrCameraNullException();
            }

            GameObject weaponHolder = new GameObject("Weapon Holder");
            weaponHolder.transform.parent = cameraFollow.transform;

            GameObject weaponRecoil = new GameObject("Weapon Recoils");
            weaponRecoil.transform.parent = weaponHolder.transform;

            GameObject weaponObject = new GameObject(weaponModel.transform.name);
            weaponObject.transform.parent = weaponRecoil.transform;
            weaponObject.AddComponent<WeaponSystem>();

            GameObject shootPoint = new GameObject("Shoot Point");
            shootPoint.transform.parent = weaponObject.transform;

            GameObject shellEjectionPoint = new GameObject("Shell Ejection Point");
            shellEjectionPoint.transform.parent = weaponObject.transform;

            GameObject instantiatedWeaponModel = (GameObject)PrefabUtility.InstantiatePrefab(weaponModel);
            instantiatedWeaponModel.transform.parent = weaponObject.transform;
            instantiatedWeaponModel.transform.name = "Weapon Model";

            GameObject instantiatedEffect = (GameObject)PrefabUtility.InstantiatePrefab(particleEffect);
            instantiatedEffect.transform.parent = shootPoint.transform;
        }
        #endregion
    }

    public class ParentOrCameraNullException : Exception { }
}