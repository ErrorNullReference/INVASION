using System;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

namespace SOPRO
{
    /// <summary>
    /// A class used to represent a shared container of objects
    /// </summary>
    [Serializable]
    [CreateAssetMenu(fileName = "Container", menuName = "SOPRO/Containers/SOListPlayerContainer")]
    public class SOListPlayerContainer : ScriptableObject
    {
        #if UNITY_EDITOR
        /// <summary>
        /// Description of the container, available only in UNITY_EDITOR
        /// </summary>
        [Multiline]
        [SerializeField]
        private string DEBUG_DeveloperDescription = "";
        #endif
        /// <summary>
        /// List of elements stored
        /// </summary>
        public List<Player> Elements = new List<Player>();

        void OnEnable()
        {
            Client.OnUserDisconnected += Remove;
        }

        void OnDisable()
        {
            Client.OnUserDisconnected -= Remove;
        }

        /// <summary>
        /// Get/Set element at the given index
        /// </summary>
        /// <param name="i">index</param>
        /// <returns>value stored</returns>
        public Player this [int i]
        {
            get { return i >= 0 && i < Elements.Count ? Elements[i] : null; }
            set
            {
                if (i >= 0 && i < Elements.Count)
                    Elements[i] = value;
            }
        }

        void Remove(CSteamID id)
        {
            if (Elements == null)
                return;

            for (int i = 0; i < Elements.Count; i++)
            {
                if (Elements[i] != null && Elements[i].Avatar != null && Elements[i].Avatar.UserInfo != null && Elements[i].Avatar.UserInfo.SteamID == id)
                {
                    Elements.RemoveAt(i);
                    return;
                }
            }
        }
    }
}
