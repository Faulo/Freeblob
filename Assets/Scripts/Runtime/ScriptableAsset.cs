using MyBox;
using UnityEngine;

namespace FreeBlob {
    public class ScriptableAsset : ScriptableObject {
        [SerializeField, ReadOnly]
        ScriptableAsset asset = default;
        protected virtual void OnValidate() {
            if (asset != this) {
                asset = this;
            }
        }
    }
}