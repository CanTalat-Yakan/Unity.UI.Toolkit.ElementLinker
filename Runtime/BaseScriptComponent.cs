using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    public class BaseScriptComponent<T> : MonoBehaviour where T : VisualElement
    {
        public UIElementType Type => _type ??= FetchType();
        private UIElementType? _type;
        
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
                if (link.LinkedElement is T linkedElement)
                {
                    _type = UIElementTypes.GetElementType(link.LinkedElement);
                    _document = link.FetchDocument();
                    return new T[] { link.LinkedElement as T };
                }

            var query = GetComponent<UIElementQuery>();
            if (query != null)
                if (UIElementTypes.GetElementType(query.Type) is T linkedElement)
                {
                    _type = UIElementTypes.GetElementType(link.LinkedElement);
                    _document = query.FetchDocument();
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
            FetchLinkedElements();
            return _type.Value;
        }
    }
}
