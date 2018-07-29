using System;
using System.Collections.Generic;
using UnityEngine;

namespace SOPRO
{
    /// <summary>
    /// A class used to represent a shared container of objects
    /// </summary>
    [Serializable]
    [CreateAssetMenu(fileName = "Container", menuName = "SOPRO/Containers/SOListGenericContainer")]
    public class SOListGenericContainer : ScriptableObject
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
        public List<UnityEngine.Object> Elements = new List<UnityEngine.Object>();

        /// <summary>
        /// Get/Set element at the given index
        /// </summary>
        /// <param name="i">index</param>
        /// <returns>value stored</returns>
        public UnityEngine.Object this [int i]
        {
            get { return Elements[i]; }
            set { Elements[i] = value; }
        }
    }
}
