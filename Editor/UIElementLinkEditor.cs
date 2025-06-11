#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

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

            var link = selectedGameObject.GetComponent<UIElementLink>();
            if (link == null)
                return;

            if (link.Data.Path?.Length != 0)
                UIBuilderHookUtilities.SetSelectedElement(link.Data.Path);
        }
    }
}
#endif