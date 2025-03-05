using System;
using UnityEngine;

namespace Gaskellgames
{
    // <summary>
    // Code created by Gaskellgames
    // </summary>

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class ShowAsStringAttribute : PropertyAttribute
    {
        public bool readOnly;

        public ShowAsStringAttribute(bool readOnly = false)
        {
            this.readOnly = readOnly;
        }

    } // class end
}
