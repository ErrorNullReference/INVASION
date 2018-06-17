using System.Collections.Generic;
using UnityEngine;
using System;
using SOPRO;
namespace SOPRO 
{
    /// <summary>
    /// Basic Scriptable Object event
    /// </summary>
    [CreateAssetMenu(fileName = "BasicEvent", menuName = "SOPRO/BasicEvents/Int")]
    [Serializable]
    public class SOBasicEvInt : BaseSOEvInt
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
		#endif
		        public delegate void SOBasicEvIntDel(int Value0);
        public event SOBasicEvIntDel Event;

        /// <summary>
        /// Invokes all listeners of this event
        /// </summary>
        public override void Raise(int Value0)
        {
			if(Event != null)
				Event.Invoke(Value0);
        }
    }
}
