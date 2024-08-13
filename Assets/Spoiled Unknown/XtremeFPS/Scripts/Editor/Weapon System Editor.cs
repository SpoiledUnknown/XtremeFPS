/*Copyright © Spoiled Unknown*/
/*2024*/
/*Note: This is an important editor script*/

using TMPro;
using UnityEditor;
using UnityEngine;

namespace XtremeFPS.Editor
{
    using XtremeFPS.WeaponSystem;
    using XtremeFPS.FPSController;

    [CustomEditor(typeof(UniversalWeaponSystem)), CanEditMultipleObjects]
    public class WeaponSystemEditor : UnityEditor.Editor
    {
        UniversalWeaponSystem uni_WeaponSystem;
        SerializedObject serWeaponSystem;


        private void OnEnable()
        {
            uni_WeaponSystem = (UniversalWeaponSystem)target;
            serWeaponSystem = new SerializedObject(uni_WeaponSystem);
        }

        public override void OnInspectorGUI()
        {
            serWeaponSystem.Update();
            #region Intro
            EditorGUILayout.Space();
            GUI.color = Color.black;
            GUILayout.Label("Xtreme FPS Controller", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 16 });
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUI.color = Color.green;
            GUILayout.Label("Universal Weapon Script", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 16 });
            EditorGUILayout.Space();
            #endregion
            #region References
            GUI.color = Color.black;
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("References", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
            EditorGUILayout.Space();

            //Main Movement Settings
            GUI.color = Color.white;
            uni_WeaponSystem.fpsController = (FirstPersonController)EditorGUILayout.ObjectField(new GUIContent("Player Controller", "Reference to player controller script."), uni_WeaponSystem.fpsController, typeof(FirstPersonController), true);
            uni_WeaponSystem.shootPoint = (Transform)EditorGUILayout.ObjectField(new GUIContent("Shoot Point", "Reference to the transform of the point from where bullets will spawn (Ideally it should be a child of gun model itself)."), uni_WeaponSystem.shootPoint, typeof(Transform), true);
            uni_WeaponSystem.muzzleFlash = (ParticleSystem)EditorGUILayout.ObjectField(new GUIContent("Muzzle Flash", "Reference to the particle system that will be played (the game object should be a child of shootPoint)."), uni_WeaponSystem.muzzleFlash, typeof(ParticleSystem), true);
            uni_WeaponSystem.particlesPrefab = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Hit Effect", "Reference to the GameObject that will be spawned at the point where bullet hits."), uni_WeaponSystem.particlesPrefab, typeof(GameObject), true);
            uni_WeaponSystem.bulletPrefab = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Bullet", "Reference to the bullet gameobject itself (it can be any same but should contain ParabolicBullet script)."), uni_WeaponSystem.bulletPrefab, typeof(GameObject), true);
            uni_WeaponSystem.bulletCount = (TextMeshProUGUI)EditorGUILayout.ObjectField(new GUIContent("Bullet Count", "Reference to the text that shows number of bullets on UI."), uni_WeaponSystem.bulletCount, typeof(TextMeshProUGUI), true);
            uni_WeaponSystem.Shell = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Bullet Shell", "Reference to the GameObject that spawns and works as the bullet shell."), uni_WeaponSystem.Shell, typeof(GameObject), true);
            uni_WeaponSystem.ShellPosition = (Transform)EditorGUILayout.ObjectField(new GUIContent("Shell Position", "Reference to the GameObject where shell will spawn."), uni_WeaponSystem.ShellPosition, typeof(Transform), true);
            uni_WeaponSystem.animator = (Animator)EditorGUILayout.ObjectField(new GUIContent("Animator", "Reference to the Animator where animations are stored/setted up."), uni_WeaponSystem.animator, typeof(Animator), true);
            uni_WeaponSystem.aimUIImage = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Scoped-In Image (Optional)", "(Optional) Reference to the UI gameObject that will be displayed when aiming."), uni_WeaponSystem.aimUIImage, typeof(GameObject), true);
            EditorGUILayout.Space();
            #endregion
            
            #region Weapon Stats

            GUI.color = Color.black;
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("Weapon Stats", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
            EditorGUILayout.Space();

            GUI.color = Color.white;
            uni_WeaponSystem.isGunAuto = EditorGUILayout.ToggleLeft(new GUIContent("Is Gun Auto", "Determines if the gun is automatic or tap-tap."), uni_WeaponSystem.isGunAuto);
            uni_WeaponSystem.timeBetweenEachShots = EditorGUILayout.Slider(new GUIContent("Time Between Each Shots", "Determines the time after which each shots will be fired."), uni_WeaponSystem.timeBetweenEachShots, 0f, 1f);
            uni_WeaponSystem.timeBetweenShooting = EditorGUILayout.Slider(new GUIContent("Time Between Shooting", "Determines the time it will take for weapon to load new bullets."), uni_WeaponSystem.timeBetweenShooting, 0f, 1f);
            uni_WeaponSystem.magazineSize = EditorGUILayout.IntSlider(new GUIContent("Magazine Size", "Determines the number of bullet the weapon will hold."), uni_WeaponSystem.magazineSize, 0, 200);
            uni_WeaponSystem.totalBullets = EditorGUILayout.IntSlider(new GUIContent("Total Bullets", "Determines the number of bullet the player have."), uni_WeaponSystem.totalBullets, 0, 999);
            uni_WeaponSystem.bulletsPerTap = EditorGUILayout.IntSlider(new GUIContent("Bullet Per Shoot", "Determines the number of bullet the player will shoot in single tap/shoot cycle."), uni_WeaponSystem.bulletsPerTap, 0, 30);
            uni_WeaponSystem.reloadTime = EditorGUILayout.Slider(new GUIContent("Reloading Time", "Determines the time weapon takes to reload."), uni_WeaponSystem.reloadTime, 0f, 10f);
            uni_WeaponSystem.hardMode = EditorGUILayout.ToggleLeft(new GUIContent("Hard Mode", "If enabled ammo management and reloading will ac realistically."), uni_WeaponSystem.hardMode);
            EditorGUILayout.Space();
            #endregion
            #region Bullet Physics

            GUI.color = Color.black;
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("Bullet Stats", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
            EditorGUILayout.Space();

            GUI.color = Color.white;
            uni_WeaponSystem.bulletSpeed = EditorGUILayout.Slider(new GUIContent("Bullet Velocity", "The velocity at which the bullet will move."), uni_WeaponSystem.bulletSpeed, 50f, 1500f);
            uni_WeaponSystem.bulletDamage = EditorGUILayout.Slider(new GUIContent("Bullet Damage", "The amount of damage the bullet should deal."), uni_WeaponSystem.bulletDamage, 5f, 150f);
            uni_WeaponSystem.bulletLifeTime = EditorGUILayout.Slider(new GUIContent("Bullet Life", "The time after which the bullet will despawn itself."), uni_WeaponSystem.bulletLifeTime, 1f, 100f);
            uni_WeaponSystem.bulletGravitationalForce = EditorGUILayout.Slider(new GUIContent("Bullet Gravity", "Defines the value of gravity that will act on the bullet."), uni_WeaponSystem.bulletGravitationalForce, 0f, 300f);
            EditorGUILayout.Space();
            #endregion
            #region Aiming Settings

            GUI.color = Color.black;
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("ADS Settings", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
            EditorGUILayout.Space();
            GUI.color = Color.blue;
            GUILayout.Label("Aiming", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
            GUI.color = Color.white;
            uni_WeaponSystem.canAim = EditorGUILayout.ToggleLeft(new GUIContent("ADS", "Determines if the weapon should aim or not."), uni_WeaponSystem.canAim);
            if (uni_WeaponSystem.canAim)
            {
                uni_WeaponSystem.isAimHold = EditorGUILayout.ToggleLeft(new GUIContent("Is Aim Hold", "Determines if the player has to hold aim key or press/tap."), uni_WeaponSystem.isAimHold);
                uni_WeaponSystem.weaponHolder = (Transform)EditorGUILayout.ObjectField(new GUIContent("Weapon Holder", "Reference to the Weapon Holder gameobject (can be anything but must be child of camera and parent of all object needed for weapon system to work)."), uni_WeaponSystem.weaponHolder, typeof(Transform), true);
                uni_WeaponSystem.aimingLocalPosition = EditorGUILayout.Vector3Field(new GUIContent("Aim Position", "Determines the position which the gun will saty at while aiming."), uni_WeaponSystem.aimingLocalPosition);
                uni_WeaponSystem.aimSmoothing = EditorGUILayout.Slider(new GUIContent("Aim Speed", "Determines the speed at which the gun will reach aim state."), uni_WeaponSystem.aimSmoothing, 5f, 100f);
            }
            EditorGUILayout.Space();
            #endregion
            #region Recoil Settings

            GUI.color = Color.black;
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("Recoil Settings", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
            EditorGUILayout.Space();
            GUI.color = Color.blue;
            GUILayout.Label("Weapon Recoil", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
            GUI.color = Color.white;
            uni_WeaponSystem.haveWeaponRecoil = EditorGUILayout.ToggleLeft(new GUIContent("Weapon has Recoil", "Determines if the weapon should have recoil or not."), uni_WeaponSystem.haveWeaponRecoil);
            if (uni_WeaponSystem.haveWeaponRecoil)
            {
                uni_WeaponSystem.gunPositionHolder = (Transform)EditorGUILayout.ObjectField(new GUIContent("Weapon Recoil Holder", "Reference to the transform of GameObject that holds gun for recoils (A Parent object of Gun itself and child of Weapon Holder)."), uni_WeaponSystem.gunPositionHolder, typeof(Transform), true);
                EditorGUILayout.Space(10);
                uni_WeaponSystem.gunRecoilPositionSpeed = EditorGUILayout.Slider(new GUIContent("Positional Speed", "Determines the speed at which the gun will move."), uni_WeaponSystem.gunRecoilPositionSpeed, 0f, 50f);
                uni_WeaponSystem.gunPositionReturnSpeed = EditorGUILayout.Slider(new GUIContent("Positional Return Speed", "Determines the speed at which the gun will return to its usual position."), uni_WeaponSystem.gunPositionReturnSpeed, 0f, 50f);
                uni_WeaponSystem.recoilKickBackAds = EditorGUILayout.Vector3Field(new GUIContent("Kick Back (ADS)", "Determines the positional kick of gun while aiming."), uni_WeaponSystem.recoilKickBackAds);
                uni_WeaponSystem.recoilKickBackHip = EditorGUILayout.Vector3Field(new GUIContent("Kick Back (Hip)", "Determines the positional kick of gun while hipfire."), uni_WeaponSystem.recoilKickBackHip);
                EditorGUILayout.Space(10);
                uni_WeaponSystem.gunRecoilRotationSpeed = EditorGUILayout.Slider(new GUIContent("Rotational Speed", "Determines the speed at which the gun will rotate."), uni_WeaponSystem.gunRecoilRotationSpeed, 0f, 50f);
                uni_WeaponSystem.gunRotationReturnSpeed = EditorGUILayout.Slider(new GUIContent("Rotational Return Speed", "Determines the speed at which the gun will return to its usual rotation."), uni_WeaponSystem.gunRotationReturnSpeed, 0f, 50f);
                uni_WeaponSystem.recoilRotationAds = EditorGUILayout.Vector3Field(new GUIContent("Rotation (ADS)", "Determines the rotational position change of gun while aiming."), uni_WeaponSystem.recoilRotationAds);
                uni_WeaponSystem.recoilRotationHip = EditorGUILayout.Vector3Field(new GUIContent("Rotation (Hip)", "Determines the rotational position change of gun while hipfire."), uni_WeaponSystem.recoilRotationHip);
            }
            EditorGUILayout.Space();
            GUI.color = Color.blue;
            GUILayout.Label("Camera Shaking Recoil", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
            GUI.color = Color.white;
            uni_WeaponSystem.haveCameraRecoil = EditorGUILayout.ToggleLeft(new GUIContent("Have Camera Recoil", "Determines if the camera should have recoil or not."), uni_WeaponSystem.haveCameraRecoil);
            if (uni_WeaponSystem.haveCameraRecoil)
            {
                uni_WeaponSystem.cameraRecoilHolder = (Transform)EditorGUILayout.ObjectField(new GUIContent("Camera Recoil Holder", "Reference to the transform of GameObject that holds camera."), uni_WeaponSystem.cameraRecoilHolder, typeof(Transform), true);
                uni_WeaponSystem.recoilRotationSpeed = EditorGUILayout.Slider(new GUIContent("Recoil Speed", "Determines the speed at which the camera will shake."), uni_WeaponSystem.recoilRotationSpeed, 0f, 50f);
                uni_WeaponSystem.recoilReturnSpeed = EditorGUILayout.Slider(new GUIContent("Recoil Return Speed", "Determines the speed at which the camera will return to its usual position."), uni_WeaponSystem.recoilReturnSpeed, 0f, 50f);
                uni_WeaponSystem.adsFireRecoil = EditorGUILayout.Vector3Field(new GUIContent("Recoil (ADS)", "Determines the recoil camera will feel while aiming."), uni_WeaponSystem.adsFireRecoil);
                uni_WeaponSystem.hipFireRecoil = EditorGUILayout.Vector3Field(new GUIContent("Recoil (Hip)", "Determines the recoil camera will feel while hipfire."), uni_WeaponSystem.hipFireRecoil);
                uni_WeaponSystem.hRecoil = EditorGUILayout.Slider(new GUIContent("Horizontal Recoil Sensitivity", "Determines the speed at which the camera will move horizontally."), uni_WeaponSystem.hRecoil, 0f, 1f);
                uni_WeaponSystem.vRecoil = EditorGUILayout.Slider(new GUIContent("Vertical Recoil Sensitivity", "Determines the speed at which the camera will move vertically."), uni_WeaponSystem.vRecoil, 0f, 1f);
            }
            EditorGUILayout.Space();
            #endregion
            #region Sway & Tilt
            GUI.color = Color.black;
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("Weapon Sway Settings", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
            EditorGUILayout.Space();
            GUI.color = Color.blue;
            GUILayout.Label("Rotational Sway", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
            GUI.color = Color.white;
            uni_WeaponSystem.haveRotationalSway = EditorGUILayout.ToggleLeft(new GUIContent("Have Rotational Sway", "Determines if the gun have rotational sway or not."), uni_WeaponSystem.haveRotationalSway);
            if (uni_WeaponSystem.haveRotationalSway)
            {
                uni_WeaponSystem.rotaionSwayIntensity = EditorGUILayout.Slider(new GUIContent("Intensity", "Determines the intensity at which the gun will rotationally sway."), uni_WeaponSystem.rotaionSwayIntensity, 0f, 25f);
                uni_WeaponSystem.rotationSwaySmoothness = EditorGUILayout.Slider(new GUIContent("Speed", "Determines the speed at which the gun will rotationally sway."), uni_WeaponSystem.rotationSwaySmoothness, 0f, 5f);
            }
            EditorGUILayout.Space();
            GUI.color = Color.blue;
            GUILayout.Label("Jump Sway", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
            GUI.color = Color.white;
            uni_WeaponSystem.haveJumpSway = EditorGUILayout.ToggleLeft(new GUIContent("Have Jump Sway", "Determines if the gun have jump sway or not."), uni_WeaponSystem.haveJumpSway);
            if (uni_WeaponSystem.haveJumpSway)
            {
                uni_WeaponSystem.weaponMaxClamp = EditorGUILayout.Slider(new GUIContent("Maximum Clamp Angle", "Determines the maximum clamp angle."), uni_WeaponSystem.weaponMaxClamp, 0f, 25f);
                uni_WeaponSystem.weaponMinClamp = EditorGUILayout.Slider(new GUIContent("Minimum Clamp Angle", "Determines the minimum clamp angle."), uni_WeaponSystem.weaponMinClamp, 0f, 25f);
                uni_WeaponSystem.jumpIntensity = EditorGUILayout.Slider(new GUIContent("Jump Intensity", "Determines the intensity of rotation when jumping."), uni_WeaponSystem.jumpIntensity, 0f, 25f);
                uni_WeaponSystem.jumpSmooth = EditorGUILayout.Slider(new GUIContent("Jump Smooth", "Determines the speed at which gun will change rotation when jumping."), uni_WeaponSystem.jumpSmooth, 0f, 25f);
                uni_WeaponSystem.landingIntensity = EditorGUILayout.Slider(new GUIContent("Landing Intensity", "Determines the intensity of rotation when landing."), uni_WeaponSystem.landingIntensity, 0f, 25f);
                uni_WeaponSystem.landingSmooth = EditorGUILayout.Slider(new GUIContent("Landing Smooth", "Determines the speed at which gun will change rotation when landing."), uni_WeaponSystem.landingSmooth, 0f, 25f);
                uni_WeaponSystem.recoverySpeed = EditorGUILayout.Slider(new GUIContent("Recovery Speed", "Determines the speed at which gun will recover and reach its usual position/rotation."), uni_WeaponSystem.recoverySpeed, 0f, 100f);
            }
            EditorGUILayout.Space();
            #endregion
            #region Weapon Bobbing
            GUI.color = Color.black;
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("Weapon Bobbing", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
            EditorGUILayout.Space();
            GUI.color = Color.white;
            uni_WeaponSystem.haveBobbing = EditorGUILayout.ToggleLeft(new GUIContent("have Bobbing", "Determines if the gun have bobbing effect or not."), uni_WeaponSystem.haveBobbing);
            if (uni_WeaponSystem.haveBobbing)
            {
                uni_WeaponSystem.magnitude = EditorGUILayout.Slider(new GUIContent("Magnitude", "Determines the magnitude of the bobbing effect."), uni_WeaponSystem.magnitude, 0f, 0.02f);
                uni_WeaponSystem.idleSpeed = EditorGUILayout.Slider(new GUIContent("Idle Speed", "Determines speed of bobbing when not moving."), uni_WeaponSystem.idleSpeed, 0f, 5f);
                uni_WeaponSystem.walkSpeedMultiplier = EditorGUILayout.Slider(new GUIContent("Walk Speed Multiplier", "Determines how fast the weapon should bob depending upon the speed of player."), uni_WeaponSystem.walkSpeedMultiplier, 0f, 10f);
                uni_WeaponSystem.walkSpeedMax = EditorGUILayout.Slider(new GUIContent("Walk Maximum Speed", "Determines the maximum speed the weapon can bob."), uni_WeaponSystem.walkSpeedMax, 0f, 15f);
                uni_WeaponSystem.aimReduction = EditorGUILayout.Slider(new GUIContent("ADS Reduction", "Determines the reduction of speed of bobbing effect when aiming/ taking ADS."), uni_WeaponSystem.aimReduction, 0f, 10f);
            }
            EditorGUILayout.Space();
            #endregion
            #region Sound Setup
            GUI.color = Color.black;
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("Sound Setup", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
            EditorGUILayout.Space();
            GUI.color = Color.white;
            uni_WeaponSystem.bulletSoundClip = (AudioClip)EditorGUILayout.ObjectField(new GUIContent("Fire fx", "The sound that plays when player shoots."), uni_WeaponSystem.bulletSoundClip, typeof(AudioClip), true);
            uni_WeaponSystem.bulletReloadClip = (AudioClip)EditorGUILayout.ObjectField(new GUIContent("Reload fx", "The sound that plays when the player reloads the gun."), uni_WeaponSystem.bulletReloadClip, typeof(AudioClip), true);
            EditorGUILayout.Space();
            GUI.color = Color.black;
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            #endregion
            #region Update Changes
            //Sets any changes from the prefab
            if (GUI.changed)
            {
                EditorUtility.SetDirty(uni_WeaponSystem);
                Undo.RecordObject(uni_WeaponSystem, "Weapon System Change");
                serWeaponSystem.ApplyModifiedProperties();
            }
            #endregion
        }
    }
}

