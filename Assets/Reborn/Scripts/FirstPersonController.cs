using UnityEngine;
using UnityEngine.InputSystem;

namespace Reborn
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class FirstPersonController : MonoBehaviour
    {
        [SerializeField] private Transform view;
        [SerializeField, Min(0.1f)] private float moveSpeed = 1.6f;
        [SerializeField, Range(0.01f, 1f)] private float lookSensitivity = 0.1f;
        [SerializeField, Range(30f, 89f)] private float pitchLimit = 80f;
        private CharacterController controller;
        private InputAction move;
        private InputAction look;
        private InputAction release;
        private InputAction capture;
        private float pitch;
        private float verticalSpeed;
        private bool captured;
        private bool skipLook;

        public float MoveSpeed => moveSpeed;
        public float Pitch => pitch;
        public bool IsCaptured => captured && Cursor.lockState == CursorLockMode.Locked;

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            if (view == null) view = GetComponentInChildren<Camera>()?.transform;
            if (view == null) { Debug.LogError("FirstPersonController requires a child camera.", this); enabled = false; return; }
            pitch = Mathf.DeltaAngle(0f, view.localEulerAngles.x);
            move = new InputAction("Move", InputActionType.Value);
            move.AddCompositeBinding("2DVector").With("Up", "<Keyboard>/w").With("Down", "<Keyboard>/s")
                .With("Left", "<Keyboard>/a").With("Right", "<Keyboard>/d");
            look = new InputAction("Look", InputActionType.Value, "<Mouse>/delta");
            release = new InputAction("Release", InputActionType.Button, "<Keyboard>/escape");
            capture = new InputAction("Capture", InputActionType.Button, "<Mouse>/leftButton");
        }

        private void OnEnable() { move?.Enable(); look?.Enable(); release?.Enable(); capture?.Enable(); }
        private void OnDisable()
        {
            move?.Disable(); look?.Disable(); release?.Disable(); capture?.Disable();
            ReleaseCursor();
        }
        private void OnDestroy() { move?.Dispose(); look?.Dispose(); release?.Dispose(); capture?.Dispose(); }
        private void OnApplicationFocus(bool focused) { if (!focused) ReleaseCursor(); }
        private void OnApplicationPause(bool paused) { if (paused) ReleaseCursor(); }

        private void Update()
        {
            if (release.WasPressedThisFrame()) { ReleaseCursor(); return; }
            if (captured && Cursor.lockState != CursorLockMode.Locked) ReleaseCursor();
            if (!Application.isFocused) return;
            if (!IsCaptured)
            {
                if (capture.WasPressedThisFrame())
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                    captured = true;
                    skipLook = true;
                }
                return;
            }
            // Mouse delta already describes a frame's movement; do not multiply by deltaTime.
            if (skipLook) skipLook = false;
            else ApplyLook(look.ReadValue<Vector2>());
            StepMovement(move.ReadValue<Vector2>(), Time.deltaTime);
        }

        public void ReleaseCursor()
        {
            if (captured) { Cursor.lockState = CursorLockMode.None; Cursor.visible = true; }
            captured = false;
            verticalSpeed = 0f;
        }

        public void ApplyLook(Vector2 delta)
        {
            if (view == null) return;
            transform.Rotate(0f, delta.x * lookSensitivity, 0f);
            pitch = Mathf.Clamp(pitch - delta.y * lookSensitivity, -pitchLimit, pitchLimit);
            view.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        }

        public void StepMovement(Vector2 input, float deltaTime)
        {
            if (controller == null) controller = GetComponent<CharacterController>();
            if (deltaTime <= 0f || !controller.enabled) return;
            input = Vector2.ClampMagnitude(input, 1f);
            if (controller.isGrounded && verticalSpeed < 0f) verticalSpeed = -2f;
            verticalSpeed = Mathf.Max(verticalSpeed + Physics.gravity.y * deltaTime, -30f);
            Vector3 velocity = (transform.right * input.x + transform.forward * input.y) * moveSpeed;
            velocity.y = verticalSpeed;
            var flags = controller.Move(velocity * deltaTime);
            if ((flags & CollisionFlags.Above) != 0 && verticalSpeed > 0f) verticalSpeed = 0f;
        }

        private void OnGUI()
        {
            GUI.Box(new Rect(16, 16, 390, 54), IsCaptured
                ? "WASD: Walk  |  Mouse: Look  |  Esc: Release cursor"
                : "Click to start / resume\nWASD: Walk  |  Mouse: Look  |  Esc: Release cursor");
        }
    }
}
