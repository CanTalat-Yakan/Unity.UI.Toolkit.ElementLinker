#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UnityEssentials
{
    public class UIElementLinkEditor
    {
        [InitializeOnLoadMethod]
        public static void Initialization() =>
            HierarchyHook.OnSelectionChanged += HandleSelectionChanged;

        private static void HandleSelectionChanged(GameObject[] gameObjects)
        {
            var selectedGameObject = gameObjects.First();

            var uiElementLink = selectedGameObject.GetComponent<UIElementLink>();
            if (uiElementLink == null)
                return;

            UIBuilderHookUtilities.SetSelectedElement(uiElementLink.Data.Path);
        }
    }
}
#endif