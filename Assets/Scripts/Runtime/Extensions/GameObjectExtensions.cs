using UnityEngine;

namespace FreeBlob.Extensions {
    public static class GameObjectExtensions {
        public static TComponent GetOrAddComponent<TComponent>(this GameObject gameObject)
            where TComponent : Component {
            return gameObject.TryGetComponent<TComponent>(out var component)
                ? component
                : gameObject.AddComponent<TComponent>();
        }
        public static void DestroyComponent<TComponent>(this GameObject gameObject)
           where TComponent : Component {
            if (gameObject.TryGetComponent<TComponent>(out var component)) {
#if UNITY_EDITOR
                if (!UnityEditor.PrefabUtility.IsPartOfPrefabAsset(gameObject)) {
                    if (UnityEditor.EditorApplication.isPlaying) {
                        UnityEngine.Object.Destroy(component);
                    } else {
                        UnityEngine.Object.DestroyImmediate(component);
                    }
                }
#else
                UnityEngine.Object.Destroy(component);
#endif
            }
        }
    }
}