using ClubPenguin.Core;
using ClubPenguin.Tags;
using HutongGames.PlayMaker;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Quest")]
	public class CheckTags : FsmStateAction
	{
		public FsmGameObject GameObject;

		private TagMatcher TagMatcher;

		public MatchType Match;

		public TagDefinition[] Tags;

		public TagCategoryDefinition[] Categories;

		public FsmEvent TrueEvent;

		public FsmEvent FalseEvent;

		public override void OnEnter()
		{
			TagsData component = GameObject.Value.GetComponent<TagsData>();
			TagMatcher = new TagMatcher();
			TagMatcher.MatchType = Match;
			TagMatcher.Tags = Tags;
			TagMatcher.Categories = Categories;
			if (component != null && TagMatcher.isMatch(component.Data))
			{
				base.Fsm.Event(TrueEvent);
			}
			else
			{
				base.Fsm.Event(FalseEvent);
			}
			Finish();
		}
	}
}
