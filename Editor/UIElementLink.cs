using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    [AddComponentMenu("UI Toolkit/UI Element Link")]
    public class UIElementLink : MonoBehaviour
    {
        [SerializeField] UIDocument _document;
        [SerializeField] string _targetElementPath;

        VisualElement _linkedElement;

        public VisualElement LinkedElement => _linkedElement;

        void Reset() => FindDocument();
        void OnEnable() => RefreshLink();

        void FindDocument()
        {
            if (!_document)
                _document = GetComponentInParent<UIDocument>();
        }

        public void RefreshLink()
        {
            FindDocument();
            _linkedElement = null;

            if (_document?.rootVisualElement != null)
            {
                _linkedElement = _document.rootVisualElement.Q<VisualElement>(_targetElementPath);

                if (_linkedElement == null)
                    Debug.LogWarning($"Element not found: {_targetElementPath}", this);
            }
        }

        public void SetElementPath(string path) =>
            _targetElementPath = path;
    }

    public static class VisualElementExtensions
    {
        public static TemplateContainer GetFirstAncestorOfType<TemplateContainer>(this VisualElement element)
        {
            var current = element;
            while (current != null)
            {
                if (current is TemplateContainer tc)
                    return tc;
                current = current.parent;
            }
            return default;
        }
    }
}