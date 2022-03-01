using System;
using System.Collections;
using MyBox;
using Slothsoft.UnityExtensions;
using UnityEngine;
using UnityObject = UnityEngine.Object;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FreeBlob.Art {
    [Serializable]
    public class Substance : IComparable<Substance> {
        [Header("Identification")]
        [SerializeField, Expandable, ReadOnly]
        public UnityObject asepriteFile = default;
        [SerializeField, Expandable, ReadOnly]
        public Texture2DArray baseTexture = default;

        [Header("Configuration")]
        [SerializeField, Expandable]
        public PhysicMaterial physicsMaterial = default;

        [Header("Generated assets")]
        [SerializeField, Expandable, ReadOnly]
        public Material renderMaterial = default;
        [SerializeField, Expandable, ReadOnly]
        public TerrainLayer terrainLayer = default;
        [SerializeField, Expandable, ReadOnly]
        public Texture2D diffuseTexture = default;
        [SerializeField, Expandable, ReadOnly]
        public Texture2D heightTexture = default;
        [SerializeField, Expandable, ReadOnly]
        public Texture2D maskTexture = default;

        public override string ToString() => baseTexture ? baseTexture.name : "???";
        public int CompareTo(Substance other) => baseTexture.name.CompareTo(other.baseTexture.name);

#if UNITY_EDITOR
        public IEnumerator Co_Import(Shader shader, Material terrainMaterial) {
            ImportBaseTexture();

            yield return null;

            ExtractTextures();

            yield return null;

            ImportRenderMaterial(shader, terrainMaterial);

            yield return null;

            ImportTerrainLayer();

            yield return null;
        }


        void ImportBaseTexture() {
            var importer = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(baseTexture)) as TextureImporter;

            importer.alphaIsTransparency = true;
            importer.alphaSource = TextureImporterAlphaSource.FromInput;
            importer.filterMode = FilterMode.Point;
            importer.maxTextureSize = 1024;
            importer.mipmapEnabled = true;
            importer.sRGBTexture = true;
            importer.textureCompression = TextureImporterCompression.Uncompressed;

            importer.SaveAndReimport();
        }

        void ExtractTextures() {
            diffuseTexture = LoadOrCreateTexture("Diffuse");

            diffuseTexture.SetPixels32(baseTexture.GetPixels32(0));
            diffuseTexture.Apply();

            heightTexture = LoadOrCreateTexture("Height");

            heightTexture.SetPixels32(baseTexture.GetPixels32(1));
            heightTexture.Apply();

            maskTexture = LoadOrCreateTexture("Mask");

            var height = baseTexture.GetPixels32(1);
            var metallic = baseTexture.GetPixels32(2);
            var smoothness = baseTexture.GetPixels32(3);
            var emission = baseTexture.GetPixels32(4);
            var mask = new Color32[metallic.Length];
            for (int i = 0; i < metallic.Length; i++) {
                mask[i] = new Color32(metallic[i].r, height[i].r, emission[i].r, smoothness[i].r);
            }
            maskTexture.SetPixels32(mask);
            maskTexture.Apply();
        }

        Texture2D LoadOrCreateTexture(string suffix) {
            string path = AssetDatabase.GetAssetPath(baseTexture);
            path = path.Replace("Textures", "Textures" + Path.DirectorySeparatorChar + "Extracted");
            path = path.Replace(".png", $"_{suffix}.asset");
            var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            if (!texture) {
                texture = new Texture2D(baseTexture.width, baseTexture.height);
                AssetDatabase.CreateAsset(texture, path);
            }
            texture.filterMode = FilterMode.Point;
            return texture;
        }

        void ImportRenderMaterial(Shader shader, Material terrainMaterial) {
            string path = AssetDatabase.GetAssetPath(baseTexture);
            path = path.Replace("Textures", "Materials");
            path = path.Replace(".png", ".mat");
            renderMaterial = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (renderMaterial) {
                renderMaterial.shader = shader;
            } else {
                renderMaterial = new Material(shader);
                AssetDatabase.CreateAsset(renderMaterial, path);
            }
            renderMaterial.SetTexture("_MainTex", diffuseTexture);
            renderMaterial.SetTexture("_Height", heightTexture);
            renderMaterial.SetTexture("_Mask", maskTexture);

            renderMaterial.SetFloat("_Curvature", terrainMaterial.GetFloat("_Curvature"));
            renderMaterial.SetFloat("_Emission_Strength", terrainMaterial.GetFloat("_Emission_Strength"));
            renderMaterial.SetFloat("_Parallax_Strength", terrainMaterial.GetFloat("_Parallax_Strength"));
            renderMaterial.SetFloat("_Normal_Strength", terrainMaterial.GetFloat("_Normal_Strength"));
        }

        void ImportTerrainLayer() {
            string path = AssetDatabase.GetAssetPath(baseTexture);
            path = path.Replace("Textures", "TerrainLayers");
            path = path.Replace(".png", ".terrainlayer");
            terrainLayer = AssetDatabase.LoadAssetAtPath<TerrainLayer>(path);
            if (!terrainLayer) {
                terrainLayer = new TerrainLayer();
                AssetDatabase.CreateAsset(terrainLayer, path);
            }
            terrainLayer.diffuseTexture = diffuseTexture;
            terrainLayer.normalMapTexture = heightTexture;
            terrainLayer.maskMapTexture = maskTexture;
            EditorUtility.SetDirty(terrainLayer);
        }
#endif
    }
}