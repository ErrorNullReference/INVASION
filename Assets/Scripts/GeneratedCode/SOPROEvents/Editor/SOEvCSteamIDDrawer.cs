using UnityEngine;
using UnityEditor;
namespace SOPRO.Editor
{
    /// <summary>
    /// Class that modifies SOEvent inspector view
    /// </summary>
    [CustomEditor(typeof(SOEvCSteamID))]
    public class SOEvCSteamIDDrawer : UnityEditor.Editor
    {
		private SOEvCSteamID obj;
        /// <summary>
        /// Method that modifies SOEvent inspector view
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUI.enabled = Application.isPlaying;

            if (GUILayout.Button("Raise"))
                obj.Raise(obj.DEBUG_CSteamID_0);
        }
		void OnEnable()
		{
			this.obj = target as SOEvCSteamID;
		}
    }
}
