using Disney.LaunchPadFramework;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Interactables")]
	public class SetControlsScreenOptionsCommand : FsmStateAction
	{
		public FsmString LeftOptionPrefabPath;

		public FsmString RightOptionPrefabPath;

		public override void OnEnter()
		{
			if (!string.IsNullOrEmpty(LeftOptionPrefabPath.Value))
			{
				Disney.LaunchPadFramework.Log.LogErrorFormatted(this, "This command is no longer supported. Prefab {0} will not be loaded", LeftOptionPrefabPath);
			}
			if (!string.IsNullOrEmpty(RightOptionPrefabPath.Value))
			{
				Disney.LaunchPadFramework.Log.LogErrorFormatted(this, "This command is no longer supported. Prefab {0} will not be loaded", RightOptionPrefabPath);
			}
			Finish();
		}
	}
}
