using System;
using UnityEngine;

namespace ClubPenguin.Tags
{
	[Serializable]
	public class TagMatcher : BaseRecursiveTagMatcher
	{
		[SerializeField]
		private TagMatcher2[] matchers;

		public override BaseTagMatcher[] Matchers
		{
			get
			{
				return matchers;
			}
		}
	}
}
