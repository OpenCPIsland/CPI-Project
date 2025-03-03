using Disney.Kelowna.Common;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class ParentLayoutElementUpdater : MonoBehaviour
	{
		public LayoutGroup SourceLayoutGroup;

		private LayoutElement parentLayoutElement;

		private void OnEnable()
		{
			CoroutineRunner.Start(waitForParent(), this, "waitForParent");
		}

		private IEnumerator waitForParent()
		{
			while (base.transform.parent == null || base.transform.parent.GetComponent<LayoutElement>() == null)
			{
				yield return null;
			}
			parentLayoutElement = base.transform.parent.GetComponent<LayoutElement>();
			CoroutineRunner.Start(waitForLayout(), this, "waitForLayout");
		}

		private IEnumerator waitForLayout()
		{
			while (Mathf.Abs(SourceLayoutGroup.preferredWidth) < float.Epsilon || Mathf.Abs(SourceLayoutGroup.preferredHeight) < float.Epsilon)
			{
				yield return null;
			}
			parentLayoutElement.preferredWidth = SourceLayoutGroup.preferredWidth;
			parentLayoutElement.preferredHeight = SourceLayoutGroup.preferredHeight;
		}

		private void OnDisable()
		{
			CoroutineRunner.StopAllForOwner(this);
			parentLayoutElement = null;
		}
	}
}
