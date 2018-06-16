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
    public abstract class BaseSOEvIntCSteamID : ScriptableObject
    {
        /// <summary>
        /// Invokes all listeners of this event
        /// </summary>
        public abstract void Raise(int Value0, CSteamID Value1);
    }
}
