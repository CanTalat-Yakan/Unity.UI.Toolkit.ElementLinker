using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    [RequireComponent(typeof(UIElementLink))]
    public class UIElementProvider : MonoBehaviour
    {
        UIElementLink _link;
        public VisualElement Element => _link?.LinkedElement;
        public void SetLink(UIElementLink element) => _link = element;

        public void Awake() => _link ??= GetComponent<UIElementLink>();
        public void OnEnable() => RegisterEvents();
        public void OnDisable() => UnregisterEvents();

        private void RegisterEvents()
        {
            if (Element == null)
                return;

            Element.RegisterCallback<ClickEvent>(HandleClick);
            Element.RegisterCallback<ChangeEvent<bool>>(HandleToggle);
        }

        private void UnregisterEvents()
        {
            if (Element == null)
                return;

            Element.UnregisterCallback<ClickEvent>(HandleClick);
            Element.UnregisterCallback<ChangeEvent<bool>>(HandleToggle);
        }

        private void HandleClick(ClickEvent evt) => Debug.Log($"{Element.name} clicked!");
        private void HandleToggle(ChangeEvent<bool> evt) => Debug.Log($"{Element.name} toggled: {evt.newValue}");
    }
}