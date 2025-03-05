using System;
using UnityEngine;

namespace Gaskellgames
{
    // <summary>
    // Code created by Gaskellgames
    // </summary>

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class UnitAttribute : PropertyAttribute
    {
        public GgMaths.Units unit;
        
        public UnitAttribute(GgMaths.Units unit)
        {
            this.unit = unit;
        }
        
    } // class end
}