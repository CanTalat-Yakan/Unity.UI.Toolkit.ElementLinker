#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    public class UIElementLinker
    {
        static Button s_buttonElement;

        [InitializeOnLoadMethod]
        public static void Initialize()
        {
            UIBuilderHook.OnFocusedChanged += OnFocusedChanged;
            UIBuilderHook.OnSelectionChanged += OnSelectionChanged;
        }

        private static void OnFocusedChanged()
        {
            if (s_buttonElement == null)
                AddButtonToUIBuilder(UIBuilderHook.Inspector);
        }

        private static void OnSelectionChanged()
        {
            if (s_buttonElement != null)
                ChangeButtonState();
        }

        private static void AddButtonToUIBuilder(VisualElement root)
        {
            if (root == null)
                return;

            s_buttonElement = new Button();
            s_buttonElement.text = "Click Me!";
            s_buttonElement.RegisterCallback<ClickEvent>(evt => InstantiateLinkToVisualElement());
            s_buttonElement.visible = UIBuilderHook.GetSelectedElement() != null;

            root.Add(s_buttonElement);
        }

        private static void ChangeButtonState()
        {
            var element = UIBuilderHook.GetSelectedElement();
            s_buttonElement.visible = element != null;
            if (element != null)
                s_buttonElement.text = "Create Link for " + UIBuilderHook.GetElementName(element);
        }

        private static void InstantiateLinkToVisualElement()
        {
            var element = UIBuilderHook.GetSelectedElement();
            var name = UIBuilderHook.GetElementName(element);
            var path = UIBuilderHook.GetElementPath(element);

            var asset = UIBuilderHook.VisualTreeAsset;

            var ui = Object.FindObjectsByType<UIDocument>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
                .Where(d => d.visualTreeAsset == asset).First();

            var go = new GameObject(name);
            go.transform.parent = ui.transform;
            go.AddComponent<UIElementLink>().SetElementPath(path);

            Selection.activeGameObject = go;
        }
    }
}
#endif