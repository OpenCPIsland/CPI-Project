using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TextMinSizeUpdaterTMP : AbstractMinSizeUpdater
    {
        protected override ILayoutElement getTargetLayoutElement()
        {
            return GetComponent<TextMeshProUGUI>();
        }
    }
}
