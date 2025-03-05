using System;
using UnityEngine;

namespace Gaskellgames
{
    // <summary>
    // Code created by Gaskellgames
    // </summary>

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class ContainsTypeAttribute : PropertyAttribute
    {
        public Type type;

        public ContainsTypeAttribute(Type type)
        {
            this.type = type;
        }

    } // class end
}
