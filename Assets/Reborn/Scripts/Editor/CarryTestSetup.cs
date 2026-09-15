using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Reborn.Editor
{
    public static class CarryTestSetup
    {
        public const string ScenePath="Assets/Reborn/Scenes/CarryTest.unity";
        [MenuItem("Reborn/Create Carry Test")]
        public static void Create()
        {
            if(EditorApplication.isPlayingOrWillChangePlaymode) throw new InvalidOperationException("Exit Play mode.");
            if(AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath)) throw new InvalidOperationException("Carry test already exists.");
            var previous=SceneManager.GetActiveScene();
            if(!AssetDatabase.CopyAsset(RoomBlockoutBuilder.ScenePath,ScenePath)) throw new Exception("Room copy failed.");
            var scene=EditorSceneManager.OpenScene(ScenePath,OpenSceneMode.Additive);
            SceneManager.SetActiveScene(scene);
            try
            {
                var root=new GameObject("Carry test fixtures");
                var paper=Mat("CarryPaper",new Color(.85f,.8f,.65f));
                var blue=Mat("CarryTarget",new Color(.08f,.3f,.55f));
                var orange=Mat("CarryOtherTarget",new Color(.6f,.25f,.08f));
                var green=Mat("CarryComplete",new Color(.1f,.7f,.22f));
                var item=Box("Test envelope",new Vector3(1.38f,.73f,3.05f),new Vector3(.25f,.035f,.18f),paper,root.transform);
                item.AddComponent<InspectableObject>().Configure("test.envelope","자료 봉투 (검증용)","들고 가기를 선택한 뒤 주방의 파란 함에 놓아보자.\n주황 함은 다른 자료용이다.\n\n본편 퍼즐과 무관한 운반·사용 검증용 소품.");
                item.AddComponent<CarryableObject>();
                Target("Blue tray",new Vector3(-1.4f,.985f,2.35f),"test.envelope","자료 보관함",blue,green,root.transform);
                Target("Orange tray",new Vector3(-1.4f,.985f,1.5f),"test.other","다른 자료 보관함",orange,green,root.transform);
                EditorSceneManager.SaveScene(scene); AssetDatabase.SaveAssets();
            }
            finally
            {
                EditorSceneManager.CloseScene(scene,true);
                if(previous.IsValid()) SceneManager.SetActiveScene(previous);
            }
        }
        private static void Target(string name,Vector3 pos,string accepted,string title,Material mat,Material green,Transform parent)
        {
            var go=Box(name,pos,new Vector3(.44f,.06f,.5f),mat,parent);
            var anchor=new GameObject(name+" placement").transform;
            anchor.SetParent(parent); anchor.position=pos+Vector3.up*.055f;
            var indicator=Box(name+" complete",pos+new Vector3(-.15f,.13f,.17f),Vector3.one*.1f,green,parent);
            indicator.SetActive(false);
            go.AddComponent<ItemPlacementTarget>().Configure(accepted,title,anchor,indicator);
        }
        private static GameObject Box(string name,Vector3 pos,Vector3 scale,Material mat,Transform parent)
        {
            var go=GameObject.CreatePrimitive(PrimitiveType.Cube);go.name=name;go.transform.SetParent(parent);
            go.transform.position=pos;go.transform.localScale=scale;go.GetComponent<Renderer>().sharedMaterial=mat;return go;
        }
        private static Material Mat(string name,Color color)
        {
            var path="Assets/Reborn/Materials/"+name+".mat";
            var m=AssetDatabase.LoadAssetAtPath<Material>(path); if(m) return m;
            m=new Material(Shader.Find("HDRP/Lit"));m.SetColor("_BaseColor",color);m.SetFloat("_Smoothness",.15f);
            AssetDatabase.CreateAsset(m,path);return m;
        }
    }
}
