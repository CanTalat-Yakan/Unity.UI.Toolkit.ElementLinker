using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    public class BaseScriptComponent<T> : BaseScriptComponent where T : VisualElement
    {
        public new T[] LinkedElements => base.LinkedElements?.OfType<T>().ToArray();
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

    public class BaseScriptComponent : MonoBehaviour
    {
        public UIElementType Type => FetchType();
        private UIElementType _type;

        public UIDocument Document => _document ??= FetchDocument();
        private UIDocument _document;

        public VisualElement[] LinkedElements => _linkedElements ??= FetchLinkedElements();
        public VisualElement[] _linkedElements;

        public bool HasElements => LinkedElements != null && LinkedElements.Length > 0;

        public void IterateLinkedElements(Action<VisualElement> action)
        {
            foreach (var element in LinkedElements)
            {
                if (element == null)
                    continue;

                action.Invoke(element);
            }
        }

        private VisualElement[] FetchLinkedElements()
        {
            var link = GetComponent<UIElementLink>();
            if (link != null)
            {
                _type = UIElementTypes.GetElementType(link.LinkedElement);
                _document = link.FetchDocument();
                link.OnRefreshLink += (e) => { _linkedElements = new VisualElement[] { e }; };
                return new VisualElement[] { link.LinkedElement };
            }

            var query = GetComponent<UIElementQuery>();
            if (query != null)
            {
                _type = query.Type;
                _document = query.FetchDocument();
                query.OnRefreshLinks += (e) => { _linkedElements = e; };
                return query.LinkedElements;
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
            return _type;
        }
    }
}