using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Assertions;

namespace FreeBlob.Extensions {
    public static class Texture2DExtensions {
#if UNITY_EDITOR
        public static Texture2D AsReadable(this Texture2D texture) {
            if (texture.isReadable) {
                return texture;
            }
            var tempTexture = new Texture2D(texture.width, texture.height, texture.format, false) {
                hideFlags = HideFlags.DontSave
            };
            tempTexture.LoadRawTextureData(texture.GetRawTextureData());
            return tempTexture;
        }
        public static Color[] GetPixels(this Texture2D texture, RectInt rect) {
            return texture.GetPixels(rect.x, rect.y, rect.width, rect.height);
        }
        public static Color[] GetPixels(this Texture2D texture, Rect rect) {
            int x = Mathf.RoundToInt(rect.x);
            int y = Mathf.RoundToInt(rect.y);
            int width = Mathf.RoundToInt(rect.width);
            int height = Mathf.RoundToInt(rect.height);
            return texture.GetPixels(x, y, width, height);
        }
        public static Sprite[] GetSpritesRow(this Texture2D texture, int rowIndex) {
            var sprites = texture.GetSprites();
            Assert.AreNotEqual(0, sprites.Length);
            int spritesPerRow = Mathf.RoundToInt(texture.width / sprites[0].rect.width);
            return sprites[(rowIndex * spritesPerRow)..((rowIndex + 1) * spritesPerRow)]
                .Where(sprite => texture.GetPixels(sprite.rect).Any(color => color.a > 0))
                .ToArray();
        }
        public static Sprite[][] GetSpritesMatrix(this Texture2D texture) {
            var sprites = texture.GetSprites();
            Assert.AreNotEqual(0, sprites.Length);
            var sprite = sprites[0];
            int rows = Mathf.RoundToInt(texture.height / sprite.rect.height);
            int columns = Mathf.RoundToInt(texture.width / sprite.rect.width);
            var matrix = new Sprite[rows][];
            for (int i = 0; i < rows; i++) {
                matrix[i] = sprites[(i * columns)..((i + 1) * columns)]
                    .Where(sprite => texture.GetPixels(sprite.rect).Any(color => color.a > 0))
                    .ToArray();
            }
            return matrix;
        }
        public static Sprite[] GetSprites(this Texture2D texture) {
            return UnityEditor.AssetDatabase
                .LoadAllAssetsAtPath(UnityEditor.AssetDatabase.GetAssetPath(texture))
                .OfType<Sprite>()
                .OrderBy(Comparer)
                .ToArray();
        }
        static int Comparer(Object obj) {
            var match = Regex.Match(obj.name, "\\d+$");
            return match.Success
                ? int.Parse(match.Value)
                : 0;
        }
#endif
    }
}