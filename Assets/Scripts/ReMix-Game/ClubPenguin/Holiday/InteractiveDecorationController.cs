using Disney.LaunchPadFramework;
using UnityEngine;

namespace ClubPenguin.Holiday
{
	public class InteractiveDecorationController : MonoBehaviour
	{
		public InteractiveDecorationTarget TargetObject;

		private int groupIndex;

		private int groupMax;

		private void Start()
		{
			groupIndex = 0;
			groupMax = base.transform.childCount;
			if (TargetObject == null)
			{
				Log.LogError(this, string.Format("O_o\t Error: {0} does not have it's target object set", base.gameObject.GetPath()));
			}
		}

		public void OnTargetHit()
		{
			Transform child = base.transform.GetChild(groupIndex);
			InteractiveDecorationGroup component = child.GetComponent<InteractiveDecorationGroup>();
			if (!(component != null))
			{
				return;
			}
			component.OnColorChange();
			if (++groupIndex >= groupMax)
			{
				groupIndex = 0;
				if (TargetObject != null)
				{
					TargetObject.OnColorChange();
				}
			}
		}
	}
}
