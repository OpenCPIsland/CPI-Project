#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Gaskellgames.EditorOnly
{
    /// <summary>
    /// Code created by Gaskellgames
    /// </summary>

    [CustomPropertyDrawer(typeof(LineSeparatorAttribute))]
    public class LineSeparatorDrawer : DecoratorDrawer
    {
        #region Variables

        private LineSeparatorAttribute attributeAsType;

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region Height

        public override float GetHeight()
        {
            attributeAsType = attribute as LineSeparatorAttribute;

            float totalSpacing = attributeAsType.thickness;
            if (attributeAsType.spacingBefore)
            {
                totalSpacing += 10;
            }

            if (attributeAsType.spacingAfter)
            {
                totalSpacing += 10;
            }

            return totalSpacing;
        }

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region OnGUI

        public override void OnGUI(Rect position)
        {
            float positionY = position.yMin;
            if (attributeAsType.spacingBefore)
            {
                positionY += 10;
            }

            Rect separatorRect = new Rect(position.xMin, positionY, position.width, attributeAsType.thickness);
            Color color = new Color32(attributeAsType.R, attributeAsType.G, attributeAsType.B, attributeAsType.A);
            EditorGUI.DrawRect(separatorRect, color);
        }

        #endregion

    } // class end
}

#endif