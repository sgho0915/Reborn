using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Reborn.Editor
{
    public static class CarryValidation
    {
        public static string Run()
        {
            if(!EditorApplication.isPlaying || SceneManager.GetActiveScene().path!=CarryTestSetup.ScenePath)
                throw new InvalidOperationException("Run in CarryTest Play mode.");
            var i=UnityEngine.Object.FindFirstObjectByType<InteractionController>();
            var player=i.GetComponent<FirstPersonController>();var cc=i.GetComponent<CharacterController>();
            var camera=i.GetComponentInChildren<Camera>().transform;
            var old=player.transform.position;var rot=camera.localRotation;
            var item=GameObject.Find("Test envelope").GetComponent<CarryableObject>();
            var home=item.transform.position;var originalScale=item.transform.localScale;
            var target=GameObject.Find("Blue tray").GetComponent<ItemPlacementTarget>();
            var wrong=GameObject.Find("Orange tray").GetComponent<ItemPlacementTarget>();
            GameObject blocker=null;
            try
            {
                i.EndInspect();i.CancelCarry();item.Recover();
                Aim(cc,camera,new Vector3(.1f,.03f,3.05f),item.transform.position);
                Check(i.TryInspect() && i.TryCarryInspected(),"inspect to carry");
                Check(!player.ExplorationBlocked && i.Carried==item,"carry enables walking");
                Check(!item.GetComponent<Collider>().enabled,"held collider disabled");
                Check(!i.TryInspect() && !i.TryCarryInspected(),"single ownership");
                Aim(cc,camera,new Vector3(.1f,.03f,1.5f),wrong.transform.position);
                Check(!i.TryUseCarried() && i.Carried==item && !wrong.IsCompleted,"wrong target rejected");
                Aim(cc,camera,new Vector3(.1f,.03f,4.9f),target.transform.position);
                Check(!i.TryUseCarried(),"out of range rejected");
                Aim(cc,camera,new Vector3(.1f,.03f,2.35f),target.transform.position);
                blocker=GameObject.CreatePrimitive(PrimitiveType.Cube);
                blocker.transform.position=Vector3.Lerp(camera.position,target.transform.position,.5f);
                blocker.transform.localScale=Vector3.one*.3f;Physics.SyncTransforms();
                Check(!i.TryUseCarried(),"occlusion rejected");
                blocker.SetActive(false);Physics.SyncTransforms();
                i.CancelCarry();
                Check(!i.Carried && item.Current==CarryableObject.Location.Home && Vector3.Distance(home,item.transform.position)<.001f
                    && item.GetComponent<Collider>().enabled && item.transform.localScale==originalScale,"cancel restores pose and collision");
                Aim(cc,camera,new Vector3(.1f,.03f,3.05f),item.transform.position);
                Check(i.TryInspect() && i.TryCarryInspected(),"repeat pickup");
                Aim(cc,camera,new Vector3(.1f,.03f,2.35f),target.transform.position);
                Check(i.TryUseCarried() && !i.Carried && target.IsCompleted && target.Occupant==item,"placement commits completion");
                Check(!i.TryUseCarried() && !target.TryAccept(item),"duplicate placement rejected");
                target.enabled=false;
                Check(item.Current==CarryableObject.Location.Home && !target.IsCompleted,"disabled target recovers item");
                target.enabled=true;
                Aim(cc,camera,new Vector3(.1f,.03f,3.05f),item.transform.position);
                Check(i.TryInspect() && i.TryCarryInspected(),"pickup after recovery");
                i.enabled=false;
                Check(item.Current==CarryableObject.Location.Home && !player.ExplorationBlocked,"controller disable recovers");
                i.enabled=true;
                return "PASS: pickup, walking context, single ownership, wrong target, distance, occlusion, cancel restoration, repeat pickup, completion, duplicate guard, target and controller disable recovery";
            }
            finally
            {
                target.enabled=true;i.enabled=true;i.EndInspect();i.CancelCarry();item.Recover();
                if(blocker) UnityEngine.Object.Destroy(blocker);
                cc.enabled=false;player.transform.position=old;cc.enabled=true;camera.localRotation=rot;Physics.SyncTransforms();
            }
        }
        private static void Aim(CharacterController cc,Transform camera,Vector3 position,Vector3 target)
        {cc.enabled=false;cc.transform.position=position;cc.enabled=true;camera.LookAt(target);Physics.SyncTransforms();}
        private static void Check(bool condition,string message) {if(!condition)throw new Exception("FAIL: "+message);}
    }
}
