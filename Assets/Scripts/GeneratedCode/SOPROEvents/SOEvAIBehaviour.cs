using System.Collections.Generic;
using UnityEngine;
using System;
namespace SOPRO 
{
    /// <summary>
    /// Scriptable Object event
    /// </summary>
    [CreateAssetMenu(fileName = "Event", menuName = "SOPRO/Events/AIBehaviour")]
    [Serializable]
    public class SOEvAIBehaviour : ScriptableObject
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
		 				public AIBehaviour DEBUG_AIBehaviour_0 = default(AIBehaviour);
				#endif
		        [SerializeField]
        private readonly List<SOEvAIBehaviourListener> listeners = new List<SOEvAIBehaviourListener>();

        /// <summary>
        /// Invokes all listeners of this event
        /// </summary>
        public void Raise(AIBehaviour Value0)
        {
            for (int i = listeners.Count - 1; i >= 0; i--)
                listeners[i].OnEventRaised(Value0);
        }
        /// <summary>
        /// Adds listener to the event
        /// </summary>
        /// <param name="listener">listener to add</param>
        internal void AddListener(SOEvAIBehaviourListener listener)
        {
            listeners.Add(listener);
        }
        /// <summary>
        /// Removes listener from the event
        /// </summary>
        /// <param name="listener">listener to remove</param>
        internal void RemoveListener(SOEvAIBehaviourListener listener)
        {
            listeners.Remove(listener);
        }
    }
}
