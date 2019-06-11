using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace YUR.SDK.Unity.Utilities
{
    public static class GameObjectExtensions
    {
        /// <summary>
        /// Checks if a GameObject has been destroyed.
        /// </summary>
        /// <param name="gameObject">GameObject reference to check for destructedness</param>
        /// <returns>If the game object has been marked as destroyed by UnityEngine</returns>
        public static bool IsDestroyed(this GameObject gameObject)
        {
            // UnityEngine overloads the == opeator for the GameObject type
            // and returns null when the object has been destroyed, but 
            // actually the object is still there but has not been cleaned up yet
            // if we test both we can determine if the object has been destroyed.
            return gameObject == null && !ReferenceEquals(gameObject, null);
        }
    }

    public class ParentGameObjectImmortality : MonoBehaviour
    {
        public List<GameObject> ChildrenToKeep = new List<GameObject>();

        void OnDestroy()
        {
            foreach(var child in ChildrenToKeep)
            {
                child.GetComponent<ChildGameObjectImmortal>().OnParentToBeDestroyed();
            }
        }

    }

    public class ChildGameObjectImmortal : MonoBehaviour
    {
        public virtual void OnParentToBeDestroyed()
        {
            Debug.Log("Parent object is being destroy", this);

        }
    }
}
