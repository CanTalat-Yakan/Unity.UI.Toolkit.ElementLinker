using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityEssentials
{
    [ExecuteAlways]
    [AddComponentMenu("UI Toolkit/UI Element Query")]
    public class UIElementQuery : MonoBehaviour
    {
        [Info]
        [SerializeField] private string _info;

        [Space]
        public UIElementType Type = UIElementType.Button;
        [OnValueChanged("Type")] public void OnTypeValueChanged() => RefreshLinks();

        public Action<VisualElement[]> OnRefreshLinks;

        public VisualElement[] LinkedElements => _linkedElements ??= RefreshLinks();
        private VisualElement[] _linkedElements;

        private UIDocument _document;

        public void Reset() => FetchDocument();
        public void OnEnable() => RefreshLinks();
        public void Awake() => RefreshLinks();

        public UIDocument FetchDocument() =>
            _document ??= GetComponentInParent<UIDocument>();

        [Button]
        public VisualElement[] RefreshLinks()
        {
            FetchDocument();

            _linkedElements = null;
            if (_document?.rootVisualElement != null)
                _linkedElements = QueryElementsOfType(Type);

            SetHelpBoxMessage();

            OnRefreshLinks?.Invoke(_linkedElements);

            return _linkedElements;
        }

        public void SetElementType(UIElementType type)
        {
            Type = type;
            OnTypeValueChanged();
        }

        public VisualElement[] QueryElementsOfType(UIElementType type)
        {
            var result = new List<VisualElement>();
            TraverseAndCollect(_document.rootVisualElement, type, result, isRoot: true);
            return result.ToArray();
        }

        private void TraverseAndCollect(VisualElement element, UIElementType type, List<VisualElement> result, bool isRoot = false)
        {
            if (!isRoot && UIElementTypes.GetElementType(element) == type)
                result.Add(element);

            foreach (var child in element.Children())
                TraverseAndCollect(child, type, result, isRoot: false);
        }

        private void SetHelpBoxMessage()
        {
#if UNITY_EDITOR
            if (_linkedElements == null)
                return;

            var linkedElementCount = _linkedElements.Length;
            var linkedElementType = Type;
            var uiAssetName = _document.visualTreeAsset.name;

            _info = $"Querying {linkedElementCount} elements of type {linkedElementType} in {uiAssetName}";
#endif
        }

#if UNITY_EDITOR
        [MenuItem("GameObject/ UI Toolkit/ Add Query", true, priority = 100)]
        private static bool ValidateAddQuery()
        {
            if (Selection.activeGameObject == null)
                return false;

            return Selection.activeGameObject.GetComponent<UIDocument>() != null;
        }

        [MenuItem("GameObject/ UI Toolkit/ Add Query", false, priority = 100)]
        private static void InstantiateQuery(MenuCommand menuCommand)
        {
            GameObject parent = Selection.activeGameObject;
            GameObject go = new GameObject("Query UIElements");
            go.AddComponent<UIElementQuery>();
            go.transform.SetParent(parent.transform, false);

            GameObjectUtility.SetParentAndAlign(go, parent);

            Undo.RegisterCreatedObjectUndo(go, "Create Query UIElements");
            Selection.activeObject = go;
        }

        public void Update() => SetGameObjectName();
        private void SetGameObjectName() =>
            gameObject.name = $"Query {ObjectNames.NicifyVariableName(Type.ToString())}s";

#endif
    }
}
