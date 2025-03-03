using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Actions
{
	public class ToggleVisibilityAction : Action
	{
		public List<GameObject> Objects = new List<GameObject>();

		public bool Toggle;

		public bool On;

		public bool IncludeChildren = false;

		public float Delay;

		private float elapsedTime;

		protected override void CopyTo(Action _destWarper)
		{
			ToggleVisibilityAction toggleVisibilityAction = _destWarper as ToggleVisibilityAction;
			toggleVisibilityAction.On = On;
			toggleVisibilityAction.Toggle = Toggle;
			toggleVisibilityAction.IncludeChildren = IncludeChildren;
			toggleVisibilityAction.Objects = new List<GameObject>(Objects);
			toggleVisibilityAction.Delay = Delay;
			base.CopyTo(_destWarper);
		}

		private void hideObject(GameObject obj)
		{
			Renderer[] array = (!IncludeChildren) ? new Renderer[1]
			{
				obj.GetComponentInChildren<Renderer>()
			} : obj.GetComponentsInChildren<Renderer>();
			if (array.Length > 0)
			{
				Renderer[] array2 = array;
				foreach (Renderer renderer in array2)
				{
					if (Toggle)
					{
						renderer.enabled = !renderer.enabled;
					}
					else
					{
						renderer.enabled = On;
					}
				}
				return;
			}
			LODGroup componentInChildren = obj.GetComponentInChildren<LODGroup>();
			if (componentInChildren != null)
			{
				if (Toggle)
				{
					componentInChildren.enabled = !componentInChildren.enabled;
				}
				else
				{
					componentInChildren.enabled = On;
				}
			}
		}

		protected override void Update()
		{
			if (elapsedTime < Delay)
			{
				elapsedTime += Time.deltaTime;
				return;
			}
			if (Objects.Count == 0)
			{
				hideObject(GetTarget());
			}
			else
			{
				for (int i = 0; i < Objects.Count; i++)
				{
					if (Objects[i] != null)
					{
						hideObject(Objects[i]);
					}
				}
			}
			Completed();
		}
	}
}
