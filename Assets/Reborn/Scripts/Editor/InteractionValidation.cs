using System;
using UnityEditor;
using UnityEngine;

namespace Reborn.Editor
{
    public static class InteractionValidation
    {
        public static string Run()
        {
            if (!EditorApplication.isPlaying) throw new InvalidOperationException("Play mode required.");
            var interaction = UnityEngine.Object.FindFirstObjectByType<InteractionController>();
            var player = interaction.GetComponent<FirstPersonController>();
            var cc = player.GetComponent<CharacterController>();
            var camera = player.GetComponentInChildren<Camera>().transform;
            var position = player.transform.position;
            var rotation = player.transform.rotation;
            var cameraRotation = camera.localRotation;
            GameObject blocker = null;
            try
            {
                interaction.EndInspect();
                cc.enabled=false; player.transform.position=new Vector3(.1f,.03f,3.5f); cc.enabled=true;
                var diary=GameObject.Find("Diary").GetComponent<InspectableObject>();
                camera.LookAt(diary.transform.position); Physics.SyncTransforms();
                Check(interaction.FindTarget()==diary,"Diary reachable from aisle");
                blocker=GameObject.CreatePrimitive(PrimitiveType.Cube);
                blocker.transform.position=Vector3.Lerp(camera.position,diary.transform.position,.5f);
                blocker.transform.localScale=Vector3.one*.25f; Physics.SyncTransforms();
                Check(interaction.FindTarget()==null && !interaction.TryInspect(),"Solid occlusion rejects interaction");
                blocker.SetActive(false); Physics.SyncTransforms();
                Check(interaction.TryInspect(),"Inspection opens");
                Check(player.ExplorationBlocked,"Exploration locked");
                var before=player.transform.position; var beforeView=camera.rotation;
                player.StepMovement(Vector2.up,1); player.ApplyLook(new Vector2(50,50));
                Check(Vector3.Distance(before,player.transform.position)<.001f && Quaternion.Angle(beforeView,camera.rotation)<.001f,"No movement or look during inspection");
                Check(!interaction.TryInspect(),"Duplicate open rejected");
                interaction.EndInspect();
                Check(!player.ExplorationBlocked && !interaction.Inspected,"Close restores exploration");
                cc.enabled=false; player.transform.position=new Vector3(.1f,.03f,1.15f); cc.enabled=true;
                camera.LookAt(diary.transform.position); Physics.SyncTransforms();
                Check(!interaction.TryInspect(),"Out-of-range rejected");
                return "PASS: reachable diary; occlusion; open; input lock; duplicate guard; close; distance limit";
            }
            finally
            {
                interaction.EndInspect();
                if(blocker) UnityEngine.Object.Destroy(blocker);
                cc.enabled=false; player.transform.SetPositionAndRotation(position,rotation); cc.enabled=true;
                camera.localRotation=cameraRotation; Physics.SyncTransforms();
            }
        }

        private static void Check(bool condition,string label)
        { if(!condition) throw new Exception("FAIL: "+label); }
    }
}
