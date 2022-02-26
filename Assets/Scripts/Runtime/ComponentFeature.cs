using FreeBlob.Extensions;
using Slothsoft.UnityExtensions;
using UnityEngine;

namespace FreeBlob {
    public class ComponentFeature<TComponent> : MonoBehaviour where TComponent : Component {
        [SerializeField, Expandable]
        TComponent m_observedComponent = default;
        public TComponent observedCompopnent => m_observedComponent;
        protected virtual void Awake() {
            SetUpComponents();
        }
        protected virtual void OnValidate() {
            SetUpComponents();
        }
        protected virtual void SetUpComponents() {
            _ = m_observedComponent
                || transform.TryGetComponentInParent(out m_observedComponent)
                || transform.TryGetComponentInChildren(out m_observedComponent);
        }
    }
}