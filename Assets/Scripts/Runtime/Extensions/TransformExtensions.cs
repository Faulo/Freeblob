using UnityEngine;

namespace FreeBlob.Extensions {
    public static class TransformExtensions {
        public static Transform[] GetChildren(this Transform parent) {
            var children = new Transform[parent.childCount];
            for (int i = 0; i < parent.childCount; i++) {
                children[i] = parent.GetChild(i);
            }
            return children;
        }
        public static void Clear(this Transform parent) {
            foreach (var child in parent.GetChildren()) {
                Object.Destroy(child.gameObject);
            }
        }
        public static bool TryGetComponentInParent<T>(this Transform context, out T target)
            where T : class {
            for (var ancestor = context; ancestor; ancestor = ancestor.parent) {
                if (ancestor.TryGetComponent(out target)) {
                    return true;
                }
            }
            target = default;
            return false;
        }
        public static bool TryGetComponentInChildren<T>(this Transform context, out T target)
            where T : class {
            if (context.TryGetComponent(out target)) {
                return true;
            }
            for (int i = 0; i < context.childCount; i++) {
                if (context.GetChild(i).TryGetComponentInChildren(out target)) {
                    return true;
                }
            }
            return false;
        }
    }
}