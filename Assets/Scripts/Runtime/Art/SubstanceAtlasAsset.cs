using System;
using Slothsoft.UnityExtensions;
using UnityEngine;
using UnityObject = UnityEngine.Object;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FreeBlob.Art {
    [CreateAssetMenu]
    public class SubstanceAtlasAsset : ScriptableAsset {
        [SerializeField]
        UnityObject[] sourceFolders = Array.Empty<UnityObject>();
        [SerializeField, Expandable]
        Shader materialShader = default;
        [SerializeField, Expandable]
        Material terrainMaterial = default;

        [Header("All substances")]
        [SerializeField]
        List<Substance> substances = new List<Substance>();

        Dictionary<Material, PhysicMaterial> m_materials;
        Dictionary<Material, PhysicMaterial> materials {
            get {
                if (m_materials == null) {
                    m_materials = new Dictionary<Material, PhysicMaterial>();
                    for (int i = 0; i < substances.Count; i++) {
                        if (substances[i].physicsMaterial) {
                            m_materials[substances[i].renderMaterial] = substances[i].physicsMaterial;
                        }
                    }
                }
                return m_materials;
            }
        }

        public bool TryGetPhysicsMaterial(Material renderMaterial, out PhysicMaterial physicsMaterial)
            => materials.TryGetValue(renderMaterial, out physicsMaterial);

#if UNITY_EDITOR
        [CustomEditor(typeof(SubstanceAtlasAsset))]
        class SubstanceAtlasAssetEditor : RuntimeEditorTools<SubstanceAtlasAsset> {
            protected override void DrawEditorTools() {
                DrawButton("RefreshAtlas", target.Co_RefreshAtlas);
            }
            protected void DrawButton(string label, Func<IEnumerator> coroutine)
                => DrawButton(label, () => Unity.EditorCoroutines.Editor.EditorCoroutineUtility.StartCoroutine(coroutine(), target));
        }
        IEnumerator Co_RefreshAtlas() {
            yield return null;

            string[] sourcePaths = sourceFolders
                .Select(AssetDatabase.GetAssetPath)
                .ToArray();

            yield return null;

            var allTextures = AssetDatabase
                .FindAssets("*", sourcePaths)
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadMainAssetAtPath)
                .OfType<Texture2DArray>()
                .OrderBy(texture => texture.name)
                .ToList();

            substances.RemoveAll(substance => !substance.baseTexture || !allTextures.Contains(substance.baseTexture));

            foreach (var texture in allTextures) {
                AddBaseTexture(texture);
            }

            substances.Sort();

            m_materials = null;

            yield return null;

            for (int i = 0; i < substances.Count; i++) {
                yield return substances[i].Co_Import(materialShader, terrainMaterial);
            }

            EditorUtility.SetDirty(this);

            AssetDatabase.SaveAssets();

            yield return null;
        }

        void AddBaseTexture(Texture2DArray texture) {
            for (int i = 0; i < substances.Count; i++) {
                if (substances[i].baseTexture == texture) {
                    return;
                }
            }
            substances.Add(new Substance { baseTexture = texture });
        }
#endif
    }
}