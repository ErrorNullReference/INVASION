using System;
using UnityEngine;
using Steamworks;
namespace SOPRO
{
    /// <summary>
    /// Listener for Scriptable Object event
    /// </summary>
    [Serializable]
    public class SOEvIntCSteamIDListener : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Event to register with.")]
        private SOEvIntCSteamID Event;
        [SerializeField]
        [Tooltip("Response to invoke when Event is raised.")]
        private UnEvIntCSteamID Actions;

        /// <summary>
        /// Adds listener to event
        /// </summary>
        protected void RegisterToEvent()
        {
            Event.AddListener(this);
        }
        /// <summary>
        /// Removes listener from event
        /// </summary>
        protected void UnregisterToEvent()
        {
            Event.RemoveListener(this);
        }
				protected virtual void Awake()
        {
            Event.AddListener(this);
        }
		protected virtual void OnDestroy()
        {
            Event.RemoveListener(this);
        }
		        /// <summary>
        /// Invokes unity event
        /// </summary>
        internal void OnEventRaised(int Value0, CSteamID Value1)
        {
            Actions.Invoke(Value0, Value1);
        }
    }
}
