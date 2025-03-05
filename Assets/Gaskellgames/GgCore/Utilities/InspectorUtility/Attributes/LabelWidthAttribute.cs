using System;
using UnityEngine;

namespace Gaskellgames
{
    // <summary>
    // Code created by Gaskellgames
    // </summary>

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class LabelWidthAttribute : PropertyAttribute
    {
        public int labelWidth;
        
        public LabelWidthAttribute(int labelWidth)
        {
            this.labelWidth = labelWidth;
        }
        
    } // class end
}
