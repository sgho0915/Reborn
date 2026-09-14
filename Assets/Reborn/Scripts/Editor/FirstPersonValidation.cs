using System;
using UnityEditor;
using UnityEngine;

namespace Reborn.Editor
{
    public static class FirstPersonValidation
    {
        // Run in Play mode in FirstPersonTest. Restore the player after deterministic checks.
        public static string Run()
        {
            if (!EditorApplication.isPlaying) throw new InvalidOperationException("Play mode required.");
            var player = UnityEngine.Object.FindFirstObjectByType<FirstPersonController>();
            if (player == null) throw new InvalidOperationException("Player missing.");
            var cc = player.GetComponent<CharacterController>();
            var origin = player.transform.position;
            var rotation = player.transform.rotation;
            var camera = player.GetComponentInChildren<Camera>();
            var viewRotation = camera.transform.localRotation;
            float initialPitch = player.Pitch;
            try
            {
                void Reset(Vector3 position)
                {
                    cc.enabled = false;
                    player.transform.SetPositionAndRotation(position, Quaternion.identity);
                    cc.enabled = true;
                    player.ReleaseCursor();
                    Physics.SyncTransforms();
                }
                void Step(Vector2 input, int frames, float dt)
                { for (int i = 0; i < frames; i++) player.StepMovement(input, dt); }
                void Check(bool condition, string message)
                { if (!condition) throw new Exception(message); }

                Reset(new Vector3(3, 0.05f, -4));
                Step(Vector2.up, 60, 1f / 60f);
                float forward = player.transform.position.z + 4;
                Check(Mathf.Abs(forward - 1.6f) < 0.03f, "Forward speed mismatch: " + forward);
                Check(Mathf.Abs(player.transform.position.y) < 0.06f, "Floor contact failed.");
                Reset(new Vector3(3, 0.05f, -4));
                Step(Vector2.one, 60, 1f / 60f);
                var diagonal = player.transform.position - new Vector3(3, 0.05f, -4);
                Check(Mathf.Abs(new Vector2(diagonal.x, diagonal.z).magnitude - forward) < 0.03f, "Diagonal speed boost.");
                Reset(new Vector3(3, 0.05f, -4));
                Step(Vector2.up, 30, 1f / 30f);
                Check(Mathf.Abs(player.transform.position.z + 4 - forward) < 0.03f, "Frame rate dependent movement.");
                Reset(new Vector3(0, 0.05f, -4));
                Step(Vector2.up, 300, 1f / 60f);
                Check(player.transform.position.z < 0.3f && player.transform.position.z > 0.15f, "Obstacle collision failed.");
                Reset(new Vector3(-2.2f, 0.05f, 1));
                Step(Vector2.up, 120, 1f / 60f);
                Check(player.transform.position.z > 4f, "Doorway passage failed.");
                Reset(new Vector3(3, 0.05f, -4));
                player.ApplyLook(new Vector2(900, 100000));
                Check(Mathf.Abs(player.Pitch + 80f) < 0.01f, "Upper pitch limit failed.");
                player.ApplyLook(new Vector2(0, -200000));
                Check(Mathf.Abs(player.Pitch - 80f) < 0.01f, "Lower pitch limit failed.");
                Check(Mathf.Abs(Mathf.DeltaAngle(player.transform.eulerAngles.y, 90f)) < 0.01f, "Yaw failed.");
                Check(Mathf.Abs(camera.transform.localEulerAngles.z) < 0.01f, "Unexpected camera roll.");
                Check(Mathf.Abs(camera.fieldOfView - 60f) < 0.01f && Mathf.Abs(camera.transform.localPosition.y - 1.6f) < 0.01f, "Camera specification mismatch.");
                return "PASS: speed, floor, diagonal normalization, 30/60fps equivalence, obstacle collision, doorway, pitch limits, yaw, no roll, camera specification. Native keyboard/mouse and OS focus require manual play check.";
            }
            finally
            {
                player.ApplyLook(new Vector2(0, (player.Pitch - initialPitch) / 0.1f));
                cc.enabled = false;
                player.transform.SetPositionAndRotation(origin, rotation);
                camera.transform.localRotation = viewRotation;
                cc.enabled = true;
                player.ReleaseCursor();
                Physics.SyncTransforms();
            }
        }
    }
}
