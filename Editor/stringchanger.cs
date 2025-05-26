using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    public class stringchanger : MonoBehaviour
    {
        [SerializeField]
        UIElementLink link;

        void Start()
        {
            if(link.LinkedElement is Label label)
            {
                label.text = "New Text"; // Change the text of the label
            }
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
