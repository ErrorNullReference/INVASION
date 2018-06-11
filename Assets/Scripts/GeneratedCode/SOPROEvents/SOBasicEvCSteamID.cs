using System.Collections.Generic;
using UnityEngine;
using System;
using Steamworks;
using SOPRO;
namespace SOPRO 
{
    /// <summary>
    /// Basic Scriptable Object event
    /// </summary>
    [CreateAssetMenu(fileName = "BasicEvent", menuName = "SOPRO/BasicEvents/CSteamID")]
    [Serializable]
    public class SOBasicEvCSteamID : BaseSOEvCSteamID
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
				public CSteamID DEBUG_CSteamID_0 = default(CSteamID);
		#endif
		        public delegate void SOBasicEvCSteamIDDel(CSteamID Value0);
        public event SOBasicEvCSteamIDDel Event;

        /// <summary>
        /// Invokes all listeners of this event
        /// </summary>
        public override void Raise(CSteamID Value0)
        {
			Event.Invoke(Value0);
        }
    }
}
