using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Actions
{
	public class AbortOnSpecifiedTargetAction : Action
	{
		private HashSet<GameObject> targetsToIgnore = new HashSet<GameObject>();

		public void AddTargetToIgnore(GameObject go)
		{
			targetsToIgnore.Add(go);
		}

		protected override void CopyTo(Action _destWarper)
		{
			AbortOnSpecifiedTargetAction abortOnSpecifiedTargetAction = _destWarper as AbortOnSpecifiedTargetAction;
			base.CopyTo(_destWarper);
			abortOnSpecifiedTargetAction.targetsToIgnore = targetsToIgnore;
		}

		protected override void OnEnable()
		{
			base.OnEnable();
		}

		protected override void Update()
		{
			GameObject target = GetTarget();
			if (targetsToIgnore.Contains(target))
			{
				targetsToIgnore.Remove(target);
				Abort();
			}
			else
			{
				Completed();
			}
		}
	}
}
