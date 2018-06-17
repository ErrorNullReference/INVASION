using System.Collections.Generic;
using UnityEngine;
using System;
using SOPRO;
namespace SOPRO 
{
    /// <summary>
    /// Base class for Scriptable Object event
    /// </summary>
    [Serializable]
    public abstract class BaseSOEvInt : ScriptableObject
    {
        /// <summary>
        /// Invokes all listeners of this event
        /// </summary>
        public abstract void Raise(int Value0);
    }
}
