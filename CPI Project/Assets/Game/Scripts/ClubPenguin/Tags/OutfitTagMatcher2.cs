using System;
using UnityEngine;

namespace ClubPenguin.Tags
{
	[Serializable]
	public class OutfitTagMatcher2 : BaseOutfitTagMatcher, ITagMatcher
	{
		[SerializeField]
		private OutfitTagMatcher3[] matchers;

		public BaseTagMatcher[] Matchers
		{
			get
			{
				return matchers;
			}
		}
	}
}
