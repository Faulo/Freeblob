using UnityEngine;

namespace FreeBlob.Extensions {
    public static class SpriteExtensions {
        public static RectInt GetRect(this Sprite sprite) {
            var rect = sprite.rect;
            return new RectInt(
                Mathf.RoundToInt(rect.x),
                Mathf.RoundToInt(rect.y),
                Mathf.RoundToInt(rect.width),
                Mathf.RoundToInt(rect.height)
            );
        }
    }
}