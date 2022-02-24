using System;
using UnityEngine;

namespace FreeBlob.Player {
    public interface IAvatar {
        event Action<ControllerColliderHit> onControllerColliderHit;
        GameObject gameObject { get; }
        Vector3 forward { get; }
        Vector3 position { get; }
        Quaternion rotation { get; }
        Vector3 velocity { get; }
        int jumpCount { get; set; }
        bool isRunning { get; }
    }
}