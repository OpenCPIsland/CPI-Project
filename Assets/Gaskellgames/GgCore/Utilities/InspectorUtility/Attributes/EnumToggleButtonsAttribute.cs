using System;
using UnityEngine;

namespace Gaskellgames
{
    // <summary>
    // Code created by Gaskellgames
    // </summary>

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class EnumToggleButtonsAttribute : PropertyAttribute
    {
        public bool showLabel;
        
        public bool enumFlags;
        public bool enumIncludesNothing;
        public bool enumIncludesEverything;
        public int minWidth;

        public EnumToggleButtonsAttribute()
        {
            this.showLabel = false;
            
            this.enumFlags = false;
            this.enumIncludesNothing = false;
            this.enumIncludesEverything = false;
            this.minWidth = 50;
        }

        public EnumToggleButtonsAttribute(bool showLabel)
        {
            this.showLabel = showLabel;
            
            this.enumFlags = false;
            this.enumIncludesNothing = false;
            this.enumIncludesEverything = false;
            this.minWidth = 50;
        }

        public EnumToggleButtonsAttribute(bool showLabel = false, bool enumFlags = false, bool enumIncludesNothing = false, bool enumIncludesEverything = false, int minWidth = 50)
        {
            this.showLabel = showLabel;
            
            this.enumFlags = enumFlags;
            this.enumIncludesNothing = enumIncludesNothing;
            this.enumIncludesEverything = enumIncludesEverything;
            this.minWidth = minWidth;
        }

    } // class end
}

