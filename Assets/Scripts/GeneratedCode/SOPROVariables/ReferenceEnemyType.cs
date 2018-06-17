using System;
using UnityEngine;
namespace SOPRO
{
    /// <summary>
    /// Class that holds a reference to a value
    /// </summary>
    [Serializable]
    public class ReferenceEnemyType
    {
        /// <summary>
        /// Determines whenever reference should use a given value or a Variable value
        /// </summary>
        public bool UseConstant;
        /// <summary>
        /// Variable currently stored
        /// </summary>
        public SOVariableEnemyType Variable;
        /// <summary>
        /// Current value
        /// </summary>
        public EnemyType Value
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
        private EnemyType constantValue;
        /// <summary>
        /// Construct a reference with default state
        /// </summary>
        public ReferenceEnemyType()
        {
        }
        /// <summary>
        /// Construct reference given an initial value
        /// </summary>
        /// <param name="value"></param>
        public ReferenceEnemyType(EnemyType value)
        {
            UseConstant = true;
            constantValue = value;
        }
        /// <summary>
        /// Conversion between reference to underlying value
        /// </summary>
        /// <param name="reference">reference to convert</param>
        public static implicit operator EnemyType(ReferenceEnemyType reference)
        {
            return reference.Value;
        }
    }
}
