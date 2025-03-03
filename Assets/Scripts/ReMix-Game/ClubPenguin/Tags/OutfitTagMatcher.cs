using System;
using UnityEngine;

namespace ClubPenguin.Tags
{
	[Serializable]
	public class OutfitTagMatcher : BaseOutfitTagMatcher, ITagMatcher
	{
		[SerializeField]
		private OutfitTagMatcher2[] matchers;

		public BaseTagMatcher[] Matchers
		{
			get
			{
				return matchers;
			}
		}
	}
}
