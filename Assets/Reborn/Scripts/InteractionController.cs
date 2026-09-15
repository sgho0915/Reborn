using UnityEngine;
using UnityEngine.InputSystem;

namespace Reborn
{
    [RequireComponent(typeof(FirstPersonController))]
    [DefaultExecutionOrder(100)]
    public sealed class InteractionController : MonoBehaviour
    {
        [SerializeField, Min(.1f)] private float reach = 2f;
        private FirstPersonController player;
        private Camera view;
        private InputAction inspect;
        private InputAction close;
        private InspectableObject selected;
        private InspectableObject inspected;
        private Font font;
        private GUIStyle textStyle;
        private Vector2 scroll;
        public InspectableObject Selected => selected;
        public InspectableObject Inspected => inspected;
        public float Reach => reach;

        private void Awake()
        {
            player = GetComponent<FirstPersonController>();
            view = GetComponentInChildren<Camera>();
            inspect = new InputAction("Inspect", InputActionType.Button, "<Keyboard>/e");
            close = new InputAction("CloseInspect", InputActionType.Button, "<Keyboard>/escape");
        }
        private void OnEnable() { inspect?.Enable(); close?.Enable(); }
        private void OnDisable()
        {
            inspect?.Disable(); close?.Disable(); EndInspect(); selected = null;
        }
        private void OnDestroy()
        { inspect?.Dispose(); close?.Dispose(); if (font) Destroy(font); }

        private void LateUpdate()
        {
            if (player.ExplorationBlocked)
            {
                if (!inspected || !inspected.isActiveAndEnabled || close.WasPressedThisFrame()) EndInspect();
                return;
            }
            selected = Application.isFocused && player.IsCaptured ? FindTarget() : null;
            if (selected && inspect.WasPressedThisFrame()) TryInspect();
        }

        // Only the first solid surface counts: never select through walls or furniture.
        public InspectableObject FindTarget()
        {
            if (!view) return null;
            if (!Physics.Raycast(view.transform.position, view.transform.forward, out var hit,
                    reach, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore)) return null;
            var target = hit.collider.GetComponentInParent<InspectableObject>();
            return target && target.isActiveAndEnabled ? target : null;
        }

        public bool TryInspect()
        {
            if (player.ExplorationBlocked) return false;
            // Re-check distance, visibility and existence at action time.
            var target = FindTarget();
            if (!target) return false;
            inspected = target; selected = null; scroll = Vector2.zero;
            player.SetExplorationBlocked(true);
            return true;
        }

        public void EndInspect()
        {
            inspected = null; selected = null;
            if (player) player.SetExplorationBlocked(false);
            // Stay released; the next deliberate click resumes exploration.
        }

        private void OnGUI()
        {
            if (!player) return;
            if (textStyle == null)
            {
                font = Font.CreateDynamicFontFromOSFont(new[] { "Apple SD Gothic Neo", "Malgun Gothic", "Arial" }, 20);
                textStyle = new GUIStyle(GUI.skin.label) { font = font, fontSize = 20, wordWrap = true };
            }
            if (inspected)
            {
                float w = Mathf.Min(640, Screen.width - 32);
                float h = Mathf.Min(360, Screen.height - 32);
                var panel = new Rect((Screen.width-w)/2, (Screen.height-h)/2, w, h);
                GUI.Box(panel, GUIContent.none);
                GUILayout.BeginArea(new Rect(panel.x+24, panel.y+20, w-48, h-40));
                GUILayout.Label(inspected.DisplayName, textStyle);
                scroll = GUILayout.BeginScrollView(scroll);
                GUILayout.Label(inspected.Description, textStyle);
                GUILayout.EndScrollView();
                if (GUILayout.Button("닫기 (Esc)", textStyle, GUILayout.Height(40))) EndInspect();
                GUILayout.EndArea();
                return;
            }
            if (!player.IsCaptured) return;
            GUI.Label(new Rect(Screen.width/2f-5, Screen.height/2f-12, 20, 24), selected ? "○" : "·", textStyle);
            if (selected)
                GUI.Label(new Rect(Screen.width/2f-180, Screen.height/2f+28, 360, 70), "E  조사 — " + selected.DisplayName, textStyle);
        }
    }
}
