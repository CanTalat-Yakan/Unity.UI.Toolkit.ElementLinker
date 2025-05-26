using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    public class stringchanger : MonoBehaviour
    {
        UIElementLink _link;
        public void Awake() => _link = GetComponent<UIElementLink>();
        public void Start() => Change();

        [Button]
        public void Change()
        {
            if (_link.LinkedElement is Label label)
            {
                label.text = "New Text"; // Change the text of the label
                Debug.Log("Linked element is a Label: " + label.text); // Log the current text of the label
            }
        }
    }
}
