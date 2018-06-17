using UnityEngine;
namespace SOPRO
{
    /// <summary>
    /// SO that holds a variable
    /// </summary>
    [CreateAssetMenu(fileName = "SOVariablePowerUpType", menuName = "SOPRO/Variables/PowerUpType")]
    public class SOVariablePowerUpType : ScriptableObject
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
        public PowerUpType Value;

        /// <summary>
        /// Sets value to given value
        /// </summary>
        /// <param name="value">new value</param>
        public void SetValue(PowerUpType value)
        {
            this.Value = value;
        }
        /// <summary>
        /// Sets value to given value
        /// </summary>
        /// <param name="value">new value</param>
        public void SetValue(SOVariablePowerUpType value)
        {
            this.Value = value.Value;
        }
		/// <summary>
        /// Conversion between variable to underlying value
        /// </summary>
        /// <param name="variable">variable to convert</param>
        public static implicit operator PowerUpType(SOVariablePowerUpType variable)
        {
            return variable.Value;
        }
    }
}
