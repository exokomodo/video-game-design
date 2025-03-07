using UnityEngine;

/// <summary>
/// GameObjectExtensions provides a library of helpful methods for GameObject interactions
/// Author: James Orson
/// </summary>

namespace KittyHawk.Extensions
{
    public static class GameObjectExtensions
    {
        public static GameObject GetChildByTag(this GameObject parent, string tag)
        {
            foreach (Transform child in parent.transform)
            {
                if (child.CompareTag(tag))
                {
                    return child.gameObject;
                }
            }
            return null;
        }        
    }
}