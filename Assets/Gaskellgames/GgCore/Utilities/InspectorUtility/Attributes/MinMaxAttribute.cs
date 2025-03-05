using System;
using UnityEngine;

namespace Gaskellgames
{
    // <summary>
    // Code created by Gaskellgames
    // </summary>

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class MinMaxAttribute : PropertyAttribute
    {
        public float min;
        public float max;

        public MinMaxAttribute(float min, float max)
        {
            if (min < max)
            {
                this.min = min;
                this.max = max;
            }
            else
            {
                this.min = max;
                this.max = min;
            }
        }

    } // class end
}
