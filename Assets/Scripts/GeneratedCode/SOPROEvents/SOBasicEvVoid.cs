using System.Collections.Generic;
using UnityEngine;
using System;
using SOPRO;
namespace SOPRO 
{
    /// <summary>
    /// Basic Scriptable Object event
    /// </summary>
    [CreateAssetMenu(fileName = "BasicEvent", menuName = "SOPRO/BasicEvents/Void")]
    [Serializable]
    public class SOBasicEvVoid : BaseSOEvVoid
    {
#if UNITY_EDITOR
        /// <summary>
        /// Description of the event, available only in UNITY_EDITOR
        /// </summary>
        [Multiline]
		[SerializeField]
        private string DEBUG_DeveloperDescription = "";
#endif

		        public delegate void SOBasicEvVoidDel();
        public event SOBasicEvVoidDel Event;

        /// <summary>
        /// Invokes all listeners of this event
        /// </summary>
        public override void Raise()
        {
			Event.Invoke();
        }
    }
}
