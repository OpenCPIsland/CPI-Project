using ClubPenguin.UI;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.Catalog
{
	public class CatalogStatsShopItemScroller : CatalogShopItemScroller
	{
		public float SubmissionTextPreferredHieght = 70f;

		protected override void setupListeners()
		{
			base.setupListeners();
			VerticalScrollingLayoutElementPool scroller = Scroller;
			scroller.OnPoolReady = (Action)Delegate.Combine(scroller.OnPoolReady, new Action(onPoolReady));
		}

		private void onPoolReady()
		{
			Scroller.AddElement(1, 1);
		}

		protected override void onElementShown(int index, GameObject element)
		{
			base.onElementShown(index, element);
			if (index == 0)
			{
				LayoutElement component = element.transform.parent.gameObject.GetComponent<LayoutElement>();
				component.preferredHeight = SubmissionTextPreferredHieght;
			}
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			VerticalScrollingLayoutElementPool scroller = Scroller;
			scroller.OnPoolReady = (Action)Delegate.Remove(scroller.OnPoolReady, new Action(onPoolReady));
		}
	}
}
