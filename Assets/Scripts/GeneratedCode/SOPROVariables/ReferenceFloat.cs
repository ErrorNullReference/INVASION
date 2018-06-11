using System;
using UnityEngine;
namespace SOPRO
{
    /// <summary>
    /// Class that holds a reference to a value
    /// </summary>
    [Serializable]
    public class ReferenceFloat
    {
        /// <summary>
        /// Determines whenever reference should use a given value or a Variable value
        /// </summary>
        public bool UseConstant;
        /// <summary>
        /// Variable currently stored
        /// </summary>
        public SOVariableFloat Variable;
        /// <summary>
        /// Current value
        /// </summary>
        public float Value
        {
            get { return UseConstant ? constantValue : Variable.Value; }
			set
			{
				if (UseConstant)
				    constantValue = value;
				else
				    Variable.Value = value;
			}
        }

        [SerializeField]
        private float constantValue;
        /// <summary>
        /// Construct a reference with default state
        /// </summary>
        public ReferenceFloat()
        {
        }
        /// <summary>
        /// Construct reference given an initial value
        /// </summary>
        /// <param name="value"></param>
        public ReferenceFloat(float value)
        {
            UseConstant = true;
            constantValue = value;
        }
        /// <summary>
        /// Conversion between reference to underlying value
        /// </summary>
        /// <param name="reference">reference to convert</param>
        public static implicit operator float(ReferenceFloat reference)
        {
            return reference.Value;
        }
    }
}
