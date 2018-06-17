using System.Collections.Generic;
using UnityEngine;
using System;
using SOPRO;
using Steamworks;
namespace SOPRO
{
    /// <summary>
    /// Scriptable Object event
    /// </summary>
    [CreateAssetMenu(fileName = "Event", menuName = "SOPRO/Events/IntCSteamID")]
    [Serializable]
    public class SOEvIntCSteamID : BaseSOEvIntCSteamID
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
		 				/// <summary>
				/// Debug field for inspector view, available only in UNITY_EDITOR
				/// </summary>
				public int DEBUG_int_0 = default(int);
						/// <summary>
				/// Debug field for inspector view, available only in UNITY_EDITOR
				/// </summary>
				public CSteamID DEBUG_CSteamID_1 = default(CSteamID);
				#endif
		        [SerializeField]
        private readonly List<SOEvIntCSteamIDListener> listeners = new List<SOEvIntCSteamIDListener>();

        /// <summary>
        /// Invokes all listeners of this event
        /// </summary>
        public override void Raise(int Value0, CSteamID Value1)
        {
            for (int i = listeners.Count - 1; i >= 0; i--)
                listeners[i].OnEventRaised(Value0, Value1);
        }
        /// <summary>
        /// Adds listener to the event
        /// </summary>
        /// <param name="listener">listener to add</param>
        internal void AddListener(SOEvIntCSteamIDListener listener)
        {
            listeners.Add(listener);
        }
        /// <summary>
        /// Removes listener from the event
        /// </summary>
        /// <param name="listener">listener to remove</param>
        internal void RemoveListener(SOEvIntCSteamIDListener listener)
        {
            listeners.Remove(listener);
        }
    }
}
