using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.SceneManagement;

namespace Reborn.Editor
{
    public static class RoomBlockoutBuilder
    {
        public const string ScenePath = "Assets/Reborn/Scenes/RoomBlockout.unity";
        private static Material wall, wood, white, metal, fabric, dark;
        private static Transform root;

        [MenuItem("Reborn/Create Room Blockout")]
        public static void Create()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode) throw new InvalidOperationException("Exit Play mode first.");
            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath)) throw new InvalidOperationException("Room scene already exists.");
            var previous = SceneManager.GetActiveScene();
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
            SceneManager.SetActiveScene(scene);
            try
            {
                wall = Mat("RoomPlaster", new Color(.82f,.81f,.77f));
                wood = Mat("RoomVinyl", new Color(.57f,.42f,.26f));
                white = Mat("RoomFurniture", new Color(.76f,.77f,.74f));
                metal = Mat("RoomMetal", new Color(.38f,.42f,.43f));
                fabric = Mat("RoomFabric", new Color(.30f,.36f,.39f));
                dark = Mat("RoomDark", new Color(.055f,.065f,.07f));
                root = Group("Architecture");
                Box("Floor", V(0,-.1f,2.7f), V(3.4f,.2f,5.4f), wood);
                Box("Ceiling", V(0,2.5f,2.7f), V(3.6f,.15f,5.6f), wall);
                Box("Left wall", V(-1.8f,1.25f,2.7f), V(.2f,2.5f,5.8f), wall);
                Box("Right wall", V(1.8f,1.25f,2.7f), V(.2f,2.5f,5.8f), wall);
                Box("Entry left", V(-.9f,1.25f,-.1f), V(1.6f,2.5f,.2f), wall);
                Box("Entry right", V(1.325f,1.25f,-.1f), V(.75f,2.5f,.2f), wall);
                Box("Entry lintel", V(.425f,2.3f,-.1f), V(1.05f,.4f,.2f), wall);
                Box("Window sill wall", V(0,.55f,5.5f), V(3.6f,1.1f,.2f), wall);
                Box("Window left", V(-1.25f,1.65f,5.5f), V(.9f,1.1f,.2f), wall);
                Box("Window right", V(1.25f,1.65f,5.5f), V(.9f,1.1f,.2f), wall);
                Box("Window upper", V(0,2.35f,5.5f), V(3.6f,.3f,.2f), wall);
                Box("Window frame bottom", V(0,1.12f,5.45f), V(1.65f,.05f,.16f), white);
                Box("Window mullion", V(0,1.65f,5.45f), V(.045f,1.05f,.08f), white);
                Box("Neighbor building", V(0,2.5f,8), V(8,5,.3f), wall);
                for (int i=-2;i<=2;i++) Box("Neighbor window", V(i*1.4f,2.1f,7.82f),V(.75f,1,.025f),metal);
                root = Group("Furniture");
                Box("Kitchen counter", V(-1.4f,.45f,1.85f),V(.6f,.9f,1.7f),white);
                Box("Countertop", V(-1.4f,.92f,1.85f),V(.64f,.045f,1.74f),metal);
                Box("Sink placeholder", V(-1.4f,.95f,2.2f),V(.42f,.025f,.5f),dark);
                Box("Wall cupboard", V(-1.5f,1.95f,1.85f),V(.4f,.65f,1.7f),white);
                Box("Washer front", V(-1.085f,.46f,1.35f),V(.025f,.78f,.58f),metal);
                Box("Washer opening", V(-1.065f,.46f,1.35f),V(.025f,.4f,.4f),dark);
                Box("Refrigerator", V(-1.38f,.85f,3.06f),V(.64f,1.7f,.62f),metal);
                Box("Fridge handle", V(-1.045f,1.1f,2.86f),V(.035f,.3f,.04f),dark);
                Box("Bed base", V(-1.12f,.18f,4.4f),V(1.08f,.36f,1.95f),wood);
                Box("Mattress", V(-1.12f,.43f,4.4f),V(1.06f,.2f,1.95f),white);
                Box("Blanket", V(-1.12f,.545f,4.07f),V(1.065f,.035f,1.24f),fabric);
                Box("Pillow", V(-1.12f,.58f,5.04f),V(.7f,.12f,.32f),white);
                Box("Desk top", V(1.35f,.74f,4.3f),V(.65f,.055f,1.55f),white);
                foreach(float x in new[]{1.08f,1.61f}) foreach(float z in new[]{3.6f,5f})
                    Box("Desk leg", V(x,.36f,z),V(.045f,.72f,.045f),metal);
                Box("Chair seat", V(.72f,.45f,4.23f),V(.43f,.08f,.43f),dark);
                Box("Chair back", V(.49f,.77f,4.23f),V(.06f,.57f,.43f),dark);
                Box("Chair base", V(.72f,.22f,4.23f),V(.12f,.44f,.12f),metal);
                Box("Clothes rail", V(1.43f,1.68f,2.15f),V(.04f,.04f,1.05f),metal);
                foreach(float z in new[]{1.65f,2.65f}) Box("Rail upright",V(1.43f,.84f,z),V(.04f,1.68f,.04f),metal);
                Box("Hanging shirt silhouette", V(1.43f,1.15f,2.1f),V(.38f,.85f,.12f),fabric);
                root = Group("Story placeholders");
                Box("Laptop base",V(1.35f,.79f,4.2f),V(.29f,.025f,.38f),dark);
                Box("Laptop screen",V(1.49f,.92f,4.2f),V(.025f,.26f,.38f),dark);
                Box("Printer",V(1.35f,.89f,4.82f),V(.42f,.23f,.37f),white);
                Box("Diary",V(1.26f,.79f,3.78f),V(.25f,.035f,.19f),fabric);
                Box("Calendar February placeholder",V(-.97f,1.73f,5.38f),V(.29f,.36f,.025f),white);
                Box("Umbrella placeholder",V(1.05f,.34f,.65f),V(.065f,.68f,.065f),dark);
                var small = Cabinet("Reality cabinet",V(1.38f,0,3.05f),.5f,.7f,.45f,out _);
                var large = Cabinet("Subjective cabinet",V(.45f,0,.45f),1.25f,2.1f,.65f,out var drawers);
                var entrance = Box("Real entrance door",V(.425f,1.05f,-.055f),V(1.05f,2.1f,.06f),metal);
                var memory = Box("Memory door - reserved portal",V(1.685f,1.05f,.87f),V(.025f,2.1f,.9f),metal);
                root = Group("Corridor");
                Box("Corridor floor",V(.425f,-.1f,-1.35f),V(1.3f,.2f,2.5f),metal);
                Box("Corridor left",V(-.275f,1.25f,-1.35f),V(.1f,2.5f,2.5f),wall);
                Box("Corridor right",V(1.125f,1.25f,-1.35f),V(.1f,2.5f,2.5f),wall);
                Box("Corridor end",V(.425f,1.25f,-2.65f),V(1.5f,2.5f,.1f),wall);
                var state = new GameObject("Room layout review").AddComponent<RoomBlockoutState>();
                state.Configure(small,large.transform,memory,entrance,drawers);
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Reborn/Prefabs/FirstPersonPlayer.prefab");
                if (!prefab) throw new InvalidOperationException("FirstPersonPlayer prefab missing.");
                var player = (GameObject)PrefabUtility.InstantiatePrefab(prefab,scene);
                player.transform.position = V(.1f,.03f,1.15f);
                Lighting();
                EditorSceneManager.SaveScene(scene,ScenePath); AssetDatabase.SaveAssets();
            }
            finally
            {
                EditorSceneManager.CloseScene(scene,true);
                if(previous.IsValid()) SceneManager.SetActiveScene(previous);
                root = null;
            }
        }

        private static GameObject Cabinet(string name, Vector3 position, float width, float height, float depth, out GameObject drawers)
        {
            var parent = root;
            var group = new GameObject(name); group.transform.SetParent(parent,false);
            root = group.transform;
            Box("Back",V(0,height/2,-depth/2+.025f),V(width,height,.05f),white);
            foreach(float x in new[]{-width/2+.03f,width/2-.03f}) Box("Side",V(x,height/2,0),V(.06f,height,depth),white);
            foreach(float y in new[]{.1f,height-.03f}) Box("Shelf",V(0,y,0),V(width,.06f,depth),white);
            drawers = new GameObject("Three drawers"); drawers.transform.SetParent(root,false); root=drawers.transform;
            for(int i=0;i<3;i++)
            {
                float y=.16f+(height-.2f)/3*(i+.5f);
                Box("Drawer "+(i+1),V(0,y,0),V(width-.13f,(height-.2f)/3-.025f,depth-.07f),white);
                Box("Handle "+(i+1),V(0,y,depth/2+.01f),V(width*.3f,.035f,.045f),metal);
            }
            root=parent; group.transform.localPosition=position; return group;
        }

        private static void Lighting()
        {
            var volume = new GameObject("Blockout exposure",typeof(Volume)).GetComponent<Volume>(); volume.isGlobal=true;
            var profile=ScriptableObject.CreateInstance<VolumeProfile>();
            var exposure=profile.Add<Exposure>(); exposure.mode.Override(ExposureMode.Fixed); exposure.fixedExposure.Override(8f);
            AssetDatabase.CreateAsset(profile,"Assets/Reborn/Materials/RoomExposure.asset"); AssetDatabase.AddObjectToAsset(exposure,profile); volume.sharedProfile=profile;
            foreach(var p in new[]{V(0,2.2f,4.6f),V(0,2.2f,2.5f),V(.2f,2.2f,.5f),V(.4f,2,-1.3f)})
            {
                var go=new GameObject("Blockout soft fill",typeof(Light),typeof(HDAdditionalLightData)); go.transform.position=p;
                var light=go.GetComponent<Light>(); light.type=LightType.Point; light.intensity=180; light.range=8; light.shadows=LightShadows.None;
            }
        }

        private static Vector3 V(float x,float y,float z)=>new Vector3(x,y,z);
        private static Transform Group(string name)=>new GameObject(name).transform;
        private static Material Mat(string name,Color color)
        {
            string path="Assets/Reborn/Materials/"+name+".mat";
            var existing=AssetDatabase.LoadAssetAtPath<Material>(path); if(existing) return existing;
            var m=new Material(Shader.Find("HDRP/Lit")); m.SetColor("_BaseColor",color); m.SetFloat("_Smoothness",.15f);
            AssetDatabase.CreateAsset(m,path); return m;
        }
        private static GameObject Box(string name,Vector3 position,Vector3 scale,Material material)
        {
            var go=GameObject.CreatePrimitive(PrimitiveType.Cube); go.name=name; go.transform.SetParent(root,false);
            go.transform.localPosition=position; go.transform.localScale=scale; go.GetComponent<Renderer>().sharedMaterial=material; return go;
        }
    }
}
