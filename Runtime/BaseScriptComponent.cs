using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    public class BaseScriptComponent<T> : MonoBehaviour where T : VisualElement
    {
        public UIElementType Type => FetchType();

        public UIDocument Document => _document ??= FetchDocument();
        private UIDocument _document;

        public T[] LinkedElements => _linkedElements ??= FetchLinkedElements();
        private T[] _linkedElements;

        public bool HasElements => LinkedElements != null && LinkedElements.Length > 0;

        public void IterateLinkedElements(Action<T> action)
        {
            foreach (var element in LinkedElements)
            {
                if (element == null)
                    continue;

                action.Invoke(element);
            }
        }

        private T[] FetchLinkedElements()
        {
            var link = GetComponent<UIElementLink>();
            if (link != null)
                if (link.LinkedElement is T)
                {
                    _document = link.FetchDocument();

                    link.OnRefreshLink += (e) => { _linkedElements = new T[] { e as T }; };
                    return new T[] { link.LinkedElement as T };
                }

            var query = GetComponent<UIElementQuery>();
            if (query != null)
                if (UIElementTypes.GetElementType(query.Type) is T)
                {
                    _document = query.FetchDocument();

                    query.OnRefreshLinks += (e) => { _linkedElements = e as T[]; };
                    return (T[])query.LinkedElements;
                }

            return null;
        }

        private UIDocument FetchDocument()
        {
            FetchLinkedElements();
            return _document;
        }

        private UIElementType FetchType()
        {
            if (!HasElements)
                return UIElementType.None;

            return UIElementTypes.GetElementType(LinkedElements[0]);
        }
    }
}
