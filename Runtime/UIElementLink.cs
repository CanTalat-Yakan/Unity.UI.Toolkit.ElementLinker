using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    [Serializable]
    public class UIElementLinkData
    {
        public UIElementPathEntry[] Path;
    }

    [AddComponentMenu("UI Toolkit/UI Element Link")]
    public partial class UIElementLink : MonoBehaviour
    {
        [SerializeField, HideInInspector] public UIElementLinkData Data = new();

        private UIDocument _document;
        private VisualElement _linkedElement;

        public VisualElement LinkedElement => _linkedElement ??= RefreshLink();

        void Reset() => FindDocument();
        void OnEnable() => RefreshLink();
        void Awake() => RefreshLink();

        void FindDocument()
        {
            if (!_document)
                _document = GetComponentInParent<UIDocument>();
        }

        [Button]
        public VisualElement RefreshLink()
        {
            FindDocument();
            _linkedElement = null;

            if (_document?.rootVisualElement != null && Data.Path != null)
            {
                _linkedElement = UIBuilderHookUtilities.FindElementByPath(_document.rootVisualElement, Data.Path);

                if (_linkedElement == null)
                    Debug.LogWarning($"No element found at path: {string.Join(" > ", Data.Path.Select(e => e.Name))}", this);
                else
                {
                    var orderPath = string.Join(" > ", Data.Path.Select(p => $"{p.Name}[{p.OrderIndex}]"));
                    Debug.Log($"Linked Element: {_linkedElement.name} (Order Path: {orderPath})", this);
                }
            }

            return _linkedElement;
        }

        public void SetElementPath(IEnumerable<UIElementPathEntry> path) =>
            Data.Path = path.ToArray();
    }
}