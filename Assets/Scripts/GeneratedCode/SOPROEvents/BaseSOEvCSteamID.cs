using System.Collections.Generic;
using UnityEngine;
using System;
using SOPRO;
using Steamworks;
namespace SOPRO
{
    /// <summary>
    /// Base class for Scriptable Object event
    /// </summary>
    [Serializable]
    public abstract class BaseSOEvCSteamID : ScriptableObject
    {
        /// <summary>
        /// Invokes all listeners of this event
        /// </summary>
        public abstract void Raise(CSteamID Value0);
    }
}
