/*Copyright © Spoiled Unknown*/
/*2024*/
/*Note: This is an important editor script*/

using UnityEditor;
using Cinemachine;
using UnityEngine;
using UnityEngine.UI;
using XtremeFPS.FPSController;

namespace XtremeFPS.Editor
{
    [CustomEditor(typeof(FirstPersonController)), CanEditMultipleObjects]
    public class FirstPersonControllerEditor : UnityEditor.Editor
    {
        FirstPersonController fpsController;
        SerializedObject serFPS;

        private void OnEnable()
        {
            fpsController = (FirstPersonController)target;
            serFPS = new SerializedObject(fpsController);
        }

        public override void OnInspectorGUI()
        {
            serFPS.Update();
            #region Intro
            EditorGUILayout.Space();
            GUI.color = Color.black;
            GUILayout.Label("Xtreme FPS Controller", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 16 });
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUI.color = Color.green;
            GUILayout.Label("First Person Controller Script", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 16 });
            EditorGUILayout.Space();
            #endregion
            #region Player Movement
            GUI.color = Color.black;
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("Player Movement Setup", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
            EditorGUILayout.Space();

            //Main Movement Settings
            GUI.color = Color.blue;
            GUILayout.Label("Walk Settings", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
            GUI.color = Color.white;
            fpsController.walkSpeed = EditorGUILayout.Slider(new GUIContent("Walk Speed", "Determines how fast the player will move while walking."), fpsController.walkSpeed, .1f, fpsController.sprintSpeed);
            fpsController.walkSoundSpeed = EditorGUILayout.Slider(new GUIContent("Sound Playback Speed", "Determines the speed at which footstep sounds will play while walking."), fpsController.walkSoundSpeed, 0.1f, 0.5f);
            fpsController.transitionSpeed = EditorGUILayout.Slider(new GUIContent("Transition Speed", "The speed at which any animation should play."), fpsController.transitionSpeed, 1f, 30f);
            EditorGUILayout.Space();
            GUI.color = Color.blue;
            GUILayout.Label("Sprint Settings", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
            GUI.color = Color.white;
            fpsController.canPlayerSprint = EditorGUILayout.ToggleLeft(new GUIContent("Enable Sprinting", "Determines if the player is allowed to sprint."), fpsController.canPlayerSprint);
            if (fpsController.canPlayerSprint)
            {
                fpsController.isSprintHold = EditorGUILayout.ToggleLeft(new GUIContent("Is Sprint Hold", "Determines if the player has to hold sprint key or press/tap."), fpsController.isSprintHold);
                fpsController.sprintSpeed = EditorGUILayout.Slider(new GUIContent("Sprint Speed", "Determines how fast the player will move while sprinting."), fpsController.sprintSpeed, fpsController.walkSpeed, 20f);
                fpsController.sprintSoundSpeed = EditorGUILayout.Slider(new GUIContent("Sound Playback Speed", "Determines the speed at which footstep sounds will play while sprinting."), fpsController.sprintSoundSpeed, 0.1f, 0.5f);
                fpsController.sprintFOV = EditorGUILayout.Slider(new GUIContent("Sprint FOV", "Determines the change in fov while sprinting."), fpsController.sprintFOV, fpsController.FOV, fpsController.FOV + 30f);

                EditorGUI.indentLevel++;
                fpsController.unlimitedSprinting = EditorGUILayout.ToggleLeft(new GUIContent("Unlimited Sprint", "Determines if 'Sprint Duration' is enabled. Turning this on will allow for unlimited sprint."), fpsController.unlimitedSprinting);
                GUI.enabled = !fpsController.unlimitedSprinting;
                fpsController.sprintDuration = EditorGUILayout.Slider(new GUIContent("Sprint Duration", "Determines how long the player can sprint while unlimited sprint is disabled."), fpsController.sprintDuration, 1f, 20f);
                fpsController.sprintCooldown = EditorGUILayout.Slider(new GUIContent("Sprint Cooldown", "Determines how long the recovery time is when the player runs out of sprint."), fpsController.sprintCooldown, .1f, fpsController.sprintDuration);
                fpsController.staminaBar = (Slider)EditorGUILayout.ObjectField(new GUIContent("Stamina Bar (Optional)", "Reference to the stamina bar itself."), fpsController.staminaBar, typeof(Slider), true);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.Space();
            GUI.enabled = true;


            //Jumping and gravity settings
            GUI.color = Color.blue;
            GUILayout.Label("Jump And Gravity Settings", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
            GUI.color = Color.white;
            fpsController.canJump = EditorGUILayout.ToggleLeft(new GUIContent("Enable Player Jump", "Determines if the player is allowed to jump."), fpsController.canJump);
            if (fpsController.canJump)
            {
                fpsController.jumpHeight = EditorGUILayout.Slider(new GUIContent("Jump Height", "Determines how high can the player jump."), fpsController.jumpHeight, 0.1f, 10f);
            }
            fpsController.gravitationalForce = EditorGUILayout.Slider(new GUIContent("Gravitational Force", "Sets the the gravitation force which will act on the player."), fpsController.gravitationalForce, 5f, 40f);
            EditorGUILayout.Space();

            //Crouching settings
            GUI.color = Color.blue;
            GUILayout.Label("Crouch Settings", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
            GUI.color = Color.white;
            fpsController.canPlayerCrouch = EditorGUILayout.ToggleLeft(new GUIContent("Enable Player Crouch", "Determines if the player is allowed to crouch."), fpsController.canPlayerCrouch);
            if (fpsController.canPlayerCrouch)
            {
                fpsController.isCrouchHold = EditorGUILayout.ToggleLeft(new GUIContent("Is Crouch Hold", "Determines if the player has to hold crouch key or press/tap."), fpsController.isCrouchHold);
                fpsController.crouchedHeight = EditorGUILayout.FloatField(new GUIContent("Crouched Height", "Determines the height at which player should crouch."), fpsController.crouchedHeight);
                fpsController.crouchedSpeed = EditorGUILayout.Slider(new GUIContent("Crouched Speed", "Determines the speed at which player will move while crouched."), fpsController.crouchedSpeed, 1f, 5f);
                fpsController.crouchSoundPlayTime = EditorGUILayout.Slider(new GUIContent("Sound Playback Speed", "Determines the speed at which footstep sounds will play while crouched."), fpsController.crouchSoundPlayTime, 0.1f, 0.5f);
                EditorGUILayout.Space();
                fpsController.slidingSpeed = EditorGUILayout.Slider(new GUIContent("Sliding Speed", "Determines the speed at which the player will slide."), fpsController.slidingSpeed, fpsController.sprintSpeed, fpsController.sprintSpeed + 15);
                fpsController.slidingDuration = EditorGUILayout.Slider(new GUIContent("sliding Duration", "Determines how long the player will slide"), fpsController.slidingDuration, 0f, 5f);
            }
            EditorGUILayout.Space();
            #endregion
            #region Camera Setup
            GUI.color = Color.black;
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("Player Camera Setup", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
            EditorGUILayout.Space();

            //Main Camera Settings
            GUI.color = Color.blue;
            GUILayout.Label("Camera Settings", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
            GUI.color = Color.white;
            fpsController.isCursorLocked = EditorGUILayout.ToggleLeft(new GUIContent("Is Cursor Locked", "Defines whether Cursor is locked."), fpsController.isCursorLocked);
            fpsController.cameraFollow = (Transform)EditorGUILayout.ObjectField(new GUIContent("Camera Root", "Camera root object which acts as look at point for cinemachine."), fpsController.cameraFollow, typeof(Transform), true);
            fpsController.playerVirtualCamera = (CinemachineVirtualCamera)EditorGUILayout.ObjectField(new GUIContent("Player Virtual Camera", "virtual Camera which player uses."), fpsController.playerVirtualCamera, typeof(CinemachineVirtualCamera), true);
            fpsController.FOV = EditorGUILayout.Slider(new GUIContent("Field Of View", "Determines the default Field Of View for the camera."), fpsController.FOV, 60f, 110f);
            fpsController.mouseSensitivity = EditorGUILayout.Slider(new GUIContent("Sensitivity", "Determines the senstivity at which camera will rotate."), fpsController.mouseSensitivity, 0f, 200f);
            fpsController.maximumClamp = EditorGUILayout.Slider(new GUIContent("Maximum Clamp Angle", "Determines the maximum angle at which the camera can reach while being rotated."), fpsController.maximumClamp, 0f, 90f);
            fpsController.minimumClamp = EditorGUILayout.Slider(new GUIContent("Minimum Clamp Angle", "Determines the minimum angle at which the camera can reach while being rotated."), fpsController.minimumClamp, 0f, -90f);
            EditorGUILayout.Space();

            //Zoom Settings
            GUI.color = Color.blue;
            GUILayout.Label("Zoom Settings", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
            GUI.color = Color.white;
            fpsController.enableZoom = EditorGUILayout.ToggleLeft(new GUIContent("Enable Zoom", "Determines if the player is able to zoom in while playing."), fpsController.enableZoom);
            if (fpsController.enableZoom)
            {
                fpsController.isZoomingHold = EditorGUILayout.ToggleLeft(new GUIContent("Is Zoom Hold", "Determines if the player has to hold zoom key or press/tap."), fpsController.isZoomingHold);
                fpsController.zoomFOV = EditorGUILayout.Slider(new GUIContent("Zoom FOV", "Determines the field of view the camera zooms to."), fpsController.zoomFOV, 20f, fpsController.FOV / 2f);
            }

            //Head Bobbing Settings
            GUI.color = Color.blue;
            GUILayout.Label("Head Bob Settings", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
            GUI.color = Color.white;
            fpsController.canHeadBob = EditorGUILayout.ToggleLeft(new GUIContent("Can Head Bob", "Defines whether player's head can bob or not."), fpsController.canHeadBob);
            if (fpsController.canHeadBob)
            {
                fpsController.headBobAmplitude = EditorGUILayout.Slider(new GUIContent("Head Bob Amplitude", "Determines the amplitude at which nthe head will bob."), fpsController.headBobAmplitude, 0f, 0.1f);
                fpsController.headBobFrequency = EditorGUILayout.Slider(new GUIContent("Head Bob Frequency", "Defines how frequently the head will bob."), fpsController.headBobFrequency, 15f, 25f);
            }
            EditorGUILayout.Space();
            #endregion
            #region Audio Setup
            GUI.color = Color.black;
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("Audio Setup", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
            EditorGUILayout.Space();

            GUI.color = Color.white;
            fpsController.grassTag = EditorGUILayout.TagField(new GUIContent("Grass Tag", "Tag of the gameObject that will act as grass."), fpsController.grassTag);
            SerializedProperty soundGrassProperty = serializedObject.FindProperty("soundGrass");
            EditorGUILayout.PropertyField(soundGrassProperty, new GUIContent("Grass Sound Effect", "The sound that plays as footstep while walking on a grassy surface."), true);
            serializedObject.ApplyModifiedProperties();
            fpsController.concreteTag = EditorGUILayout.TagField(new GUIContent("Concrete Tag", "Tag of the gameObject that will act as concrete."), fpsController.concreteTag);
            SerializedProperty soundConcreteProperty = serializedObject.FindProperty("soundConcrete");
            EditorGUILayout.PropertyField(soundConcreteProperty, new GUIContent("Concrete Sound Effect", "The sound that plays as footstep while walking on a concrete."), true);
            serializedObject.ApplyModifiedProperties();
            fpsController.waterTag = EditorGUILayout.TagField(new GUIContent("Water Tag", "Tag of the gameObject that will act as water."), fpsController.waterTag);
            SerializedProperty soundWaterProperty = serializedObject.FindProperty("soundWater");
            EditorGUILayout.PropertyField(soundWaterProperty, new GUIContent("Water Sound Effect", "The sound that plays as footstep while walking on a water."), true);
            serializedObject.ApplyModifiedProperties();
            fpsController.metalTag = EditorGUILayout.TagField(new GUIContent("Metal Tag", "Tag of the gameObject that will act as metal."), fpsController.metalTag);
            SerializedProperty soundMetalProperty = serializedObject.FindProperty("soundMetal");
            EditorGUILayout.PropertyField(soundMetalProperty, new GUIContent("Metal Sound Effect", "The sound that plays as footstep while walking on a metallic surface."), true);
            serializedObject.ApplyModifiedProperties();
            fpsController.gravelTag = EditorGUILayout.TagField(new GUIContent("Gravel Tag", "Tag of the gameObject that will act as gravel."), fpsController.gravelTag);
            SerializedProperty soundGravelProperty = serializedObject.FindProperty("soundGravel");
            EditorGUILayout.PropertyField(soundGravelProperty, new GUIContent("Gravel Sound Effect", "The sound that plays as footstep while walking on a gravel."), true);
            fpsController.woodTag = EditorGUILayout.TagField(new GUIContent("Wood Tag", "Tag of the gameObject that will act as wood."), fpsController.woodTag);
            serializedObject.ApplyModifiedProperties();
            SerializedProperty soundWoodProperty = serializedObject.FindProperty("soundWood");
            EditorGUILayout.PropertyField(soundWoodProperty, new GUIContent("Wood Sound Effect", "The sound that plays as footstep while walking on wooden surface."), true);
            serializedObject.ApplyModifiedProperties();
            fpsController.jumpingAudioClip = (AudioClip)EditorGUILayout.ObjectField(new GUIContent("Jump Sound Effect", "The sound that plays when the player jumps."), fpsController.jumpingAudioClip, typeof(AudioClip), true);
            fpsController.landingAudioClip = (AudioClip)EditorGUILayout.ObjectField(new GUIContent("Land Sound Effect", "The sound that plays when the player Lands."), fpsController.landingAudioClip, typeof(AudioClip), true);
            fpsController.slidingAudioClip = (AudioClip)EditorGUILayout.ObjectField(new GUIContent("Sliding Sound Effect", "The sound that plays when the player is sliding."), fpsController.slidingAudioClip, typeof(AudioClip), true);
            fpsController.footstepSensitivity = EditorGUILayout.Slider(new GUIContent("Footstep Sensitivity", "Determines how fast the player should move before the footstep plays."), fpsController.footstepSensitivity, 0f, 5f);
            EditorGUILayout.Space();
            #endregion
            #region Physics
            GUI.color = Color.black;
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("Physics Settings", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
            EditorGUILayout.Space();
            GUI.color = Color.white;
            fpsController.interactionRange = EditorGUILayout.Slider(new GUIContent("Interaction Range", "Determines the range in which the player can interact."), fpsController.interactionRange, 0f, 5f);
            fpsController.interactionLayersID = EditorGUILayout.LayerField(new GUIContent("What can be interacted?", "Determines what layers can the player interact with."), fpsController.interactionLayersID);
            fpsController.canPush = EditorGUILayout.ToggleLeft(new GUIContent("Can Push", "Defines whether player can push other objects or not."), fpsController.canPush);
            if (fpsController.canPush)
            {
                fpsController.pushLayersID = EditorGUILayout.LayerField(new GUIContent("What can be pushed?", "Determines what layers can the player push."), fpsController.pushLayersID);
                fpsController.pushStrength = EditorGUILayout.Slider(new GUIContent("Push Strength", "Determines the strength at which the player should push."), fpsController.pushStrength, 0f, 10f);
            }
            GUI.color = Color.black;
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            EditorGUILayout.Space();
            #endregion
            #region Update Changes
            //Sets any changes from the prefab
            if (GUI.changed)
            {
                EditorUtility.SetDirty(fpsController);
                Undo.RecordObject(fpsController, "First Person Controller Change");
                serFPS.ApplyModifiedProperties();
            }
            #endregion
        }

    }
}

