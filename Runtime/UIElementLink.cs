using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityEssentials
{
    [ExecuteAlways]
    [AddComponentMenu("UI Toolkit/UI Element Link")]
    public partial class UIElementLink : MonoBehaviour
    {
        [Info] public string _;

        [Space]
        [SerializeField, HideInInspector] public UIElementPathEntry[] Data;

        public VisualElement LinkedElement => _linkedElement ??= RefreshLink();
        private VisualElement _linkedElement;

        private UIDocument _document;

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
                _linkedElement = UIBuilderHookUtilities.FindElementByPath(_document.rootVisualElement, Data);

            SetHelpBoxMessage();

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

            Data = selectedPath.ToArray();
            Data[^1].OrderIndex = orderIndex;

            _linkedElement = UIBuilderHookUtilities.FindElementByPath(_document.rootVisualElement, selectedPath);

            SetHelpBoxMessage();
        }

        public void SetElementPath(IEnumerable<UIElementPathEntry> path)
        {
            Data = path.ToArray();
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

                _ = $"Linked Element {linkedElementName}of type {linkedElementType} in {uiAssetName}";
            }
            else _ = "Error - Path not found";
#endif
        }

#if UNITY_EDITOR
        public void Update() => SetGameObjectName();
        private void SetGameObjectName()
        {
            if (Data?.Length == 0)
                return;

            string linkedElementName = string.IsNullOrEmpty(Data[^1].Name) 
                ? string.Empty 
                : $" ({Data[^1].DisplayName})";
            string linkedElementType = UIElementTypes.GetElementType(_linkedElement).ToString();

            gameObject.name = linkedElementType + linkedElementName;
        }
#endif
    }
}