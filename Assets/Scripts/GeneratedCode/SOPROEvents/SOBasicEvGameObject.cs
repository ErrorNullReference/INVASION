using System.Collections.Generic;
using UnityEngine;
using System;
using SOPRO;
namespace SOPRO 
{
    /// <summary>
    /// Basic Scriptable Object event
    /// </summary>
    [CreateAssetMenu(fileName = "BasicEvent", menuName = "SOPRO/BasicEvents/GameObject")]
    [Serializable]
    public class SOBasicEvGameObject : BaseSOEvGameObject
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
				public GameObject DEBUG_GameObject_0 = default(GameObject);
		#endif
		        public delegate void SOBasicEvGameObjectDel(GameObject Value0);
        public event SOBasicEvGameObjectDel Event;

        /// <summary>
        /// Invokes all listeners of this event
        /// </summary>
        public override void Raise(GameObject Value0)
        {
			if(Event != null)
				Event.Invoke(Value0);
        }
    }
}
