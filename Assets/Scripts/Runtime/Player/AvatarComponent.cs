using System;
using Cinemachine;
using FreeBlob.Input;
using Slothsoft.UnityExtensions;
using UnityEngine;

namespace FreeBlob.Player {
    public class AvatarComponent : MonoBehaviour, IAvatar {
        [Header("MonoBehaviour Configuration")]
        [SerializeField, Expandable]
        AvatarSettings settings = default;
        [SerializeField, Expandable]
        CharacterController character = default;
        [SerializeField, Expandable]
        Transform body = default;
        [SerializeField, Expandable]
        Transform eyes = default;
        [SerializeField, Expandable]
        CinemachineVirtualCamera cinemachineCamera = default;

        [Header("Unity Configuration")]
        [SerializeField]
        UpdateMethod updateMethod = UpdateMethod.FixedUpdate;

        Controls controls;
        Movement movement;
        Look look;

        public event Action<ControllerColliderHit> onControllerColliderHit;
        public event Action onJumpCountChanged;

        public Vector3 forward => eyes.forward;
        public Vector3 velocity => movement.currentVelocity;
        public Vector3 position => eyes.position;
        public Quaternion rotation => eyes.rotation;
        public bool isRunning => movement.isRunning;
        [SerializeField]
        int m_jumpCount = 1;
        public int jumpCount {
            get => m_jumpCount;
            set {
                if (m_jumpCount != value) {
                    m_jumpCount = value;
                    onJumpCountChanged?.Invoke();
                }
            }
        }

        protected void Awake() {
            controls = new Controls();
            movement = new Movement(this, settings, controls.Avatar, character);
            look = new Look(this, settings, controls.Avatar, body, eyes, cinemachineCamera);

            onJumpCountChanged += () => settings.onJumpCountChanged.Invoke(gameObject);
        }
        protected void OnEnable() {
            controls.Enable();
        }
        protected void OnDisable() {
            controls.Disable();
        }
        protected void OnDestroy() {
            movement.Dispose();
            look.Dispose();
        }
        protected void Update() {
            if (updateMethod == UpdateMethod.Update) {
                UpdateAvatar(Time.deltaTime);
            }
        }
        protected void FixedUpdate() {
            if (updateMethod == UpdateMethod.FixedUpdate) {
                UpdateAvatar(Time.deltaTime);
            }
        }
        protected void LateUpdate() {
            if (updateMethod == UpdateMethod.LateUpdate) {
                UpdateAvatar(Time.deltaTime);
            }
        }
        void UpdateAvatar(float deltaTime) {
            look.Update(deltaTime);
            movement.Update(deltaTime);
        }
        protected void OnControllerColliderHit(ControllerColliderHit hit) {
            onControllerColliderHit?.Invoke(hit);
        }
    }
}