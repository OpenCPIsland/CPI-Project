using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[Tooltip("Waits for the specific time to trigger an event")]
	[ActionCategory("GUI")]
	public class SetTextOnTextMesh : FsmStateAction
	{
		[RequiredField]
		public FsmGameObject TextMeshGameobject;

		public FsmString Text;

		public bool EveryFrame;

		public override void Reset()
		{
			TextMeshGameobject = null;
			Text = null;
			EveryFrame = false;
		}

		public override void OnEnter()
		{
			updateText();
			if (!EveryFrame)
			{
				Finish();
			}
		}

		private void updateText()
		{
			TextMesh component = TextMeshGameobject.Value.GetComponent<TextMesh>();
			component.text = Text.Value;
		}

		public override void OnUpdate()
		{
			updateText();
		}
	}
}
