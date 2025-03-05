using System;
using UnityEngine;

namespace Gaskellgames
{
    // <summary>
    // Code created by Gaskellgames
    // </summary>

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class TagDropdownAttribute : PropertyAttribute
    {
        public bool UseDefaultTagFieldDrawer = false;
        
    } // class end
}
