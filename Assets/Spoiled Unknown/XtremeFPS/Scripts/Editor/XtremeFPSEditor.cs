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
        #region Editor Setup
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

        private const string version = "Version 1.1.0";
        #endregion
        #region Varibales

        #region Editor Bools
        private bool enableAboutPanel = true;
        private bool enableInitialSetupPanel = false;
        private bool enableNonArmatureSetup = false;
        #endregion

        #region Player Setup
        private GameObject playerParent;
        private FirstPersonController playerArmature;
        private Camera playerCamera;
        private CinemachineVirtualCamera virtualCamera;
        private GameObject cameraHolder;
        private GameObject cameraFollow;
        private PoolManager objectPoolerManager;

        private DefaultPlayerTypes defaultPlayerTypes;
        enum DefaultPlayerTypes
        {
            Default
        }

        private const string physicsLayer = "Physics";

        private const string concreteTag = "Concrete";
        private const string grassTag = "Grass";
        private const string gravelTag = "Gravel";
        private const string waterTag = "Water";
        private const string metalTag = "Metals";
        private const string woodTag = "Wood";
        #endregion

        #region Weapon Setup
        //Weapon Related
        private GameObject weaponHolder;
        private GameObject weaponRecoil;
        private WeaponSystem weaponObject;
        private GameObject shootPoint;
        private GameObject shellEjectionPoint;
        private GameObject weaponModel;
        private GameObject instantiatedWeaponModel;
        private GameObject particleEffect;
        private ParticleSystem instantiatedEffect;

        private WeaponTypes weaponTypes;
        enum WeaponTypes
        {
            Pistol,
            AssualtRifle,
            Shotgun,
            Sniper,
            MachineGun,
            SubMachineGun
        }
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
                GUILayout.Label("Made By Spoiled Unknown", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Normal, fontSize = 12 });
                GUI.color = Color.red;
                GUILayout.Label(version, new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Italic, fontSize = 12 });
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
                playerCamera = (Camera)EditorGUILayout.ObjectField(new GUIContent("Player Camera", "The referrence to the player camera (Leave empty if none exists already)."), playerCamera, typeof(Camera), true);
                virtualCamera = (CinemachineVirtualCamera)EditorGUILayout.ObjectField(new GUIContent("Virtual Camera", "The referrence to the virtual camera (Leave empty if none exists already)."), virtualCamera, typeof(CinemachineVirtualCamera), true);
                objectPoolerManager = (PoolManager)EditorGUILayout.ObjectField(new GUIContent("Pool Manager", "The referrence to the object pool manager (Leave empty if none exists already)."), objectPoolerManager, typeof(PoolManager), true);
                Rect buttonRect = GUILayoutUtility.GetRect(200, 50);
                if (GUI.Button(buttonRect, "Create Player"))
                {
                    CreateThePlayer();
                }
                playerArmature = (FirstPersonController)EditorGUILayout.ObjectField(new GUIContent("Player Armature", "The referrence to the player armature gameObject (Leave empty if none exists already)."), playerArmature, typeof(FirstPersonController), true);
                cameraHolder = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Camera Holder", "The referrence to the player camera holder object (Leave empty if none exists already)."), cameraHolder, typeof(GameObject), true);
                cameraFollow = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Camera Follow", "The referrence to the player camera follow (Leave empty if none exists already)."), cameraFollow, typeof(GameObject), true);
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
                weaponModel = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Weapon Model", "Please drag and drop the weapon model which you want to use.."), weaponModel, typeof(GameObject), true);
                particleEffect = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Particle Effect", "Please drag and drop the particle effect which you want to use.."), particleEffect, typeof(GameObject), true);
                Rect createWeaponParent = GUILayoutUtility.GetRect(200, 50);
                if (GUI.Button(createWeaponParent, "Create Weapon"))
                {
                    SetupTheWeapon();
                }
                weaponHolder = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Weapon Holder", "The referrence to the weapon holder object (Leave empty if none exists already)."), weaponHolder, typeof(GameObject), true);
                weaponRecoil = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Weapon Recoil", "The referrence to the weapon recoil object (Leave empty if none exists already)."), weaponRecoil, typeof(GameObject), true);
                weaponObject = (WeaponSystem)EditorGUILayout.ObjectField(new GUIContent("Weapon", "The referrence to the weapon (Leave empty if none exists already)."), weaponObject, typeof(WeaponSystem), true);
                instantiatedWeaponModel = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Model", "The referrence to the model of weapon used, usually the child of 'Weapon' (Leave empty if none exists already)."), instantiatedWeaponModel, typeof(GameObject), true);
                shootPoint = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Shoot Point", "The referrence to the shoot point (Leave empty if none exists already)."), shootPoint, typeof(GameObject), true);
                instantiatedEffect = (ParticleSystem)EditorGUILayout.ObjectField(new GUIContent("Muzzle Particle Effect", "The referrence to the muzzle particle effect, usually the child of 'Model' (Leave empty if none exists already)."), instantiatedEffect, typeof(ParticleSystem), true);
                shellEjectionPoint = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Shell Ejection Point", "The referrence to the shell ejection point (Leave empty if none exists already)."), shellEjectionPoint, typeof(GameObject), true);
                GUI.color = Color.black;
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                GUI.color = Color.white;
                EditorGUILayout.Space(20f);
                weaponTypes = (WeaponTypes)EditorGUILayout.EnumPopup(new GUIContent("Weapon Types", "Select an option from the weapon type for the default settings."), weaponTypes);
                Rect createWeapon = GUILayoutUtility.GetRect(200, 50);
                if (GUI.Button(createWeapon, "Set Weapon Values"))
                {
                    switch (weaponTypes)
                    {
                        case WeaponTypes.Pistol:
                            SetGun();
                            SetPistol();
                            break;

                        case WeaponTypes.AssualtRifle:
                            SetGun();
                            SetAssualtRifle();
                            break;

                        case WeaponTypes.Shotgun: 
                            SetGun();
                            SetShotgun();
                            break;

                        case WeaponTypes.Sniper: 
                            SetGun();
                            SetSniper();
                            break;

                        case WeaponTypes.MachineGun: 
                            SetGun();
                            SetMachineGun();
                            break;

                        case WeaponTypes.SubMachineGun: 
                            SetGun();
                            SetSubMachineGun();
                            break;

                        default: 
                            break;
                    }
                }
                GUI.color = Color.black;
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                GUI.color = Color.white;
                #endregion
            }
            EditorGUILayout.EndVertical();
            #endregion

            EditorGUILayout.EndHorizontal();
        }

        private void CompleteTheSettup()
        {
            float firstTime = Time.realtimeSinceStartup;
            Debug.Log("Cleaning Up....");
            playerParent = null;
            playerArmature = null;
            playerCamera = null;
            virtualCamera = null;
            cameraHolder = null;
            cameraFollow = null;
            objectPoolerManager = null;
            defaultPlayerTypes = DefaultPlayerTypes.Default;

            weaponHolder = null;
            weaponRecoil = null;
            weaponObject = null;
            shellEjectionPoint = null;

            weaponModel = null;
            instantiatedWeaponModel = null;

            particleEffect = null;
            instantiatedEffect = null;
            float secondTime = Time.realtimeSinceStartup;
            Debug.Log($"Setup Finished after {(secondTime - firstTime)} seconds!");
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
            if (playerParent == null)  playerParent = GameObject.Find("Player Parent") ?? new GameObject("Player Parent");

            if (playerCamera == null ||
                !playerCamera.TryGetComponent<Camera>(out _) ||
                !playerCamera.TryGetComponent<AudioListener>(out _) ||
                !playerCamera.TryGetComponent<CinemachineBrain>(out _))
            {
                GameObject playerCameraObject = GameObject.Find("Camera Brain") ?? new GameObject("Camera Brain");
                if (!playerCameraObject.TryGetComponent<Camera>(out _)) playerCameraObject.AddComponent<Camera>();
                if (!playerCameraObject.TryGetComponent<AudioListener>(out _)) playerCameraObject.AddComponent<AudioListener>();
                if (!playerCameraObject.TryGetComponent<CinemachineBrain>(out _)) playerCameraObject.AddComponent<CinemachineBrain>();
                playerCamera = playerCameraObject.GetComponent<Camera>();

#if UNITY_PIPELINE_URP
                if (!playerCameraObject.TryGetComponent<UniversalAdditionalCameraData>(out _))
                    playerCameraObject.AddComponent<UniversalAdditionalCameraData>();

#elif UNITY_PIPELINE_HDRP
                if (!playerCameraObject.TryGetComponent<HDAdditionalCameraData>(out _))
                    playerCameraObject.gameObject.AddComponent<HDAdditionalCameraData>();
#endif
            }
            playerCamera.transform.parent = playerParent.transform;

            if (virtualCamera == null ||
                !virtualCamera.TryGetComponent<CinemachineVirtualCamera>(out _))
            {
                GameObject playerVirtualCamera = GameObject.Find("Virtual Camera") ?? new GameObject("Virtual Camera");
                if (!playerVirtualCamera.TryGetComponent<CinemachineVirtualCamera>(out _)) playerVirtualCamera.AddComponent<CinemachineVirtualCamera>();
                virtualCamera = playerVirtualCamera.GetComponent<CinemachineVirtualCamera>();
            }
            virtualCamera.transform.parent = playerParent.transform;

            if (objectPoolerManager == null ||
                !objectPoolerManager.TryGetComponent<PoolManager>(out _))
            {
                GameObject objectPooler = GameObject.Find("Pool Manager") ?? new GameObject("Pool Manager");
                if (!objectPooler.TryGetComponent<PoolManager>(out _)) objectPooler.AddComponent<PoolManager>();
                objectPoolerManager = objectPooler.GetComponent<PoolManager>();
            }
            objectPoolerManager.transform.parent = playerParent.transform;
        }
        private void CreateThePlayer()
        {
            if (playerParent == null || virtualCamera == null)
            {
                Debug.LogError($"Player Parent or Virtual Camera is null!");
                return;
            }

            if (playerArmature == null ||
                !playerArmature.TryGetComponent<FirstPersonController>(out _))
            {
                GameObject player = GameObject.Find("Player Armature") ?? new GameObject("Player Armature");
                if (!player.TryGetComponent<FirstPersonController>(out _)) player.AddComponent<FirstPersonController>();
                playerArmature = player.GetComponent<FirstPersonController>();
            }
            playerArmature.transform.parent = playerParent.transform;

            if (cameraHolder == null) cameraHolder = GameObject.Find("Camera Holder") ?? new GameObject("Camera Holder");
            cameraHolder.transform.parent = playerArmature.transform;

            if (cameraFollow == null) cameraFollow = GameObject.Find("Camera Follow") ?? new GameObject("Camera Follow");
            cameraFollow.transform.parent = cameraHolder.transform;
        }
        private void SetDefaultValues()
        {
            if (defaultPlayerTypes == DefaultPlayerTypes.Default) DefaultPlayerValues();
        }
        private void DefaultPlayerValues()
        {
            if (virtualCamera == null ||
                cameraFollow == null ||
                playerArmature == null)
            {
                Debug.LogError($"Virtual Camera or Camera Follow or Player Armature is null!");
            }
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
            Debug.LogWarning("If you want to add the touch screen support then switch the player profile to Android/IOS and drag and drop the UI,\n and everything will be handled by the Input Manager script.");
        }
        #endregion

        #region Weapon Setup
        private void SetupTheWeapon()
        {
            if (cameraFollow == null)
            {
                Debug.LogError("Camera Follow is null. Please asign the 'Camera Follow' variable in the 'Player Setup'.");
                return;
            }
            if (weaponModel == null ||
                particleEffect == null)
            {
                Debug.LogError("'Weapon Model' or 'Particle Effect' is null. Please asign them then proceed with setup.");
                return;
            }
            if (weaponHolder == null) weaponHolder = GameObject.Find("Weapon Holder") ?? new GameObject("Weapon Holder");
            weaponHolder.transform.parent = cameraFollow.transform;

            if (weaponRecoil == null) weaponRecoil = GameObject.Find("Weapon Recoils") ?? new GameObject("Weapon Recoils");
            weaponRecoil.transform.parent = weaponHolder.transform;

            if (weaponObject == null ||
                !weaponObject.TryGetComponent<WeaponSystem>(out _)) 
            {
                GameObject tempWeaponObject = GameObject.Find(weaponModel.transform.name) ?? new GameObject(weaponModel.transform.name);
                if (!tempWeaponObject.TryGetComponent<WeaponSystem>(out _)) tempWeaponObject.AddComponent<WeaponSystem>();
                weaponObject = tempWeaponObject.GetComponent<WeaponSystem>();
            }
            weaponObject.transform.parent = weaponRecoil.transform;

            if (shootPoint == null) shootPoint = GameObject.Find("Shoot Point") ?? new GameObject("Shoot Point");
            shootPoint.transform.parent = weaponObject.transform;

            if (shellEjectionPoint == null) shellEjectionPoint = GameObject.Find("Shell Ejection Point") ?? new GameObject("Shell Ejection Point");
            shellEjectionPoint.transform.parent = weaponObject.transform;

            if (instantiatedWeaponModel == null) 
            {
                instantiatedWeaponModel = GameObject.Find("Model") ?? (GameObject)PrefabUtility.InstantiatePrefab(weaponModel);
                if (IsPrefabInstance(instantiatedWeaponModel))PrefabUtility.UnpackPrefabInstance(instantiatedWeaponModel, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
            }
            instantiatedWeaponModel.transform.parent = weaponObject.transform;
            instantiatedWeaponModel.transform.name = "Model";

            if (instantiatedEffect == null) 
            {
                GameObject tempInstantiatedEffect = GameObject.Find(particleEffect.transform.name) ?? (GameObject)PrefabUtility.InstantiatePrefab(particleEffect);
                if (IsPrefabInstance(tempInstantiatedEffect)) PrefabUtility.UnpackPrefabInstance(tempInstantiatedEffect, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
                instantiatedEffect = tempInstantiatedEffect.GetComponent<ParticleSystem>();
            }
            instantiatedEffect.transform.parent = shootPoint.transform;
        }

        private bool IsPrefabInstance(GameObject obj)
        {
            return PrefabUtility.GetPrefabInstanceStatus(obj) == PrefabInstanceStatus.Connected;
        }

        private void SetPistol()
        {
            weaponObject.timeBetweenEachShots = 0.1f;
            weaponObject.timeBetweenShooting = 0.1f;
            weaponObject.magazineSize = 7;
            weaponObject.totalBullets = 35;
            weaponObject.bulletsPerTap = 1;
            weaponObject.reloadTime = 2.3f;

            weaponObject.bulletSpeed = 200;
            weaponObject.bulletLifeTime = 3f;
            weaponObject.bulletGravitationalForce = 200;
        }

        private void SetAssualtRifle()
        {
            weaponObject.timeBetweenEachShots = 0.1f;
            weaponObject.timeBetweenShooting = 0.1f;
            weaponObject.magazineSize = 30;
            weaponObject.totalBullets = 180;
            weaponObject.bulletsPerTap = 1;
            weaponObject.reloadTime = 2.3f;

            weaponObject.bulletSpeed = 300;
            weaponObject.bulletLifeTime = 5f;
            weaponObject.bulletGravitationalForce = 100;
        }

        private void SetShotgun()
        {
            weaponObject.timeBetweenEachShots = 0.1f;
            weaponObject.timeBetweenShooting = 0.1f;
            weaponObject.magazineSize = 25;
            weaponObject.totalBullets = 180;
            weaponObject.bulletsPerTap = 25;
            weaponObject.reloadTime = 0.25f;

            weaponObject.bulletSpeed = 200;
            weaponObject.bulletLifeTime = 2f;
            weaponObject.bulletGravitationalForce = 200;
        }

        private void SetSniper()
        {
            weaponObject.timeBetweenEachShots = 1f;
            weaponObject.timeBetweenShooting = 1f;
            weaponObject.magazineSize = 1;
            weaponObject.totalBullets = 7;
            weaponObject.bulletsPerTap = 1;
            weaponObject.reloadTime = 2.3f;

            weaponObject.bulletSpeed = 600;
            weaponObject.bulletLifeTime = 7f;
            weaponObject.bulletGravitationalForce = 50;
        }

        private void SetMachineGun()
        {
            weaponObject.timeBetweenEachShots = 0.2f;
            weaponObject.timeBetweenShooting = 0.2f;
            weaponObject.magazineSize = 75;
            weaponObject.totalBullets = 300;
            weaponObject.bulletsPerTap = 1;
            weaponObject.reloadTime = 2.3f;

            weaponObject.bulletSpeed = 300;
            weaponObject.bulletLifeTime = 3f;
            weaponObject.bulletGravitationalForce = 150;
        }

        private void SetSubMachineGun()
        {
            weaponObject.timeBetweenEachShots = 0.075f;
            weaponObject.timeBetweenShooting = 0.075f;
            weaponObject.magazineSize = 30;
            weaponObject.totalBullets = 180;
            weaponObject.bulletsPerTap = 1;
            weaponObject.reloadTime = 1.7f;

            weaponObject.bulletSpeed = 200;
            weaponObject.bulletLifeTime = 2f;
            weaponObject.bulletGravitationalForce = 50;
        }

        private void SetGun()
        {
            weaponObject.fpsController = playerArmature;
            weaponObject.shootPoint = shootPoint.transform;
            weaponObject.muzzleFlash = instantiatedEffect;
            weaponObject.ShellPosition = shellEjectionPoint.transform;

            weaponObject.weaponHolder = weaponHolder.transform;
            weaponObject.gunPositionHolder = weaponRecoil.transform;
            weaponObject.cameraRecoilHolder = cameraHolder.transform;

            Debug.LogWarning($"Note: Although all values are automatically set to {weaponTypes},\n but you still have to set some values yourself (like sound files and position of gameObjects).");
        }
        #endregion
    }
}