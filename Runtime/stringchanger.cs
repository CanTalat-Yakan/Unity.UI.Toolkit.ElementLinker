using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    public class stringchanger : MonoBehaviour
    {
        UIElementLink _link;
        public void Start() => _link = GetComponent<UIElementLink>();

        [Button]
        public void Change()
        {
            _link.RefreshLink(); // Refresh the link to ensure it is up-to-date
            _link.PrintLinkedElement(); // Refresh the link to ensure it is up-to-date
            Debug.Log(_link.LinkedElement); // Log the linked element to the console
            if (_link.LinkedElement is Label label)
            {
                label.text = "New Text"; // Change the text of the label
                Debug.Log("Linked element is a Label: " + label.text); // Log the current text of the label
            }
        }
    }
}
