using System;
using Cinemachine;
using FreeBlob.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FreeBlob.Player {
    [Serializable]
    public class Look : IUpdatable, IDisposable {
        readonly IAvatar avatar;
        readonly AvatarSettings settings;
        readonly Controls.AvatarActions input;
        readonly Transform body;
        readonly Transform eyes;
        readonly CinemachineVirtualCamera cinemachine;

        float horizontalCurrentAngle;
        float horizontalTargetAngle;
        float horizontalSpeed;
        void UpdateHorizontalAngle(float deltaTime) {
            horizontalCurrentAngle = Mathf.SmoothDampAngle(
                horizontalCurrentAngle,
                horizontalTargetAngle,
                ref horizontalSpeed,
                settings.cameraSmoothing,
                float.PositiveInfinity,
                deltaTime
            );
            body.localRotation = Quaternion.Euler(0, horizontalCurrentAngle, 0);
        }

        float verticalCurrentAngle;
        float verticalTargetAngle;
        float verticalSpeed;
        void UpdateVerticalAngle(float deltaTime) {
            verticalCurrentAngle = Mathf.SmoothDampAngle(
                verticalCurrentAngle,
                verticalTargetAngle,
                ref verticalSpeed,
                settings.cameraSmoothing,
                float.PositiveInfinity,
                deltaTime
            );
            eyes.localRotation = Quaternion.Euler(verticalCurrentAngle, 0, 0);
        }

        float fieldOfViewAngle;
        float fieldOfViewSpeed;
        void UpdateFOV(float deltaTime) {
            fieldOfViewAngle = Mathf.SmoothDamp(
                fieldOfViewAngle,
                avatar.isRunning ? settings.runningFieldOfView : settings.defaultFieldOfView,
                ref fieldOfViewSpeed,
                settings.fieldOfViewSmoothingTime,
                float.PositiveInfinity,
                deltaTime
            );
            cinemachine.m_Lens.FieldOfView = fieldOfViewAngle;
        }

        bool isLocked {
            get => Cursor.lockState == CursorLockMode.Locked;
            set => Cursor.lockState = value
                ? CursorLockMode.Locked
                : CursorLockMode.None;
        }

        public Look(IAvatar avatar, AvatarSettings settings, Controls.AvatarActions input, Transform body, Transform eyes, CinemachineVirtualCamera cinemachine) {
            this.avatar = avatar;
            this.settings = settings;
            this.input = input;
            this.body = body;
            this.eyes = eyes;
            this.cinemachine = cinemachine;

            RegisterInput();

            isLocked = true;
        }

        public void Dispose() {
            isLocked = false;
            UnregisterInput();
        }

        void RegisterInput() {
            input.Look.performed += HandleLook;
            input.Menu.performed += HandleMenu;
        }
        void UnregisterInput() {
            input.Look.performed -= HandleLook;
            input.Menu.performed -= HandleMenu;
        }

        void HandleLook(InputAction.CallbackContext context) {
            if (isLocked) {
                var deltaLook = context.ReadValue<Vector2>() * settings.cameraSpeed;

                horizontalTargetAngle += deltaLook.x;
                verticalTargetAngle -= deltaLook.y;
                verticalTargetAngle = Mathf.Clamp(verticalTargetAngle, settings.cameraMinX, settings.cameraMaxX);
            }
        }

        void HandleMenu(InputAction.CallbackContext context) {
            isLocked = !isLocked;
        }

        public void Update(float deltaTime) {
            if (isLocked) {
                UpdateHorizontalAngle(deltaTime);
                UpdateVerticalAngle(deltaTime);
                UpdateFOV(deltaTime);
            }
        }
    }
}