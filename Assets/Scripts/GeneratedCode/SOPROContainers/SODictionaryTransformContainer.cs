using System;
using System.Collections.Generic;
using UnityEngine;

namespace SOPRO
{
    /// <summary>
    /// A class used to represent a shared container of objects
    /// </summary>
    [Serializable]
    [CreateAssetMenu(fileName = "Container", menuName = "SOPRO/Containers/SODictionaryTransformContainer")]
    public class SODictionaryTransformContainer : ScriptableObject
    {
        #if UNITY_EDITOR
        /// <summary>
        /// Description of the container, available only in UNITY_EDITOR
        /// </summary>
        [Multiline]
        [SerializeField]
        private string DEBUG_DeveloperDescription = "";
        #endif
        /// <summary>
        /// List of elements stored
        /// </summary>
        public Dictionary<int,Transform> Elements = new Dictionary<int,Transform>();

        /// <summary>
        /// Get/Set element at the given index
        /// </summary>
        /// <param name="i">index</param>
        /// <returns>value stored</returns>
        public Transform this [int i]
        {
            get { return Elements[i]; }
            set { Elements[i] = value; }
        }
    }
}
