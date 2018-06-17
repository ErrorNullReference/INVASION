using System;
using UnityEngine.Events;
using UnityEngine;
namespace SOPRO
{
    /// <summary>
    /// Wrapper for a unity event, usefull in order to see generic unityevent-T- in inspector
    /// </summary>
    [Serializable]
    public class UnEvInt : UnityEvent<int> { }
}
