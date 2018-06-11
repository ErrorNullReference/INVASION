using System.Collections.Generic;
using UnityEngine;
using System;
using SOPRO;
namespace SOPRO 
{
    /// <summary>
    /// Scriptable Object event
    /// </summary>
    [CreateAssetMenu(fileName = "Event", menuName = "SOPRO/Events/GameNetworkObject")]
    [Serializable]
    public class SOEvGameNetworkObject : BaseSOEvGameNetworkObject
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
				public GameNetworkObject DEBUG_GameNetworkObject_0 = default(GameNetworkObject);
				#endif
		        [SerializeField]
        private readonly List<SOEvGameNetworkObjectListener> listeners = new List<SOEvGameNetworkObjectListener>();

        /// <summary>
        /// Invokes all listeners of this event
        /// </summary>
        public override void Raise(GameNetworkObject Value0)
        {
            for (int i = listeners.Count - 1; i >= 0; i--)
                listeners[i].OnEventRaised(Value0);
        }
        /// <summary>
        /// Adds listener to the event
        /// </summary>
        /// <param name="listener">listener to add</param>
        internal void AddListener(SOEvGameNetworkObjectListener listener)
        {
            listeners.Add(listener);
        }
        /// <summary>
        /// Removes listener from the event
        /// </summary>
        /// <param name="listener">listener to remove</param>
        internal void RemoveListener(SOEvGameNetworkObjectListener listener)
        {
            listeners.Remove(listener);
        }
    }
}
