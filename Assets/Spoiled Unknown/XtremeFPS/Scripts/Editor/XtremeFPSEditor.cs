/*Copyright � Spoiled Unknown*/
/*2024*/
/*Note: This is an important editor script*/

using Cinemachine;
using System;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;
#if UNITY_PIPELINE_URP
using UnityEngine.Rendering.Universal;
#endif
#if UNITY_PIPELINE_HDRP
using UnityEngine.Rendering.HighDefinition;
#endif
using XtremeFPS.PoolingSystem;

namespace XtremeFPS.Editor
{
    using XtremeFPS.FPSController;
    using XtremeFPS.InputHandler;
    using XtremeFPS.WeaponSystem;
    using static UnityEngine.GridBrushBase;

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
            this.maxSize = new Vector2(650, 410);
        }


        #endregion
        #region Varibales

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
        private FirstPersonController playerArmature;
        private CinemachineVirtualCamera virtualCamera;
        private GameObject cameraFollow;

        //Weapon Related
        private GameObject weaponModel;
        private GameObject particleEffect;

        //others
        private PoolManager objectPoolerManager;
        private GameObject playerCamera;
        private GameObject cameraHolder;
        #endregion

        enum DefaultPlayerTypes
        {
            None,
            Realistic
        }

        private DefaultPlayerTypes defaultPlayerTypes;

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
            if (GUILayout.Button("Player Setup", GUILayout.Width(200), GUILayout.Height(100)))
            {
                enableInitialSetupPanel = true;
                enableAboutPanel = false;
                enableNonArmatureSetup = false;
            }
            if (GUILayout.Button("Weapon Setup", GUILayout.Width(200), GUILayout.Height(100)))
            {
                enableInitialSetupPanel = false;
                enableAboutPanel = false;
                enableNonArmatureSetup = true;
            }
            if (GUILayout.Button("Complete/Reset Setup", GUILayout.Width(200), GUILayout.Height(100)))
            {
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
                EditorGUILayout.LabelField("XtremeFPS Player Setup");
                #region Create Character Controller
                GUI.color = Color.black;
                GUILayout.Label("Player Setup:-", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
                GUI.color = Color.white;
                Rect tagInputButtonRect = GUILayoutUtility.GetRect(200, 47);
                if (GUI.Button(tagInputButtonRect, "Create Tags & Layers"))
                {
                    CreateTag(concreteTag);
                    CreateTag(grassTag);
                    CreateTag(gravelTag);
                    CreateTag(waterTag);
                    CreateTag(metalTag);
                    CreateTag(woodTag);
                    CreateLayer(physicsLayer);

                }
                EditorGUILayout.Space();
                Rect parentInputButtonRect = GUILayoutUtility.GetRect(200, 47);
                if (GUI.Button(parentInputButtonRect, "Create Parent Gameobject"))
                {
                    CreateParentObjectAndOtherComponents();
                }
                playerParent = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Player Parent", "The referrence to the player parent gameObject (Leave empty if none exists already)."), playerParent, typeof(GameObject), true);
                playerCamera = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Player Camera", "The referrence to the player camera (Leave empty if none exists already)."), playerCamera, typeof(GameObject), true);
                virtualCamera = (CinemachineVirtualCamera)EditorGUILayout.ObjectField(new GUIContent("Virtual Camera", "The referrence to the virtual camera (Leave empty if none exists already)."), virtualCamera, typeof(CinemachineVirtualCamera), true);
                objectPoolerManager = (PoolManager)EditorGUILayout.ObjectField(new GUIContent("Pool Manager", "The referrence to the object pool manager (Leave empty if none exists already)."), objectPoolerManager, typeof(PoolManager), true);
                Rect buttonRect = GUILayoutUtility.GetRect(200, 50);
                if (GUI.Button(buttonRect, "Create Player"))
                {
                    CreateThePlayer();
                }
                playerArmature = (FirstPersonController)EditorGUILayout.ObjectField(new GUIContent("Player Armature", "The referrence to the player armature gameObject (Leave empty if none exists already)."), playerArmature, typeof(FirstPersonController), true);
                cameraHolder = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Camera Holder", "The referrence to the player camera holder object (Leave empty if none exists already)."), cameraHolder, typeof(GameObject), true);
                cameraFollow = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Camera Follow Root", "The referrence to the player camera root (Leave empty if none exists already)."), cameraFollow, typeof(GameObject), true);
                defaultPlayerTypes = (DefaultPlayerTypes)EditorGUILayout.EnumPopup(new GUIContent("Default Values", "Select an option from the player type for the default settings."), defaultPlayerTypes);
                Rect setDefaultValues = GUILayoutUtility.GetRect(200, 50);
                if (GUI.Button(setDefaultValues, "Set Recommended Values"))
                {
                    SetDefaultValues();
                }
                EditorGUILayout.Space(25);
                #endregion
            }
            if (enableNonArmatureSetup)
            {
                EditorGUILayout.LabelField("XtremeFPS Weapon Setup");
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
            objectPoolerManager = null;
            playerCamera = null;
            cameraHolder = null;
            Debug.Log("Setup Finish.");
        }
        #region Create Tags And Layer
        void CreateLayer(string layerName)
        {
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty layers = tagManager.FindProperty("layers");

            for (int i = 0; i < layers.arraySize; i++)
            {
                SerializedProperty layerSP = layers.GetArrayElementAtIndex(i);
                if (layerSP.stringValue.Equals(layerName))
                {
                    Debug.Log("Layer already exists: " + layerName);
                    return;
                }
            }

            for (int i = 0; i < layers.arraySize; i++)
            {
                SerializedProperty layerSP = layers.GetArrayElementAtIndex(i);
                if (string.IsNullOrEmpty(layerSP.stringValue))
                {
                    layerSP.stringValue = layerName;
                    tagManager.ApplyModifiedProperties();
                    Debug.Log("Layer created: " + layerName);
                    return;
                }
            }

            Debug.LogError("No available layer slot to create the layer: " + layerName);
        }

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
                playerParent = GameObject.Find("Player Parent") ?? new GameObject("Player Parent");
            }

            if (playerCamera == null)
            {
                playerCamera = GameObject.Find("Camera Brain") ?? new GameObject("Camera Brain");
                playerCamera.transform.parent = playerParent.transform;
                playerCamera.AddComponent<Camera>();
                playerCamera.AddComponent<AudioListener>();
                playerCamera.AddComponent<CinemachineBrain>();

#if UNITY_PIPELINE_URP
                playerCamera.AddComponent<UniversalAdditionalCameraData>();

#elif UNITY_PIPELINE_HDRP
                playerCamera.gameObject.AddComponent<HDAdditionalCameraData>();

#endif
            }

            if (virtualCamera == null)
            {
                GameObject playerVirtualCamera = GameObject.Find("Virtual Camera") ?? new GameObject("Virtual Camera");
                playerVirtualCamera.transform.parent = playerParent.transform;
                playerVirtualCamera.AddComponent<CinemachineVirtualCamera>();
                virtualCamera = playerVirtualCamera.GetComponent<CinemachineVirtualCamera>();
            }

            if (objectPoolerManager == null)
            {
                GameObject objectPooler = GameObject.Find("Pool Manager") ?? new GameObject("Pool Manager");
                objectPooler.transform.parent = playerParent.transform;
                objectPoolerManager = objectPooler.AddComponent<PoolManager>();
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
                GameObject player = GameObject.Find("Player Armature") ?? new GameObject("Player Armature");
                playerArmature = player.AddComponent<FirstPersonController>();
            }

            playerArmature.transform.parent = playerParent.transform;

            if (cameraHolder == null)
            {
                cameraHolder = GameObject.Find("Camera Holder") ?? new GameObject("Camera Holder");
                cameraHolder.transform.parent = playerArmature.transform;
            }

            if (cameraFollow == null)
            {
                cameraFollow = GameObject.Find("Camera Root") ?? new GameObject("Camera Root");
                cameraFollow.transform.parent = cameraHolder.transform;
            }
        }
        private void SetDefaultValues()
        {
            if (defaultPlayerTypes == DefaultPlayerTypes.None) return;
            else if (defaultPlayerTypes == DefaultPlayerTypes.Realistic) RealisticPlayerValues();
        }

        void RealisticPlayerValues()
        {
            if (virtualCamera == null ||
                cameraFollow == null ||
                playerArmature == null) throw new VirtualCameraOrCameraFollowOrPlayerArmatureNullException();
            virtualCamera.Follow = cameraFollow.transform;

            playerArmature.transitionSpeed = 10f;
            playerArmature.walkSpeed = 2f;
            playerArmature.walkSoundSpeed = 0.3f;
            playerArmature.canPlayerSprint = true;
            playerArmature.sprintSpeed = 4f;
            playerArmature.sprintDuration = 8f;
            playerArmature.sprintCooldown = 8f;
            playerArmature.sprintSoundSpeed = 0.25f;
            playerArmature.canJump = true;
            playerArmature.jumpHeight = 1.89f;
            playerArmature.gravitationalForce = 10f;
            playerArmature.canPlayerCrouch = true;
            playerArmature.crouchedHeight = 1f;
            playerArmature.crouchedSpeed = 1f;
            playerArmature.crouchSoundPlayTime = 0.3f;
            playerArmature.slidingSpeed = 10f;
            playerArmature.slidingDuration = 0.75f;
            playerArmature.isCursorLocked = true;
            playerArmature.mouseSensitivity = 50f;
            playerArmature.maximumClamp = 90f;
            playerArmature.minimumClamp = -90f;
            playerArmature.sprintFOV = 75f;
            playerArmature.FOV = 50f;
            playerArmature.enableZoom = false;
            playerArmature.canHeadBob = false;

            playerArmature.playerVirtualCamera = virtualCamera;
            playerArmature.cameraFollow = cameraFollow.transform;

            cameraHolder.transform.position = new Vector3(0f, 0.6150001f, 0.1719999f);
            playerParent.transform.position = new Vector3(
                playerParent.transform.position.x,
                playerParent.transform.position.y + 2f,
                playerParent.transform.position.z
                );

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
            Debug.LogWarning($"Note: Although all values are automatically set to {defaultPlayerTypes},\n but you still have to set some values yourself (like sound files).");
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

            GameObject weaponHolder = GameObject.Find("Weapon Holder") ?? new GameObject("Weapon Holder");
            weaponHolder.transform.parent = cameraFollow.transform;

            GameObject weaponRecoil = GameObject.Find("Weapon Recoils") ?? new GameObject("Weapon Recoils");
            weaponRecoil.transform.parent = weaponHolder.transform;

            GameObject weaponObject = new GameObject(weaponModel.transform.name);
            weaponObject.transform.parent = weaponRecoil.transform;
            weaponObject.AddComponent<WeaponSystem>();

            GameObject shootPoint = GameObject.Find("Shoot Point") ?? new GameObject("Shoot Point");
            shootPoint.transform.parent = weaponObject.transform;

            GameObject shellEjectionPoint = GameObject.Find("Shell Ejection Point") ?? new GameObject("Shell Ejection Point");
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
    public class VirtualCameraOrCameraFollowOrPlayerArmatureNullException : Exception { }
}