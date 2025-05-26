using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    [Serializable]
    public struct UIElementPathEntry
    {
        public string Name;
        public int TypeIndex;
        public int OrderIndex;
    }

    [Serializable]
    public class UIElementLinkData
    {
        public UIElementPathEntry[] Path;
    }

    [AddComponentMenu("UI Toolkit/UI Element Link")]
    public partial class UIElementLink : MonoBehaviour
    {
        [SerializeField] private UIElementLinkData _data = new();

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
                _linkedElement = FindElementByPath(_document.rootVisualElement, _data.Path);

                if (_linkedElement == null)
                    Debug.LogWarning($"No element found at path: {string.Join(" > ", _data.Path.Select(p => p.Name))}", this);
            }
        }

        public void SetElementPath(IEnumerable<(string Name, int TypeIndex, int OrderIndex)> path) =>
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

        VisualElement FindElementByPath(VisualElement root, IEnumerable<UIElementPathEntry> path)
        {
            var current = root;
            foreach (var (name, typeIndex, orderIndex) in path.Select(p => (p.Name, p.TypeIndex, p.OrderIndex)))
            {
                if (current == null)
                    return null;

                // Find all children that match name and type index
                var matchingChildren = current.Children()
                    .Where(child =>
                        child.name == name &&
                        (int)UIElementTypes.GetElementType(child) == typeIndex)
                    .ToList();

                // Select the child at the specified order index, if available
                if (orderIndex < 0 || orderIndex >= matchingChildren.Count)
                    return null;

                current = matchingChildren[orderIndex];
            }
            return current;
        }
    }
}