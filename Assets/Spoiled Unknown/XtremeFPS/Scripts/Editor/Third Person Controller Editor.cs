/*Copyright © Spoiled Unknown*/
/*2024*/
/*Note: This is an important editor script*/

using UnityEditor;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;
using XtremeFPS.TPSController;

namespace XtremeFPS.CustomEditors
{
    [CustomEditor(typeof(ThirdPersonController)), CanEditMultipleObjects]
    public class ThirdPersonControllerEditor : Editor
    {
        ThirdPersonController tpsController;
        SerializedObject serFPS;

        private void OnEnable()
        {
            tpsController = (ThirdPersonController)target;
            serFPS = new SerializedObject(tpsController);
        }

        public override void OnInspectorGUI()
        {
            serFPS.Update();
            #region Intro
            EditorGUILayout.Space();
            GUI.color = Color.black;
            GUILayout.Label("XtremeFPS Controller", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 16 });
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
            tpsController.walkSpeed = EditorGUILayout.Slider(new GUIContent("Walk Speed", "Determines how fast the player will move while walking."), tpsController.walkSpeed, .1f, tpsController.sprintSpeed);
            tpsController.walkSoundSpeed = EditorGUILayout.Slider(new GUIContent("Sound Playback Speed", "Determines the speed at which footstep sounds will play while walking."), tpsController.walkSoundSpeed, 0.1f, 0.5f);
            tpsController.transitionSpeed = EditorGUILayout.Slider(new GUIContent("Transition Speed", "The speed at which any animation should play."), tpsController.transitionSpeed, 1f, 30f);
            EditorGUILayout.Space();
            GUI.color = Color.blue;
            GUILayout.Label("Sprint Settings", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
            GUI.color = Color.white;
            tpsController.canPlayerSprint = EditorGUILayout.ToggleLeft(new GUIContent("Enable Sprinting", "Determines if the player is allowed to sprint."), tpsController.canPlayerSprint);
            if (tpsController.canPlayerSprint)
            {
                tpsController.isSprintHold = EditorGUILayout.ToggleLeft(new GUIContent("Is Sprint Hold", "Determines if the player has to hold sprint key or press/tap."), tpsController.isSprintHold);
                tpsController.sprintSpeed = EditorGUILayout.Slider(new GUIContent("Sprint Speed", "Determines how fast the player will move while sprinting."), tpsController.sprintSpeed, tpsController.walkSpeed, 20f);
                tpsController.sprintSoundSpeed = EditorGUILayout.Slider(new GUIContent("Sound Playback Speed", "Determines the speed at which footstep sounds will play while sprinting."), tpsController.sprintSoundSpeed, 0.1f, 0.5f);
                tpsController.sprintFOV = EditorGUILayout.Slider(new GUIContent("Sprint FOV", "Determines the change in fov while sprinting."), tpsController.sprintFOV, tpsController.FOV, tpsController.FOV + 30f);

                EditorGUI.indentLevel++;
                tpsController.unlimitedSprinting = EditorGUILayout.ToggleLeft(new GUIContent("Unlimited Sprint", "Determines if 'Sprint Duration' is enabled. Turning this on will allow for unlimited sprint."), tpsController.unlimitedSprinting);
                GUI.enabled = !tpsController.unlimitedSprinting;
                tpsController.sprintDuration = EditorGUILayout.Slider(new GUIContent("Sprint Duration", "Determines how long the player can sprint while unlimited sprint is disabled."), tpsController.sprintDuration, 1f, 20f);
                tpsController.sprintCooldown = EditorGUILayout.Slider(new GUIContent("Sprint Cooldown", "Determines how long the recovery time is when the player runs out of sprint."), tpsController.sprintCooldown, .1f, tpsController.sprintDuration);
                tpsController.staminaBar = (Slider)EditorGUILayout.ObjectField(new GUIContent("Stamina Bar (Optional)", "Reference to the stamina bar itself."), tpsController.staminaBar, typeof(Slider), true);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.Space();
            GUI.enabled = true;


            //Jumping and gravity settings
            GUI.color = Color.blue;
            GUILayout.Label("Jump And Gravity Settings", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
            GUI.color = Color.white;
            tpsController.canJump = EditorGUILayout.ToggleLeft(new GUIContent("Enable Player Jump", "Determines if the player is allowed to jump."), tpsController.canJump);
            if (tpsController.canJump)
            {
                tpsController.jumpHeight = EditorGUILayout.Slider(new GUIContent("Jump Height", "Determines how high can the player jump."), tpsController.jumpHeight, 0.1f, 10f);
            }
            tpsController.gravitationalForce = EditorGUILayout.Slider(new GUIContent("Gravitational Force", "Sets the the gravitation force which will act on the player."), tpsController.gravitationalForce, 5f, 40f);
            EditorGUILayout.Space();

            //Crouching settings
            GUI.color = Color.blue;
            GUILayout.Label("Crouch Settings", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
            GUI.color = Color.white;
            tpsController.canPlayerCrouch = EditorGUILayout.ToggleLeft(new GUIContent("Enable Player Crouch", "Determines if the player is allowed to crouch."), tpsController.canPlayerCrouch);
            if (tpsController.canPlayerCrouch)
            {
                tpsController.isCrouchHold = EditorGUILayout.ToggleLeft(new GUIContent("Is Crouch Hold", "Determines if the player has to hold crouch key or press/tap."), tpsController.isCrouchHold);
                tpsController.crouchedHeight = EditorGUILayout.FloatField(new GUIContent("Crouched Height", "Determines the height at which player should crouch."), tpsController.crouchedHeight);
                tpsController.crouchedSpeed = EditorGUILayout.Slider(new GUIContent("Crouched Speed", "Determines the speed at which player will move while crouched."), tpsController.crouchedSpeed, 1f, 5f);
                tpsController.crouchSoundPlayTime = EditorGUILayout.Slider(new GUIContent("Sound Playback Speed", "Determines the speed at which footstep sounds will play while crouched."), tpsController.crouchSoundPlayTime, 0.1f, 0.5f);
                EditorGUILayout.Space();
                tpsController.slidingSpeed = EditorGUILayout.Slider(new GUIContent("Sliding Speed", "Determines the speed at which the player will slide."), tpsController.slidingSpeed, tpsController.sprintSpeed, tpsController.sprintSpeed + 15);
                tpsController.slidingDuration = EditorGUILayout.Slider(new GUIContent("sliding Duration", "Determines how long the player will slide"), tpsController.slidingDuration, 0f, 5f);
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
            tpsController.isCursorLocked = EditorGUILayout.ToggleLeft(new GUIContent("Is Cursor Locked", "Defines whether Cursor is locked."), tpsController.isCursorLocked);
            tpsController.cameraFollow = (Transform)EditorGUILayout.ObjectField(new GUIContent("Camera Root", "Camera root object which acts as look at point for cinemachine."), tpsController.cameraFollow, typeof(Transform), true);
            tpsController.cinemachineCamera = (CinemachineCamera)EditorGUILayout.ObjectField(new GUIContent("Cinemachine Camera", "cinemachine Camera which player uses."), tpsController.cinemachineCamera, typeof(CinemachineCamera), true);
            tpsController.FOV = EditorGUILayout.Slider(new GUIContent("Field Of View", "Determines the default Field Of View for the camera."), tpsController.FOV, 60f, 110f);
            tpsController.mouseSensitivity = EditorGUILayout.Slider(new GUIContent("Sensitivity", "Determines the senstivity at which camera will rotate."), tpsController.mouseSensitivity, 0f, 200f);
            tpsController.maximumClamp = EditorGUILayout.Slider(new GUIContent("Maximum Clamp Angle", "Determines the maximum angle at which the camera can reach while being rotated."), tpsController.maximumClamp, 0f, 90f);
            tpsController.minimumClamp = EditorGUILayout.Slider(new GUIContent("Minimum Clamp Angle", "Determines the minimum angle at which the camera can reach while being rotated."), tpsController.minimumClamp, 0f, -90f);
            EditorGUILayout.Space();

            //Zoom Settings
            GUI.color = Color.blue;
            GUILayout.Label("Zoom Settings", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
            GUI.color = Color.white;
            tpsController.enableZoom = EditorGUILayout.ToggleLeft(new GUIContent("Enable Zoom", "Determines if the player is able to zoom in while playing."), tpsController.enableZoom);
            if (tpsController.enableZoom)
            {
                tpsController.isZoomingHold = EditorGUILayout.ToggleLeft(new GUIContent("Is Zoom Hold", "Determines if the player has to hold zoom key or press/tap."), tpsController.isZoomingHold);
                tpsController.zoomFOV = EditorGUILayout.Slider(new GUIContent("Zoom FOV", "Determines the field of view the camera zooms to."), tpsController.zoomFOV, 20f, tpsController.FOV / 2f);
            }

            //Head Bobbing Settings
            GUI.color = Color.blue;
            GUILayout.Label("Head Bob Settings", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
            GUI.color = Color.white;
            tpsController.canHeadBob = EditorGUILayout.ToggleLeft(new GUIContent("Can Head Bob", "Defines whether player's head can bob or not."), tpsController.canHeadBob);
            if (tpsController.canHeadBob)
            {
                tpsController.headBobAmplitude = EditorGUILayout.Slider(new GUIContent("Head Bob Amplitude", "Determines the amplitude at which nthe head will bob."), tpsController.headBobAmplitude, 0f, 0.1f);
                tpsController.headBobFrequency = EditorGUILayout.Slider(new GUIContent("Head Bob Frequency", "Defines how frequently the head will bob."), tpsController.headBobFrequency, 15f, 25f);
            }
            EditorGUILayout.Space();
            #endregion
            #region Audio Setup
            GUI.color = Color.black;
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("Audio Setup", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
            EditorGUILayout.Space();

            GUI.color = Color.white;
            tpsController.grassTag = EditorGUILayout.TagField(new GUIContent("Grass Tag", "Tag of the gameObject that will act as grass."), tpsController.grassTag);
            SerializedProperty soundGrassProperty = serializedObject.FindProperty("soundGrass");
            EditorGUILayout.PropertyField(soundGrassProperty, new GUIContent("Grass Sound Effect", "The sound that plays as footstep while walking on a grassy surface."), true);
            serializedObject.ApplyModifiedProperties();
            tpsController.concreteTag = EditorGUILayout.TagField(new GUIContent("Concrete Tag", "Tag of the gameObject that will act as concrete."), tpsController.concreteTag);
            SerializedProperty soundConcreteProperty = serializedObject.FindProperty("soundConcrete");
            EditorGUILayout.PropertyField(soundConcreteProperty, new GUIContent("Concrete Sound Effect", "The sound that plays as footstep while walking on a concrete."), true);
            serializedObject.ApplyModifiedProperties();
            tpsController.waterTag = EditorGUILayout.TagField(new GUIContent("Water Tag", "Tag of the gameObject that will act as water."), tpsController.waterTag);
            SerializedProperty soundWaterProperty = serializedObject.FindProperty("soundWater");
            EditorGUILayout.PropertyField(soundWaterProperty, new GUIContent("Water Sound Effect", "The sound that plays as footstep while walking on a water."), true);
            serializedObject.ApplyModifiedProperties();
            tpsController.metalTag = EditorGUILayout.TagField(new GUIContent("Metal Tag", "Tag of the gameObject that will act as metal."), tpsController.metalTag);
            SerializedProperty soundMetalProperty = serializedObject.FindProperty("soundMetal");
            EditorGUILayout.PropertyField(soundMetalProperty, new GUIContent("Metal Sound Effect", "The sound that plays as footstep while walking on a metallic surface."), true);
            serializedObject.ApplyModifiedProperties();
            tpsController.gravelTag = EditorGUILayout.TagField(new GUIContent("Gravel Tag", "Tag of the gameObject that will act as gravel."), tpsController.gravelTag);
            SerializedProperty soundGravelProperty = serializedObject.FindProperty("soundGravel");
            EditorGUILayout.PropertyField(soundGravelProperty, new GUIContent("Gravel Sound Effect", "The sound that plays as footstep while walking on a gravel."), true);
            tpsController.woodTag = EditorGUILayout.TagField(new GUIContent("Wood Tag", "Tag of the gameObject that will act as wood."), tpsController.woodTag);
            serializedObject.ApplyModifiedProperties();
            SerializedProperty soundWoodProperty = serializedObject.FindProperty("soundWood");
            EditorGUILayout.PropertyField(soundWoodProperty, new GUIContent("Wood Sound Effect", "The sound that plays as footstep while walking on wooden surface."), true);
            serializedObject.ApplyModifiedProperties();
            tpsController.jumpingAudioClip = (AudioClip)EditorGUILayout.ObjectField(new GUIContent("Jump Sound Effect", "The sound that plays when the player jumps."), tpsController.jumpingAudioClip, typeof(AudioClip), true);
            tpsController.landingAudioClip = (AudioClip)EditorGUILayout.ObjectField(new GUIContent("Land Sound Effect", "The sound that plays when the player Lands."), tpsController.landingAudioClip, typeof(AudioClip), true);
            tpsController.slidingAudioClip = (AudioClip)EditorGUILayout.ObjectField(new GUIContent("Sliding Sound Effect", "The sound that plays when the player is sliding."), tpsController.slidingAudioClip, typeof(AudioClip), true);
            tpsController.footstepSensitivity = EditorGUILayout.Slider(new GUIContent("Footstep Sensitivity", "Determines how fast the player should move before the footstep plays."), tpsController.footstepSensitivity, 0f, 5f);
            EditorGUILayout.Space();
            #endregion
            #region Physics
            GUI.color = Color.black;
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("Physics Settings", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
            EditorGUILayout.Space();
            GUI.color = Color.white;
            tpsController.interactionRange = EditorGUILayout.Slider(new GUIContent("Interaction Range", "Determines the range in which the player can interact."), tpsController.interactionRange, 0f, 5f);
            tpsController.interactionLayerId = EditorGUILayout.LayerField(new GUIContent("What can be interacted?", "Determines what layers can the player interact with."), tpsController.interactionLayerId);

            tpsController.canPush = EditorGUILayout.ToggleLeft(new GUIContent("Can Push", "Defines whether player can push other objects or not."), tpsController.canPush);
            if (tpsController.canPush)
            {
                tpsController.pushLayerId = EditorGUILayout.LayerField(new GUIContent("What can be pushed?", "Determines what layers can the player push."), tpsController.pushLayerId);
                tpsController.pushStrength = EditorGUILayout.Slider(new GUIContent("Push Strength", "Determines the strength at which the player should push."), tpsController.pushStrength, 0f, 10f);
            }
            GUI.color = Color.black;
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            EditorGUILayout.Space();
            #endregion
            #region Update Changes
            //Sets any changes from the prefab
            if (GUI.changed)
            {
                EditorUtility.SetDirty(tpsController);
                Undo.RecordObject(tpsController, "First Person Controller Change");
                serFPS.ApplyModifiedProperties();
            }
            #endregion
        }
    }
}

