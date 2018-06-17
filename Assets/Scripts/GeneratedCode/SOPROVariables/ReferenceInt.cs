using System;
using UnityEngine;
namespace SOPRO
{
    /// <summary>
    /// Class that holds a reference to a value
    /// </summary>
    [Serializable]
    public class ReferenceInt
    {
        /// <summary>
        /// Determines whenever reference should use a given value or a Variable value
        /// </summary>
        public bool UseConstant;
        /// <summary>
        /// Variable currently stored
        /// </summary>
        public SOVariableInt Variable;
        /// <summary>
        /// Current value
        /// </summary>
        public int Value
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
        private int constantValue;
        /// <summary>
        /// Construct a reference with default state
        /// </summary>
        public ReferenceInt()
        {
        }
        /// <summary>
        /// Construct reference given an initial value
        /// </summary>
        /// <param name="value"></param>
        public ReferenceInt(int value)
        {
            UseConstant = true;
            constantValue = value;
        }
        /// <summary>
        /// Conversion between reference to underlying value
        /// </summary>
        /// <param name="reference">reference to convert</param>
        public static implicit operator int(ReferenceInt reference)
        {
            return reference.Value;
        }
    }
}
