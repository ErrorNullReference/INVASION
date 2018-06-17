using UnityEngine;
namespace SOPRO
{
    /// <summary>
    /// SO that holds a variable
    /// </summary>
    [CreateAssetMenu(fileName = "SOVariableVector3", menuName = "SOPRO/Variables/Vector3")]
    public class SOVariableVector3 : ScriptableObject
    {
#if UNITY_EDITOR
        /// <summary>
        /// Description of the variable, available only in UNITY_EDITOR
        /// </summary>
        [Multiline]
		[SerializeField]
        private string DEBUG_DeveloperDescription = "";
#endif
		/// <summary>
        /// Value stored in the variable
        /// </summary>
        public Vector3 Value;

        /// <summary>
        /// Sets value to given value
        /// </summary>
        /// <param name="value">new value</param>
        public void SetValue(Vector3 value)
        {
            this.Value = value;
        }
        /// <summary>
        /// Sets value to given value
        /// </summary>
        /// <param name="value">new value</param>
        public void SetValue(SOVariableVector3 value)
        {
            this.Value = value.Value;
        }
		/// <summary>
        /// Conversion between variable to underlying value
        /// </summary>
        /// <param name="variable">variable to convert</param>
        public static implicit operator Vector3(SOVariableVector3 variable)
        {
            return variable.Value;
        }
    }
}
