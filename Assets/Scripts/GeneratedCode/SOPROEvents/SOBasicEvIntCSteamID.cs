using System.Collections.Generic;
using UnityEngine;
using System;
using SOPRO;
using Steamworks;
namespace SOPRO
{
    /// <summary>
    /// Basic Scriptable Object event
    /// </summary>
    [CreateAssetMenu(fileName = "BasicEvent", menuName = "SOPRO/BasicEvents/IntCSteamID")]
    [Serializable]
    public class SOBasicEvIntCSteamID : BaseSOEvIntCSteamID
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
		        public delegate void SOBasicEvIntCSteamIDDel(int Value0, CSteamID Value1);
        public event SOBasicEvIntCSteamIDDel Event;

        /// <summary>
        /// Invokes all listeners of this event
        /// </summary>
        public override void Raise(int Value0, CSteamID Value1)
        {
			if(Event != null)
				Event.Invoke(Value0, Value1);
        }
    }
}
