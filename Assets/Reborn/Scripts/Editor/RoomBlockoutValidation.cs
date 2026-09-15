using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Reborn.Editor
{
    public static class RoomBlockoutValidation
    {
        public static string Run()
        {
            if (!EditorApplication.isPlaying || SceneManager.GetActiveScene().path != RoomBlockoutBuilder.ScenePath)
                throw new InvalidOperationException("Run in RoomBlockout Play mode.");
            var state=UnityEngine.Object.FindFirstObjectByType<RoomBlockoutState>();
            var player=UnityEngine.Object.FindFirstObjectByType<FirstPersonController>();
            var cc=player.GetComponent<CharacterController>();
            var position=player.transform.position;
            var original=state.Current;
            var passed=new List<string>();
            try
            {
                foreach(var layout in new[]{RoomBlockoutState.Layout.Reality,RoomBlockoutState.Layout.Opening,RoomBlockoutState.Layout.Exit})
                {
                    state.Apply(layout); Physics.SyncTransforms();
                    Teleport(cc,new Vector3(.1f,.03f,1.15f));
                    Walk(cc,new Vector3(.1f,0,3.5f));
                    Walk(cc,new Vector3(.1f,0,4.9f));
                    Walk(cc,new Vector3(.1f,0,1.15f));
                    passed.Add(layout+": central aisle, desk approach, window approach");
                    Walk(cc,new Vector3(.425f,0,1.15f));
                    for(int i=0;i<180;i++) cc.Move(new Vector3(0,-.02f,-.02f));
                    if(layout==RoomBlockoutState.Layout.Exit)
                    {
                        if(cc.transform.position.z>-.8f) throw new Exception("Exit route blocked.");
                        passed.Add("Exit: corridor reachable");
                    }
                    else
                    {
                        if(cc.transform.position.z<0) throw new Exception("Closed entrance penetrated.");
                        if(layout==RoomBlockoutState.Layout.Opening && cc.transform.position.z<.95f) throw new Exception("Cabinet collision missing.");
                        passed.Add(layout+": entrance blocked as intended");
                    }
                }
                return string.Join("\n",passed);
            }
            finally { state.Apply(original); Teleport(cc,position); }
        }

        private static void Teleport(CharacterController cc,Vector3 p)
        { cc.enabled=false; cc.transform.position=p; cc.enabled=true; Physics.SyncTransforms(); }
        private static void Walk(CharacterController cc,Vector3 target)
        {
            for(int i=0;i<400;i++)
            {
                var delta=target-cc.transform.position; delta.y=0;
                if(delta.magnitude<.04f) return;
                cc.Move(Vector3.ClampMagnitude(delta,.02f)+Vector3.down*.02f);
            }
            throw new Exception("Route blocked toward "+target+" at "+cc.transform.position);
        }
    }
}
