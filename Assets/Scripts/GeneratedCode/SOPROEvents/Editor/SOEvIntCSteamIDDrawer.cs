using UnityEngine;
using UnityEditor;
namespace SOPRO.Editor
{
    /// <summary>
    /// Class that modifies SOEvent inspector view
    /// </summary>
    [CustomEditor(typeof(SOEvIntCSteamID))]
    public class SOEvIntCSteamIDDrawer : UnityEditor.Editor
    {
		private SOEvIntCSteamID obj;
        /// <summary>
        /// Method that modifies SOEvent inspector view
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUI.enabled = Application.isPlaying;

            if (GUILayout.Button("Raise"))
                obj.Raise(obj.DEBUG_int_0 ,obj.DEBUG_CSteamID_1);
        }
		void OnEnable()
		{
			this.obj = target as SOEvIntCSteamID;
		}
    }
}