using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    public struct UIElementLinkData
    {
        public string Name;
        public UIElementType Type;
    }

    [AddComponentMenu("UI Toolkit/UI Element Link")]
    public class UIElementLink : MonoBehaviour
    {
        private UIDocument _document;
        private IEnumerable<(string Name, int TypeIndex, int OrderIndex)> _targetElementPath;
        private VisualElement _linkedElement;

        public VisualElement LinkedElement => _linkedElement;

        void Reset() => FindDocument();
        void OnEnable() => RefreshLink();

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

            if (_document?.rootVisualElement != null && _targetElementPath != null)
            {
                _linkedElement = FindElementByPath(_document.rootVisualElement, _targetElementPath);

                if (_linkedElement == null)
                    Debug.LogWarning($"No element found at path: {string.Join(" > ", _targetElementPath.Select(p => p.Name))}", this);
            }
        }

        public void SetElementPath(IEnumerable<(string Name, int TypeIndex, int OrderIndex)> path) =>
            _targetElementPath = path;

        [Button]
        public void PrintLinkedElement()
        {
            RefreshLink();
            if (_linkedElement != null && _targetElementPath != null)
            {
                var orderPath = string.Join(" > ", _targetElementPath.Select(p => $"{p.Name}[{p.OrderIndex}]"));
                Debug.Log($"Linked Element: {_linkedElement.name} (Order Path: {orderPath})", this);
            }
            else
            {
                Debug.Log("Linked Element: None", this);
            }
        }

        VisualElement FindElementByPath(VisualElement root, IEnumerable<(string Name, int TypeIndex, int OrderIndex)> path)
        {
            var current = root;
            foreach (var (name, typeIndex, orderIndex) in path)
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