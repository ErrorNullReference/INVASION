using UnityEngine;
using UnityEditor;
namespace SOPRO.Editor
{
    /// <summary>
    /// Class that modifies SOEvent inspector view
    /// </summary>
    [CustomEditor(typeof(SOBasicEvVoid))]
    public class SOBasicEvVoidDrawer : UnityEditor.Editor
    {
		private SOBasicEvVoid obj;
        /// <summary>
        /// Method that modifies SOEvent inspector view
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUI.enabled = Application.isPlaying;

            if (GUILayout.Button("Raise"))
                obj.Raise();
        }
		void OnEnable()
		{
			this.obj = target as SOBasicEvVoid;
		}
    }
}
