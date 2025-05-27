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

    [ExecuteAlways]
    [AddComponentMenu("UI Toolkit/UI Element Link")]
    public partial class UIElementLink : MonoBehaviour
    {
        [SerializeField, HideInInspector] public UIElementLinkData Data = new();

        private UIDocument _document;
        private VisualElement _linkedElement;

        public VisualElement LinkedElement => _linkedElement ??= RefreshLink();

        public void Reset() => FetchDocument();
        public void OnEnable() => RefreshLink();
        public void Awake() => RefreshLink();
        public void Update() => SetGameObjectName();

        public UIDocument FetchDocument() =>
            _document ??= GetComponentInParent<UIDocument>();

        [Button]
        public VisualElement RefreshLink()
        {
            FetchDocument();
            _linkedElement = null;

            if (_document?.rootVisualElement != null && Data.Path != null)
                _linkedElement = UIBuilderHookUtilities.FindElementByPath(_document.rootVisualElement, Data.Path);

            return _linkedElement;
        }

        [Button]
        public void LinkToThisVisualElement()
        {
            if (_document?.rootVisualElement == null)
                return;

            var selectedPath = UIBuilderHookUtilities.GetSelectedElementPath(out var orderIndex);
            if(selectedPath == null || !selectedPath.Any())
                return;

            Data.Path = selectedPath.ToArray();
            Data.Path[^1].OrderIndex = orderIndex;

            _linkedElement = UIBuilderHookUtilities.FindElementByPath(_document.rootVisualElement, selectedPath);
        }

        public void SetElementPath(IEnumerable<UIElementPathEntry> path) =>
            Data.Path = path.ToArray();

        private void SetGameObjectName() =>
            gameObject.name = Data.Path[^1].DisplayName;
    }
}