using System.Collections.Generic;
using UnityEngine;
using System;
using SOPRO;
namespace SOPRO 
{
    /// <summary>
    /// Basic Scriptable Object event
    /// </summary>
    [CreateAssetMenu(fileName = "BasicEvent", menuName = "SOPRO/BasicEvents/GameNetworkObject")]
    [Serializable]
    public class SOBasicEvGameNetworkObject : BaseSOEvGameNetworkObject
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
		        public delegate void SOBasicEvGameNetworkObjectDel(GameNetworkObject Value0);
        public event SOBasicEvGameNetworkObjectDel Event;

        /// <summary>
        /// Invokes all listeners of this event
        /// </summary>
        public override void Raise(GameNetworkObject Value0)
        {
			if(Event != null)
				Event.Invoke(Value0);
        }
    }
}
