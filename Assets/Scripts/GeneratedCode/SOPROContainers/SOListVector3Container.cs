using System;
using System.Collections.Generic;
using UnityEngine;
namespace SOPRO
{
	/// <summary>
    /// A class used to represent a shared container of objects
    /// </summary>
    [Serializable]
	[CreateAssetMenu(fileName = "Container", menuName = "SOPRO/Containers/SOListVector3Container")]
    public class SOListVector3Container : ScriptableObject
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
        public List<Vector3> Elements = new List<Vector3>();
		        /// <summary>
        /// Get/Set element at the given index
        /// </summary>
        /// <param name="i">index</param>
        /// <returns>value stored</returns>
        public Vector3 this[int i]
        {
            get { return Elements[i]; }
            set { Elements[i] = value; }
        }
		    }
}
