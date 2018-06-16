using System.Collections.Generic;
using UnityEngine;
using System;
using SOPRO;
namespace SOPRO 
{
    /// <summary>
    /// Basic Scriptable Object event
    /// </summary>
    [CreateAssetMenu(fileName = "BasicEvent", menuName = "SOPRO/BasicEvents/Bool")]
    [Serializable]
    public class SOBasicEvBool : BaseSOEvBool
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
				public bool DEBUG_bool_0 = default(bool);
		#endif
		        public delegate void SOBasicEvBoolDel(bool Value0);
        public event SOBasicEvBoolDel Event;

        /// <summary>
        /// Invokes all listeners of this event
        /// </summary>
        public override void Raise(bool Value0)
        {
			if(Event != null)
				Event.Invoke(Value0);
        }
    }
}
