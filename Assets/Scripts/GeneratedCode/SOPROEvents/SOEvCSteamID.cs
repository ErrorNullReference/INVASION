using System.Collections.Generic;
using UnityEngine;
using System;
using Steamworks;
namespace SOPRO 
{
    /// <summary>
    /// Scriptable Object event
    /// </summary>
    [CreateAssetMenu(fileName = "Event", menuName = "SOPRO/Events/CSteamID")]
    [Serializable]
    public class SOEvCSteamID : ScriptableObject
    {
#if UNITY_EDITOR
        /// <summary>
        /// Description of the event, available only in UNITY_EDITOR
        /// </summary>
        [Multiline]
		[SerializeField]
        private string DEBUG_DeveloperDescription = "";
#endif

				#if UNITY_EDITOR
		 				public CSteamID DEBUG_CSteamID_0 = default(CSteamID);
				#endif
		        [SerializeField]
        private readonly List<SOEvCSteamIDListener> listeners = new List<SOEvCSteamIDListener>();

        /// <summary>
        /// Invokes all listeners of this event
        /// </summary>
        public void Raise(CSteamID Value0)
        {
            for (int i = listeners.Count - 1; i >= 0; i--)
                listeners[i].OnEventRaised(Value0);
        }
        /// <summary>
        /// Adds listener to the event
        /// </summary>
        /// <param name="listener">listener to add</param>
        internal void AddListener(SOEvCSteamIDListener listener)
        {
            listeners.Add(listener);
        }
        /// <summary>
        /// Removes listener from the event
        /// </summary>
        /// <param name="listener">listener to remove</param>
        internal void RemoveListener(SOEvCSteamIDListener listener)
        {
            listeners.Remove(listener);
        }
    }
}
