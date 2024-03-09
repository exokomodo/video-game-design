using UnityEngine;

/// <summary>
/// TagSelectorAttribute exposes an attribute that allows inspector selection of the tag values configured
/// Author: James Orson
/// </summary>

namespace KittyHawk.Attributes
{ 
    public class TagSelectorAttribute : PropertyAttribute
    {
        public bool UseDefaultTagFieldDrawer = false;
    }
}
