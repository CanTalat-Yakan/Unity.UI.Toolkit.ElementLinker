using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    [Serializable]
    public class UIElementPathData
    {
        public UIElementPathEntry[] Path;
    }

    [ExecuteAlways]
    [AddComponentMenu("UI Toolkit/UI Element Link")]
    public partial class UIElementLink : MonoBehaviour
    {
        [Info]
        [SerializeField] private string _info;

        [Space]
        [HideInInspector] public UIElementPathData Data;

        public Action<VisualElement> OnRefreshLink;

        public VisualElement LinkedElement => _linkedElement ??= RefreshLink();
        private VisualElement _linkedElement;

        [SerializeField, HideInInspector] private UIDocument _document;

        public void Reset() => FetchDocument();
        public void OnEnable() => RefreshLink();
        public void Awake() => RefreshLink();

        public UIDocument FetchDocument() =>
            _document ??= GetComponentInParent<UIDocument>();

        [Button]
        public VisualElement RefreshLink()
        {
            FetchDocument();

            _linkedElement = null;
            if (_document?.rootVisualElement != null && Data != null)
                _linkedElement = UIBuilderHookUtilities.FindElementByPath(_document.rootVisualElement, Data.Path);

            SetHelpBoxMessage();

            OnRefreshLink?.Invoke(_linkedElement);

            return _linkedElement;
        }

        [Button]
        public void LinkToThisVisualElement()
        {
            if (_document?.rootVisualElement == null)
                return;

            var selectedPath = UIBuilderHookUtilities.GetSelectedElementPath(out var orderIndex);
            if (selectedPath == null || !selectedPath.Any())
                return;

            Data ??= new();
            Data.Path = selectedPath.ToArray();
            Data.Path[^1].OrderIndex = orderIndex;

            _linkedElement = UIBuilderHookUtilities.FindElementByPath(_document.rootVisualElement, selectedPath);

            SetHelpBoxMessage();
        }

        public void SetElementPath(IEnumerable<UIElementPathEntry> path)
        {
            if (path == null || _document?.rootVisualElement == null)
                return;

            Data ??= new();
            Data.Path = path.ToArray();
            RefreshLink();
        }

        public void SetElementPath(VisualElement element)
        {
            if(element == null || _document?.rootVisualElement == null)
                return;

            Data ??= new();
            Data.Path = UIBuilderHookUtilities.GetElementPath(element, out _).ToArray(); 
            RefreshLink();
        }

        private void SetHelpBoxMessage()
        {
#if UNITY_EDITOR
            if (_linkedElement != null)
            {
                var linkedElementName = string.IsNullOrEmpty(_linkedElement.name)
                    ? string.Empty
                    : $"#{_linkedElement.name} ";
                var linkedElementType = UIElementTypes.GetElementType(_linkedElement);
                var uiAssetName = _document.visualTreeAsset.name;

                _info = $"Linked Element {linkedElementName}of type {linkedElementType} in {uiAssetName}";
            }
            else _info = "Error - Linked Path Not Found!";
#endif
        }

#if UNITY_EDITOR
        public void Update() => 
            SetGameObjectName();

        private void SetGameObjectName()
        {
            if (Data == null)
                return;

            if (Data.Path?.Length == 0)
                return;

            string linkedElementName = string.IsNullOrEmpty(GetName())
                ? string.Empty
                : $" ({GetDisplayName()})";
            var linkedElementType = UIElementTypes.GetElementType(_linkedElement);

            gameObject.name = linkedElementType + linkedElementName;
        }
        
        private string GetName()
        {
            if (Data == null || Data.Path == null || Data.Path.Length == 0)
                return string.Empty;

            if (Data.Path.Length == 1)
                return Data.Path[0].Name;
            else if (Data.Path.Length > 1)
                return Data.Path[^1].Name;

            return string.Empty;
        }

        private string GetDisplayName()
        {
            if (Data == null || Data.Path == null || Data.Path.Length == 0)
                return string.Empty;

            if (Data.Path.Length == 1)
                return Data.Path[0].DisplayName;
            else if (Data.Path.Length > 1)
                return Data.Path[^1].DisplayName;

            return string.Empty;
        }
#endif
    }
}