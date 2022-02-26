using UnityEngine;
using UnityEngine.Rendering;

namespace FreeBlob {
    public class ForceCullingMatrix : ComponentFeature<Camera> {
        [SerializeField]
        Matrix4x4 cullingMatrix = Matrix4x4.Ortho(-99999, 99999, -99999, 99999, 0.001f, 99999) * Matrix4x4.Translate(Vector3.forward * -99999 / 2f);
        protected void OnEnable() {
            RenderPipelineManager.beginCameraRendering += UpdateCullingMatrix;
        }
        protected void OnDisable() {
            RenderPipelineManager.beginCameraRendering -= UpdateCullingMatrix;
        }

        void UpdateCullingMatrix(ScriptableRenderContext context, Camera camera) {
            observedCompopnent.cullingMatrix = cullingMatrix * observedCompopnent.worldToCameraMatrix;
        }
    }
}