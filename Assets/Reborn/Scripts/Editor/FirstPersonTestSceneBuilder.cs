using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.SceneManagement;

namespace Reborn.Editor
{
    public static class FirstPersonTestSceneBuilder
    {
        public const string ScenePath = "Assets/Reborn/Scenes/FirstPersonTest.unity";

        [MenuItem("Reborn/Create First Person Test Scene")]
        public static void Create()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode) throw new InvalidOperationException("Exit Play mode first.");
            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath) != null)
                throw new InvalidOperationException("Test scene already exists; open it instead of overwriting.");
            var previous = SceneManager.GetActiveScene();
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
            SceneManager.SetActiveScene(scene);
            try
            {
                var floor = Material("TestFloor", new Color(0.34f, 0.40f, 0.43f));
                var wall = Material("TestWall", new Color(0.72f, 0.74f, 0.70f));
                var orange = Material("TestMarker", new Color(0.85f, 0.32f, 0.08f));
                Box("Floor", new Vector3(0, -0.15f, 0), new Vector3(12, 0.3f, 12), floor);
                Box("North wall", new Vector3(0, 1.5f, 6), new Vector3(12, 3, 0.3f), wall);
                Box("South wall", new Vector3(0, 1.5f, -6), new Vector3(12, 3, 0.3f), wall);
                Box("East wall", new Vector3(6, 1.5f, 0), new Vector3(0.3f, 3, 12), wall);
                Box("West wall", new Vector3(-6, 1.5f, 0), new Vector3(0.3f, 3, 12), wall);
                Box("Collision block", new Vector3(0, 0.75f, 1), new Vector3(2, 1.5f, 1), orange);
                Box("Door left", new Vector3(-3.1f, 1.2f, 2.5f), new Vector3(0.8f, 2.4f, 0.3f), wall);
                Box("Door right", new Vector3(-1.3f, 1.2f, 2.5f), new Vector3(0.8f, 2.4f, 0.3f), wall);
                Box("Door lintel", new Vector3(-2.2f, 2.3f, 2.5f), new Vector3(1, 0.2f, 0.3f), wall);
                for (int i = -5; i <= 5; i++)
                    Box("Meter stripe " + i, new Vector3(3, 0.006f, i), new Vector3(1, 0.01f, 0.04f), orange);

                var player = new GameObject("Player");
                player.transform.position = new Vector3(0, 0.05f, -4);
                var cc = player.AddComponent<CharacterController>();
                cc.height = 1.8f; cc.radius = 0.25f; cc.center = new Vector3(0, 0.9f, 0);
                cc.stepOffset = 0.2f; cc.skinWidth = 0.025f; cc.minMoveDistance = 0f;
                var cameraObject = new GameObject("PlayerCamera", typeof(Camera), typeof(AudioListener), typeof(HDAdditionalCameraData));
                cameraObject.tag = "MainCamera";
                cameraObject.transform.SetParent(player.transform, false);
                cameraObject.transform.localPosition = new Vector3(0, 1.6f, 0);
                var camera = cameraObject.GetComponent<Camera>();
                camera.fieldOfView = 60; camera.nearClipPlane = 0.05f;
                cameraObject.GetComponent<HDAdditionalCameraData>().backgroundColorHDR = new Color(0.3f, 0.4f, 0.5f);
                player.AddComponent<FirstPersonController>();
                PrefabUtility.SaveAsPrefabAsset(player, "Assets/Reborn/Prefabs/FirstPersonPlayer.prefab");

                var sun = new GameObject("Sun", typeof(Light), typeof(HDAdditionalLightData));
                sun.transform.rotation = Quaternion.Euler(50, -30, 0);
                var light = sun.GetComponent<Light>(); light.type = LightType.Directional;
                light.intensity = 10000f;
                var volume = new GameObject("Exposure", typeof(Volume)).GetComponent<Volume>();
                volume.isGlobal = true;
                var profile = ScriptableObject.CreateInstance<VolumeProfile>();
                var exposure = profile.Add<Exposure>();
                exposure.mode.Override(ExposureMode.Fixed); exposure.fixedExposure.Override(10f);
                AssetDatabase.CreateAsset(profile, "Assets/Reborn/Materials/TestExposure.asset");
                AssetDatabase.AddObjectToAsset(exposure, profile);
                volume.sharedProfile = profile;
                EditorSceneManager.SaveScene(scene, ScenePath);
                AssetDatabase.SaveAssets();
                Debug.Log("Reborn first person test scene created: " + ScenePath);
            }
            finally
            {
                EditorSceneManager.CloseScene(scene, true);
                if (previous.IsValid()) SceneManager.SetActiveScene(previous);
            }
        }

        private static Material Material(string name, Color color)
        {
            var material = new Material(Shader.Find("HDRP/Lit"));
            material.SetColor("_BaseColor", color); material.SetFloat("_Smoothness", 0.15f);
            AssetDatabase.CreateAsset(material, "Assets/Reborn/Materials/" + name + ".mat");
            return material;
        }

        private static void Box(string name, Vector3 position, Vector3 scale, Material material)
        {
            var obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obj.name = name; obj.transform.position = position; obj.transform.localScale = scale;
            obj.GetComponent<Renderer>().sharedMaterial = material;
        }
    }
}
