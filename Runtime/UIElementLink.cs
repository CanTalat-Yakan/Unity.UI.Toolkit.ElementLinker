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
        [SerializeField, HideInInspector] private UIElementLinkData _data = new();

        private UIDocument _document;
        private VisualElement _linkedElement;

        public VisualElement LinkedElement => _linkedElement;

        void Reset() => FindDocument();
        void OnEnable() => RefreshLink();
        void Awake() => RefreshLink();

        void FindDocument()
        {
            if (!_document)
                _document = GetComponentInParent<UIDocument>();
        }

        [Button]
        public void RefreshLink()
        {
            FindDocument();
            _linkedElement = null;

            if (_document?.rootVisualElement != null && _data.Path != null)
            {
                _linkedElement = UIBuilderHookUtilities.FindElementByPath(_document.rootVisualElement, _data.Path);

                if (_linkedElement == null)
                    Debug.LogWarning($"No element found at path: {string.Join(" > ", _data.Path.Select(p => p.Name))}", this);
            }
        }

        public void SetElementPath(IEnumerable<UIElementPathEntry> path) =>
            _data.Path = path.Select(e => new UIElementPathEntry
            {
                Name = e.Name,
                TypeIndex = e.TypeIndex,
                OrderIndex = e.OrderIndex
            }).ToArray();

        [Button]
        public void PrintLinkedElement()
        {
            RefreshLink();
            if (_linkedElement != null && _data != null)
            {
                var orderPath = string.Join(" > ", _data.Path.Select(p => $"{p.Name}[{p.OrderIndex}]"));
                Debug.Log($"Linked Element: {_linkedElement.name} (Order Path: {orderPath})", this);
            }
            else
            {
                Debug.Log("Linked Element: None", this);
            }
        }

    }
}