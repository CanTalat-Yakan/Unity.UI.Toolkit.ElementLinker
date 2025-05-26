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
                s_buttonElement.text = "Create Link for " + UIBuilderHookUtilities.GetElementName(element);
        }

        private static void InstantiateLinkToVisualElement()
        {
            var element = UIBuilderHook.GetSelectedElement();
            var path = UIBuilderHookUtilities.GetElementPath(element, out var orderIndex);
                
            var name = UIBuilderHookUtilities.GetElementName(element);
            var displayName = UIBuilderHookUtilities.GetElementDisplayName(element);
            if(orderIndex > 0)
                displayName += $" {orderIndex}";

            var asset = UIBuilderHook.VisualTreeAsset;

            var ui = Object.FindObjectsByType<UIDocument>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
                .Where(d => d.visualTreeAsset == asset).First();

            var go = new GameObject(displayName);
            go.transform.parent = ui.transform;
            go.AddComponent<UIElementLink>().SetElementPath(path);

            Selection.activeGameObject = go;
        }
    }
}
#endif