using UnityEngine;

namespace ClubPenguin.Input
{
	public class PromptControllerInputHandler : InputMapHandler<PromptControllerInputMap.Result>
	{
		[SerializeField]
		private InputMappedButton btnAccept;

		[SerializeField]
		private InputMappedButton btnCancel;

		public void Initialize(ButtonClickListener accept, ButtonClickListener cancel)
		{
			btnAccept = ((accept != null) ? accept.GetComponent<InputMappedButton>() : null);
			btnCancel = ((cancel != null) ? cancel.GetComponent<InputMappedButton>() : null);
			if (btnAccept == null || btnCancel == null)
			{
				btnAccept = ((btnAccept != null) ? btnAccept : btnCancel);
				btnCancel = ((btnCancel != null) ? btnCancel : btnAccept);
			}
		}

		protected override void onHandle(PromptControllerInputMap.Result inputResult)
		{
			if (btnAccept != null)
			{
				btnAccept.HandleMappedInput(inputResult.Accept);
			}
			if (btnCancel != null)
			{
				btnCancel.HandleMappedInput(inputResult.Cancel);
			}
		}

		protected override void onReset()
		{
			if (btnAccept != null)
			{
				btnAccept.HandleMappedInput();
			}
			if (btnCancel != null)
			{
				btnCancel.HandleMappedInput();
			}
		}
	}
}
