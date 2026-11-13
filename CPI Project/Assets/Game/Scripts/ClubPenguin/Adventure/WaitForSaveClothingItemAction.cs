using ClubPenguin.ClothingDesigner.ItemCustomizer;
using HutongGames.PlayMaker;

namespace ClubPenguin.Adventure
{
	[ActionCategory("GUI")]
	public class WaitForSaveClothingItemAction : FsmStateAction
	{
		public override void OnEnter()
		{
			CustomizationContext.EventBus.AddListener<CustomizerUIEvents.SaveItem>(onSaveItem);
		}

		public override void OnExit()
		{
			CustomizationContext.EventBus.RemoveListener<CustomizerUIEvents.SaveItem>(onSaveItem);
		}

		private bool onSaveItem(CustomizerUIEvents.SaveItem evt)
		{
			Finish();
			return false;
		}
	}
}
