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
    }
}
#endif