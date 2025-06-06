using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    public class BaseScriptComponent<T> : MonoBehaviour where T : VisualElement
    {
        public UIDocument Document { get; private set; }

        private T[] _linkedElements;
        public T[] LinkedElements => _linkedElements ??= FetchLinkedElements();
        public bool HasElements => LinkedElements != null && LinkedElements.Length > 0;

        public T[] FetchLinkedElements()
        {
            var link = GetComponent<UIElementLink>();
            if (link != null)
            {
                if (link.LinkedElement is T linkedElement)
                    return new T[] { link.LinkedElement as T };

                Document = link.FetchDocument();
            }

            var query = GetComponent<UIElementQuery>();
            if (query != null)
            {
                if (UIElementTypes.GetElementType(query.Type) is T linkedElement)
                    return (T[])query.LinkedElements;

                Document = query.FetchDocument();
            }

            return null;
        }

        public void IterateLinkedElements(Action<T> action)
        {
            foreach (var element in LinkedElements)
            {
                if (element == null)
                    continue;

                action.Invoke(element);
            }
        }
    }
}
